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
    public class Period_AdjustmentController : ControllerBase
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
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_Programme_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
        [HttpGet("departments")]
        public IActionResult GetDepartments([FromQuery] string? empId, [FromQuery] string? access)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_Load_Emp_Dept_AS_Filter", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EMPID", empId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACCESS", access ?? (object)DBNull.Value);
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
        [HttpPost("faculty")]
        public IActionResult GetFaculty([FromBody] FacultyFilterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Faculty filter request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_SPLLOadEmpID_New", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EMPID", request.EmpId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dept", request.Dept ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@workMode", request.WorkMode ?? "Teaching");
                        cmd.Parameters.AddWithValue("@ACCESS", request.Access ?? (object)DBNull.Value);
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
        [HttpPost("absent-faculty-timetable")]
        public IActionResult GetAbsentFacultyTimeTable([FromBody] AbsFacTimeTableRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Absent faculty request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("PROC_LOAD_ABS_FAC_TT", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EMPID", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DAY", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@WDate", request.Wdate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSE", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Sem", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Academicyear", request.AcdYr ?? (object)DBNull.Value);
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
        [HttpPost("available-faculty")]
        public IActionResult GetAvailableFaculty([FromBody] AvailableFacultyRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Available faculty request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("PROC_LOAD_AVL_FACULTY", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@avl", request.Fac ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@yr", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@sem", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRanch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DAY", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FromPERIOD", request.PeriodFrom ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToPERIOD", request.PeriodTo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@WDATE", request.Wdate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcdYr ?? (object)DBNull.Value);
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
        [HttpPost("available-faculty-subjects")]
        public IActionResult GetAvailableFacultySubjects([FromBody] AvailableFacultySubRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Available faculty subjects request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("PROC_LOAD_AVL_FACULTY_SUB", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@yr", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@sem", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRanch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DAY", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FromPERIOD", request.PeriodFrom ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToPERIOD", request.PeriodTo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@WDATE", request.Wdate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcdYr ?? (object)DBNull.Value);
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
        [HttpPost("save-adjustment")]
        public IActionResult SavePeriodAdjustment([FromBody] TimeTableAdjustSaveRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Adjustment save request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TIMETABLE_ADJUST_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SHIFT", request.ShiftNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DAY", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@WDATE", request.Wdate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMISTER", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SECTION", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PERIOD", request.Period ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUBJECT", request.Subject ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@P_SUBJECT", request.AVL_Subject ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DEPARTMENT", request.Department ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@LECTURER", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PRESENT_LECTURER", request.AVL_Lecturert ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STREAM", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SPTIME", request.SPTime ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PERIOD_TYPE", request.PeriodType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FRM_TO_PERIODS", request.Frrom_To_Periods ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TOPERIOD", request.Toperiod ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Reason", request.Reason ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@P_SUBJECTCode", request.P_SubjectCode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@L_SUBJECTCode", request.L_SubjectCode ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Period adjustment saved successfully.", affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("delete-adjustment")]
        public IActionResult DeletePeriodAdjustment([FromBody] TimeTableDeleteAdjustmentRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Adjustment delete request is required." });
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
                        cmd.Parameters.AddWithValue("@TYPE", request.Proc_type ?? "TT_ADJUST");
                        cmd.Parameters.AddWithValue("@WDATE", request.Wdate ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Period adjustment deleted successfully.", affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("save-period-cancel")]
        public IActionResult SavePeriodCancel([FromBody] PeriodCancelSaveRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Period cancel request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("PROC_PERIOD_CANCEL", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Shift", request.ShiftNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Day", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@WDate", request.Wdate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STREAM", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period", request.Period ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CSubject", request.Subject ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@LECTURER", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PERIOD_TYPE", request.PeriodType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FRM_TO_PERIODS", request.Frrom_To_Periods ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SPTIME", request.SPTime ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TOPERIOD", request.Toperiod ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Reason", request.Reason ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Period cancellation saved successfully.", affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("delete-period-cancel")]
        public IActionResult DeletePeriodCancel([FromBody] PeriodCancelDeleteRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Delete period cancel request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    string query = @"DELETE FROM tbl_PERIOD_CANCEL 
                                     WHERE Shift = @ShiftNo AND WDate = @Wdate AND Period BETWEEN @Period AND @Toperiod 
                                     AND Branch = @Branch AND Section = @Section AND LECTURER = @Lecturer";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ShiftNo", request.ShiftNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Wdate", request.Wdate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Period", request.Period ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Toperiod", request.Toperiod ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Period cancellation deleted successfully.", affectedRows = rows });
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

