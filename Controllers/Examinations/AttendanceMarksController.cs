using IcampusBoatBackend.Models.Examinations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace IcampusBoatBackend.Controllers.Examinations
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/Attendance_Marks")]
    public class AttendanceMarksController : ControllerBase
    {
        /// <summary>
        /// Fetch student attendance marks subject-wise.
        /// </summary>
        [HttpGet]
        [Route("student-list")]
        public IActionResult GetStudentList([FromQuery] AttendanceMarksFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand("SP_ATTMARKSLOAD_SUBJECTWISE", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CLASS", request.Programme ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GRPID", request.Branch ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SYEAR", request.Year ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Subject", request.SubjectCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ACADAMICYEAR", request.AcademicYear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", request.UserId ?? (object)DBNull.Value);

                    using SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Check allowed date limits for attendance marks entry.
        /// </summary>
        [HttpGet]
        [Route("check-dates")]
        public IActionResult CheckDates([FromQuery] AttendanceMarksFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand("SP_CHECKINTERNALDATES", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@COURSECODE", request.Programme ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@sYear", request.Year ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ACYR", request.AcademicYear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EXAMTYPE", "Attendance");

                    using SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Bulk save student attendance marks.
        /// </summary>
        [HttpPost]
        [Route("save")]
        public IActionResult Save([FromBody] AttendanceMarksSaveModel request)
        {
            try
            {
                if (request == null || request.Students == null || request.Students.Count == 0)
                {
                    return BadRequest(new { success = false, message = "Student list is required." });
                }

                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();
                int successCount = 0;

                foreach (var student in request.Students)
                {
                    using SqlCommand cmd = new SqlCommand("SP_ADDATTENDANCEMARKS", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", student.Id ?? "0");
                    cmd.Parameters.AddWithValue("@REGNO", student.RegistrationNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Sem", request.Semester ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Date", string.IsNullOrWhiteSpace(request.Date) ? DateTime.Now.ToString("dd-MM-yyyy") : request.Date);
                    cmd.Parameters.AddWithValue("@SubjectName", request.SubjectCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Marks", student.Marks ?? "0");
                    cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Stream", "1");
                    cmd.Parameters.AddWithValue("@TempCode", student.TLMCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", request.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TC", student.TotalClasses ?? "0");
                    cmd.Parameters.AddWithValue("@PC", student.PresentClasses ?? "0");
                    cmd.Parameters.AddWithValue("@Perc", student.Percentage ?? "0");

                    int res = cmd.ExecuteNonQuery();
                    if (res > 0) successCount++;
                }

                return Ok(new { success = true, message = $"{successCount} attendance mark records saved successfully.", data = successCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
