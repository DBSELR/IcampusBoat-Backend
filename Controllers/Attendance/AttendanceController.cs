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
        public IActionResult SaveAttendanceSubjectWise([FromBody] SaveAttendanceSubWiseRequest request)
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
                    using (SqlCommand cmd = new SqlCommand("sp_Attendance_PapWise_Save", con))
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
                        cmd.Parameters.AddWithValue("@UpdateDate", request.UpdateDate ?? "NULL");
                        cmd.Parameters.AddWithValue("@minRoll", request.SrNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@maxRoll", request.ErNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DayTaught", request.DayTaught ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AttStat", request.AttStat ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@QUERY", request.Query ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period_Range", request.PeriodRange ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TeachingLearningMethod", request.TLM ?? (object)DBNull.Value);

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
    }
}

