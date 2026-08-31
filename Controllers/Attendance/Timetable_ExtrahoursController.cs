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
    public class Timetable_ExtrahoursController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("courses")]
        public IActionResult GetCourses([FromQuery] string? acdYr)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_Timetable_COURSEBRANCH_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@STATUS", "COURSE");
                        cmd.Parameters.AddWithValue("@COURSE", "");
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
                    using (SqlCommand cmd = new SqlCommand("SP_Timetable_COURSEBRANCH_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@STATUS", "BRANCH");
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
        [HttpGet("departments")]
        public IActionResult GetDepartments()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_Timetable_Dept_Load", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
        [HttpGet("periods")]
        public IActionResult GetPeriods([FromQuery] string? programme, [FromQuery] string? year)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_PERIODLOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Programme", programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PSYear", year ?? (object)DBNull.Value);
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
        [HttpGet("faculty-dept")]
        public IActionResult GetFacultyDepartment([FromQuery] string? lecturer)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "SELECT DEPT FROM tbl_EmployeeDetails WHERE EMPID = @Lecturer";
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
        [HttpDelete("delete-extra/{id}")]
        public IActionResult DeleteTimeTableExtra(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    string query = "DELETE FROM [TBL_TIMETABLE_EXTRA] WHERE ID = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Extra timetable entry deleted successfully.", affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpPost("sections")]
        public IActionResult GetSections([FromBody] ExtraSectionSearchRequest request)
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
                    string query = @"select distinct Section from tbl_sectionmaster 
                                     where academicyear = @AcdYr and CourseCode = @Programme and BranchCode = @Branch and STDYEAR = @Year and SEMESTER = @Semister";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AcdYr", request.AcdYr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semister ?? (object)DBNull.Value);
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
        public IActionResult GetLecturers([FromBody] ExtraLecturerSearchRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Lecturer search request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = @"Select FName FName1, EmpID+'- ' + FName +' (' + Designation +')' as Fname, EmpID 
                                     from tbl_EmployeeDetails WHERE EMPID IN (
                                         SELECT EMPID FROM TBL_FACULTY WHERE CourseCode = @Programme AND [YEAR] = @Year AND Semester = @Semister AND SubjectCode = @Subcode
                                     ) ORDER BY FNAME1";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Subcode", request.Subcode ?? (object)DBNull.Value);
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
        [HttpPost("subjects")]
        public IActionResult GetSubjects([FromBody] ExtraSubjectLoadRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Subject load request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_Timetable_Subject_Load", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcdYr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semester", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Stream", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PeriodType", request.PeriodType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmpID", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Regu", request.Regu ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@subtype", request.Subtype ?? (object)DBNull.Value);
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
        [HttpPost("timings")]
        public IActionResult GetTimings([FromBody] ExtraTimingsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Timings request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("PROC_TIME_GET", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SHIFT", request.ShiftNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PERIOD", request.Period ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Toperiod", request.Toperiod ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@year", request.Year ?? (object)DBNull.Value);
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
        [HttpPost("view-extra")]
        public IActionResult ViewTimeTableExtra([FromBody] TimeTableExtraViewRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Extra view request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TIMETABLE_VIEW_EXTRA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@YEAR", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEM", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Stream", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEC", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ShiftNo", request.ShiftNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
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
        [HttpPost("save-extra")]
        public IActionResult SaveTimeTableExtra([FromBody] TimeTableExtraSaveRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Extra save request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TIMETABLE_SAVE_EXTRA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", request.Id ?? "0");
                        cmd.Parameters.AddWithValue("@EDATE", request.Wdate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SHIFT", request.ShiftNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DAY", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMISTER", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STREAM", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SECTION", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PERIOD", "99");
                        cmd.Parameters.AddWithValue("@SUBJECT", request.Subject ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DEPARTMENT", request.Department ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@LECTURER", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SPTIME", request.SPTime ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PERIOD_TYPE", request.PeriodType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TOPERIOD", "99");
                        cmd.Parameters.AddWithValue("@FRM_TO_PERIODS", "99");
                        cmd.Parameters.AddWithValue("@SUB_CODE", request.Subcode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EPTIME", request.EPTime ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Extra timetable entry saved successfully.", affectedRows = rows });
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
