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
    public class AddAttendanceController : ControllerBase
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
        [HttpGet("verify-otp")]
        public IActionResult VerifyOtp([FromQuery] string enteredOtp, [FromQuery] string generatedOtp)
        {
            if (string.IsNullOrEmpty(enteredOtp) || string.IsNullOrEmpty(generatedOtp))
            {
                return BadRequest(new { success = false, message = "OTP is required." });
            }

            if (enteredOtp.Trim() == generatedOtp.Trim())
            {
                return Ok(new { success = true, message = "OTP verified successfully." });
            }
            else
            {
                return BadRequest(new { success = false, message = "Invalid OTP entered." });
            }
        }

        [AllowAnonymous]
        [HttpPost("sections")]
        public IActionResult GetSections([FromBody] AddAttSectionRequest request)
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
                                     where CourseCode=@Programme and BranchCode=@Branch and StdYear=@Year and Semester=@Semister and AcademicYear=@AcdYr";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semister ?? (object)DBNull.Value);
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
        [HttpPost("attendance-list")]
        public IActionResult GetAttendanceList([FromBody] LoadAddAttRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Attendance list request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GetATTByBetwnPercent", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SEMISTER", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CLASS", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@GRPID", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYEAR", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SECTION", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADAMICYEAR", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FrmPercent", request.FPerc ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToPercent", request.TPerc ?? (object)DBNull.Value);
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
        [HttpPost("student-abs-subjects")]
        public IActionResult GetStudentAbsSubjects([FromBody] LoadStudentAbsSubjectsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Student abs subjects request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ATTABSENT_New", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEM", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEC", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@REGNO", request.Regno ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUBJECT", request.Subcode ?? "");
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
        [HttpPost("student-absent-details")]
        public IActionResult GetStudentAbsentDetails([FromBody] LoadStudentAbsDataRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Student absent details request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ATTABSENT", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEM", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEC", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@REGNO", request.Regno ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUBJECT", request.Subcode ?? (object)DBNull.Value);
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
        [HttpPost("save-add-attendance")]
        public IActionResult SaveAdditionalAttendance([FromBody] SaveAddAttendanceRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Save attendance request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    int totalSaved = 0;

                    if (request.AbsenceItems != null)
                    {
                        foreach (var item in request.AbsenceItems)
                        {
                            if (item.Selected)
                            {
                                using (SqlCommand cmd = new SqlCommand("SP_ADDATTENDANCE", con))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@ID", request.Id ?? "");
                                    cmd.Parameters.AddWithValue("@REGNO", request.Regno ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@YEAR", request.Year ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@SEM", request.Semister ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@SECTION", request.Section ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@DATE", string.IsNullOrEmpty(item.Date) ? (object)DBNull.Value : Convert.ToDateTime(item.Date).ToString("yyyy-MM-dd"));
                                    cmd.Parameters.AddWithValue("@FACULTY", item.Faculty ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@SUBJECT", item.Subcode ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@Period", item.Period ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@FPerc", request.FPerc ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@TPerc", request.TPerc ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@ACADEMICYEAR", request.AcademicYear ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@CREATEDBYUSER", request.UserId ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@MODIFIEDUSER", request.UserId ?? (object)DBNull.Value);

                                    totalSaved += cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    return Ok(new { success = true, message = "Additional attendance saved successfully.", totalSaved });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("send-otp")]
        public IActionResult SendOtp([FromBody] SendAddAttOtpRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Send OTP request is required." });
            }

            try
            {
                string subGroup = request.SubGroup ?? "";
                if (subGroup.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                    subGroup.Equals("Admissions", StringComparison.OrdinalIgnoreCase))
                {
                    Random r = new Random();
                    int otpNum = r.Next(111111, 999999);

                    return Ok(new
                    {
                        success = true,
                        otp = otpNum.ToString(),
                        message = "OTP generated successfully."
                    });
                }
                else
                {
                    return BadRequest(new { success = false, message = "You have no rights to access this form." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
