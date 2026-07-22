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
    public class GroupChangeController : ControllerBase
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
        /// Initial load API returning next auto receipt number, list of branch changes, and current default date.
        /// </summary>
        [HttpGet("load")]
        public IActionResult Load([FromQuery] string academicYear)
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

                    // 1. Get Auto Receipt Number
                    string autoReceiptNo = "";
                    using (SqlCommand cmd = new SqlCommand("SP_Branch_AUTORECEIPTNO_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            autoReceiptNo = result.ToString() ?? "";
                        }
                    }

                    // 2. Get Branch Change List
                    DataTable dtList = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_BranchChange_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
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
                            autoReceiptNo = autoReceiptNo,
                            branchChangeList = DAL.DataTableToList(dtList),
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
        /// Get next auto-generated receipt number.
        /// </summary>
        //[HttpGet("auto-receiptno")]
        //public IActionResult GetAutoReceiptNo([FromQuery] string academicYear)
        //{
        //    if (string.IsNullOrWhiteSpace(academicYear))
        //    {
        //        return BadRequest(new { success = false, message = "Academic year is required." });
        //    }

        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
        //        {
        //            con.Open();
        //            using (SqlCommand cmd = new SqlCommand("SP_Branch_AUTORECEIPTNO_LIST", con))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
        //                object result = cmd.ExecuteScalar();
        //                string receiptNo = result != null ? result.ToString() ?? "" : "";
        //                return Ok(new { success = true, autoReceiptNo = receiptNo });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { success = false, message = ex.Message });
        //    }
        //}

        /// <summary>
        /// Fetch student details and available change branches by Registration/Admission Number.
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
                    string ssNo = "0";
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_BranchChange_SSNO_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RegNo", regNo);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ssNo = reader[0]?.ToString() ?? "0";
                            }
                        }
                    }

                    // 2. Load student details
                    DataTable dtStudent = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_BranchChange_AdminNo_Load", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", ssNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtStudent);
                        }
                    }

                    if (dtStudent.Rows.Count == 0)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "SSNO not exist in StudentData.",
                            ssNo = ssNo
                        });
                    }

                    DataRow row = dtStudent.Rows[0];
                    string courseCode = row["CourseCode"]?.ToString() ?? "";

                    // 3. Load available branches for the student's current course
                    DataTable dtBranches = new DataTable();
                    string branchesQuery = "SELECT DISTINCT BranchCode, BranchCode + '-' + BranchName AS BRNAME FROM tbl_Adm_Branch WHERE CourseCode = @Course";
                    using (SqlCommand cmd = new SqlCommand(branchesQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@Course", courseCode);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtBranches);
                        }
                    }

                    var studentData = new
                    {
                        studentName = row["SName"]?.ToString() ?? "",
                        registrationNo = row["Registrationno"]?.ToString() ?? "",
                        courseDisplay = $"{row["CourseCode"]}-{row["CourseName"]}-{row["BranchName"]}",
                        courseCode = courseCode,
                        branchCode = row["BranchCode"]?.ToString() ?? "",
                        studyingYear = row["SYear"]?.ToString() ?? ""
                    };

                    return Ok(new
                    {
                        success = true,
                        message = "Student details loaded successfully.",
                        student = studentData,
                        ssNo = ssNo,
                        branches = DAL.DataTableToList(dtBranches)
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Load sections list for new branch selection.
        /// </summary>
        [HttpGet("sections")]
        public IActionResult GetSections([FromQuery] string academicYear, [FromQuery] string courseCode, [FromQuery] string branchCode, [FromQuery] string year)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    string query = @"SELECT DISTINCT SECTION FROM tbl_sectionmaster 
                                     WHERE AcademicYear = @AcademicYear 
                                       AND COURSECODE = @Course 
                                       AND BRANCHCODE = @Branch 
                                       AND StdYear = @Year";

                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? "");
                        cmd.Parameters.AddWithValue("@Course", courseCode ?? "");
                        cmd.Parameters.AddWithValue("@Branch", branchCode ?? "");
                        cmd.Parameters.AddWithValue("@Year", year ?? "");

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
        /// Check if the new registration number already exists.
        /// </summary>
        [HttpGet("validate-new-regno")]
        public IActionResult ValidateNewRegNo([FromQuery] string academicYear, [FromQuery] string courseCode, [FromQuery] string year, [FromQuery] string newRegNo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_CHECK_NEWREGNO", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACYR", academicYear ?? "");
                        cmd.Parameters.AddWithValue("@Course", courseCode ?? "");
                        cmd.Parameters.AddWithValue("@Year", year ?? "");
                        cmd.Parameters.AddWithValue("@NewRegNo", newRegNo ?? "");

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    bool exists = false;
                    if (dt.Rows.Count > 0 && dt.Rows[0]["Count"] != DBNull.Value)
                    {
                        exists = Convert.ToInt32(dt.Rows[0]["Count"]) > 0;
                    }

                    return Ok(new { success = true, exists = exists });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Save or Update Group / Branch Change details.
        /// </summary>
        [HttpPost("save")]
        public IActionResult SaveBranchChange([FromBody] GroupChangeSaveRequest request)
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

                    // 1. Resolve SSNO
                    string ssNo = "0";
                    using (SqlCommand cmdSSNO = new SqlCommand("SP_ADMIN_BranchChange_SSNO_LOAD", con))
                    {
                        cmdSSNO.CommandType = CommandType.StoredProcedure;
                        cmdSSNO.Parameters.AddWithValue("@RegNo ", request.RegNo);
                        using (SqlDataReader reader = cmdSSNO.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ssNo = reader[0]?.ToString() ?? "0";
                            }
                        }
                    }

                    string hid = string.IsNullOrWhiteSpace(request.Hid) ? "0" : request.Hid;

                    // 2. Fetch auto receipt number if blank and it's a new record
                    string receiptNo = request.ReceiptNo ?? "";
                    if (hid == "0" && string.IsNullOrWhiteSpace(receiptNo))
                    {
                        using (SqlCommand cmdReceipt = new SqlCommand("SP_Branch_AUTORECEIPTNO_LIST", con))
                        {
                            cmdReceipt.CommandType = CommandType.StoredProcedure;
                            cmdReceipt.Parameters.AddWithValue("@AcademicYear", request.AcademicYear);
                            object res = cmdReceipt.ExecuteScalar();
                            if (res != null) receiptNo = res.ToString() ?? "";
                        }
                    }

                    string query = @"EXEC SP_ADMIN_BranchChange_SAVE 
                        @Hid, @Date, @ReceiptNo, @SSNO, @Course, @StudentName, 
                        @Branch, @ChangedGroup, @RollNo, @Section, @AcademicYear, 
                        @Year, @NewRegno, @Regno, @Remarks";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Hid", hid);
                        cmd.Parameters.AddWithValue("@Date", FormatDateForSql(request.Date));
                        cmd.Parameters.AddWithValue("@ReceiptNo", string.IsNullOrEmpty(receiptNo) ? "0" : receiptNo);
                        cmd.Parameters.AddWithValue("@SSNO", ssNo);
                        cmd.Parameters.AddWithValue("@Course", request.Course ?? "");
                        cmd.Parameters.AddWithValue("@StudentName", request.StudentName ?? "");
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? "");
                        cmd.Parameters.AddWithValue("@ChangedGroup", request.ChangedGroup ?? "");
                        cmd.Parameters.AddWithValue("@RollNo", request.RollNo ?? "");
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? "");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? "");
                        cmd.Parameters.AddWithValue("@NewRegno", request.NewRegNo ?? "");
                        cmd.Parameters.AddWithValue("@Regno", request.RegNo ?? "");
                        cmd.Parameters.AddWithValue("@Remarks", request.Remarks ?? "");

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                success = true,
                                message = hid == "0" ? "Student Data Saved Successfully" : "Student Data Updated Successfully"
                            });
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Unable to save branch change details." });
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
        /// Delete branch change record by Hid.
        /// </summary>
        [HttpDelete("delete/{hid}")]
        public IActionResult DeleteBranchChange(string hid)
        {
            if (string.IsNullOrWhiteSpace(hid))
            {
                return BadRequest(new { success = false, message = "ID (Hid) is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_BranchChange_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Hid", hid);
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
    }
}
