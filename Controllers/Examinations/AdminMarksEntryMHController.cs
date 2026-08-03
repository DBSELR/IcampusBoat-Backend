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
    public class AdminMarksEntryMHController : ControllerBase
    {
        /// <summary>
        /// Get Auto ID / Student Serial Number for Admin Marks Entry MH.
        /// </summary>
        [HttpGet]
        [Route("autoid")]
        public IActionResult GetAutoId([FromQuery] string academicYear)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
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

                return Ok(new { success = true, message = "Success", data = autoSSNo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch list of programmes for the selected academic year and department.
        /// </summary>
        [HttpGet]
        [Route("programmes")]
        public IActionResult GetProgrammes([FromQuery] string academicYear, [FromQuery] string? department = "", [FromQuery] string? userId = "")
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
        /// Fetch list of branches for the selected programme.
        /// </summary>
        [HttpGet]
        [Route("branches")]
        public IActionResult GetBranches([FromQuery] string Programme, string AcademicYear, [FromQuery] string? Department = "", [FromQuery] string? UserId = "")
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                var spList = new List<(string spName, Dictionary<string, object?> paramsDict)>
                {
                    ("SP_USERWISE_LoadBranch", new Dictionary<string, object?>
                    {
                        { "@DEPT", Department },
                        { "@CourseCode", Programme },
                        { "@AcademicYear", AcademicYear },
                        { "@EmpID", UserId }
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
        /// Fetch sections for selected programme, branch, and year.
        /// </summary>
        [HttpGet]
        [Route("sections")]
        public IActionResult GetSections([FromQuery] string Programme, [FromQuery] string Branch, [FromQuery] string Year)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_Get_Section", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@COURSECODE", Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BRANCHCODE", Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@STDYEAR", Year ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new {success = true, message = "Success", data = DAL.DataTableToList(dt) });
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
        public IActionResult GetSubjects([FromQuery] string AcademicYear, [FromQuery] string Programme, [FromQuery] string Branch, [FromQuery] string Year, [FromQuery] string Semester, [FromQuery] string? Section = "", [FromQuery] string? UserId = "")
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                var spList = new List<(string spName, Dictionary<string, object?> paramsDict)>
                {
                    ("SP_LOADSUBJECTS_MH", new Dictionary<string, object?>
                    {
                        { "@COURSECODE", Programme},
                        { "@Sem", Semester},
                        { "@EMPID", UserId}
                    }),
                  
                };

                DataTable dt = ExecuteSpWithFallback(con, spList);
                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt)});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message});
            }
        }

        /// <summary>
        /// Save single student mark record.
        /// </summary>
        [HttpPost]
        [Route("save-marks")]
        public IActionResult SaveMarks([FromBody] AdminMarksEntryMHSingleSaveModel request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Invalid request payload." });
                }

                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_RESULTENTRY_SAVE", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", request.Id ?? "0");
                cmd.Parameters.AddWithValue("@REGISTRATIONNO", request.RegistrationNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Date", string.IsNullOrWhiteSpace(request.Date) ? DateTime.Now.ToString("dd-MM-yyyy") : request.Date);
                cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@YEAR", request.Year ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SEMESTER", request.Semester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SECTION", request.Section ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Stream", request.Stream ?? "1");
                cmd.Parameters.AddWithValue("@SUBJECTNAME", request.SubjectCode ?? request.SubjectName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SUBMAXMRK", request.SubMaxMrk ?? request.MaxMarks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MAXMARKS", request.MaxMarks ?? request.SubMaxMrk ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MARKS", request.Marks ?? "0");
                cmd.Parameters.AddWithValue("@GRADE", request.Grade ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CGPA", request.CGPA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SGPA", request.SGPA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ACADEMICYEAR", request.AcademicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UserId", request.UserId ?? (object)DBNull.Value);

                int res = 0;
                try
                {
                    res = cmd.ExecuteNonQuery();
                }
                catch (SqlException ex) when (ex.Number == 2812 || ex.Number == 15009)
                {
                    // Fallback to direct SQL statement if Stored Procedure doesn't exist
                    res = ExecuteSqlSaveResultFallback(con, request);
                }

                if (res <= 0)
                {
                    // If no exception, but still not saved, try raw SQL fallback
                    res = ExecuteSqlSaveResultFallback(con, request);
                }

                if (res <= 0)
                {
                    return BadRequest(new { success = false, message = "Data not saved." });
                }

                return Ok(new { success = true, message = "Data Saved Successfully", data = res });
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
                    var singleSave = new AdminMarksEntryMHSingleSaveModel
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
                        SubjectCode = request.SubjectCode ?? request.SubjectName,
                        SubjectName = request.SubjectName ?? request.SubjectCode,
                        SubMaxMrk = student.SubMaxMrk ?? student.MaxMarks,
                        MaxMarks = student.MaxMarks ?? student.SubMaxMrk,
                        Marks = student.Marks ?? "0",
                        Grade = student.Grade,
                        CGPA = student.CGPA,
                        SGPA = student.SGPA,
                        AcademicYear = request.AcademicYear,
                        UserId = request.UserId
                    };

                    using SqlCommand cmd = new SqlCommand("SP_RESULTENTRY_SAVE", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ID", singleSave.Id ?? "0");
                    cmd.Parameters.AddWithValue("@REGISTRATIONNO", singleSave.RegistrationNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Date", string.IsNullOrWhiteSpace(singleSave.Date) ? DateTime.Now.ToString("dd-MM-yyyy") : singleSave.Date);
                    cmd.Parameters.AddWithValue("@PROGRAMME", singleSave.Programme ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BRANCH", singleSave.Branch ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@YEAR", singleSave.Year ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SEMESTER", singleSave.Semester ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SECTION", singleSave.Section ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Stream", singleSave.Stream ?? "1");
                    cmd.Parameters.AddWithValue("@SUBJECTNAME", singleSave.SubjectName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SUBMAXMRK", singleSave.SubMaxMrk ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MAXMARKS", singleSave.MaxMarks ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MARKS", singleSave.Marks ?? "0");
                    cmd.Parameters.AddWithValue("@GRADE", singleSave.Grade ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CGPA", singleSave.CGPA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SGPA", singleSave.SGPA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ACADEMICYEAR", singleSave.AcademicYear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", singleSave.UserId ?? (object)DBNull.Value);

                    int res = 0;
                    try
                    {
                        res = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex) when (ex.Number == 2812 || ex.Number == 15009)
                    {
                        res = ExecuteSqlSaveResultFallback(con, singleSave);
                    }

                    if (res <= 0)
                    {
                        res = ExecuteSqlSaveResultFallback(con, singleSave);
                    }

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
            var spCandidates = new List<(string spName, Dictionary<string, object?> paramsDict)>
            {
                ("SP_ADM_STDDATA_Programme_LIST", new Dictionary<string, object?> { { "@AcademicYear", academicYear } }),
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

        private static int ExecuteSqlSaveResultFallback(SqlConnection con, AdminMarksEntryMHSingleSaveModel item)
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
                cmd.Parameters.AddWithValue("@SubjectName", item.SubjectCode ?? item.SubjectName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Sub_Max_MRK", item.SubMaxMrk ?? item.MaxMarks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Max_MRK", item.MaxMarks ?? item.SubMaxMrk ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Marks", item.Marks ?? "0");
                cmd.Parameters.AddWithValue("@Grade", item.Grade ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SGPA", item.SGPA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CGPA", item.CGPA ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicYear", item.AcademicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Lecturer", item.UserId ?? (object)DBNull.Value);

                return cmd.ExecuteNonQuery();
            }
            catch
            {
                return 0;
            }
        }

        #endregion
    }

}
