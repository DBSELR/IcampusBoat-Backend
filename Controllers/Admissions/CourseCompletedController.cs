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
    public class CourseCompletedController : ControllerBase
    {
        private static string FormatDateForSql(string? inputDate)
        {
            if (string.IsNullOrWhiteSpace(inputDate))
                return string.Empty;

            if (DateTime.TryParse(inputDate, out DateTime parsedDate))
            {
                return parsedDate.ToString("yyyy-MM-dd");
            }
            return inputDate;
        }

        private static string FormatDateForDisplay(object? dbDate)
        {
            if (dbDate == null || dbDate == DBNull.Value || string.IsNullOrWhiteSpace(dbDate.ToString()))
                return string.Empty;

            if (DateTime.TryParse(dbDate.ToString(), out DateTime parsedDate))
            {
                return parsedDate.ToString("dd-MM-yyyy");
            }
            return dbDate.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Initial load API returning next Study Certificate Course Completed number, academic years, list of issued certificates, and default date.
        /// </summary>
        [HttpGet("load")]
        public IActionResult Load()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // 1. Get next auto Course Certificate number
                    string autoScNo = "";
                    using (SqlCommand cmd = new SqlCommand("SP_SCNo_AUTOSCNO_Course_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SCNO", "");
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            autoScNo = result.ToString() ?? "";
                        }
                    }

                    // 2. Get Academic Years List
                    DataTable dtYears = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_Course_AcademicYear_list", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtYears);
                        }
                    }

                    // 3. Get Course Completed List
                    DataTable dtList = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_CourseCompleted_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtList);
                        }
                    }

                    string todayDate = DateTime.UtcNow.AddHours(5).AddMinutes(30).ToString("dd-MM-yyyy");

                    return Ok(new
                    {
                        success = true,
                        message = "Success",
                        data = new
                        {
                            autoScNo = autoScNo,
                            academicYears = DAL.DataTableToList(dtYears),
                            courseCompletedList = DAL.DataTableToList(dtList),
                            defaultDate = todayDate
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
        /// Get next auto Course Completed Certificate number.
        /// </summary>
        [HttpGet("auto-scno")]
        public IActionResult GetAutoScNo([FromQuery] string? scNo = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_SCNo_AUTOSCNO_Course_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SCNO", scNo ?? "");
                        object result = cmd.ExecuteScalar();
                        string generatedNo = result != null ? result.ToString() ?? "" : "";
                        return Ok(new { success = true, autoScNo = generatedNo });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Search course completed certificates list.
        /// </summary>
        [HttpGet("list")]
        public IActionResult GetCourseCompletedCertificates([FromQuery] string? searchName = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();

                    if (!string.IsNullOrWhiteSpace(searchName))
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_Course_SSNO_Search", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SearchName", searchName);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dt);
                            }
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_CourseCompleted_LIST", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dt);
                            }
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
        /// Fetch student details by Registration/Admission Number.
        /// </summary>
        [HttpGet("student-details/{regNo}")]
        public IActionResult GetStudentDetails(string regNo)
        {
            if (string.IsNullOrWhiteSpace(regNo))
            {
                return BadRequest(new { success = false, message = "Registration Number is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // Load student course certificate data
                    DataTable dtStudent = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_CourseCertificate_SSno_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", regNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtStudent);
                        }
                    }

                    if (dtStudent.Rows.Count == 0)
                    {
                        return Ok(new { success = false, message = "SSNO not existed in student data." });
                    }

                    DataRow row = dtStudent.Rows[0];

                    bool isIssued = string.Equals(row["ISSUED"]?.ToString(), "YES", StringComparison.OrdinalIgnoreCase);

                    var studentData = new
                    {
                        admissionDate = FormatDateForDisplay(row["AdmissionDate"]),
                        studentName = row["SName"]?.ToString() ?? "",
                        fatherName = row["FName"]?.ToString() ?? "",
                        programme = row["COURSE"]?.ToString() ?? "",
                        branch = row["BRANCHNAME"]?.ToString() ?? "",
                        year = row["SYear"]?.ToString() ?? "",
                        fromAcademicYear = row["JAcadamicYear"]?.ToString() ?? "",
                        toAcademicYear = isIssued ? row["ToAcademicYear"]?.ToString() : "",
                        id = isIssued ? row["id"]?.ToString() : "0",
                        scNo = isIssued ? row["SCNO"]?.ToString() : "",
                        isIssued = isIssued
                    };

                    return Ok(new
                    {
                        success = true,
                        message = "Student details loaded successfully.",
                        student = studentData
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Save or Update Course Completed Certificate details.
        /// </summary>
        [HttpPost("save")]
        public IActionResult SaveCourseCertificate([FromBody] CourseCompletedSaveRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RegNo))
            {
                return BadRequest(new { success = false, message = "Student registration number is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    string id = string.IsNullOrWhiteSpace(request.Id) ? "0" : request.Id;

                    // Fetch next auto SC number if blank and it's a new record
                    string scNo = request.SCNO ?? "";
                    if (id == "0" && string.IsNullOrWhiteSpace(scNo))
                    {
                        using (SqlCommand cmdSC = new SqlCommand("SP_SCNo_AUTOSCNO_Course_LIST", con))
                        {
                            cmdSC.CommandType = CommandType.StoredProcedure;
                            cmdSC.Parameters.AddWithValue("@SCNO", "");
                            object res = cmdSC.ExecuteScalar();
                            if (res != null) scNo = res.ToString() ?? "";
                        }
                    }

                    string query = @"EXEC SP_CourseCompleted_SAVE 
                        @id, @SCNO, @Date, @SSNO, @AdmissionDate, @StudentName, 
                        @FatherName, @Programme, @Branch, @Year, @FromAcademicYear, 
                        @ToAcademicYear, @AcademicYear, @SCType, @Conduct";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@SCNO", scNo);
                        cmd.Parameters.AddWithValue("@Date", FormatDateForSql(request.Date));
                        cmd.Parameters.AddWithValue("@SSNO", request.RegNo);
                        cmd.Parameters.AddWithValue("@AdmissionDate", FormatDateForSql(request.AdmissionDate));
                        cmd.Parameters.AddWithValue("@StudentName", request.StudentName ?? "");
                        cmd.Parameters.AddWithValue("@FatherName", request.FatherName ?? "");
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? "");
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? "");
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? "0");
                        cmd.Parameters.AddWithValue("@FromAcademicYear", request.FromAcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@ToAcademicYear", request.ToAcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@SCType", request.SCType ?? "");
                        cmd.Parameters.AddWithValue("@Conduct", request.Conduct ?? "");

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                success = true,
                                message = id == "0" ? "Student Data Saved Successfully" : "Student Data Updated Successfully"
                            });
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Unable to save course completed certificate details." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete course completed certificate record by ID.
        /// </summary>
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteCourseCompleted(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { success = false, message = "ID is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_CourseCompleted_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        return Ok(new
                        {
                            success = rowsAffected > 0,
                            message = rowsAffected > 0 ? "Record Deleted Successfully" : "No record deleted."
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get dataset required for printing Course Completed Certificate.
        /// </summary>
        [HttpGet("print-data")]
        public IActionResult GetPrintData([FromQuery] string id, [FromQuery] string ssno)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_Adm_PrintCourse", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id ?? "");
                        cmd.Parameters.AddWithValue("@SSNO", ssno ?? "");

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
    }
}
