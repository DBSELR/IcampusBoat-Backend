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
    public class Admin_PermissionsController : ControllerBase
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
                    using (SqlCommand cmd = new SqlCommand("SP_MarksEntry_Programme_List", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Academicyear", acdYr ?? (object)DBNull.Value);
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
        [HttpGet("branches")]
        public IActionResult GetBranches([FromQuery] string? programme, [FromQuery] string? acdYr)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_SubjectMaster_Branch_Load", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Course", programme ?? (object)DBNull.Value);
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
        [HttpPost("sections")]
        public IActionResult GetSections([FromBody] AdminPermissionsSectionRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Section request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = @"select distinct Section from tbl_sectionmaster 
                                     where CourseCode=@Programme and BranchCode=@Branch and StdYear=@SYear and AcademicYear=@AcdYr";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcdYr", request.AcdYr ?? (object)DBNull.Value);
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
        [HttpPost("no-attendance-lecturers")]
        public IActionResult GetNoAttendanceLecturers([FromBody] NoAttendanceLecturersRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Lecturer list request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_LOAD_NOATTEMPLOYEE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@day", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@semister", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADAMICYEAR", request.AcdYr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DATE", request.Date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToDATE", request.ToDate ?? (object)DBNull.Value);
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
        [HttpPost("faculty-grid")]
        public IActionResult GetFacultyGrid([FromBody] FacultyGridRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Faculty grid request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_FACGRID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SECTION", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMESTER", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DATE", request.Date ?? (object)DBNull.Value);
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
        [HttpPost("save-permissions")]
        public IActionResult SaveAdminPermissionsBatch([FromBody] SaveAdminPermissionsBatchRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Permissions batch request is required." });
            }

            try
            {
                DateTime fromDt = DateTime.Parse(request.FromDate ?? DateTime.Today.ToString("yyyy-MM-dd"));
                DateTime toDt = DateTime.Parse(request.ToDate ?? fromDt.ToString("yyyy-MM-dd"));
                double noDays = (toDt - fromDt).TotalDays;

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    int totalSaved = 0;

                    for (int v = 0; v <= noDays; v++)
                    {
                        DateTime currentDate = fromDt.AddDays(v);
                        string currentDateStr = currentDate.ToString("yyyy-MM-dd");

                        if (request.Permissions != null)
                        {
                            foreach (var item in request.Permissions)
                            {
                                if (item.Granted)
                                {
                                    string lecturerCode = item.Lecturer ?? "";
                                    if (lecturerCode.Contains("-"))
                                    {
                                        lecturerCode = lecturerCode.Split('-')[0].Trim();
                                    }

                                    using (SqlCommand cmd = new SqlCommand("SP_SAVEADMIN_PERMISSIONS", con))
                                    {
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                                        cmd.Parameters.AddWithValue("@YEAR", request.SYear ?? (object)DBNull.Value);
                                        cmd.Parameters.AddWithValue("@section", string.IsNullOrEmpty(request.Section) ? "0" : request.Section);
                                        cmd.Parameters.AddWithValue("@semister", request.Semester ?? (object)DBNull.Value);
                                        cmd.Parameters.AddWithValue("@LECTURER", lecturerCode);
                                        cmd.Parameters.AddWithValue("@SUB_CODE", item.Subject ?? (object)DBNull.Value);
                                        cmd.Parameters.AddWithValue("@DATE", currentDateStr);
                                        cmd.Parameters.AddWithValue("@ACADAMICYEAR", request.AcdYr ?? (object)DBNull.Value);

                                        totalSaved += cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }

                    return Ok(new { success = true, message = "Admin permissions saved successfully.", totalSaved });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
