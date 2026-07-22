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
    public class ExpenditureController : ControllerBase
    {
        /// <summary>
        /// Fetch student details and the corresponding expenditure heads list for the student's course.
        /// </summary>
        [HttpGet("student-details/{regNo}")]
        public IActionResult GetStudentDetails(string regNo)
        {
            if (string.IsNullOrWhiteSpace(regNo))
            {
                return BadRequest(new { success = false, message = "Register number is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // 1. Get Student Name, Course, and Branch Details
                    DataTable dtStudent = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_EXP_SSNO", con))
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
                        return Ok(new
                        {
                            success = false,
                            message = "Register Number Not Exist in Student Data"
                        });
                    }

                    DataRow row = dtStudent.Rows[0];
                    string course = row["Course"]?.ToString() ?? "";

                    // 2. Load Expenditure Heads for the student's Course
                    DataTable dtHeads = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_EXE_Cert_Load", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@COURSE", course);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtHeads);
                        }
                    }

                    var studentData = new
                    {
                        studentName = row["SName"]?.ToString() ?? "",
                        course = course,
                        branch = row["BranchName"]?.ToString() ?? ""
                    };

                    return Ok(new
                    {
                        success = true,
                        message = "Student details and expenditure heads loaded successfully.",
                        student = studentData,
                        expHeadsList = DAL.DataTableToList(dtHeads)
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Save the selected expenditure heads for a student.
        /// </summary>
        [HttpPost("save")]
        public IActionResult SaveExpenditureCertificate([FromBody] ExpenditureSaveRequest request)
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

                    // 1. Delete existing records for the student
                    string deleteQuery = "DELETE FROM TBL_EXPENDITURE_CERTIFICATE WHERE SSNO = @RegNo";
                    using (SqlCommand cmdDelete = new SqlCommand(deleteQuery, con))
                    {
                        cmdDelete.Parameters.AddWithValue("@RegNo", request.RegNo);
                        cmdDelete.ExecuteNonQuery();
                    }

                    int savedCount = 0;

                    // 2. Loop and save checked expenditure heads
                    if (request.Heads != null && request.Heads.Count > 0)
                    {
                        foreach (var item in request.Heads)
                        {
                            if (item.IsActive)
                            {
                                using (SqlCommand cmdSave = new SqlCommand("SP_EXP_CERTIFICATE_SAVE", con))
                                {
                                    cmdSave.CommandType = CommandType.StoredProcedure;

                                    cmdSave.Parameters.Add("@SSNO", SqlDbType.VarChar, 50).Value = request.RegNo;
                                    cmdSave.Parameters.Add("@SNAME", SqlDbType.VarChar, 225).Value = request.StudentName ?? "";
                                    cmdSave.Parameters.Add("@COURSE", SqlDbType.VarChar, 30).Value = request.Course ?? "";
                                    cmdSave.Parameters.Add("@YEAR", SqlDbType.Int).Value = Convert.ToInt32(item.Year);
                                    cmdSave.Parameters.Add("@EXPHEADS", SqlDbType.VarChar, 225).Value = item.ExpenditureHeads ?? "";
                                    cmdSave.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(item.Amount);
                                    cmdSave.Parameters.Add("@ACYR", SqlDbType.VarChar, 225).Value = request.AcademicYear ?? "";

                                    cmdSave.ExecuteNonQuery();
                                    savedCount++;
                                }
                            }
                        }
                    }

                    return Ok(new
                    {
                        success = true,
                        message = "Data saved successfully",
                        savedCount = savedCount
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get report print dataset for Expenditure Certificate.
        /// </summary>
        [HttpGet("print-data")]
        public IActionResult GetPrintData([FromQuery] string regNo, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(regNo) || string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Register number and academic year are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SPR_EXPENDITURE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@REGNO", regNo);
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
    }
}
