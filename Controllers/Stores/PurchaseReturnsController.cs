using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using IcampusBoatBackend.Models.Stores;

namespace IcampusBoatBackend.Controllers.Stores
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseReturnsController : ControllerBase
    {
        private static double CalculateTotalAmount(DataTable dt)
        {
            double totalAmount = 0;
            if (dt != null && dt.Columns.Contains("Amount"))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Amount"] != DBNull.Value && double.TryParse(row["Amount"].ToString(), out double amt))
                    {
                        totalAmount += amt;
                    }
                }
            }
            return totalAmount;
        }

        [AllowAnonymous]
        [HttpGet("suppliers")]
        public IActionResult GetSuppliers()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT SupName FROM TBL_VENDORMASTER ORDER BY SupName", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("items")]
        public IActionResult GetItems()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT Prod_Code, PRODUCTNAME + '-' + Prod_Code AS PRODUCTNAME FROM TBL_ProductMaster", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("list")]
        public IActionResult GetPurchaseReturnsList([FromQuery] string? billNo)
        {
            try
            {
                string trimmedBillNo = (billNo ?? "").Trim();
                string sql = string.IsNullOrEmpty(trimmedBillNo)
                    ? "SELECT * FROM TBL_INVEN_STOCKREC WHERE billno = billno"
                    : "SELECT * FROM TBL_INVEN_STOCKREC WHERE billno = @BillNo";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        if (!string.IsNullOrEmpty(trimmedBillNo))
                        {
                            cmd.Parameters.AddWithValue("@BillNo", trimmedBillNo);
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                double totalAmount = CalculateTotalAmount(dt);
                return Ok(new
                {
                    success = true,
                    totalAmount,
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("by-supplier")]
        public IActionResult GetBySupplier([FromQuery] string? supplierName)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_INVEN_STOCKREC WHERE SupplierName = @SupplierName", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@SupplierName", supplierName ?? "");
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                double totalAmount = CalculateTotalAmount(dt);
                return Ok(new
                {
                    success = true,
                    totalAmount,
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("by-bill")]
        public IActionResult GetByBill([FromQuery] string? billNo, [FromQuery] string? supplierName)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    string sql = @"SELECT * FROM TBL_INVEN_STOCKREC 
                                   WHERE (@BillNo = '' OR BillNo = @BillNo) 
                                     AND (@SupplierName = '' OR SupplierName = @SupplierName)";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@BillNo", (billNo ?? "").Trim());
                        cmd.Parameters.AddWithValue("@SupplierName", (supplierName ?? "").Trim());
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                double totalAmount = CalculateTotalAmount(dt);
                return Ok(new
                {
                    success = true,
                    totalAmount,
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("supplier-address")]
        public IActionResult GetSupplierAddress([FromQuery] string? supplierName)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT supaddress AS SupplierAddress FROM TBL_VENDORMASTER WHERE SupName = @SupplierName", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@SupplierName", supplierName ?? "");
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                string supplierAddress = (dt.Rows.Count > 0 && dt.Columns.Contains("SupplierAddress"))
                    ? (dt.Rows[0]["SupplierAddress"]?.ToString() ?? "")
                    : "";

                return Ok(new
                {
                    success = true,
                    supplierAddress,
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("bill-details")]
        public IActionResult GetBillDetails([FromQuery] string? billNo, [FromQuery] string? supplierName)
        {
            if (string.IsNullOrWhiteSpace(billNo) || string.IsNullOrWhiteSpace(supplierName))
            {
                return BadRequest(new { success = false, message = "Both billNo and supplierName are required." });
            }

            try
            {
                DataTable dt = new DataTable();
                DataTable dtBillItems = new DataTable();

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    // Execute TXTBILL stored procedure
                    using (SqlCommand cmd = new SqlCommand("TXTBILL", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BillNo", billNo.Trim());
                        cmd.Parameters.AddWithValue("@suppliername", supplierName.Trim());
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    // Load_Items_Billwise() -> Get_BillItemNames
                    string itemsSql = @"SELECT DISTINCT ItemsName + '_' + ProductCode AS ItemsName, ProductCode 
                                        FROM TBL_INVEN_STOCKREC 
                                        WHERE BillNo = @BillNo AND SupplierName = @SupplierName";
                    using (SqlCommand itemsCmd = new SqlCommand(itemsSql, con))
                    {
                        itemsCmd.CommandType = CommandType.Text;
                        itemsCmd.Parameters.AddWithValue("@BillNo", billNo.Trim());
                        itemsCmd.Parameters.AddWithValue("@SupplierName", supplierName.Trim());
                        using (SqlDataAdapter da = new SqlDataAdapter(itemsCmd))
                        {
                            da.Fill(dtBillItems);
                        }
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "No Data Found",
                        data = new List<Dictionary<string, object>>(),
                        billItems = new List<Dictionary<string, object>>()
                    });
                }

                DataRow firstRow = dt.Rows[0];
                string billDateFormatted = "";
                if (dt.Columns.Contains("BillDate") && firstRow["BillDate"] != DBNull.Value &&
                    DateTime.TryParse(firstRow["BillDate"].ToString(), out DateTime parsedDt))
                {
                    billDateFormatted = parsedDt.ToString("dd-MM-yyyy");
                }

                double totalAmount = CalculateTotalAmount(dt);

                return Ok(new
                {
                    success = true,
                    billDateFormatted,
                    supplierName = dt.Columns.Contains("SupplierName") ? firstRow["SupplierName"]?.ToString() : supplierName,
                    supplierAddress = dt.Columns.Contains("SupplierAddress") ? firstRow["SupplierAddress"]?.ToString() : "",
                    productCode = dt.Columns.Contains("productcode") ? firstRow["productcode"]?.ToString() : (dt.Columns.Contains("ProductCode") ? firstRow["ProductCode"]?.ToString() : ""),
                    pQuantity = dt.Columns.Contains("PQuantity") ? firstRow["PQuantity"]?.ToString() : "",
                    purchasePrice = dt.Columns.Contains("PurchasePrice") ? firstRow["PurchasePrice"]?.ToString() : "",
                    marketPrice = dt.Columns.Contains("MARKETPRICE") ? firstRow["MARKETPRICE"]?.ToString() : "",
                    amount = dt.Columns.Contains("Amount") ? firstRow["Amount"]?.ToString() : "",
                    totalAmount,
                    billItems = DAL.DataTableToList(dtBillItems),
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("bill-items")]
        public IActionResult GetBillItems([FromQuery] string? billNo, [FromQuery] string? supplierName)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    string sql = @"SELECT DISTINCT ItemsName + '_' + ProductCode AS ItemsName, ProductCode 
                                   FROM TBL_INVEN_STOCKREC 
                                   WHERE BillNo = @BillNo AND SupplierName = @SupplierName";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@BillNo", (billNo ?? "").Trim());
                        cmd.Parameters.AddWithValue("@SupplierName", (supplierName ?? "").Trim());
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("item-mrp")]
        public IActionResult GetItemMrp([FromQuery] string? prodCode)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    string sql = "SELECT MARKETPRICE FROM TBL_ProductMaster WHERE Prod_Code = @ProdCode OR PRODUCTNAME = @ProdCode";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ProdCode", (prodCode ?? "").Trim());
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                string marketPrice = (dt.Rows.Count > 0 && dt.Columns.Contains("MARKETPRICE"))
                    ? (dt.Rows[0]["MARKETPRICE"]?.ToString() ?? "0")
                    : "0";

                return Ok(new
                {
                    success = true,
                    marketPrice,
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("by-id")]
        public IActionResult GetPurchaseReturnById([FromQuery] int id)
        {
            try
            {
                DataTable dt = new DataTable();
                string marketPrice = "0";
                string billDateFormatted = "";

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_INVEN_STOCKREC WHERE id = @ID", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ID", id);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        if (dt.Columns.Contains("BillDate") && row["BillDate"] != DBNull.Value &&
                            DateTime.TryParse(row["BillDate"].ToString(), out DateTime parsedDt))
                        {
                            billDateFormatted = parsedDt.ToString("dd-MM-yyyy");
                        }

                        string prodCode = "";
                        if (dt.Columns.Contains("productcode") && row["productcode"] != DBNull.Value)
                        {
                            prodCode = row["productcode"].ToString()?.Replace(" ", "") ?? "";
                        }
                        else if (dt.Columns.Contains("ProductCode") && row["ProductCode"] != DBNull.Value)
                        {
                            prodCode = row["ProductCode"].ToString()?.Replace(" ", "") ?? "";
                        }

                        if (!string.IsNullOrEmpty(prodCode))
                        {
                            DataTable dtMrp = new DataTable();
                            using (SqlCommand mrpCmd = new SqlCommand("SELECT MARKETPRICE FROM TBL_ProductMaster WHERE Prod_Code = @ProdCode", con))
                            {
                                mrpCmd.CommandType = CommandType.Text;
                                mrpCmd.Parameters.AddWithValue("@ProdCode", prodCode);
                                using (SqlDataAdapter da = new SqlDataAdapter(mrpCmd))
                                {
                                    da.Fill(dtMrp);
                                }
                            }

                            if (dtMrp.Rows.Count > 0 && dtMrp.Columns.Count > 0)
                            {
                                marketPrice = dtMrp.Rows[0][0]?.ToString() ?? "0";
                            }
                        }
                    }
                }

                return Ok(new
                {
                    success = true,
                    billDateFormatted,
                    marketPrice,
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("autocomplete-bill")]
        public IActionResult AutocompleteBillNo([FromQuery] string? prefixText)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    string sql = "SELECT DISTINCT BillNo FROM TBL_INVEN_STOCKREC WHERE (@Prefix = '' OR BillNo LIKE @Prefix + '%')";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Prefix", (prefixText ?? "").Trim());
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("autocomplete-item")]
        public IActionResult AutocompleteItemName([FromQuery] string? prefixText, [FromQuery] string? billNo)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    string sql = @"SELECT DISTINCT ItemsName + '_' + ProductCode AS ItemsName, ProductCode 
                                   FROM TBL_INVEN_STOCKREC 
                                   WHERE (@Prefix = '' OR ItemsName LIKE @Prefix + '%')
                                     AND (@BillNo = '' OR BillNo = @BillNo)";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Prefix", (prefixText ?? "").Trim());
                        cmd.Parameters.AddWithValue("@BillNo", (billNo ?? "").Trim());
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("calculate-amount")]
        public IActionResult CalculateAmount([FromQuery] int qty, [FromQuery] decimal purchasePrice)
        {
            decimal amount = qty * purchasePrice;
            return Ok(new
            {
                success = true,
                qty,
                purchasePrice,
                amount = amount.ToString()
            });
        }

        [AllowAnonymous]
        [HttpPost("save")]
        public IActionResult SavePurchaseReturn([FromBody] SavePurchaseReturnRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            if (string.IsNullOrWhiteSpace(request.RQty) || !int.TryParse(request.RQty.Trim(), out int rQtyInt) || rQtyInt <= 0)
            {
                return BadRequest(new { success = false, message = "RQty Must be >0 " });
            }

            if (!string.IsNullOrWhiteSpace(request.PQty) && int.TryParse(request.PQty.Trim(), out int pQtyInt))
            {
                if (rQtyInt > pQtyInt)
                {
                    return BadRequest(new { success = false, message = "RQty Must be <= Qty." });
                }
            }

            try
            {
                bool isNew = string.IsNullOrWhiteSpace(request.Id) || request.Id.Trim() == "0";
                int rowsAffected = 0;

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_INV_STOCKRECR_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@FinancialYear", request.FinancialYear ?? "");
                        cmd.Parameters.AddWithValue("@BillNo", request.BillNo ?? "");
                        cmd.Parameters.AddWithValue("@ItemsName", request.ItemsName ?? "");
                        cmd.Parameters.AddWithValue("@RQty", rQtyInt);
                        cmd.Parameters.AddWithValue("@Remarks", request.Remarks ?? "");
                        cmd.Parameters.AddWithValue("@UserId", request.UserId ?? "");
                        cmd.Parameters.AddWithValue("@SupplierName", request.SupplierName ?? "");

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                if (rowsAffected > 0)
                {
                    string message = isNew ? "Data Saved Successfully" : "Data Updated Successfully";
                    return Ok(new { success = true, message, rowsAffected });
                }

                return Ok(new { success = false, message = "No matching stock receive record found to update.", rowsAffected = 0 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpDelete("delete")]
        public IActionResult DeletePurchaseReturn([FromQuery] int id, [FromQuery] string? itemsName)
        {
            if (id <= 0)
            {
                return BadRequest(new { success = false, message = "Valid id is required." });
            }

            try
            {
                int rowsAffected = 0;
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_INV_STOCKREC_Delete", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@ItemsName", itemsName ?? "");
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                if (rowsAffected > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        deleted = true,
                        message = "Record Deleted Successfully",
                        deletedCount = rowsAffected
                    });
                }

                return Ok(new
                {
                    success = false,
                    deleted = false,
                    message = "Record Not Found or Not Deleted",
                    deletedCount = 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
