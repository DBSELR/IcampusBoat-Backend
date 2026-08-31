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
    public class TimeTableController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("regulations")]
        public IActionResult GetRegulations()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "SELECT DISTINCT Regulation FROM tbl_Regulation";
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
                        cmd.Parameters.AddWithValue("@Status", "COURSE");
                        cmd.Parameters.AddWithValue("@Programme", "");
                        cmd.Parameters.AddWithValue("@AcdYr", acdYr ?? (object)DBNull.Value);

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
                        cmd.Parameters.AddWithValue("@Status", "BRANCH");
                        cmd.Parameters.AddWithValue("@Programme", programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcdYr", acdYr ?? (object)DBNull.Value);

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
                        cmd.Parameters.AddWithValue("@Course", programme ?? (object)DBNull.Value);
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
        public IActionResult GetSections([FromBody] SectionFilterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Filter request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = @"SELECT DISTINCT Section FROM tbl_sectionmaster 
                                     WHERE academicyear = @AcdYr AND CourseCode = @Programme AND BranchCode = @Branch AND STDYEAR = @Year AND SEMESTER = @Semister";
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
        [HttpPost("lecturers")]
        public IActionResult GetLecturers([FromBody] LecturerFilterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Filter request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = @"SELECT FName FName1, EmpID + '- ' + FName + ' (' + Designation + ')' AS Fname, EmpID 
                                     FROM tbl_EmployeeDetails 
                                     WHERE EMPID IN (
                                         SELECT EMPID FROM TBL_FACULTY 
                                         WHERE CourseCode = @Programme AND [YEAR] = @Year AND Semester = @Semister AND SubjectCode = @Subcode
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
        public IActionResult GetSubjects([FromBody] SubjectFilterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Filter request is required." });
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
        [HttpPost("timings")]
        public IActionResult GetTimings([FromBody] TimingsFilterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Filter request is required." });
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
        [HttpPost("view")]
        public IActionResult ViewTimeTable([FromBody] TimeTableSearchRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Search request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TIMETABLE_VIEW", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@YEAR", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEM", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STREAM", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEC", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SHITF", request.ShiftNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", request.AcdYr ?? (object)DBNull.Value);

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
        [HttpPost("check-count")]
        public IActionResult CheckCount([FromBody] CheckCountRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Check count request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = @"SELECT count(*) COUNT FROM TBL_TIMETABLE 
                                     WHERE [SHIFT]=@ShiftNo AND [DAY]=@Day AND programme=@Programme AND BRANCH=@Branch AND [YEAR]=@Year 
                                     AND SEMISTER=@Semister AND SECTION=@Section AND STREAM=@Stream AND Period=@Period 
                                     AND SUB_CODE=@Subcode AND Lecturer=@Lecturer AND AcademicYear=@AcademicYear";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ShiftNo", request.ShiftNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Day", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Stream", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period", request.Period ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Subcode", request.Subcode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);

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
        public IActionResult SaveTimeTable([FromBody] TimeTableSaveRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Save request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TIMETABLE_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", request.Id ?? "0");
                        cmd.Parameters.AddWithValue("@SHIFT", request.ShiftNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DAY", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMISTER", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Stream", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SECTION", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PERIOD", request.Period ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUBJECT", request.Subject ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DEPARTMENT", request.Department ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@LECTURER", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SPTime", request.SPTime ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period_type", request.PeriodType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Toperiod", request.Toperiod ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FRM_TO_PERIODS", request.Frrom_To_Periods ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUB_CODE", request.Subcode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EPTime", request.EPTime ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@status", request.LecStatus ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EFRMDATE", request.EFRMDATE ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@subtype", request.Subtype ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "TimeTable saved successfully.", affectedRows = rows });
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
        public IActionResult DeleteTimeTable([FromBody] TimeTableDeleteRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Delete request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("PROC_DELETE_TIMETABLE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SHIFT", request.ShiftNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DAY", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMISTER", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SECTION", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STREAM", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FRM_TO_PERIODS", request.Frrom_To_Periods ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TYPE", request.Proc_type ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@WDATE", request.Wdate ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "TimeTable entry deleted successfully.", affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("sub-type")]
        public IActionResult GetSubType([FromBody] SubTypeFilterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "SubType request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_SUBTYPE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SUBCODE", request.Subcode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ADATE", request.EFRMDATE ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADAMICYEAR", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEM", request.Semister ?? (object)DBNull.Value);

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
        [HttpPost("report")]
        public IActionResult GetTimeTableReport([FromBody] TimeTableReportRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Report request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SPR_Timetable", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Course", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Stream", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Sem", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Sec", request.Section ?? (object)DBNull.Value);

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
        [HttpGet("faculty-report")]
        public IActionResult GetFacultyTimeTableReport([FromQuery] string? userId, [FromQuery] string? programme)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SPR_Fac_Timetable", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Faculty", userId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Sem", programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Accyr", programme ?? (object)DBNull.Value);

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

