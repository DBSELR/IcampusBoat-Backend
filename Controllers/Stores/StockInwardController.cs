using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using IcampusBoatBackend.Models.Stores;

namespace IcampusBoatBackend.Controllers.Stores
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class StockInwardController : ControllerBase
    {
        private static double CalculateBillTotalAmount(DataTable dt)
        {
            double totalAmount = 0;
            if (dt != null && dt.Columns.Contains("TOTALAMOUNT"))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["TOTALAMOUNT"] != DBNull.Value && double.TryParse(row["TOTALAMOUNT"].ToString(), out double amt))
                    {
                        totalAmount += amt;
                    }
                }
            }
            return totalAmount;
        }

        private static string FormatSqlDate(string? dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }

            string trimmed = dateStr.Trim();
            string[] formats = { "dd-MM-yyyy", "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy", "d-M-yyyy", "yyyy/MM/dd" };
            if (DateTime.TryParseExact(trimmed, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime exactDt))
            {
                return exactDt.ToString("yyyy-MM-dd");
            }

            if (DateTime.TryParse(trimmed, out DateTime parsedDt))
            {
                return parsedDt.ToString("yyyy-MM-dd");
            }

            return trimmed;
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
        [HttpGet("list")]
        public IActionResult GetStockInwardList([FromQuery] string? billNo, [FromQuery] string? billDate)
        {
            if (string.IsNullOrWhiteSpace(billNo))
            {
                return BadRequest(new { success = false, message = "billNo is required." });
            }

            try
            {
                string sqlDate = FormatSqlDate(billDate);
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_INV_STOCKREC_SAVE_List", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BillNo", billNo.Trim());
                        cmd.Parameters.AddWithValue("@BillDate", sqlDate);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                double totalAmount = CalculateBillTotalAmount(dt);
                string billDateFormatted = "";
                string taxType = "";
                string supplierName = "";
                string supplierAddress = "";
                bool igstEnabled = true;
                bool cgstEnabled = true;
                bool sgstEnabled = true;

                if (dt.Rows.Count > 0)
                {
                    DataRow firstRow = dt.Rows[0];
                    if (dt.Columns.Contains("BillDate") && firstRow["BillDate"] != DBNull.Value &&
                        DateTime.TryParse(firstRow["BillDate"].ToString(), out DateTime parsedDt))
                    {
                        billDateFormatted = parsedDt.ToString("dd-MM-yyyy");
                    }

                    taxType = dt.Columns.Contains("TAXTYPE") ? (firstRow["TAXTYPE"]?.ToString() ?? "") : "";
                    supplierName = dt.Columns.Contains("SupplierName") ? (firstRow["SupplierName"]?.ToString() ?? "") : "";
                    supplierAddress = dt.Columns.Contains("SupplierAddress") ? (firstRow["SupplierAddress"]?.ToString() ?? "") : "";

                    if (string.Equals(taxType, "COMPOSITE DEALER", StringComparison.OrdinalIgnoreCase))
                    {
                        igstEnabled = false;
                        cgstEnabled = false;
                        sgstEnabled = false;
                    }
                }

                return Ok(new
                {
                    success = true,
                    billNo = billNo.Trim(),
                    billDateFormatted,
                    taxType,
                    supplierName,
                    supplierAddress,
                    igstEnabled,
                    cgstEnabled,
                    sgstEnabled,
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
        [HttpGet("load")]
        public IActionResult LoadStockInward([FromQuery] int id)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_INV_STOCKREC_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                double totalAmount = CalculateBillTotalAmount(dt);
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
        [HttpGet("by-id")]
        public IActionResult GetStockInwardById([FromQuery] int id)
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

                        string itemName = dt.Columns.Contains("ItemsName") ? (row["ItemsName"]?.ToString() ?? "") : "";
                        if (!string.IsNullOrEmpty(itemName))
                        {
                            DataTable dtMrp = new DataTable();
                            using (SqlCommand mrpCmd = new SqlCommand("SELECT MARKETPRICE FROM TBL_ProductMaster WHERE Prod_Code = @ItemName OR PRODUCTNAME = @ItemName", con))
                            {
                                mrpCmd.CommandType = CommandType.Text;
                                mrpCmd.Parameters.AddWithValue("@ItemName", itemName);
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
        [HttpGet("by-bill")]
        public IActionResult GetByBillNo([FromQuery] string? billNo)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_INVEN_STOCKREC WHERE BillNo = @BillNo", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@BillNo", (billNo ?? "").Trim());
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                double totalAmount = CalculateBillTotalAmount(dt);
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
        [HttpGet("product-by-name")]
        public IActionResult GetProductByName([FromQuery] string? itemsName)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    string sql = @"SELECT Prod_Code,
                                          CAST(OPENINGSTOCK AS INT) AS OPENINGSTOCK,
                                          CAST(OPENINGSTOCK AS INT) AS MARKETPRICE,
                                          CGST, SGST, IGST, HSNCODE
                                   FROM TBL_ProductMaster
                                   WHERE PRODUCTNAME = @ItemsName";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ItemsName", (itemsName ?? "").Trim());
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                string prodCode = (dt.Rows.Count > 0 && dt.Columns.Contains("Prod_Code"))
                    ? (dt.Rows[0]["Prod_Code"]?.ToString() ?? "")
                    : "";

                return Ok(new
                {
                    success = true,
                    prodCode,
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("product-details")]
        public IActionResult GetProductDetails([FromQuery] string? prodCode)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_ProductMaster WHERE Prod_Code = @ProdCode", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ProdCode", (prodCode ?? "").Trim());
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
        [HttpGet("tax-type-rules")]
        public IActionResult GetTaxTypeRules([FromQuery] string? taxType)
        {
            string t = (taxType ?? "").Trim().ToUpperInvariant();
            bool igstEnabled = true;
            bool cgstEnabled = true;
            bool sgstEnabled = true;

            if (t == "1" || t == "WITH IN STATE")
            {
                igstEnabled = false;
                cgstEnabled = true;
                sgstEnabled = true;
            }
            else if (t == "2" || t == "INTRA STATE")
            {
                igstEnabled = true;
                cgstEnabled = false;
                sgstEnabled = false;
            }
            else if (t == "3" || t == "COMPOSITE DEALER")
            {
                igstEnabled = false;
                cgstEnabled = false;
                sgstEnabled = false;
            }

            return Ok(new
            {
                success = true,
                taxType,
                igstEnabled,
                cgstEnabled,
                sgstEnabled
            });
        }

        [AllowAnonymous]
        [HttpPost("calculate-tax")]
        public IActionResult CalculateTax([FromBody] CalculateStockInwardTaxRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                decimal.TryParse((request.Quantity ?? "0").Replace(".00", "").Trim(), out decimal qty);
                decimal.TryParse((request.PurchasePrice ?? "0").Replace(".00", "").Trim(), out decimal pp);
                decimal.TryParse((request.Cgst ?? "0").Replace(".00", "").Trim(), out decimal cgstPct);
                decimal.TryParse((request.Sgst ?? "0").Replace(".00", "").Trim(), out decimal sgstPct);
                decimal.TryParse((request.Igst ?? "0").Replace(".00", "").Trim(), out decimal igstPct);

                decimal totalPrice = qty * pp;
                string taxType = (request.TaxType ?? "").Trim().ToUpperInvariant();

                if (taxType == "3" || taxType == "COMPOSITE DEALER")
                {
                    return Ok(new
                    {
                        success = true,
                        amount = totalPrice.ToString(),
                        cgstAmount = "0",
                        sgstAmount = "0",
                        igstAmount = "0",
                        totalTaxAmount = "0",
                        totalAmount = totalPrice.ToString()
                    });
                }

                decimal prCGST = (totalPrice / 100m) * cgstPct;
                decimal prSGST = (totalPrice / 100m) * sgstPct;
                decimal prIGST = (totalPrice / 100m) * igstPct;

                if (taxType == "1" || taxType == "WITH IN STATE")
                {
                    prIGST = 0;
                }
                else if (taxType == "2" || taxType == "INTRA STATE")
                {
                    prCGST = 0;
                    prSGST = 0;
                }

                decimal totalTaxAmount = prCGST + prSGST + prIGST;
                decimal prTotalPrice = totalPrice + totalTaxAmount;

                return Ok(new
                {
                    success = true,
                    amount = totalPrice.ToString(),
                    cgstAmount = prCGST.ToString(),
                    sgstAmount = prSGST.ToString(),
                    igstAmount = prIGST.ToString(),
                    totalTaxAmount = totalTaxAmount.ToString(),
                    totalAmount = prTotalPrice.ToString()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("autocomplete-supplier")]
        public IActionResult AutocompleteSupplier([FromQuery] string? prefixText)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    string sql = "SELECT DISTINCT SupName FROM TBL_VENDORMASTER WHERE (@Prefix = '' OR SupName LIKE @Prefix + '%') ORDER BY SupName";
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
        [HttpGet("autocomplete-address")]
        public IActionResult AutocompleteSupplierAddress([FromQuery] string? prefixText)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    string sql = "SELECT DISTINCT supaddress AS SupplierAddress FROM TBL_VENDORMASTER WHERE supaddress LIKE @Prefix + '%'";
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
        [HttpGet("autocomplete-product-name")]
        public IActionResult AutocompleteProductName([FromQuery] string? prefixText)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    string sql = "SELECT DISTINCT PRODUCTNAME FROM TBL_ProductMaster WHERE PRODUCTNAME LIKE '%' + @Prefix + '%'";
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
        [HttpGet("autocomplete-product-code")]
        public IActionResult AutocompleteProductCode([FromQuery] string? prefixText)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    string sql = "SELECT DISTINCT Prod_Code FROM TBL_ProductMaster WHERE Prod_Code LIKE @Prefix + '%'";
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
        [HttpPost("save")]
        public IActionResult SaveStockInward([FromBody] SaveStockInwardRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                bool isNew = string.IsNullOrWhiteSpace(request.Id) || request.Id.Trim() == "0";
                int idVal = int.TryParse(request.Id, out int parsedId) ? parsedId : 0;
                int pQty = int.TryParse((request.Quantity ?? "0").Trim(), out int parsedQty) ? parsedQty : 0;
                double purchasePrice = double.TryParse((request.PurchasePrice ?? "0").Trim(), out double parsedPp) ? parsedPp : 0;

                decimal computedAmount = (decimal)pQty * (decimal)purchasePrice;
                if (!string.IsNullOrWhiteSpace(request.Amount) && decimal.TryParse(request.Amount.Trim(), out decimal explicitAmt) && explicitAmt > 0)
                {
                    computedAmount = explicitAmt;
                }

                decimal cgstVal = decimal.TryParse((request.Cgst ?? "0").Trim(), out decimal parsedCgst) ? parsedCgst : 0m;
                decimal sgstVal = decimal.TryParse((request.Sgst ?? "0").Trim(), out decimal parsedSgst) ? parsedSgst : 0m;
                decimal igstVal = decimal.TryParse((request.Igst ?? "0").Trim(), out decimal parsedIgst) ? parsedIgst : 0m;
                decimal hsnVal = decimal.TryParse((request.HsnCode ?? "0").Trim(), out decimal parsedHsn) ? parsedHsn : 0m;
                decimal totalTaxVal = decimal.TryParse((request.TotalTaxAmount ?? "0").Trim(), out decimal parsedTotTax) ? parsedTotTax : 0m;
                decimal totalAmtVal = decimal.TryParse((request.TotalAmount ?? "0").Trim(), out decimal parsedTotAmt) ? parsedTotAmt : computedAmount + totalTaxVal;

                string formattedBillDate = FormatSqlDate(request.BillDate);
                int rowsAffected = 0;

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_INV_STOCKREC_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", idVal);
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@FinancialYear", request.FinancialYear ?? "");
                        cmd.Parameters.AddWithValue("@BillNo", request.BillNo ?? "");
                        cmd.Parameters.AddWithValue("@BillDate", formattedBillDate);
                        cmd.Parameters.AddWithValue("@SupplierName", request.SupplierName ?? "");
                        cmd.Parameters.AddWithValue("@SupplierAddress", request.SupplierAddress ?? "");
                        cmd.Parameters.AddWithValue("@ItemsName", request.ItemsName ?? "");
                        cmd.Parameters.AddWithValue("@PQuantity", pQty);
                        cmd.Parameters.AddWithValue("@PurchasePrice", purchasePrice);
                        cmd.Parameters.AddWithValue("@Amount", computedAmount);
                        cmd.Parameters.AddWithValue("@UserId", request.UserId ?? "");
                        cmd.Parameters.AddWithValue("@ProCode", request.ProdCode ?? "");
                        cmd.Parameters.AddWithValue("@CGST", cgstVal);
                        cmd.Parameters.AddWithValue("@SGST", sgstVal);
                        cmd.Parameters.AddWithValue("@IGST", igstVal);
                        cmd.Parameters.AddWithValue("@HSNCODE", hsnVal);
                        cmd.Parameters.AddWithValue("@TOTALTAXPERCENTAGE", totalTaxVal);
                        cmd.Parameters.AddWithValue("@TOTALAMOUNT", totalAmtVal);
                        cmd.Parameters.AddWithValue("@TAXTYPE", request.TaxType ?? "");

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                string message = isNew ? "Data Saved Successfully" : "Data Updated Successfully";
                return Ok(new { success = true, message, rowsAffected });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpDelete("delete")]
        public IActionResult DeleteStockInward([FromQuery] int id, [FromQuery] string? itemsName)
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