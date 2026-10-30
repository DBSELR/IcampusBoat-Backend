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
    public class ProductMasterController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT Cid, Category FROM tbl_Category_master", con))
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
        [HttpGet("category-master")]
        public IActionResult GetCategoryMaster()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_Category_master", con))
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
        [HttpPost("save-category")]
        public IActionResult SaveCategory([FromBody] SaveCategoryMasterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                int idVal = int.TryParse(request.Id, out int parsedId) ? parsedId : 0;
                int rowsAffected = 0;

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_SaveCategory_Master", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", idVal);
                        cmd.Parameters.AddWithValue("@Code", request.ProductCode ?? "");
                        cmd.Parameters.AddWithValue("@category", request.Category ?? "");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@UserId", request.UserId ?? "");
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                return Ok(new
                {
                    success = true,
                    message = idVal == 0 ? "Category Saved Successfully" : "Category Updated Successfully",
                    rowsAffected
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("academic-years")]
        public IActionResult GetAcademicYears()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_APPSALES_AcademicYear_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
        [HttpGet("courses")]
        public IActionResult GetCourses()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STUDENTREGISTRATION_Programme_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
        [HttpGet("auto-code")]
        public IActionResult GetAutoProductCode([FromQuery] string? academicYear, [FromQuery] string? category)
        {
            try
            {
                DataTable tData = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_AUTO_Prod_Code", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? "");
                        cmd.Parameters.AddWithValue("@Category", category ?? "");
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(tData);
                        }
                    }

                    string prodCode = "";

                    if (tData != null && tData.Rows.Count > 0 && tData.Columns.Contains("Prod_Code"))
                    {
                        string s = tData.Rows[0]["Prod_Code"]?.ToString() ?? "";
                        string[] ar = s.Split('-');
                        if (ar.Length >= 2 && int.TryParse(ar[1], out int i))
                        {
                            i = i + 1;
                            prodCode = ar[0] + "-" + i.ToString();
                        }
                        else
                        {
                            prodCode = s;
                        }
                    }
                    else
                    {
                        DataTable dt = new DataTable();
                        using (SqlCommand fallbackCmd = new SqlCommand("SELECT Code FROM tbl_Category_master WHERE Category = @Category", con))
                        {
                            fallbackCmd.CommandType = CommandType.Text;
                            fallbackCmd.Parameters.AddWithValue("@Category", category ?? "");
                            using (SqlDataAdapter da = new SqlDataAdapter(fallbackCmd))
                            {
                                da.Fill(dt);
                            }
                        }

                        if (dt.Rows.Count > 0 && dt.Columns.Contains("Code"))
                        {
                            prodCode = dt.Rows[0]["Code"]?.ToString() ?? "";
                        }
                    }

                    return Ok(new
                    {
                        success = true,
                        prodCode,
                        data = tData != null ? DAL.DataTableToList(tData) : null
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("list")]
        public IActionResult GetProductMasterList([FromQuery] string? category)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Stores_PM__List", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Category", category ?? "");
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
        [HttpGet("by-id")]
        public IActionResult GetProductById([FromQuery] int id)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM TBL_ProductMaster WHERE id = @ID", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ID", id);
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
        [HttpGet("load")]
        public IActionResult LoadProductMaster([FromQuery] int id)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Stores_PM_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
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
        [HttpGet("check-exists")]
        public IActionResult CheckProductExists([FromQuery] string? productName, [FromQuery] string? productCode)
        {
            try
            {
                string lookupValue = !string.IsNullOrWhiteSpace(productCode)
                    ? productCode.Trim()
                    : (productName ?? "").Trim();

                if (string.IsNullOrEmpty(lookupValue))
                {
                    return Ok(new { success = true, exists = false, message = "", data = new List<Dictionary<string, object>>() });
                }

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetProductData", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PRODUCTNAME", lookupValue);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                bool exists = dt.Rows.Count > 0;
                return Ok(new
                {
                    success = true,
                    exists,
                    message = exists ? "Already Exits" : "",
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("search")]
        public IActionResult SearchByProductName([FromQuery] string? productName)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_productmaster WHERE PRODUCTNAME = @ProductName", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ProductName", (productName ?? "").Trim());
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
        [HttpGet("autocomplete-code")]
        public IActionResult AutocompleteProductCode([FromQuery] string? prefixText)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT Prod_Code FROM TBL_ProductMaster WHERE Prod_Code LIKE @Prefix + '%'", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Prefix", prefixText ?? "");
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
        [HttpGet("autocomplete-name")]
        public IActionResult AutocompleteProductName([FromQuery] string? prefixText, [FromQuery] string? procType = "PRODUCTNAME")
        {
            try
            {
                string sql = string.Equals(procType, "PRODUCTNAME", StringComparison.OrdinalIgnoreCase)
                    ? "SELECT DISTINCT PRODUCTNAME FROM TBL_ProductMaster WHERE PRODUCTNAME LIKE '%' + @Prefix + '%'"
                    : "SELECT DISTINCT PRODUCTTYPE FROM TBL_ProductMaster WHERE PRODUCTTYPE LIKE '%' + @Prefix + '%'";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Prefix", prefixText ?? "");
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
        [HttpGet("autocomplete-category")]
        public IActionResult AutocompleteCategory([FromQuery] string? prefixText)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT CATEGORY FROM TBL_ProductMaster WHERE CATEGORY LIKE @Prefix + '%'", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Prefix", prefixText ?? "");
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
        [HttpGet("autocomplete-issue")]
        public IActionResult AutocompleteProductIssue([FromQuery] string? prefixText, [FromQuery] string? procType = "PRODUCTNAME")
        {
            try
            {
                string sql = string.Equals(procType, "PRODUCTNAME", StringComparison.OrdinalIgnoreCase)
                    ? "SELECT DISTINCT PRODUCTNAME FROM TBL_ProductMaster WHERE OPENINGSTOCK >= 1 AND PRODUCTNAME LIKE '%' + @Prefix + '%'"
                    : "SELECT DISTINCT PRODUCTTYPE FROM TBL_ProductMaster WHERE OPENINGSTOCK >= 1 AND PRODUCTTYPE LIKE '%' + @Prefix + '%'";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Prefix", prefixText ?? "");
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
        public IActionResult SaveProductMaster([FromBody] SaveProductMasterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                bool isNew = string.IsNullOrWhiteSpace(request.Id) || request.Id.Trim() == "0";
                int idVal = int.TryParse(request.Id, out int parsedId) ? parsedId : 0;
                int rowsAffected = 0;

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Sp_Stores_ProductMaster_Save", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", idVal);
                        cmd.Parameters.AddWithValue("@ProductCode", request.ProductCode ?? "");
                        cmd.Parameters.AddWithValue("@PRODUCTNAME", (request.ProductName ?? "").Trim());
                        cmd.Parameters.AddWithValue("@CATEGORY", request.Category ?? "");
                        cmd.Parameters.AddWithValue("@OPENINGSTOCK", request.OpeningStock ?? "0");
                        cmd.Parameters.AddWithValue("@MARKETPRICE", request.MarketPrice ?? "0");
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@COURSE", request.Course ?? "");
                        cmd.Parameters.AddWithValue("@PRODUCTTYPE", string.IsNullOrWhiteSpace(request.ProductType) ? "A" : request.ProductType);
                        cmd.Parameters.AddWithValue("@PURCHASEPRICE", request.PurchasePrice ?? "0");
                        cmd.Parameters.AddWithValue("@PURCHASESALESTAXVAT", request.PurchaseSalesTaxVat ?? "0");
                        cmd.Parameters.AddWithValue("@PURCHASEEXCISE", request.PurchaseExcise ?? "0");
                        cmd.Parameters.AddWithValue("@PURCHASEDISCOUNT", request.PurchaseDiscount ?? "0");
                        cmd.Parameters.AddWithValue("@SALEPRICE", request.SalePrice ?? "0");
                        cmd.Parameters.AddWithValue("@SALESALESTAXVAT", request.SaleSalesTaxVat ?? "0");
                        cmd.Parameters.AddWithValue("@SALEEXCISE", request.SaleExcise ?? "0");
                        cmd.Parameters.AddWithValue("@SALEDISCOUNT", request.SaleDiscount ?? "0");
                        cmd.Parameters.AddWithValue("@FinancialYear", request.FinancialYear ?? "");
                        cmd.Parameters.AddWithValue("@MinimumQty", request.MinimumQty ?? "0");
                        cmd.Parameters.AddWithValue("@UserId", request.UserId ?? "");
                        cmd.Parameters.AddWithValue("@IGST", request.Igst ?? "0");
                        cmd.Parameters.AddWithValue("@CGST", request.Cgst ?? "0");
                        cmd.Parameters.AddWithValue("@SGST", request.Sgst ?? "0");
                        cmd.Parameters.AddWithValue("@HSNCODE", request.HsnCode ?? "0");

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
        public IActionResult DeleteProductMaster([FromQuery] string? productCode, [FromQuery] string? id)
        {
            if (string.IsNullOrWhiteSpace(productCode))
            {
                return BadRequest(new { success = false, message = "productCode is required." });
            }

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Stores_PM_Delete", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PROCODE", productCode);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                string status = (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                    ? (dt.Rows[0][0]?.ToString() ?? "0")
                    : "0";

                if (status == "1")
                {
                    return Ok(new
                    {
                        success = true,
                        deleted = true,
                        message = "Record Deleted Successfully"
                    });
                }

                return Ok(new
                {
                    success = false,
                    deleted = false,
                    message = "Product Code Existed in Issues or Inward..!!! Can't be deleted."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
