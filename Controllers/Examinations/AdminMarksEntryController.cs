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
    public class AdminMarksEntryController : ControllerBase
    {
        /// <summary>
        /// Initial load API for Admin Marks Entry returning Auto SSNo, Programmes, and Mid Exams.
        /// </summary>
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
                        using (SqlCommand cmd = new SqlCommand("SELECT StudentSerialNo FROM tbl_Internal_Marks WHERE ID IN (SELECT MAX(ID) FROM tbl_Internal_Marks WHERE StudentSerialNo LIKE '%" + academicYear + "')", con))
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
                    DataTable midExams = LoadMidExamsData(con, academicYear);

                    return Ok(new
                    {
                        success = true,
                        message = "Success",
                        data = new
                        {
                            studentSerialNo = autoSSNo,
                            programmes = DAL.DataTableToList(programmes),
                            midExams = DAL.DataTableToList(midExams)
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch list of programmes for the selected academic year.
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
        /// Fetch list of studying years for the selected programme.
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
        /// Fetch list of branches for selected programme and academic year.
        /// </summary>
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

        /// <summary>
        /// Fetch sections for selected programme, branch, and studying year.
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
        /// Fetch subjects list based on selected filters.
        /// </summary>
        [HttpGet]
        [Route("subjects")]
        public IActionResult GetSubjects([FromQuery] AdminMarksEntryFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                var spList = new List<(string spName, Dictionary<string, object?> paramsDict)>
                {
                    ("SP_LOAD_SUBJECTS_New", new Dictionary<string, object?>
                    {
                        { "@Lecturer", request.UserId },
                        { "@Programme", request.Programme },
                        { "@Branch", request.Branch },
                        { "@Year", request.Year },
                        { "@Semester", request.Semester },
                        { "@Stream", request.Stream ?? "1" },
                        { "@Section", request.Section },
                        { "@AcademicYear", request.AcademicYear }
                    }),
                    ("SP_LOADSUBJECTS_MH", new Dictionary<string, object?>
                    {
                        { "@COURSECODE", request.Programme },
                        { "@Sem", request.Semester },
                        { "@EMPID", request.UserId }
                    })
                };

                DataTable dt = ExecuteSpWithFallback(con, spList);
                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch mid exam types for selected subject and parameters.
        /// </summary>
        [HttpGet]
        [Route("mid-types")]
        public IActionResult GetMidTypes([FromQuery] AdminMarksEntryFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable dt = new DataTable();

                if (!string.IsNullOrWhiteSpace(request.SubjectCode) && !string.IsNullOrWhiteSpace(request.Programme))
                {
                    var spList = new List<(string spName, Dictionary<string, object?> paramsDict)>
                    {
                        ("SP_LOAD_MIDTYPE_SUBJECTMASTER", new Dictionary<string, object?>
                        {
                            { "@PROGRAMME", request.Programme },
                            { "@BRANCH", request.Branch },
                            { "@YEAR", request.Year },
                            { "@SEMISTER", request.Semester },
                            { "@SUBJECT", request.SubjectCode },
                            { "@SECTION", request.Section },
                            { "@ACADEMICYEAR", request.AcademicYear }
                        }),
                        ("SP_LoadMid_Exams", new Dictionary<string, object?>
                        {
                            { "@AcademicYear", request.AcademicYear }
                        })
                    };
                    dt = ExecuteSpWithFallback(con, spList);
                }
                else
                {
                    dt = LoadMidExamsData(con, request.AcademicYear ?? "");
                }

                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch max and min marks configuration.
        /// </summary>
        [HttpGet]
        [Route("max-min-marks")]
        public IActionResult GetMaxMinMarks([FromQuery] AdminMarksEntryFilterModel request)
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

        /// <summary>
        /// Check allowed date ranges for internal marks entry.
        /// </summary>
        [HttpGet]
        [Route("check-internal-dates")]
        public IActionResult CheckInternalDates([FromQuery] AdminMarksEntryFilterModel request)
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

        /// <summary>
        /// Fetch student list along with existing marks.
        /// </summary>
        [HttpGet]
        [Route("student-marks")]
        public IActionResult GetStudentMarks([FromQuery] AdminMarksEntryFilterModel request)
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
                    using SqlCommand cmd = new SqlCommand("SP_Marks_LIST", con);
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

        /// <summary>
        /// Save single student mark record.
        /// </summary>
        [HttpPost]
        [Route("save-All")]
        public IActionResult SaveAll([FromBody] AdminMarksEntrySaveModel request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Invalid request payload." });
                }

                string midType1 = MapMidType1(request.MidType ?? request.MidType1);

                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_Internal_MarksEntry_Save", con);
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
                cmd.Parameters.AddWithValue("@SubjectName", request.SubjectCode ?? request.SubjectName ?? (object)DBNull.Value);
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

        /// <summary>
        /// Save bulk attendance/internal marks for students.
        /// </summary>
        [HttpPost]
        [Route("save")]
        public IActionResult Save([FromBody] AdminMarksEntryAttendanceSaveModel request)
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
                    cmd.Parameters.AddWithValue("@SubjectName", request.SubjectCode ?? request.SubjectName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Marks", student.Marks ?? "0");
                    cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Stream", request.Stream ?? "1");
                    cmd.Parameters.AddWithValue("@TempCode", student.TLMCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", request.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TC", student.TC ?? "0");
                    cmd.Parameters.AddWithValue("@PC", student.PC ?? "0");
                    cmd.Parameters.AddWithValue("@Perc", student.Perc ?? "0");

                    int res = cmd.ExecuteNonQuery();
                    if (res > 0) successCount++;
                }

                return Ok(new { success = true, message = $"{successCount} student attendance marks saved successfully", data = successCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        #region Helper Execution & Mapping Methods

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

        private static DataTable LoadMidExamsData(SqlConnection con, string? academicYear)
        {
            var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
            {
                ("SP_LoadMid_Exams", new Dictionary<string, object?> { { "@AcademicYear", academicYear } }),
                ("SP_GET_EXAMTYPE", new Dictionary<string, object?>())
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
