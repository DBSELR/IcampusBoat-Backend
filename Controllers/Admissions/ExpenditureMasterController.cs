using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using IcampusBoatBackend.Models.Admissions;

namespace IcampusBoatBackend.Controllers.Admissions
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenditureMasterController : ControllerBase
    {
        /// <summary>
        /// Initial load API returning programme list and the list of existing expenditure heads.
        /// </summary>
        [HttpGet("load")]
        public IActionResult Load([FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Academic year is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // 1. Get Program List
                    DataTable dtPrograms = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_Programme_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtPrograms);
                        }
                    }

                    // 2. Load Expenditure Heads Grid Data
                    DataTable dtList = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_EXE_Master_Load", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtList);
                        }
                    }

                    return Ok(new
                    {
                        success = true,
                        message = "Success",
                        data = new
                        {
                            courses = DAL.DataTableToList(dtPrograms),
                            expHeadsList = DAL.DataTableToList(dtList)
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get years for selected course.
        /// </summary>
        [HttpGet("years")]
        public IActionResult GetYears([FromQuery] string courseCode, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Course code and academic year are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_YEARS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Course", courseCode);
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
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

        /// <summary>
        /// Fetch amount and ID of a given expenditure head for selected course and year.
        /// </summary>
        [HttpGet("amount")]
        public IActionResult GetAmount([FromQuery] string expenditureHeads, [FromQuery] string courseCode, [FromQuery] string year)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    string query = @"SELECT id, Amount FROM tbl_Expenditure_Master 
                                     WHERE ExpenditureHeads = @Heads 
                                       AND CourseCode = @Course 
                                       AND Year = @Year";

                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Heads", expenditureHeads ?? "");
                        cmd.Parameters.AddWithValue("@Course", courseCode ?? "");
                        cmd.Parameters.AddWithValue("@Year", year ?? "");

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        return Ok(new
                        {
                            success = true,
                            id = dt.Rows[0]["id"]?.ToString() ?? "0",
                            amount = dt.Rows[0]["Amount"]?.ToString() ?? ""
                        });
                    }
                    else
                    {
                        return Ok(new { success = true, id = "0", amount = "" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Save or Update Expenditure Head details.
        /// </summary>
        [HttpPost("save")]
        public IActionResult SaveExpenditureHead([FromBody] ExpenditureMasterSaveRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Course))
            {
                return BadRequest(new { success = false, message = "Course/programme is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    string id = string.IsNullOrWhiteSpace(request.Id) ? "0" : request.Id;

                    using (SqlCommand cmd = new SqlCommand("SP_EXPHEADS_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Course", request.Course);
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? "0");
                        cmd.Parameters.AddWithValue("@EXPHEADS", request.ExpenditureHeads ?? "");
                        cmd.Parameters.AddWithValue("@Amount", request.Amount ?? "0");

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                success = true,
                                message = id == "0" ? "Data Saved Successfully" : "Data Updated Successfully"
                            });
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Unable to save expenditure head details." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete expenditure head by ID.
        /// </summary>
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteExpenditureHead(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { success = false, message = "ID is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    string query = "DELETE FROM tbl_Expenditure_Master WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
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
    }
}
