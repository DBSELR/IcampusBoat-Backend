using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using IcampusBoatBackend.Models.Attendance;

namespace IcampusBoatBackend.Controllers.Attendance
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class BatchDeleteController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("programmes")]
        public IActionResult GetProgrammes([FromQuery] string? acdYr)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_Programme_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", acdYr ?? (object)DBNull.Value);
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
        public IActionResult GetBranches([FromQuery] string? programme, [FromQuery] string? acdYr)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_Branch_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@COURSE", programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", acdYr ?? (object)DBNull.Value);
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
        [HttpPost("sections")]
        public IActionResult GetSections([FromBody] BatchDeleteSectionRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Section search request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = @"select DISTINCT Section from tbl_sectionmaster 
                                     where AcademicYear=@AcademicYear and CourseCode=@Programme and StdYear=@SYear and Semester=@Semester and BranchCode=@Branch";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
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
        [HttpPost("period-load")]
        public IActionResult GetPeriodLoad([FromBody] BatchDeletePeriodLoadRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Period load request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_Batch_PERIODLOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Shift", string.IsNullOrEmpty(request.Shift) ? "1" : request.Shift);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Sem", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Stream", string.IsNullOrEmpty(request.Stream) ? "1" : request.Stream);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Day", request.Day ?? (object)DBNull.Value);
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
        [HttpPost("faculty")]
        public IActionResult GetFaculty([FromBody] BatchDeleteFacultyRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Faculty search request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_Get_Faculty", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcadamicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Course", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Day", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period", request.Period ?? (object)DBNull.Value);
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
        [HttpPost("batch-students")]
        public IActionResult GetBatchStudents([FromBody] BatchDeleteStudentsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Batch students request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_Get_Batch_Students", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcadamicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Course", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Faculty", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Day", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period", request.Period ?? (object)DBNull.Value);
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
        [HttpPost("delete-batch")]
        public IActionResult DeleteBatch([FromBody] BatchDeleteExecuteRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Batch delete request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Sp_Delete_Batch_Students", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Day", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcadamicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Course", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Faculty", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@userid", request.UserId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period", request.Period ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Batch deleted successfully.", affectedRows = rows });
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
