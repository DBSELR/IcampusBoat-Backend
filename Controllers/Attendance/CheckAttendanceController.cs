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
    public class CheckAttendanceController : ControllerBase
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
        public IActionResult GetSections([FromBody] CheckAttendanceSectionRequest request)
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
        [HttpPost("periods")]
        public IActionResult GetPeriods([FromBody] CheckAttendancePeriodLoadRequest request)
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
        [HttpPost("validate-date")]
        public IActionResult ValidateDate([FromBody] CheckDateValidityRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Date validity request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // Step 1: Check Valid Date range
                    DataTable validDt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_VALIDDATE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSE", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@sem", request.Semester ?? (object)DBNull.Value);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(validDt);
                        }
                    }

                    // Step 2: Check Holidays
                    DataTable holidayDt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ATTENDANCE_CHECK_HOLIDAYS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DATE", request.Date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcdYr", request.AcademicYear ?? (object)DBNull.Value);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(holidayDt);
                        }
                    }

                    bool isHoliday = holidayDt.Rows.Count > 0;
                    string holidayRemark = isHoliday ? holidayDt.Rows[0]["Remark"].ToString() : "";

                    return Ok(new
                    {
                        success = true,
                        validDates = DAL.DataTableToList(validDt),
                        isHoliday,
                        holidayRemark
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("students")]
        public IActionResult GetStudentAttendanceRoster([FromBody] CheckAttendanceStudentsRequest request)
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

                    string periodVal = request.Period ?? "";
                    if (!request.IsPractical && periodVal != "99" && periodVal.Length <= 2)
                    {
                        periodVal = periodVal + "," + periodVal;
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
    }
}
