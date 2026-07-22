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
    public class StudyCertificateController : ControllerBase
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
        /// Initial load API returning next Study Certificate number, list of issued study certificates, and default dates.
        /// </summary>
        [HttpGet("load")]
        public IActionResult Load()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // 1. Get next auto Study Certificate number
                    string autoScNo = "";
                    using (SqlCommand cmd = new SqlCommand("SP_SCNo_AUTOSCNO_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SCNO", "");
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            autoScNo = result.ToString() ?? "";
                        }
                    }

                    // 2. Get Study Certificate List
                    DataTable dtList = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_StudyCertificate_LIST", con))
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
                            studyCertificatesList = DAL.DataTableToList(dtList),
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
        /// Get next auto Study Certificate number.
        /// </summary>
        [HttpGet("auto-scno")]
        public IActionResult GetAutoScNo([FromQuery] string? scNo = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_SCNo_AUTOSCNO_LIST", con))
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
        /// Search study certificates list.
        /// </summary>
        [HttpGet("list")]
        public IActionResult GetStudyCertificates([FromQuery] string? searchName = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();

                    if (!string.IsNullOrWhiteSpace(searchName))
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_Study_SSNO_Search", con))
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
                        using (SqlCommand cmd = new SqlCommand("SP_StudyCertificate_LIST", con))
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

                    // 1. Resolve internal SSNO
                    string ssNo = "";
                    using (SqlCommand cmdSSNO = new SqlCommand("[SP_StdCertificate_REGNO_SSNO_LOAD]", con))
                    {
                        cmdSSNO.CommandType = CommandType.StoredProcedure;
                        cmdSSNO.Parameters.AddWithValue("@SSNO", regNo);
                        using (SqlDataReader reader = cmdSSNO.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ssNo = reader[0]?.ToString() ?? "";
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(ssNo))
                    {
                        return Ok(new { success = false, message = "RegNo not existed in student data." });
                    }

                    // 2. Load student study details
                    DataTable dtStudent = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_Study_SSno_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RegistrationNo", ssNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtStudent);
                        }
                    }

                    if (dtStudent.Rows.Count == 0)
                    {
                        return Ok(new { success = false, message = "RegNo not existed in student data." });
                    }

                    DataRow row = dtStudent.Rows[0];

                    var studentData = new
                    {
                        admissionDate = FormatDateForDisplay(row["AdmissionDate"]),
                        studentName = row["SName"]?.ToString() ?? "",
                        fatherName = row["FName"]?.ToString() ?? "",
                        programme = row["Course"]?.ToString() ?? "",
                        programmeCode = row["CourseCode"]?.ToString() ?? "",
                        branch = row["Branch"]?.ToString() ?? "",
                        branchCode = row["BranchCode"]?.ToString() ?? "",
                        year = row["SYear"]?.ToString() ?? "",
                        fromDate = FormatDateForDisplay(row["AdmissionDate"]),
                        facYr = row["FACYR"]?.ToString() ?? "",
                        tacYr = row["TACYR"]?.ToString() ?? "",
                        registrationNo = row["RegistrationNo"]?.ToString() ?? "",
                        semester = row["SSemester"]?.ToString() ?? "",
                        purpose = "Bus Pass"
                    };

                    return Ok(new
                    {
                        success = true,
                        message = "Student details loaded successfully.",
                        student = studentData,
                        ssNo = ssNo
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Save or Update Study Certificate details.
        /// </summary>
        [HttpPost("save")]
        public IActionResult SaveStudyCertificate([FromBody] StudyCertificateSaveRequest request)
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

                    // 1. Resolve internal SSNO
                    string ssNo = "";
                    using (SqlCommand cmdSSNO = new SqlCommand("[SP_StdCertificate_REGNO_SSNO_LOAD]", con))
                    {
                        cmdSSNO.CommandType = CommandType.StoredProcedure;
                        cmdSSNO.Parameters.AddWithValue("@SSNO", request.RegNo);
                        using (SqlDataReader reader = cmdSSNO.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ssNo = reader[0]?.ToString() ?? "";
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(ssNo))
                    {
                        ssNo = request.RegNo; // Fallback to raw registration no
                    }

                    string id = string.IsNullOrWhiteSpace(request.Id) ? "0" : request.Id;

                    // 2. Fetch next auto SC number if blank and it's a new record
                    string scNo = request.SCNO ?? "";
                    if (id == "0" && string.IsNullOrWhiteSpace(scNo))
                    {
                        using (SqlCommand cmdSC = new SqlCommand("SP_SCNo_AUTOSCNO_LIST", con))
                        {
                            cmdSC.CommandType = CommandType.StoredProcedure;
                            cmdSC.Parameters.AddWithValue("@SCNO", "");
                            object res = cmdSC.ExecuteScalar();
                            if (res != null) scNo = res.ToString() ?? "";
                        }
                    }

                    string query = @"EXEC SP_StudyCertificate_SAVE 
                        @id, @SCNO, @Date, @SSNO, @AdmissionDate, @StudentName, 
                        @FatherName, @Programme, @Branch, @Year, @FromDate, @ToDate, 
                        @SCType, @AcademicYear, @FACYR, @TACYR, @Conduct, @Type, @purpose";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@SCNO", scNo);
                        cmd.Parameters.AddWithValue("@Date", FormatDateForSql(request.Date));
                        cmd.Parameters.AddWithValue("@SSNO", ssNo);
                        cmd.Parameters.AddWithValue("@AdmissionDate", FormatDateForSql(request.AdmissionDate));
                        cmd.Parameters.AddWithValue("@StudentName", request.StudentName ?? "");
                        cmd.Parameters.AddWithValue("@FatherName", request.FatherName ?? "");
                        
                        // Handle splitting of programme similar to split in Web Forms logic
                        string prog = request.Programme ?? "";
                        if (prog.Contains('-'))
                        {
                            prog = prog.Split('-')[0];
                        }
                        cmd.Parameters.AddWithValue("@Programme", prog);

                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? "");
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? "0");
                        cmd.Parameters.AddWithValue("@FromDate", FormatDateForSql(request.FromDate));
                        cmd.Parameters.AddWithValue("@ToDate", FormatDateForSql(request.ToDate));
                        cmd.Parameters.AddWithValue("@SCType", request.SCType ?? "");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@FACYR", request.FACYR ?? "");
                        cmd.Parameters.AddWithValue("@TACYR", request.TACYR ?? "");
                        cmd.Parameters.AddWithValue("@Conduct", request.Conduct ?? "");
                        cmd.Parameters.AddWithValue("@Type", request.Type ?? "");
                        cmd.Parameters.AddWithValue("@purpose", request.Purpose ?? "");

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
                            return BadRequest(new { success = false, message = "Unable to save study certificate details." });
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
        /// Delete study certificate record by ID.
        /// </summary>
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteStudyCertificate(string id)
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
                    using (SqlCommand cmd = new SqlCommand("SP_StudyCertificate_DELETE", con))
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
        /// Get dataset required for printing Study Certificate report.
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
                    using (SqlCommand cmd = new SqlCommand("Sp_Adm_PrintStudy", con))
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
