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
    public class Admin_AttendanceController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("current-acyr")]
        public IActionResult GetCurrentAcyr()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "select AcademicYear from tbl_AcademicYear where ISACTIVE = 'y'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
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
        [HttpGet("branch-list")]
        public IActionResult GetBranchListByFaculty([FromQuery] string? lecturer)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "Select Programme,Branch from TBL_TIMETABLE where Lecturer=@Lecturer";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Lecturer", lecturer ?? (object)DBNull.Value);
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
        [HttpGet("tlm")]
        public IActionResult GetTLM([FromQuery] string? academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "select TLMCode+'-'+TLMName as TLM from tbl_TeachingLearningMethods where AcYr=@AcademicYear";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
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
        public IActionResult GetSections([FromBody] AdminSectionSearchRequest request)
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
        [HttpPost("lecturers")]
        public IActionResult GetLecturers([FromBody] LecturerLoadRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Lecturer load request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string spName = string.IsNullOrEmpty(request.SubType) ? "SP_ATT_FACULTY_LIST" : "SP_ATT_FACULTY_LIST_MH";

                    using (SqlCommand cmd = new SqlCommand(spName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@@AcademicYear", request.AcdYr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Shift", request.Shift ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ADATE", request.Date ?? (object)DBNull.Value);
                        if (!string.IsNullOrEmpty(request.SubType))
                        {
                            cmd.Parameters.AddWithValue("@SUBTYPE", request.SubType);
                        }

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
        [HttpPost("periods")]
        public IActionResult GetPeriods([FromBody] AdminPeriodLoadRequest request)
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
                    using (SqlCommand cmd = new SqlCommand("Proc_Attendance_Period_Load", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcdYr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Shift", request.Shift ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Sem", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ADATE", request.Date ?? (object)DBNull.Value);
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
        [HttpPost("students")]
        public IActionResult GetStudents([FromBody] AdminLoadStudentsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Student roster request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string spName = "Sp_Attendance_Students_Load";
                    if (request.IsPractical)
                    {
                        spName = "Sp_Attendance_Practical_Students_Load";
                    }
                    else if (request.Period == "99")
                    {
                        spName = "PROC_ATT_EXTRAHOURS_GET";
                    }

                    using (SqlCommand cmd = new SqlCommand(spName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EDATE", request.Date ?? (object)DBNull.Value);

                        if (spName == "Sp_Attendance_Practical_Students_Load")
                        {
                            cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Period", request.Period ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ADATE", request.Date ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@AcadamicYear", request.AcademicYear ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@SRegdno", request.SrNo ?? "0");
                            cmd.Parameters.AddWithValue("@ERegdno", request.ErNo ?? "0");
                        }
                        else if (spName == "Sp_Attendance_Students_Load")
                        {
                            cmd.Parameters.AddWithValue("@LECTURER", request.Lecturer ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@PERIOD", request.Period ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ADATE", request.Date ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ACADAMICYEAR", request.AcademicYear ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@YEAR", request.SYear ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@SEM", request.Semester ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@SEC", request.Section ?? (object)DBNull.Value);
                        }

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
        [HttpPost("save-subjectwise")]
        public IActionResult SaveAdminAttendanceSubjectWise([FromBody] SaveAdminAttendanceSubWiseRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Attendance save request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(" ", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FacultyID", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FacultyName", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Class", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@grpID", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Peroid", request.Period ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@aSubject", request.Subjects ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcadamicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@aDay", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@tDate", request.Date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@minRoll", request.SrNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@maxRoll", request.ErNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DayTaught", request.DayTaught ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AttStat", request.AttStat ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@QUERY", request.Query ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period_range", request.PeriodRange ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TeachingLearningMethod", request.TLM ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Admin subject-wise attendance saved successfully.", affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("save-permissions")]
        public IActionResult SaveAdminPermissions([FromBody] SaveAdminPermissionsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Permissions request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_SAVEADMIN_PERMISSIONS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@semister", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@LECTURER", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUB_CODE", request.Subject ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DATE", request.Date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADAMICYEAR", request.AcdYr ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Admin permission saved successfully.", affectedRows = rows });
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

