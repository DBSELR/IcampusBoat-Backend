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
    public class Student_DayToDay_AttendanceController : ControllerBase
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
        [HttpPost("view")]
        public IActionResult GetStudentDayToDayAttendance([FromBody] StudentDayToDayAttendanceRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Invalid request payload" });
                }

                string fDateFormatted = request.FromDate ?? "";
                if (DateTime.TryParse(request.FromDate, out DateTime parsedFDate))
                {
                    fDateFormatted = parsedFDate.ToString("yyyy-MM-dd");
                }

                string tDateFormatted = request.ToDate ?? "";
                if (DateTime.TryParse(request.ToDate, out DateTime parsedTDate))
                {
                    tDateFormatted = parsedTDate.ToString("yyyy-MM-dd");
                }

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_Std_DayToDay_Att", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Regno", request.RegistrationNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FDATE", fDateFormatted);
                        cmd.Parameters.AddWithValue("@TDATE", tDateFormatted);
                        cmd.Parameters.AddWithValue("@ACADAMICYEAR", request.AcademicYear ?? (object)DBNull.Value);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    string studentName = "";
                    string studyingDetails = "";

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        if (dt.Columns.Contains("SNAME"))
                        {
                            studentName = row["SNAME"]?.ToString() ?? "";
                        }

                        string course = dt.Columns.Contains("COURSE") ? row["COURSE"]?.ToString() ?? "" : "";
                        string bsname = dt.Columns.Contains("BSNAME") ? row["BSNAME"]?.ToString() ?? "" : "";
                        string syear = dt.Columns.Contains("SYEAR") ? row["SYEAR"]?.ToString() ?? "" : "";
                        string semester = dt.Columns.Contains("SEMISTER") ? row["SEMISTER"]?.ToString() ?? "" : "";
                        string section = dt.Columns.Contains("SECTION") ? row["SECTION"]?.ToString() ?? "" : "";

                        studyingDetails = $"{course}({bsname}) ,{syear}-YEAR , {semester}-SEM , SECTION-{section}";
                    }

                    return Ok(new
                    {
                        success = true,
                        studentName = studentName,
                        studyingDetails = studyingDetails,
                        data = DAL.DataTableToList(dt)
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
