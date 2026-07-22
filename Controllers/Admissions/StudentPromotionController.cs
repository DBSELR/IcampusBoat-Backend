using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using IcampusBoatBackend.Models.Admissions;

namespace IcampusBoatBackend.Controllers.Admissions
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentPromotionController : ControllerBase
    {
        /// <summary>
        /// Fetch academic years list.
        /// </summary>
        [HttpGet("academic-years")]
        public IActionResult GetAcademicYears()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "SELECT ACADEMICYEAR AS AcadamicYear FROM tbl_AcademicYear";
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

        /// <summary>
        /// Fetch courses list for a given academic year.
        /// </summary>
        [HttpGet("courses")]
        public IActionResult GetCourses([FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Academic year is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = @"SELECT DISTINCT S.CourseCode, S.CourseCode + '-' + Course AS Course 
                                     FROM tbl_ADm_studata S 
                                     INNER JOIN tbl_Adm_Course C ON S.CourseCode = C.CourseCode 
                                     WHERE S.AcadamicYear = @AcademicYear 
                                     ORDER BY Course";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
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

        /// <summary>
        /// Fetch branches list for selected course and academic year.
        /// </summary>
        [HttpGet("branches")]
        public IActionResult GetBranches([FromQuery] string courseCode, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Course code and academic year are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_Branch_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Course", courseCode);
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
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

        /// <summary>
        /// Fetch studying years for selected course and academic year.
        /// </summary>
        [HttpGet("years")]
        public IActionResult GetYears([FromQuery] string courseCode, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Course code and academic year are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_YEARS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Course", courseCode);
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
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

        /// <summary>
        /// Fetch semesters list for selected academic year.
        /// </summary>
        [HttpGet("semesters")]
        public IActionResult GetSemesters([FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Academic year is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = @"SELECT DISTINCT CAST(SSemester AS VARCHAR(MAX)) AS SSemester 
                                     FROM tbl_ADm_studata 
                                     WHERE AcadamicYear = @AcademicYear 
                                     ORDER BY SSemester";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
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

        /// <summary>
        /// Fetch list of students matching the search criteria and calculate promotion suggestions.
        /// </summary>
        [HttpGet("students")]
        public IActionResult GetStudents([FromQuery] StudentSearchRequest search)
        {
            if (search == null || string.IsNullOrWhiteSpace(search.AcademicYear) || string.IsNullOrWhiteSpace(search.CourseCode) || string.IsNullOrWhiteSpace(search.Semester))
            {
                return BadRequest(new { success = false, message = "AcademicYear, CourseCode, and Semester are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // 1. Build dynamic AddColumn filter
                    List<string> filters = new List<string>();
                    filters.Add($"ACADAMICYEAR = '{search.AcademicYear}'");
                    filters.Add($"SSemester = '{search.Semester}'");
                    filters.Add($"S.CourseCode = '{search.CourseCode}'");

                    if (!string.IsNullOrWhiteSpace(search.Year) && !string.Equals(search.Year, "Select Year", StringComparison.OrdinalIgnoreCase))
                    {
                        filters.Add($"SYear = '{search.Year}'");
                    }
                    if (!string.IsNullOrWhiteSpace(search.BranchCode) && !string.Equals(search.BranchCode, "Select Branch", StringComparison.OrdinalIgnoreCase))
                    {
                        filters.Add($"S.BranchCode = '{search.BranchCode}'");
                    }

                    string filterString = string.Join(" AND ", filters);

                    // 2. Fetch Student List
                    DataTable dt = new DataTable();
                    string query = $@"SELECT DISTINCT STUDENTSERIALNO, REGISTRATIONNO, ADMISSIONDATE, DOB, 
                                     SNAME, MODEOFADM, C.CourseCode + '-' + C.Course Course, B.BranchCode + '-' + B.BranchName BranchName, SECTION, 
                                     ACADAMICYEAR, AYEAR, SYEAR, ASEMESTER, SSEMESTER, SECLANG, MEDIUM, CASTE, SUBCASTE, GENDER, RELIGION, TUITIONFEE, 
                                     MISCELLANEOUSFEE FROM TBL_ADM_STUDATA S 
                                     INNER JOIN TBL_ADM_BRANCH B ON S.BranchCode = B.BRANCHCODE AND S.ACADAMICYEAR = B.AcademicYear AND S.CourseCode = B.CourseCode 
                                     INNER JOIN TBL_ADM_COURSE C ON C.COURSECODE = S.CourseCode AND C.AcademicYear = S.ACADAMICYEAR 
                                     WHERE TcStatus IS NULL AND IsActive = 'true' AND {filterString} 
                                     ORDER BY REGISTRATIONNO";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    // 3. Compute promote dropdown suggestions
                    List<string> promoteSems = new List<string>();
                    List<string> promoteYears = new List<string>();
                    List<string> promoteStudyYears = new List<string>();

                    if (dt.Rows.Count > 0)
                    {
                        if (search.Semester == "1")
                        {
                            promoteSems.Add("2");
                            promoteYears.Add(search.AcademicYear);
                        }
                        else
                        {
                            promoteSems.Add("1");
                            string[] acY = search.AcademicYear.Split('-');
                            if (acY.Length == 2 && int.TryParse(acY[1], out int rightYear))
                            {
                                promoteYears.Add($"{rightYear}-{rightYear + 1}");
                            }

                            int curYear = 0;
                            if (int.TryParse(search.Year, out curYear))
                            {
                                promoteStudyYears.Add((curYear + 1).ToString());
                            }
                            else
                            {
                                promoteStudyYears.Add("All Year");
                            }
                        }
                    }

                    return Ok(new
                    {
                        success = true,
                        message = "Success",
                        data = new
                        {
                            studentsList = DAL.DataTableToList(dt),
                            suggestions = new
                            {
                                promoteSems,
                                promoteYears,
                                promoteStudyYears
                            }
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
        /// Promote students to next year / semester.
        /// </summary>
        [HttpPost("promote")]
        public IActionResult PromoteStudents([FromBody] StudentPromotionRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.AcademicYear) || string.IsNullOrWhiteSpace(request.Course))
            {
                return BadRequest(new { success = false, message = "AcademicYear and Course are required fields." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // Determine current semester parameter for SP_PROMOTE
                    string currentSem = "";
                    if (request.PromoteSem == "2")
                    {
                        currentSem = "1";
                    }
                    else if (request.PromoteSem == "1")
                    {
                        currentSem = "2";
                    }
                    else
                    {
                        currentSem = request.Sem;
                    }

                    // Handle "All" years or a specific selected year
                    List<string> yearsToPromote = new List<string>();
                    if (string.Equals(request.Year, "All", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(request.Year))
                    {
                        // Fetch all years available for the course and academic year
                        using (SqlCommand cmdYears = new SqlCommand("SP_ADM_YEARS", con))
                        {
                            cmdYears.CommandType = CommandType.StoredProcedure;
                            cmdYears.Parameters.AddWithValue("@Course", request.Course);
                            cmdYears.Parameters.AddWithValue("@AcademicYear", request.AcademicYear);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmdYears))
                            {
                                DataTable dtYears = new DataTable();
                                da.Fill(dtYears);
                                foreach (DataRow row in dtYears.Rows)
                                {
                                    if (row["ID"] != DBNull.Value)
                                    {
                                        yearsToPromote.Add(row["ID"].ToString()!);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        yearsToPromote.Add(request.Year);
                    }

                    int promoteCount = 0;

                    foreach (var year in yearsToPromote)
                    {
                        using (SqlCommand cmdPromote = new SqlCommand("SP_PROMOTE", con))
                        {
                            cmdPromote.CommandType = CommandType.StoredProcedure;
                            cmdPromote.Parameters.AddWithValue("@AcadamicYear", request.AcademicYear);
                            cmdPromote.Parameters.AddWithValue("@Year", year);
                            cmdPromote.Parameters.AddWithValue("@Sem", currentSem);
                            cmdPromote.Parameters.AddWithValue("@Course", request.Course);
                            cmdPromote.Parameters.AddWithValue("@Branch", request.Branch ?? "");
                            cmdPromote.Parameters.AddWithValue("@NewAcadamicYear", request.PromoteYear);
                            cmdPromote.Parameters.AddWithValue("@Userid", request.UserId ?? "");
                            cmdPromote.Parameters.AddWithValue("@NewStdYear", request.SPromoteYear ?? "");

                            cmdPromote.ExecuteNonQuery();
                            promoteCount++;
                        }
                    }

                    return Ok(new
                    {
                        success = true,
                        message = "Student Promoted Successfully..",
                        promotedBatches = promoteCount
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
