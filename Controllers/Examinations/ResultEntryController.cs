using IcampusBoatBackend.Models.Examinations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace IcampusBoatBackend.Controllers.Examinations
{
    /// <summary>
    /// API Controller for Examinations Result Entry based on exact DAL_ResultEntry.cs queries and procedures.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class ResultEntryController : ControllerBase
    {
        /// <summary>
        /// Page Load Initial Data API (Auto Serial No, Programmes, Lecturers/Faculty).
        /// Matches get_SSNo(), BookEntry_Programme_List(), Get_Faculty() from DAL_ResultEntry.cs
        /// </summary>
        [HttpGet]
        [Route("load")]
        public IActionResult Load([FromQuery] string academicYear, [FromQuery] string? department = "")
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                string autoSSNo = GetAutoSSNo(con, academicYear);
                DataTable dtProgrammes = LoadProgrammesData(con, academicYear);
                DataTable dtFaculty = LoadFacultyData(con, department);

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = new
                    {
                        studentSerialNo = autoSSNo,
                        programmes = DAL.DataTableToList(dtProgrammes),
                        lecturers = DAL.DataTableToList(dtFaculty)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// BookEntry_Programme_List
        /// Procedure: SP_MarksEntry_Programme_List '@AcademicYear'
        /// </summary>
        [HttpGet]
        [Route("programmes")]
        public IActionResult GetProgrammes([FromQuery] string academicYear)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable dt = LoadProgrammesData(con, academicYear);
                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get_Faculty / USERWISE_LoadEmpID
        /// Query: select distinct EmpID, EmpID + '-' + FName AS FName from tbl_EmployeeDetails
        /// </summary>
        [HttpGet]
        [Route("lecturers")]
        public IActionResult GetLecturers([FromQuery] string? department = "")
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable dt = LoadFacultyData(con, department);
                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// MarksEntry_YEAR_LIST / USERWISE_LoadYR
        /// Procedure: SP_ADM_YEARS '@Programme' / SP_USERWISE_LoadYR
        /// </summary>
        [HttpGet]
        [Route("years")]
        public IActionResult GetYears([FromQuery] string programme, [FromQuery] string academicYear, [FromQuery] string? department = "")
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
                {
                    ("SP_ADM_YEARS", new Dictionary<string, object?>
                    {
                        { "@Course", programme },
                        { "@Programme", programme }
                    }),
                    ("SP_USERWISE_LoadYR", new Dictionary<string, object?>
                    {
                        { "@Programme", programme },
                        { "@Department", department ?? "" },
                        { "@AcademicYear", academicYear }
                    }),
                    ("SP_USERWISE_LoadYR", new Dictionary<string, object?>
                    {
                        { "@CourseCode", programme },
                        { "@DEPT", department ?? "" },
                        { "@AcademicYear", academicYear }
                    })
                };

                DataTable dt = ExecuteSpWithFallback(con, spCandidates);
                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// MarksEntry_Branch_Load / USERWISE_LoadBranch
        /// Procedure: SP_SubjectMaster_Branch_Load '@Programme', '@AcademicYear'
        /// </summary>
        [HttpGet]
        [Route("branches")]
        public IActionResult GetBranches([FromQuery] string programme, [FromQuery] string academicYear, [FromQuery] string? department = "", [FromQuery] string? userId = "")
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
                {
                    ("SP_SubjectMaster_Branch_Load", new Dictionary<string, object?>
                    {
                        { "@Course", programme },
                        { "@AcademicYear", academicYear }
                    }),
                    ("SP_USERWISE_LoadBranch", new Dictionary<string, object?>
                    {
                        { "@Programme", programme },
                        { "@Department", department ?? "" },
                        { "@AcademicYear", academicYear }
                    }),
                    ("SP_USERWISE_LoadBranch", new Dictionary<string, object?>
                    {
                        { "@CourseCode", programme },
                        { "@DEPT", department ?? "" },
                        { "@AcademicYear", academicYear },
                        { "@EmpID", userId ?? "" }
                    })
                };

                DataTable dt = ExecuteSpWithFallback(con, spCandidates);
                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get_Section
        /// Query: select distinct Section from tbl_sectionmaster where CourseCode=@Programme and BranchCode=@Branch and StdYear=@Year
        /// </summary>
        [HttpGet]
        [Route("sections")]
        public IActionResult GetSections([FromQuery] string programme, [FromQuery] string branch, [FromQuery] string year)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                string sql = "select distinct Section from tbl_sectionmaster where CourseCode=@Programme and BranchCode=@Branch and StdYear=@Year";
                using SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Programme", programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", year ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
                    {
                        ("SP_Get_Section", new Dictionary<string, object?>
                        {
                            { "@Programme", programme },
                            { "@Branch", branch },
                            { "@Year", year }
                        }),
                        ("SP_Get_Section", new Dictionary<string, object?>
                        {
                            { "@COURSECODE", programme },
                            { "@BRANCHCODE", branch },
                            { "@STDYEAR", year }
                        })
                    };
                    dt = ExecuteSpWithFallback(con, spCandidates);
                }

                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get_SubName_Faculty
        /// Procedure: SP_LOAD_SUBJECTS '@Lecturer','@Programme','@Branch','@Year','@Semester','@Stream'
        /// </summary>
        [HttpGet]
        [Route("subjects")]
        public IActionResult GetSubjects([FromQuery] ResultEntryFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
                {
                    ("SP_LOAD_SUBJECTS", new Dictionary<string, object?>
                    {
                        { "@Lecturer", request.Lecturer ?? request.UserId },
                        { "@Programme", request.Programme },
                        { "@Branch", request.Branch },
                        { "@Year", request.Year },
                        { "@Semester", request.Semester },
                        { "@Stream", request.Stream ?? "1" }
                    }),
                    ("SP_Get_SubName_Faculty", new Dictionary<string, object?>
                    {
                        { "@Lecturer", request.Lecturer ?? request.UserId },
                        { "@Programme", request.Programme },
                        { "@Branch", request.Branch },
                        { "@Year", request.Year },
                        { "@Semester", request.Semester },
                        { "@Stream", request.Stream ?? "1" }
                    }),
                    ("SP_LOAD_SUBJECTS_New", new Dictionary<string, object?>
                    {
                        { "@Lecturer", request.Lecturer ?? request.UserId },
                        { "@Programme", request.Programme },
                        { "@Branch", request.Branch },
                        { "@Year", request.Year },
                        { "@Semester", request.Semester },
                        { "@Stream", request.Stream ?? "1" },
                        { "@Section", request.Section },
                        { "@ACADEMICYEAR", request.AcademicYear }
                    })
                };

                DataTable dt = ExecuteSpWithFallback(con, spCandidates);
                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get_MaxMarks
        /// Query: select distinct SUBMAXMRK, MAXMARKS from tbl_Result_Marks where SUBJECTNAME = @SubjectName AND YEAR = @Year AND Semester = @Semester AND BRANCH = @Branch and SECTION = @Section
        /// </summary>
        [HttpGet]
        [Route("max-marks")]
        public IActionResult GetMaxMarks([FromQuery] ResultEntryFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                string sql = @"select distinct SUBMAXMRK, MAXMARKS from tbl_Result_Marks 
                               where SUBJECTNAME = @SubjectName 
                                 AND YEAR = @Year 
                                 AND Semester = @Semester 
                                 AND BRANCH = @Branch 
                                 AND SECTION = @Section";

                using SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@SubjectName", request.SubjectName ?? request.SubjectCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
                    {
                        ("SP_Get_MaxMarks", new Dictionary<string, object?>
                        {
                            { "@AcademicYear", request.AcademicYear },
                            { "@Programme", request.Programme },
                            { "@Branch", request.Branch },
                            { "@Semester", request.Semester },
                            { "@Section", request.Section },
                            { "@Year", request.Year },
                            { "@SubjectName", request.SubjectName ?? request.SubjectCode }
                        }),
                        ("SP_lOADMAXMINMARKS", new Dictionary<string, object?>
                        {
                            { "@CourseCode", request.Programme },
                            { "@Branch", request.Branch },
                            { "@Year", request.Year },
                            { "@SEMISTER", request.Semester },
                            { "@SUBJECTCODE", request.SubjectName ?? request.SubjectCode },
                            { "@AcademicYear", request.AcademicYear }
                        })
                    };
                    dt = ExecuteSpWithFallback(con, spCandidates);
                }

                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Result_GradeENtryrLoad
        /// Procedure: SP_ResultGradeList '@AcademicYear','@Programme','@Branch',@Semester,'@Section',@Year,'@SubjectName','@Lecturer'
        /// </summary>
        [HttpGet]
        [Route("student-results")]
        public IActionResult GetStudentResults([FromQuery] ResultEntryFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
                {
                    ("SP_ResultGradeList", new Dictionary<string, object?>
                    {
                        { "@AcademicYear", request.AcademicYear },
                        { "@Programme", request.Programme },
                        { "@Branch", request.Branch },
                        { "@Semester", request.Semester },
                        { "@Section", request.Section },
                        { "@Year", request.Year },
                        { "@SubjectName", request.SubjectName ?? request.SubjectCode },
                        { "@Lecturer", request.Lecturer ?? request.UserId }
                    }),
                    ("SP_Result_GradeENtryrLoad", new Dictionary<string, object?>
                    {
                        { "@AcademicYear", request.AcademicYear },
                        { "@Programme", request.Programme },
                        { "@Branch", request.Branch },
                        { "@Semester", request.Semester },
                        { "@Section", request.Section },
                        { "@Year", request.Year },
                        { "@SubjectName", request.SubjectName ?? request.SubjectCode },
                        { "@Lecturer", request.Lecturer ?? request.UserId }
                    })
                };

                DataTable dt = ExecuteSpWithFallback(con, spCandidates);
                if (dt == null || dt.Rows.Count == 0)
                {
                    dt = ExecuteSqlStudentResultsFallback(con, request);
                }

                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get_RegNo
        /// Procedure: SP_Get_RegisterNo '@RegistrationNo','@Programme','@Branch','@Year','@Semester','@SubjectName','@Section','@Sessional'
        /// </summary>
        [HttpGet]
        [Route("check-regno")]
        public IActionResult CheckRegNo([FromQuery] ResultEntryFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
                {
                    ("SP_Get_RegisterNo", new Dictionary<string, object?>
                    {
                        { "@RegistrationNo", request.RegistrationNo },
                        { "@Programme", request.Programme },
                        { "@Branch", request.Branch },
                        { "@Year", request.Year },
                        { "@Semester", request.Semester },
                        { "@SubjectName", request.SubjectName ?? request.SubjectCode },
                        { "@Section", request.Section },
                        { "@Sessional", request.SESSIONAL ?? request.SubjectCode }
                    }),
                    ("SP_Get_RegNo", new Dictionary<string, object?>
                    {
                        { "@RegistrationNo", request.RegistrationNo },
                        { "@Programme", request.Programme },
                        { "@Branch", request.Branch },
                        { "@Year", request.Year },
                        { "@Semester", request.Semester },
                        { "@Section", request.Section },
                        { "@SubjectName", request.SubjectName ?? request.SubjectCode }
                    })
                };

                DataTable dt = ExecuteSpWithFallback(con, spCandidates);
                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// SaveResultMarksEntry (Single Record Save)
        /// Procedure: SP_RESULTENTRY_SAVE '@id','@RegistrationNo','@Date','@Programme','@Branch',@Year,'@Semester','@Section','@Stream','@SubjectName','@Sub_Max_MRK','@Max_MRK','@Marks','@Grade','@CGPA','@SGPA','@AcademicYear','@UserId'
        /// </summary>
        [HttpPost]
        [Route("save-single")]
        public IActionResult SaveSingle([FromBody] ResultEntrySaveModel request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Invalid request payload." });
                }

                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                int rows = SaveResultEntryRecord(con, request);
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
        /// SaveResultMarksEntry (Bulk Record Save)
        /// Procedure: SP_RESULTENTRY_SAVE
        /// </summary>
        [HttpPost]
        [Route("save")]
        public IActionResult Save([FromBody] ResultEntrySaveModel request)
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
                    var item = new ResultEntrySaveModel
                    {
                        Id = student.Id ?? "0",
                        RegistrationNo = student.RegistrationNo,
                        Date = request.Date,
                        Programme = request.Programme,
                        Branch = request.Branch,
                        Year = request.Year,
                        Semester = request.Semester,
                        Section = request.Section,
                        Stream = request.Stream ?? "1",
                        SubjectName = request.SubjectName ?? request.SubjectCode,
                        Sub_Max_MRK = request.Sub_Max_MRK ?? request.SubMaxMrk,
                        SubMaxMrk = request.SubMaxMrk ?? request.Sub_Max_MRK,
                        Max_MRK = request.Max_MRK ?? request.MaxMrk,
                        MaxMrk = request.MaxMrk ?? request.Max_MRK,
                        Marks = student.Marks,
                        Grade = student.Grade,
                        SGPA = student.SGPA,
                        CGPA = student.CGPA,
                        AcademicYear = request.AcademicYear,
                        Lecturer = request.Lecturer ?? request.UserId,
                        UserId = request.UserId ?? request.Lecturer
                    };

                    int res = SaveResultEntryRecord(con, item);
                    if (res > 0) successCount++;
                }

                return Ok(new { success = true, message = $"{successCount} student results saved successfully.", data = successCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        #region Helpers

        private static int SaveResultEntryRecord(SqlConnection con, ResultEntrySaveModel request)
        {
            var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
            {
                ("SP_RESULTENTRY_SAVE", new Dictionary<string, object?>
                {
                    { "@id", request.Id ?? "0" },
                    { "@RegistrationNo", request.RegistrationNo ?? (object)DBNull.Value },
                    { "@Date", string.IsNullOrWhiteSpace(request.Date) ? DateTime.Now.ToString("dd-MM-yyyy") : request.Date },
                    { "@Programme", request.Programme ?? (object)DBNull.Value },
                    { "@Branch", request.Branch ?? (object)DBNull.Value },
                    { "@Year", request.Year ?? (object)DBNull.Value },
                    { "@Semester", request.Semester ?? (object)DBNull.Value },
                    { "@Section", request.Section ?? (object)DBNull.Value },
                    { "@Stream", request.Stream ?? "1" },
                    { "@SubjectName", request.SubjectName ?? request.SubjectCode ?? (object)DBNull.Value },
                    { "@Sub_Max_MRK", request.Sub_Max_MRK ?? request.SubMaxMrk ?? (object)DBNull.Value },
                    { "@Max_MRK", request.Max_MRK ?? request.MaxMrk ?? (object)DBNull.Value },
                    { "@Marks", request.Marks ?? (object)DBNull.Value },
                    { "@Grade", request.Grade ?? (object)DBNull.Value },
                    { "@CGPA", request.CGPA ?? (object)DBNull.Value },
                    { "@SGPA", request.SGPA ?? (object)DBNull.Value },
                    { "@AcademicYear", request.AcademicYear ?? (object)DBNull.Value },
                    { "@UserId", request.UserId ?? request.Lecturer ?? (object)DBNull.Value }
                }),
                ("SP_SaveResultMarksEntry", new Dictionary<string, object?>
                {
                    { "@id", request.Id ?? "0" },
                    { "@RegistrationNo", request.RegistrationNo ?? (object)DBNull.Value },
                    { "@Date", string.IsNullOrWhiteSpace(request.Date) ? DateTime.Now.ToString("dd-MM-yyyy") : request.Date },
                    { "@Programme", request.Programme ?? (object)DBNull.Value },
                    { "@Branch", request.Branch ?? (object)DBNull.Value },
                    { "@Year", request.Year ?? (object)DBNull.Value },
                    { "@Semester", request.Semester ?? (object)DBNull.Value },
                    { "@Section", request.Section ?? (object)DBNull.Value },
                    { "@Stream", request.Stream ?? "1" },
                    { "@SubjectName", request.SubjectName ?? request.SubjectCode ?? (object)DBNull.Value },
                    { "@Sub_Max_MRK", request.Sub_Max_MRK ?? request.SubMaxMrk ?? (object)DBNull.Value },
                    { "@Max_MRK", request.Max_MRK ?? request.MaxMrk ?? (object)DBNull.Value },
                    { "@Marks", request.Marks ?? (object)DBNull.Value },
                    { "@Grade", request.Grade ?? (object)DBNull.Value },
                    { "@CGPA", request.CGPA ?? (object)DBNull.Value },
                    { "@SGPA", request.SGPA ?? (object)DBNull.Value },
                    { "@AcademicYear", request.AcademicYear ?? (object)DBNull.Value },
                    { "@Lecturer", request.Lecturer ?? request.UserId ?? (object)DBNull.Value }
                })
            };

            int rows = ExecuteSpNonQueryWithFallback(con, spCandidates);
            if (rows <= 0)
            {
                rows = ExecuteSqlSaveResultFallback(con, request);
            }
            return rows;
        }

        private static int ExecuteSqlSaveResultFallback(SqlConnection con, ResultEntrySaveModel item)
        {
            try
            {
                string sql = @"
                    IF EXISTS (SELECT 1 FROM tbl_Result_Marks WHERE RegistrationNo = @RegistrationNo AND SubjectName = @SubjectName AND Semester = @Semester AND AcademicYear = @AcademicYear)
                    BEGIN
                        UPDATE tbl_Result_Marks
                        SET Date = @Date, Programme = @Programme, Branch = @Branch, Year = @Year, Section = @Section, Stream = @Stream,
                            Sub_Max_MRK = @Sub_Max_MRK, Max_MRK = @Max_MRK, Marks = @Marks, Grade = @Grade, SGPA = @SGPA, CGPA = @CGPA, Lecturer = @Lecturer
                        WHERE RegistrationNo = @RegistrationNo AND SubjectName = @SubjectName AND Semester = @Semester AND AcademicYear = @AcademicYear;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO tbl_Result_Marks (RegistrationNo, Date, Programme, Branch, Year, Semester, Section, Stream, SubjectName, Sub_Max_MRK, Max_MRK, Marks, Grade, SGPA, CGPA, AcademicYear, Lecturer)
                        VALUES (@RegistrationNo, @Date, @Programme, @Branch, @Year, @Semester, @Section, @Stream, @SubjectName, @Sub_Max_MRK, @Max_MRK, @Marks, @Grade, @SGPA, @CGPA, @AcademicYear, @Lecturer);
                    END";

                using SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@RegistrationNo", item.RegistrationNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Date", string.IsNullOrWhiteSpace(item.Date) ? DateTime.Now.ToString("dd-MM-yyyy") : item.Date);
                cmd.Parameters.AddWithValue("@Programme", item.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", item.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", item.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", item.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Section", item.Section ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Stream", item.Stream ?? "1");
                cmd.Parameters.AddWithValue("@SubjectName", item.SubjectName ?? item.SubjectCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Sub_Max_MRK", item.Sub_Max_MRK ?? item.SubMaxMrk ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Max_MRK", item.Max_MRK ?? item.MaxMrk ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Marks", item.Marks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Grade", item.Grade ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SGPA", item.SGPA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CGPA", item.CGPA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicYear", item.AcademicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Lecturer", item.Lecturer ?? item.UserId ?? (object)DBNull.Value);

                return cmd.ExecuteNonQuery();
            }
            catch
            {
                return 0;
            }
        }

        private static DataTable ExecuteSqlStudentResultsFallback(SqlConnection con, ResultEntryFilterModel request)
        {
            try
            {
                string sql = @"
                    SELECT 
                        ID AS lblid,
                        RegistrationNo AS lblRegNo,
                        Marks AS lblResultMarks,
                        Grade AS lblResultGrade,
                        SGPA AS lblSGPA,
                        CGPA AS lblCGPA
                    FROM tbl_Result_Marks
                    WHERE AcademicYear = @AcademicYear 
                      AND Programme = @Programme 
                      AND Branch = @Branch 
                      AND Semester = @Semester 
                      AND Section = @Section 
                      AND Year = @Year 
                      AND SubjectName = @SubjectName";

                using SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SubjectName", request.SubjectName ?? request.SubjectCode ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch
            {
                return new DataTable();
            }
        }

        private static string GetAutoSSNo(SqlConnection con, string academicYear)
        {
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

            return autoSSNo;
        }

        private static DataTable LoadProgrammesData(SqlConnection con, string? academicYear)
        {
            var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
            {
                ("SP_MarksEntry_Programme_List", new Dictionary<string, object?> { { "@AcademicYear", academicYear } }),
                ("SP_BookEntry_Programme_List", new Dictionary<string, object?> { { "@AcademicYear", academicYear } })
            };

            return ExecuteSpWithFallback(con, spCandidates);
        }

        private static DataTable LoadFacultyData(SqlConnection con, string? department)
        {
            try
            {
                string sql = "select distinct EmpID, EmpID + '-' + FName AS FName from tbl_EmployeeDetails";
                using SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count > 0) return dt;
            }
            catch { }

            var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
            {
                ("SP_USERWISE_LoadEmpID", new Dictionary<string, object?> { { "@Department", department ?? "" }, { "@WorkMode", "" } })
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

        private static int ExecuteSpNonQueryWithFallback(SqlConnection con, List<(string spName, Dictionary<string, object?> paramsDict)> spCandidates)
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

                    return cmd.ExecuteNonQuery();
                }
                catch (SqlException ex) when (ex.Number == 2812 || ex.Number == 15009 || ex.Number == 8144 || ex.Number == 201)
                {
                    continue;
                }
            }

            return 0;
        }

        #endregion
    }
}
