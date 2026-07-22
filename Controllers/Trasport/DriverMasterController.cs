using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using IcampusBoatBackend.Models.Trasport;

namespace IcampusBoatBackend.Controllers.Trasport
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class DriverMasterController : ControllerBase
    {
        private static string FormatDateForSql(string? inputDate)
        {
            if (string.IsNullOrWhiteSpace(inputDate))
                return string.Empty;

            if (DateTime.TryParse(inputDate, out DateTime parsedDate))
            {
                return parsedDate.ToString("yyyy-MM-dd");
            }
            return inputDate;
        }

        [HttpGet("list")]
        public IActionResult GetDriverList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_DRIVERMASTER_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("load-drivers-dropdown")]
        public IActionResult LoadDriversDropdown()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_LoadDRNAMEID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("employee-details/{empId}")]
        public IActionResult GetEmployeeDetails(string empId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SELECT Birthdate, Doj, MobileNo, [Address] FROM tbl_EmployeeDetails WHERE EmpID = @EmpID", con))
                    {
                        cmd.Parameters.AddWithValue("@EmpID", empId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        return Ok(new
                        {
                            success = true,
                            data = new
                            {
                                birthDate = row["Birthdate"] != DBNull.Value ? Convert.ToDateTime(row["Birthdate"]).ToString("dd-MM-yyyy") : "",
                                doj = row["Doj"] != DBNull.Value ? Convert.ToDateTime(row["Doj"]).ToString("dd-MM-yyyy") : "",
                                mobileNo = row["MobileNo"]?.ToString() ?? "",
                                address = row["Address"]?.ToString() ?? ""
                            }
                        });
                    }

                    return Ok(new { success = false, message = "Employee not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("save")]
        public IActionResult SaveDriver([FromBody] DriverMasterSaveRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.DriverName))
            {
                return BadRequest(new { success = false, message = "Driver Name is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    bool isInsert = string.IsNullOrWhiteSpace(request.Id) || request.Id == "0";

                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_DRIVERMASTER_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", isInsert ? "0" : request.Id);
                        cmd.Parameters.AddWithValue("@DRIVERID", request.DriverId ?? request.DriverName);
                        cmd.Parameters.AddWithValue("@DRIVERNAME", request.DriverName);
                        cmd.Parameters.AddWithValue("@DOB", FormatDateForSql(request.Dob));
                        cmd.Parameters.AddWithValue("@CONTACTNO", request.ContactNo ?? "");
                        cmd.Parameters.AddWithValue("@REFERENCENAME", request.ReferenceName ?? "");
                        cmd.Parameters.AddWithValue("@ADDRESS", request.Address ?? "");
                        cmd.Parameters.AddWithValue("@DOJ", FormatDateForSql(request.Doj));
                        cmd.Parameters.AddWithValue("@LICENCENO", request.LicenceNo ?? "");
                        cmd.Parameters.AddWithValue("@LICENCEEXPIREDATE", FormatDateForSql(request.LicenceExpireDate));
                        cmd.Parameters.AddWithValue("@NOOfYEARSEXPERIANCE", request.NoOfYearsExperience);
                        cmd.Parameters.AddWithValue("@REFERENCECONTACTNO", request.ReferenceContactNo ?? "");
                        cmd.Parameters.AddWithValue("@UserId", request.UserId ?? "");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear);
                        cmd.Parameters.AddWithValue("@FinancialYear", request.FinancialYear);
                        cmd.Parameters.AddWithValue("@isactive", request.IsActive ? 1 : 0);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                success = true,
                                message = isInsert ? "Data Saved Successfully" : "Data Updated Successfully"
                            });
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Unable to save driver details." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteDriver(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { success = false, message = "Driver ID is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_DRIVERMASTER_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", id);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        return Ok(new
                        {
                            success = rowsAffected > 0,
                            message = rowsAffected > 0 ? "Record Deleted Successfully" : "No record deleted."
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("search")]
        public IActionResult SearchDrivers([FromQuery] string searchName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_SEARCHBY_DRIVERID_DRIVERNAME", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SEARCHNAME", searchName ?? "");
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
