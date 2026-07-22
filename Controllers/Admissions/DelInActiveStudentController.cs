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
    public class DelInActiveStudentController : ControllerBase
    {
        /// <summary>
        /// Fetch list of programmes for selected academic year.
        /// </summary>
        [HttpGet("programmes")]
        public IActionResult GetProgrammes([FromQuery] string academicYear)
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
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_MarksEntry_Programme_List", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Academicyear", academicYear);
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
        /// Fetch years for selected programme and academic year.
        /// </summary>
        [HttpGet("years")]
        public IActionResult GetYears([FromQuery] string programme, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(programme) || string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Programme and academic year are required." });
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
                        cmd.Parameters.AddWithValue("@Course", programme);
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
        /// Fetch branches for selected programme and academic year.
        /// </summary>
        [HttpGet("branches")]
        public IActionResult GetBranches([FromQuery] string programme, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(programme) || string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Programme and academic year are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_SubjectMaster_Branch_Load", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Course", programme);
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
        /// Fetch sections for selected programme, branch, and studying year.
        /// </summary>
        [HttpGet("sections")]
        public IActionResult GetSections([FromQuery] string programme, [FromQuery] string branch, [FromQuery] string syear)
        {
            if (string.IsNullOrWhiteSpace(programme) || string.IsNullOrWhiteSpace(branch) || string.IsNullOrWhiteSpace(syear))
            {
                return BadRequest(new { success = false, message = "Programme, branch, and syear are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_ATTSEC", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Programme", programme);
                        cmd.Parameters.AddWithValue("@BranchCode", branch);
                        cmd.Parameters.AddWithValue("@StdYear", syear);
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
        /// Fetch distinct inactive statuses for selected filters.
        /// </summary>
        [HttpGet("statuses")]
        public IActionResult GetStatuses([FromQuery] string programme, [FromQuery] string branch, [FromQuery] string syear)
        {
            if (string.IsNullOrWhiteSpace(programme) || string.IsNullOrWhiteSpace(branch) || string.IsNullOrWhiteSpace(syear))
            {
                return BadRequest(new { success = false, message = "Programme, branch, and syear are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_STATUS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CourseCode", programme);
                        cmd.Parameters.AddWithValue("@BranchCode ", branch);
                        cmd.Parameters.AddWithValue("@SYear", syear);
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
        /// Fetch list of inactive students based on filters.
        /// </summary>
        [HttpGet("students")]
        public IActionResult GetStudents([FromQuery] string programme, [FromQuery] string branch, [FromQuery] string syear, [FromQuery] string semester, [FromQuery] string section, [FromQuery] string academicYear, [FromQuery] string? status = "")
        {
            if (string.IsNullOrWhiteSpace(programme) || string.IsNullOrWhiteSpace(branch) || string.IsNullOrWhiteSpace(syear) || string.IsNullOrWhiteSpace(semester) || string.IsNullOrWhiteSpace(section) || string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "All parameters (programme, branch, syear, semester, section, academicYear) are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_InActive_Students", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Programme", programme);
                        cmd.Parameters.AddWithValue("@Branch", branch);
                        cmd.Parameters.AddWithValue("@Year", syear);
                        cmd.Parameters.AddWithValue("@SEM", semester);
                        cmd.Parameters.AddWithValue("@SECTION", section);
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        cmd.Parameters.AddWithValue("@status", status ?? "");

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
        /// Delete inactive students in bulk.
        /// </summary>
        [HttpPost("delete")]
        public IActionResult DeleteInactiveStudents([FromBody] DelInActiveStudentDeleteRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Section) || request.RegNos == null || request.RegNos.Count == 0)
            {
                return BadRequest(new { success = false, message = "Section and selected RegNos list are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // Join selected registration numbers with commas (e.g. RegNo1,RegNo2,RegNo3)
                    string delStudentsCsv = string.Join(",", request.RegNos);

                    using (SqlCommand cmd = new SqlCommand("SP_Del_Students", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SECTION", request.Section);
                        cmd.Parameters.AddWithValue("@Del_Students", delStudentsCsv);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? "");
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? "");
                        cmd.Parameters.AddWithValue("@Year", request.SYear ?? "0");
                        cmd.Parameters.AddWithValue("@SEM", request.Semester ?? "0");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@UserId", request.UserId ?? "");
                        cmd.Parameters.AddWithValue("@status", request.Status ?? "");
                        cmd.Parameters.AddWithValue("@DeletedId", request.UserId ?? "");

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                success = true,
                                message = "InActive Students Deleted successfully"
                            });
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Unable to delete selected inactive students." });
                        }
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
