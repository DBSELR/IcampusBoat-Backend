using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using IcampusBoatBackend.Models.Attendance;

namespace IcampusBoatBackend.Controllers.Attendance
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
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
        public IActionResult GetSections([FromBody] SectionSearchRequest request)
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
                    using (SqlCommand cmd = new SqlCommand("SP_GET_ATTSEC", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BranchCode", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@StdYear", request.SYear ?? (object)DBNull.Value);
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
                    using (SqlCommand cmd = new SqlCommand("SP_GET_TLM", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACYR", academicYear ?? (object)DBNull.Value);
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
        [HttpGet("check-holidays")]
        public IActionResult CheckHolidays([FromQuery] string? acdYr, [FromQuery] string? date)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_HOLIDAY", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", acdYr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@holidaydate", date ?? (object)DBNull.Value);
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
        public IActionResult GetPeriods([FromBody] PeriodLoadRequest request)
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
        [HttpPost("check-mid-dates")]
        public IActionResult CheckMidDates([FromBody] MidAttDatesRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Mid dates request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_CHECKMID_DATES", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACYR", request.AcdYr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYEAR", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEM", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PERIOD", request.Period ?? (object)DBNull.Value);
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
        public IActionResult GetStudents([FromBody] LoadStudentsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Student load request is required." });
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

                        if (spName == "Sp_Attendance_Practical_Students_Load")
                        {
                            cmd.Parameters.AddWithValue("@Period", request.Period ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ADATE", request.Date ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@AcadamicYear", request.AcademicYear ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@SRegdno", request.SrNo ?? "0");
                            cmd.Parameters.AddWithValue("@ERegdno", request.ErNo ?? "0");
                        }
                        else if (spName == "PROC_ATT_EXTRAHOURS_GET")
                        {
                            cmd.Parameters.AddWithValue("@ADATE", request.Date ?? (object)DBNull.Value);
                        }
                        else if (spName == "Sp_Attendance_Students_Load")
                        {
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
        public IActionResult SaveAttendanceSubjectWise([FromBody] SaveAttendanceSubWiseRequest request)
        { 
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Attendance save request is required." });
            }

            try
            {
                string queryStr = request.Query ?? "";
                if (string.IsNullOrWhiteSpace(queryStr) && request.Students != null && request.Students.Count > 0)
                {
                    queryStr = BuildQueryStringFromStudents(request.Students, request.SrNo, request.ErNo);
                }

                if (string.IsNullOrWhiteSpace(queryStr))
                {
                    return BadRequest(new { success = false, message = "Attendance query string or students array is required." });
                }

                string updateDate = string.IsNullOrWhiteSpace(request.UpdateDate) || request.UpdateDate.Equals("NULL", StringComparison.OrdinalIgnoreCase)
                    ? "NULL"
                    : $"'{request.UpdateDate}'";

                string sql = $"sp_Attendance_PapWise_Save '{request.Lecturer ?? ""}', '{request.Lecturer ?? ""}', '{request.Semester ?? ""}', '{request.Programme ?? ""}', '{request.Branch ?? ""}', '{request.SYear ?? ""}', '{request.Section ?? ""}', '{request.Period ?? ""}', '{request.Subjects ?? ""}', '{request.AcademicYear ?? ""}', '{request.Day ?? ""}', '{request.Date ?? ""}', {updateDate}, '{request.SrNo ?? ""}', '{request.ErNo ?? ""}', '{request.DayTaught ?? ""}', '{request.AttStat ?? ""}', {queryStr}, '{request.PeriodRange ?? ""}', '{request.TLM ?? ""}'";

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Subject-wise attendance saved successfully.", affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("check-admin-dates")]
        public IActionResult CheckAdminDates([FromBody] AdminDatesCheckRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Admin dates check request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("sp_CheckAdminDates", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADAMICYEAR", request.AcdYr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@date", request.Date ?? (object)DBNull.Value);
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
        [HttpPost("employee-periods")]
        public IActionResult GetEmployeePeriodsFromAtt([FromBody] EmpPeriodsAttRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Employee periods request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_GetPeriodsFromAttByEmp", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@aDate", request.Date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@aDay", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYEAR", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSE", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
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
        [HttpPost("day-periods")]
        public IActionResult LoadAttDayPeriods([FromBody] DayPeriodsLoadRequest request)
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
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Proc_Attendance_DayPeriods", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Lecturer", request.UserId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
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
        [HttpPost("students-att-stat")]
        public IActionResult LoadAttendanceStudentsAttStat([FromBody] AttStudentsAttStatRequest request)
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
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_Attendance_Students_AttStat", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@aDate", request.Date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@aDay", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYEAR", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSE", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period", request.Period ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Subject", request.Subjects ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
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
        [HttpPost("delete-faculty-att-stat")]
        public IActionResult DeleteAttendanceFacultyAttStat([FromBody] DeleteFacultyAttStatRequest request)
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
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_Attendance_Delete_Faculty_AttStat", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@aDate", request.Date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@aDay", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYEAR", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSE", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period", request.Period ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Subject", request.Subjects ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
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
        [HttpGet("period-status")]
        public IActionResult GetPeriodStatus([FromQuery] string? programme, [FromQuery] string? shift)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_PERIOD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PROGRAMME", programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ShiftNo", shift ?? (object)DBNull.Value);
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
        [HttpPost("posting-dates")]
        public IActionResult GetPostingDates([FromBody] PostingDatesRequest request)
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
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_OLDADATES", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACYR", request.AcdYr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@coursecode", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@syear", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@sem", request.Semester ?? (object)DBNull.Value);
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
        [HttpPost("adm-permissions")]
        public IActionResult GetAttAdmPermissions([FromBody] AttAdmPermissionsRequest request)
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
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("sp_get_Att_AdmPermissions", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Acyr", request.AcdYr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branchcode", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@year", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@sem", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Date", request.Date ?? (object)DBNull.Value);
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
        [HttpPost("valid-dates")]
        public IActionResult GetValidDate([FromBody] ValidDatesRequest request)
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
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_VALIDDATE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSE", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@sem", request.Semester ?? (object)DBNull.Value);
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

        private static string BuildQueryStringFromStudents(List<FacultyStudentAttendanceItem> students, string? srNoStr, string? erNoStr)
        {
            string[] regNo = new string[150];
            string[] attStatus = new string[150];
            string[] remarks = new string[150];

            for (int i = 0; i < 150; i++)
            {
                regNo[i] = "";
                attStatus[i] = "";
                remarks[i] = "";
            }

            int minSno = 1;
            int maxSno = students != null && students.Count > 0 ? students.Count : 1;

            if (int.TryParse(srNoStr, out int parsedSrNo) && parsedSrNo > 0)
            {
                minSno = parsedSrNo;
            }
            if (int.TryParse(erNoStr, out int parsedErNo) && parsedErNo >= minSno)
            {
                maxSno = parsedErNo;
            }

            if (students != null)
            {
                foreach (var st in students)
                {
                    int idx = st.SNo > 0 ? st.SNo : 0;
                    if (idx >= 0 && idx < 150)
                    {
                        regNo[idx] = st.RegNo ?? "";
                        attStatus[idx] = string.IsNullOrWhiteSpace(st.Status) ? "P" : st.Status.Trim();
                        remarks[idx] = st.Remarks ?? "";
                    }
                }
            }

            string Q = "";
            string n = "NULL";

            for (int k = 0; k < minSno - 1; k++)
            {
                if (!string.IsNullOrEmpty(regNo[k]))
                {
                    Q += "'" + regNo[k] + "', ";
                    Q += !string.IsNullOrEmpty(remarks[k]) ? "'" + attStatus[k] + "/" + remarks[k] + "', " : "'" + (string.IsNullOrEmpty(attStatus[k]) ? "P" : attStatus[k]) + "', ";
                }
                else
                {
                    Q += "NULL, " + n + ", ";
                }
            }

            for (int k = minSno; k <= maxSno && k <= 149; k++)
            {
                if (!string.IsNullOrEmpty(regNo[k]))
                {
                    Q += "'" + regNo[k] + "', ";
                    Q += !string.IsNullOrEmpty(remarks[k]) ? "'" + attStatus[k] + "/" + remarks[k] + "', " : "'" + (string.IsNullOrEmpty(attStatus[k]) ? "P" : attStatus[k]) + "', ";
                }
                else
                {
                    Q += "NULL, " + n + ", ";
                }
            }

            int startEmpty = Math.Max(maxSno + 1, minSno);
            for (int j = startEmpty; j <= 149; j++)
            {
                Q += "NULL, " + n + ", ";
            }

            if (!string.IsNullOrEmpty(regNo[149]))
            {
                Q += "'" + regNo[149] + "', ";
                Q += !string.IsNullOrEmpty(remarks[149]) ? "'" + attStatus[149] + "/" + remarks[149] + "'" : "'" + (string.IsNullOrEmpty(attStatus[149]) ? "P" : attStatus[149]) + "'";
            }
            else
            {
                Q += "NULL, " + n;
            }

            return Q;
        }
    }
}
