using IcampusBoatBackend.Models.Examinations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace IcampusBoatBackend.Controllers.Examinations
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class MarksEntryMHController : ControllerBase
    {
        /// <summary>
        /// Initial load API for Marks Entry MH.
        /// </summary>
        [HttpGet]
        [Route("load")]
        public IActionResult Load([FromQuery] string academicYear, [FromQuery] string? department = "", [FromQuery] string? userId = "")
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable programmes = LoadProgrammesData(con, academicYear, department, userId);

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = new
                    {
                        programmes = DAL.DataTableToList(programmes)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch list of programmes for selected academic year and user.
        /// </summary>
        [HttpGet]
        [Route("programmes")]
        public IActionResult GetProgrammes(string academicYear, string? department = "", string? userId = "")
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable dt = LoadProgrammesData(con, academicYear, department, userId);
                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch studying years for selected programme.
        /// </summary>
        [HttpGet]
        [Route("years")]
        public IActionResult GetYears([FromQuery] string Department, string Programme, string AcademicYear)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_USERWISE_LoadYR", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DEPT", Department ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CourseCode", Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicYear", AcademicYear ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch branches for selected programme and academic year.
        /// </summary>
        [HttpGet]
        [Route("branches")]
        public IActionResult GetBranches([FromQuery] string Course, string AcademicYear, string? department = "", string? userId = "")
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_USERWISE_LoadBranch", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DEPT", department ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CourseCode", Course ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicYear", AcademicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EmpID", userId ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch sections for selected programme, branch, and year.
        /// </summary>
        [HttpGet]
        [Route("sections")]
        public IActionResult GetSections(string programme, string branch, string year)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_Get_Section", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@COURSECODE", programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BRANCHCODE", branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@STDYEAR", year ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch subjects list for MH marks entry.
        /// </summary>
        [HttpGet]
        [Route("subjects")]
        public IActionResult GetSubjects([FromQuery] MarksEntryMHFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand("SP_LOADSUBJECTS_MH", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@COURSECODE", request.Programme ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Sem", request.Semester ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EMPID", request.UserId ?? (object)DBNull.Value);

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
        /// Fetch student marks MH (multiple heads) list.
        /// </summary>
        [HttpGet]
        [Route("student-marks-mh")]
        public IActionResult GetStudentMarksMH([FromQuery] MarksEntryMHFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand("SP_MARKS_ENTRY_MH_LOAD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubjectCode", request.SubjectCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MidType", request.MidType ?? (object)DBNull.Value);
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
        /// Bulk save student marks MH (multiple heads).
        /// </summary>
        [HttpPost]
        [Route("save-mh")]
        public IActionResult SaveMH([FromBody] MarksEntryMHSaveModel request)
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
                    using SqlCommand cmd = new SqlCommand("SP_RESULTENTRY_SAVE", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ID", student.Id ?? "0");
                    cmd.Parameters.AddWithValue("@REGISTRATIONNO", student.RegistrationNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Date", string.IsNullOrWhiteSpace(request.Date) ? DateTime.Now.ToString("dd-MM-yyyy") : request.Date);
                    cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@YEAR", request.Year ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SEMESTER", request.Semester ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SECTION", request.Section ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Stream", request.Stream ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SUBJECTNAME", request.SubjectName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SUBMAXMRK", student.SubMaxMrk ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MAXMARKS", student.MaxMarks ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MARKS", student.Marks ?? "0");
                    cmd.Parameters.AddWithValue("@GRADE", student.Grade ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CGPA", student.CGPA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SGPA", student.SGPA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ACADEMICYEAR", request.AcademicYear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", request.UserId ?? (object)DBNull.Value);

                    int res = cmd.ExecuteNonQuery();
                    if (res > 0) successCount++;
                }

                return Ok(new { success = true, message = $"{successCount} student MH mark records saved successfully.", data = successCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        #region Helpers

        private static DataTable LoadProgrammesData(SqlConnection con, string? academicYear, string? department, string? userId)
        {
            DataTable dt = new DataTable();
            using (SqlCommand cmd = new SqlCommand("SP_USERWISE_LoadCourse_NEW", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Department", department ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UserID", userId ?? (object)DBNull.Value);

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        #endregion
    }
}
