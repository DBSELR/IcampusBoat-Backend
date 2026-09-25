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
    public class Admin_AttendanceController : ControllerBase
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
        [HttpGet("branch-list")]
        public IActionResult GetBranchListByFaculty([FromQuery] string? lecturer)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "Select Programme,Branch from TBL_TIMETABLE where Lecturer=@Lecturer";
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
        [HttpGet("tlm")]
        public IActionResult GetTLM([FromQuery] string? academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "select TLMCode+'-'+TLMName as TLM from tbl_TeachingLearningMethods where AcYr=@AcademicYear";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
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
        public IActionResult GetSections([FromBody] AdminSectionSearchRequest request)
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
        [HttpPost("lecturers")]
        public IActionResult GetLecturers([FromBody] LecturerLoadRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Lecturer load request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string spName = string.IsNullOrEmpty(request.SubType) ? "SP_ATT_FACULTY_LIST" : "SP_ATT_FACULTY_LIST_MH";

                    using (SqlCommand cmd = new SqlCommand(spName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcdYr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Shift", request.Shift ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ADATE", request.Date ?? (object)DBNull.Value);
                        if (!string.IsNullOrEmpty(request.SubType))
                        {
                            cmd.Parameters.AddWithValue("@SUBTYPE", request.SubType);
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
        [HttpPost("periods")]
        public IActionResult GetPeriods([FromBody] AdminPeriodLoadRequest request)
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
        [HttpPost("students")]
        public IActionResult GetStudents([FromBody] AdminLoadStudentsRequest request)
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

                    using (SqlCommand cmd = new SqlCommand(spName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (spName == "Sp_Attendance_Practical_Students_Load")
                        {
                            cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Period", request.Period ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ADATE", request.Date ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@AcadamicYear", request.AcademicYear ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@SRegdno", request.SrNo ?? "0");
                            cmd.Parameters.AddWithValue("@ERegdno", request.ErNo ?? "0");
                        }
                        else if (spName == "PROC_ATT_EXTRAHOURS_GET")
                        {
                            cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ADATE", request.Date ?? (object)DBNull.Value);
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
        public IActionResult SaveAdminAttendanceSubjectWise([FromBody] SaveAdminAttendanceSubWiseRequest request)
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

                string sql = $"sp_Attendance_Admn_PapWise_Save '{request.Lecturer ?? ""}', '{request.Lecturer ?? ""}', '{request.Semester ?? ""}', '{request.Programme ?? ""}', '{request.Branch ?? ""}', '{request.SYear ?? ""}', '{request.Section ?? ""}', '{request.Period ?? ""}', '{request.Subjects ?? ""}', '{request.AcademicYear ?? ""}', '{request.Day ?? ""}', '{request.Date ?? ""}', '{request.SrNo ?? ""}', '{request.ErNo ?? ""}', '{request.DayTaught ?? ""}', '{request.AttStat ?? ""}', {queryStr}, '{request.PeriodRange ?? ""}', '{request.TLM ?? ""}'";

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Admin subject-wise attendance saved successfully.", affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("save-permissions")]
        public IActionResult SaveAdminPermissions([FromBody] SaveAdminPermissionsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Permissions request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_SAVEADMIN_PERMISSIONS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@semister", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@LECTURER", request.Lecturer ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUB_CODE", request.Subject ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DATE", request.Date ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADAMICYEAR", request.AcdYr ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Admin permission saved successfully.", affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private static string BuildQueryStringFromStudents(List<AdminStudentAttendanceItem> students, string? srNoStr, string? erNoStr)
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
