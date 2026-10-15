using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using IcampusBoatBackend.Models.Settings;

namespace IcampusBoatBackend.Controllers.Settings
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class Faculty_FeedBackController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("programmes")]
        public IActionResult GetProgrammes([FromQuery] string? academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_FacFeedBckProgramme_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? (object)DBNull.Value);
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

        [AllowAnonymous]
        [HttpGet("branches")]
        public IActionResult GetBranches([FromQuery] string? programme, [FromQuery] string? academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_FacFeedBck_Branch_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@COURSE", programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? (object)DBNull.Value);
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

        [AllowAnonymous]
        [HttpGet("years")]
        public IActionResult GetYears([FromQuery] string? programme, [FromQuery] string? academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_YEARS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@COURSE", programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? (object)DBNull.Value);
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

        [AllowAnonymous]
        [HttpGet("grid")]
        public IActionResult GetGrid([FromQuery] string? academicYear, [FromQuery] string? programme)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP__Faculty_FeedBack_Grid", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Acyr", academicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@programme", string.IsNullOrWhiteSpace(programme) ? (object)DBNull.Value : programme);
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

        [AllowAnonymous]
        [HttpPost("save")]
        public IActionResult SaveFacultyFeedBack([FromBody] SaveFacultyFeedBackRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Setting_Faculty_Feeback_Save", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", request.Id ?? "");
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STDYEAR", request.StdYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMESTER", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Feedback", request.FeedBck ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dt", request.Dt ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToDt", request.ToDt ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        string msg = string.IsNullOrEmpty(request.Id) ? "Data Saved Successfully" : "Data Updated Successfully";
                        return Ok(new { success = true, message = msg, affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("delete")]
        public IActionResult DeleteFacultyFeedBack([FromQuery] string? id)
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
                    using (SqlCommand cmd = new SqlCommand("SP_Setting_FacultyFeedbck_Delete", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Record Deleted Successfully", affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("date-details")]
        public IActionResult GetDateDetails([FromQuery] string? academicYear, [FromQuery] string? dt)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dtResult = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TextChange_Faculty_FeedBack", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dt", dt ?? (object)DBNull.Value);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtResult);
                        }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dtResult) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("search")]
        public IActionResult SearchFacultyFeedBack([FromQuery] string? searchName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_SEARCHBY_Faculty_FeedBack", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SEARCHNAME", searchName ?? (object)DBNull.Value);
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
