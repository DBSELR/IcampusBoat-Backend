using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using IcampusBoatBackend.Models.Stores;

namespace IcampusBoatBackend.Controllers.Stores
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class VendorMasterController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("auto-code")]
        public IActionResult GetAutoSupplierCode([FromQuery] string? academicYear)
        {
            try
            {
                DataTable tData = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_AUTOSUPPCODE_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? "");
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(tData);
                        }
                    }
                }

                string supCode = "VMSPC-1";
                if (tData != null && tData.Rows.Count > 0 && tData.Columns.Contains("SupCode"))
                {
                    string s = tData.Rows[0]["SupCode"]?.ToString() ?? "";
                    string[] ar = s.Split('-');
                    if (ar.Length >= 2 && int.TryParse(ar[1], out int i))
                    {
                        i = i + 1;
                        supCode = ar[0] + "-" + i.ToString();
                    }
                    else if (!string.IsNullOrEmpty(s))
                    {
                        supCode = s;
                    }
                }

                return Ok(new
                {
                    success = true,
                    supCode,
                    data = tData != null ? DAL.DataTableToList(tData) : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("list")]
        public IActionResult GetVendorMasterList()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Stores_VM__List", con))
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
        [HttpGet("by-id")]
        public IActionResult GetVendorById([FromQuery] int id)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_VendorMaster WHERE id = @ID", con))
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
        public IActionResult LoadVendorMaster([FromQuery] int id)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Stores_VM_LOAD", con))
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
        [HttpGet("search")]
        public IActionResult SearchVendor([FromQuery] string? searchName)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_SEARCHBY_SuppCodeName", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SEARCHNAME", searchName ?? "");
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
        [HttpGet("autocomplete-bank")]
        public IActionResult AutocompleteBankName([FromQuery] string? prefixText)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT BankName FROM tbl_VendorMaster WHERE BankName LIKE @Prefix + '%'", con))
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
        [HttpGet("autocomplete-branch")]
        public IActionResult AutocompleteBranchName([FromQuery] string? prefixText)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT BranchName FROM tbl_VendorMaster WHERE BranchName LIKE @Prefix + '%'", con))
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
        public IActionResult SaveVendorMaster([FromBody] SaveVendorMasterRequest request)
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
                    using (SqlCommand cmd = new SqlCommand("SP_Stores_VM_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", idVal);
                        cmd.Parameters.AddWithValue("@SupCode", request.SupCode ?? "");
                        cmd.Parameters.AddWithValue("@SupName", request.SupName ?? "");
                        cmd.Parameters.AddWithValue("@SupType", request.SupType ?? "");
                        cmd.Parameters.AddWithValue("@SupGstin", request.SupGstin ?? "");
                        cmd.Parameters.AddWithValue("@SupEmailid", request.SupEmailid ?? "");
                        cmd.Parameters.AddWithValue("@SupCntNo", request.SupCntNo ?? "");
                        cmd.Parameters.AddWithValue("@SupAddress", request.SupAddress ?? "");
                        cmd.Parameters.AddWithValue("@HolderName", request.HolderName ?? "");
                        cmd.Parameters.AddWithValue("@AccNo", request.AccNo ?? "");
                        cmd.Parameters.AddWithValue("@BankName", request.BankName ?? "");
                        cmd.Parameters.AddWithValue("@BranchName", request.BranchName ?? "");
                        cmd.Parameters.AddWithValue("@IFSCcode", request.IFSCcode ?? "");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@FinancialYear", request.FinancialYear ?? "");
                        cmd.Parameters.AddWithValue("@UserId", request.UserId ?? "");

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                if (rowsAffected > 0)
                {
                    string message = isNew ? "Data Saved Successfully" : "Data Updated Successfully";
                    return Ok(new { success = true, message, rowsAffected });
                }

                return Ok(new { success = false, message = "Data Not Saved", rowsAffected });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpDelete("delete")]
        public IActionResult DeleteVendorMaster([FromQuery] int id)
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
                    using (SqlCommand cmd = new SqlCommand("SP_Stores_VM_Delete", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
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
