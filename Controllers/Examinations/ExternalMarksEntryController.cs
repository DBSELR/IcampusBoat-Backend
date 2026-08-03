using IcampusBoatBackend.Models.Examinations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace IcampusBoatBackend.Controllers.Examinations
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]

    public class ExternalMarksEntryController : ControllerBase
    {
        [HttpGet]
        [Route("load")]
        public IActionResult Load([FromQuery] string academicYear, [FromQuery] string? department = "", [FromQuery] string? userId = "")
        {
            try
            {

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    string autoSSNo = "1/" + academicYear;
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("select StudentSerialNo from tbl_Internal_Marks where ID in(select max(ID) from tbl_Internal_Marks WHERE StudentSerialNo like '%" + academicYear + "')", con))
                        {
                            cmd.CommandType = CommandType.Text;
                            object result = cmd.ExecuteScalar();
                            if (result != null && !string.IsNullOrEmpty(result.ToString()))
                            {
                                string s = result.ToString()!;
                                string[] ar = s.Split('/');
                                if (ar.Length > 1 && int.TryParse(ar[0], out int i))
                                {
                                    autoSSNo = (i + 1).ToString() + "/" + ar[1];
                                }
                            }
                        }
                    }
                    catch { }

                    DataTable programmes = LoadProgrammesData(con, academicYear, department, userId);

                    return Ok(new
                    {
                        success = true,
                        message = "Success",
                        data = new
                        {
                            studentSerialNo = autoSSNo,
                            programmes = DAL.DataTableToList(programmes)
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

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

        [HttpGet]
        [Route("branches")]
        public IActionResult GetBranches([FromQuery] string Course, string AcademicYear)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_SubjectMaster_Branch_Load", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Course", Course ?? (object)DBNull.Value);
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

        [HttpGet]
        [Route("subjects")]
        public IActionResult GetSubjects([FromQuery] ExternalSubjectFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_LOAD_ExtenalMarks_SUBJECTS", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Lecturer", request.UserId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Stream", request.Stream ?? "1");
                cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);

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

        [HttpGet]
        [Route("mid-types")]
        public IActionResult GetMidTypes([FromQuery] ExternalMidTypeFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_LOAD_MIDTYPE_SUBJECTMASTER_ExtMks", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubjectCode", request.SubjectCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);

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

        [HttpGet]
        [Route("max-min-marks")]
        public IActionResult GetMaxMinMarks([FromQuery] ExternalMaxMinMarksFilterModel request)
        {
            try
            {
                var (maxMarksCol, minMarksCol) = MapMaxMinColumns(request.MidType);

                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_lOADMAXMINMARKS", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaxMarks", maxMarksCol ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MinMarks", minMarksCol ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubjectName", request.SubjectCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SEMISTER", request.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CourseCode", request.Programme ?? (object)DBNull.Value);

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

        [HttpGet]
        [Route("check-internal-dates")]
        public IActionResult CheckInternalDates([FromQuery] ExternalInternalDateFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_CHECKINTERNALDATES", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@COURSECODE", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@sYear", request.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ACYR", request.AcademicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EXAMTYPE", request.MidType ?? (object)DBNull.Value);

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

        [HttpGet]
        [Route("student-marks")]
        public IActionResult GetStudentMarks([FromQuery] ExternalStudentMarksFilterModel request)
        {
            try
            {
                string midType1 = MapMidType1(request.MidType);

                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable dt = new DataTable();

                if (request.MidType == "Attendance")
                {
                    using SqlCommand cmd = new SqlCommand("SP_ATTMARKSLOAD_SUBJECTWISE", con);
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
                else
                {
                    using SqlCommand cmd = new SqlCommand("SP_Marks_LIST_ExtMks", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ACADEMICYEAR", request.AcademicYear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SSEMESTER", request.Semester ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SYEAR", request.Year ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubjectName", request.SubjectCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Lecturer", request.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Midtype", request.MidType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Midtype1", midType1 ?? (object)DBNull.Value);

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

        [HttpGet]
        [Route("validate-regno")]
        public IActionResult ValidateRegNo([FromQuery] ExternalValidateRegNoFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_Get_RegisterNo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RegistrationNo", request.RegistrationNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubjectName", request.SubjectCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Sessional", request.Sessional ?? (object)DBNull.Value);

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

        [HttpPost]
        [Route("save")]
        public IActionResult Save([FromBody] ExternalMarksEntrySaveModel request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Invalid request payload." });
                }

                string midType1 = MapMidType1(request.MidType);

                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_External_MarksEntry_Save", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", request.Id ?? "0");
                cmd.Parameters.AddWithValue("@RegistrationNo", request.RegistrationNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Date", string.IsNullOrWhiteSpace(request.Date) ? DateTime.Now.ToString("dd-MM-yyyy") : request.Date);
                cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Stream", request.Stream ?? "1");
                cmd.Parameters.AddWithValue("@SubjectName", request.SubjectCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaxMarks", request.MaxMarks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Marks", request.Marks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Midtype1", midType1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UserId", request.UserId ?? (object)DBNull.Value);

                int rows = cmd.ExecuteNonQuery();

                if (rows <= 0)
                {
                    return BadRequest(new { success = false, message = "Data not saved." });
                }

                return Ok(new { success = true, message = "Data Saved Successfully", data = rows });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("freeze")]
        public IActionResult Freeze([FromBody] ExternalMarksEntryFreezeModel request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Invalid request payload." });
                }

                string midType1 = MapMidType1(request.MidType);

                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                // 1. Check for unentered (null) marks
                using (SqlCommand cmdNull = new SqlCommand("SP_GET_External_MARKSNULL", con))
                {
                    cmdNull.CommandType = CommandType.StoredProcedure;
                    cmdNull.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                    cmdNull.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                    cmdNull.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                    cmdNull.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                    cmdNull.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                    cmdNull.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                    cmdNull.Parameters.AddWithValue("@SubjectCode", request.SubjectCode ?? (object)DBNull.Value);
                    cmdNull.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                    cmdNull.Parameters.AddWithValue("@Midtype", request.MidType ?? (object)DBNull.Value);
                    cmdNull.Parameters.AddWithValue("@Midtype1", midType1 ?? (object)DBNull.Value);

                    DataTable dtNull = new DataTable();
                    using SqlDataAdapter da = new SqlDataAdapter(cmdNull);
                    da.Fill(dtNull);

                    if (dtNull.Rows.Count > 0)
                    {
                        return BadRequest(new { success = false, message = "Please Give Marks for All Students" });
                    }
                }

                // 2. Check if marks already frozen
                using (SqlCommand cmdCheck = new SqlCommand("SP_CHECK_EXTERNAL_FREEZINGMARKS", con))
                {
                    cmdCheck.CommandType = CommandType.StoredProcedure;
                    cmdCheck.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                    cmdCheck.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                    cmdCheck.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                    cmdCheck.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                    cmdCheck.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                    cmdCheck.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                    cmdCheck.Parameters.AddWithValue("@SubjectCode", request.SubjectCode ?? (object)DBNull.Value);
                    cmdCheck.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);

                    DataTable dtCheck = new DataTable();
                    using SqlDataAdapter da = new SqlDataAdapter(cmdCheck);
                    da.Fill(dtCheck);

                    if (dtCheck.Rows.Count > 0)
                    {
                        return BadRequest(new { success = false, message = "Marks Already Freezed" });
                    }
                }

                // 3. Freeze marks
                using (SqlCommand cmdFreeze = new SqlCommand("SP_External_MarksFreeze", con))
                {
                    cmdFreeze.CommandType = CommandType.StoredProcedure;
                    cmdFreeze.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                    cmdFreeze.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                    cmdFreeze.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                    cmdFreeze.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                    cmdFreeze.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                    cmdFreeze.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                    cmdFreeze.Parameters.AddWithValue("@SubjectCode", request.SubjectCode ?? (object)DBNull.Value);
                    cmdFreeze.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);

                    int rows = cmdFreeze.ExecuteNonQuery();
                    return Ok(new { success = true, message = "Marks Freeze Successfully", data = rows });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("check-frozen")]
        public IActionResult CheckFrozen([FromQuery] ExternalMarksEntryFreezeModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_CHECK_EXTERNAL_FREEZINGMARKS", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubjectCode", request.SubjectCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                bool isFrozen = dt.Rows.Count > 0;
                return Ok(new { success = true, message = "Success", data = new { isFrozen } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("null-marks-count")]
        public IActionResult GetNullMarksCount([FromQuery] ExternalMarksEntryFreezeModel request)
        {
            try
            {
                string midType1 = MapMidType1(request.MidType);

                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_GET_External_MARKSNULL", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubjectCode", request.SubjectCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Midtype", request.MidType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Midtype1", midType1 ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new { success = true, message = "Success", data = new { count = dt.Rows.Count } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("report-data")]
        public IActionResult GetReportData([FromQuery] ExternalMarksEntryFreezeModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_Get_External_marks", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubjectCode", request.SubjectCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);

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

        #region Helpers

        private static DataTable LoadProgrammesData(SqlConnection con, string? academicYear, string? department, string? userId)
        {
            var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
            {
                ("SP_MarksEntry_Programme_List", new Dictionary<string, object?> { { "@AcademicYear", academicYear } }),
                ("SP_USERWISE_LoadCourse", new Dictionary<string, object?> { { "@Department", department }, { "@AcademicYear", academicYear } }),
                ("SP_ADM_STDDATA_Programme_LIST", new Dictionary<string, object?> { { "@AcademicYear", academicYear } }),
                ("SP_SubjectMaster_Programme_Load", new Dictionary<string, object?> { { "@AcademicYear", academicYear } }),
                ("SP_USERWISE_LoadCourse_NEW", new Dictionary<string, object?> { { "@AcademicYear", academicYear }, { "@Department", department }, { "@UserID", userId } })
            };

            return ExecuteSpWithFallback(con, spCandidates);
        }

        private static DataTable ExecuteSpWithFallback(SqlConnection con, List<(string spName, Dictionary<string, object?> paramsDict)> spCandidates)
        {
            foreach (var (spName, paramsDict) in spCandidates)
            {
                try
                {
                    using SqlCommand cmd = new SqlCommand(spName, con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (paramsDict != null)
                    {
                        foreach (var kvp in paramsDict)
                        {
                            cmd.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                    }

                    DataTable dt = new DataTable();
                    using SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    return dt;
                }
                catch (SqlException ex) when (ex.Number == 2812 || ex.Number == 15009 || ex.Number == 8144 || ex.Number == 201)
                {
                    continue;
                }
            }

            return new DataTable();
        }

        private static string MapMidType1(string? midType)
        {
            if (string.IsNullOrWhiteSpace(midType)) return "";

            switch (midType)
            {
                case "Mid-1":
                case "Descriptive-1":
                    return "SSM1";
                case "Mid-2":
                case "Descriptive-2":
                    return "SSM2";
                case "Assignment":
                case "Assignment-1":
                    return "ASNTM1";
                case "Assignment-2":
                    return "ASNTM2";
                case "OnlineQuiz-1":
                    return "OQM1";
                case "OnlineQuiz-2":
                    return "OQM2";
                case "Attendance":
                    return "ATTM1";
                case "DayToDay":
                case "DayToDay-1":
                    return "DTDM1";
                case "DayToDay-2":
                    return "DTDM2";
                case "InternalTest":
                    return "ITM1";
                case "Record":
                    return "RECM1";
                case "Viva":
                    return "VVM1";
                case "LAB Internals":
                    return "LabIntMrks";
                case "Report & Presentation":
                    return "RptPrntMrks";
                case "Report & Presentation2":
                    return "RptPrntMrks2";
                case "Oral Test":
                    return "OralMks";
                case "Continious Internal Evalution(CIE)":
                    return "CIEMrks";
                case "Continious Internal Evalution(CIE(40))":
                    return "CIEMrks40";
                case "Continious Internal Evalution(CIE(75))":
                    return "CIEMrks75";
                case "Continious Internal Evalution(CIE(50))":
                    return "CIEMrks50";
                case "Continious Internal Evalution(CIE(60))":
                    return "CIEMrks60";
                case "Drawing Sheet Marks":
                    return "DrawSheet";
                case "ObjectiveMarks-1":
                case "ObjectiveMarks-2":
                    return "ObjectiveMarks1";
                case "Lab Externals":
                    return "LabExtMks";
                default:
                    return midType;
            }
        }

        private static (string maxCol, string minCol) MapMaxMinColumns(string? midType)
        {
            if (string.IsNullOrWhiteSpace(midType)) return ("MaxMarks", "MinMarks");

            switch (midType)
            {
                case "Mid-1":
                case "Mid-2":
                case "Descriptive-1":
                case "Descriptive-2":
                    return ("SessionalMaxMarks", "SessionalMinMarks");
                case "Assignment":
                case "Assignment-1":
                case "Assignment-2":
                    return ("AssMaxMarks", "AssMinMarks");
                case "OnlineQuiz-1":
                case "OnlineQuiz-2":
                    return ("OQMaxMarks", "OQMinMarks");
                case "Attendance":
                    return ("AttMaxMarks", "AttMinMarks");
                case "InternalTest":
                    return ("intTestMaxMarks", "intTestMinMarks");
                case "DayToDay":
                case "DayToDay-1":
                case "DayToDay-2":
                    return ("DayMaxMarks", "DayMinMarks");
                case "Viva":
                    return ("VivomaxMarks", "VivoMinMarks");
                case "Record":
                    return ("RecordMaxMarks", "RecordMinMarks");
                case "LAB Internals":
                    return ("LabIntMaxMks", "LabIntMinMks");
                case "Report & Presentation":
                case "Report & Presentation2":
                    return ("RptPrstMaxMks", "RptPrstMinMks");
                case "Oral Test":
                    return ("OralMaxMks", "OralMinMks");
                case "Continious Internal Evalution(CIE)":
                    return ("MaxCIE", "MinCIE");
                case "Continious Internal Evalution(CIE(40))":
                    return ("MaxCIE40", "MinCIE40");
                case "Continious Internal Evalution(CIE(75))":
                    return ("MaxCIE75", "MinCIE75");
                case "Continious Internal Evalution(CIE(50))":
                    return ("MaxCIE50", "MinCIE50");
                case "Continious Internal Evalution(CIE(60))":
                    return ("MaxCIE60", "MinCIE60");
                case "Drawing Sheet Marks":
                    return ("DrawShtMaxMks", "DrawShtMinMks");
                case "ObjectiveMarks-1":
                case "ObjectiveMarks-2":
                    return ("ObjMaxMarks", "ObjMinMarks");
                case "Lab Externals":
                    return ("LabExtMaxMks", "LabExtMinMks");
                default:
                    return ("MaxMarks", "MinMarks");
            }
        }

        #endregion
    }
}
