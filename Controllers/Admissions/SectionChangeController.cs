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
    public class SectionChangeController : ControllerBase
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
        /// Fetch students list matching current section details for potential section change.
        /// </summary>
        [HttpGet("students")]
        public IActionResult GetStudents([FromQuery] string programme, [FromQuery] string branch, [FromQuery] string syear, [FromQuery] string semester, [FromQuery] string section, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(programme) || string.IsNullOrWhiteSpace(branch) || string.IsNullOrWhiteSpace(syear) || string.IsNullOrWhiteSpace(semester) || string.IsNullOrWhiteSpace(section) || string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "All query parameters (programme, branch, syear, semester, section, academicYear) are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_SectionChange", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Programme", programme);
                        cmd.Parameters.AddWithValue("@Branch", branch);
                        cmd.Parameters.AddWithValue("@Year", syear);
                        cmd.Parameters.AddWithValue("@Sem", semester);
                        cmd.Parameters.AddWithValue("@SECTION", section);
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
        /// Update sections for selected students in bulk.
        /// </summary>
        [HttpPost("update")]
        public IActionResult UpdateSection([FromBody] SectionChangeUpdateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.NewSection) || request.RegNos == null || request.RegNos.Count == 0)
            {
                return BadRequest(new { success = false, message = "NewSection and selected RegNos list are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // Join selected registration numbers with commas (e.g. RegNo1,RegNo2,RegNo3)
                    string sectionChangeCsv = string.Join(",", request.RegNos);

                    using (SqlCommand cmd = new SqlCommand("SP_UpDate_Section", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SECTION", request.NewSection);
                        cmd.Parameters.AddWithValue("@UpdateSection", sectionChangeCsv);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? "");
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? "");
                        cmd.Parameters.AddWithValue("@Year", request.SYear ?? "0");
                        cmd.Parameters.AddWithValue("@SEM", request.Semester ?? "0");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@UserId", request.UserId ?? "");

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                success = true,
                                message = "Section UpDated successfully"
                            });
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Unable to update student sections. Check if students exist under query parameters." });
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
