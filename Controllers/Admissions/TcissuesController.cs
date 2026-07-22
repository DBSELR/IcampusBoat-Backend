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
    public class TcissuesController : ControllerBase
    {
        private static string ToRoman(int number)
        {
            return number switch
            {
                1 => "I",
                2 => "II",
                3 => "III",
                4 => "IV",
                5 => "V",
                _ => number.ToString()
            };
        }

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
        /// Initial load API returning auto TC number, list of issued TCs, and default dates.
        /// </summary>
        [HttpGet("load")]
        public IActionResult Load()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // Get Auto TC Number
                    string autoTcNo = "";
                    using (SqlCommand cmd = new SqlCommand("SP_TC_AUTOTCNO_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TCNO", DBNull.Value);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            autoTcNo = result.ToString() ?? "";
                        }
                    }

                    // Get List of TC Issues
                    DataTable dtList = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("sp_Admin_TCissues_List", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtList);
                        }
                    }

                    List<Dictionary<string, object>> tcList = DAL.DataTableToList(dtList);

                    string todayDate = DateTime.UtcNow.AddHours(5).AddMinutes(30).ToString("dd-MM-yyyy");

                    return Ok(new
                    {
                        success = true,
                        message = "Success",
                        data = new
                        {
                            autoTcNo = autoTcNo,
                            tcIssuesList = tcList,
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
        /// Get next auto-generated TC number.
        /// </summary>
        [HttpGet("auto-tcno")]
        public IActionResult GetAutoTcNo([FromQuery] string? tcNo = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TC_AUTOTCNO_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TCNo", string.IsNullOrEmpty(tcNo) ? (object)DBNull.Value : tcNo);
                        object result = cmd.ExecuteScalar();
                        string generatedNo = result != null ? result.ToString() ?? "" : "";
                        return Ok(new { success = true, autoTcNo = generatedNo });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Search issued TCs by student name, registration number, or TC number.
        /// </summary>
        [HttpGet("list")]
        public IActionResult GetTcIssuesList([FromQuery] string? searchName = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();

                    if (!string.IsNullOrWhiteSpace(searchName))
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_AdminNo_TcNo_StdName_Search", con))
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
                        using (SqlCommand cmd = new SqlCommand("sp_Admin_TCissues_List", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dt);
                            }
                        }
                    }

                    List<Dictionary<string, object>> list = DAL.DataTableToList(dt);
                    return Ok(new { success = true, data = list });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch student details and fee due status by Registration/Admission Number.
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

                    // Step 1: Resolve SSNO
                    string ssNo = "";
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_REGNO_SSNO_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", regNo);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ssNo = reader[0].ToString() ?? "";
                            }
                        }
                    }

                    int totalFeeDue = 0;
                    bool hasFeeDueBlock = false;
                    string warningMessage = "";

                    // Step 2: Check Fee Dues if SSNO exists
                    if (!string.IsNullOrEmpty(ssNo))
                    {
                        DataTable dtFee = new DataTable();
                        using (SqlCommand cmd = new SqlCommand("SP_FEE_DUE", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SSNO", ssNo);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dtFee);
                            }
                        }

                        if (dtFee != null && dtFee.Rows.Count > 0)
                        {
                            foreach (DataRow row in dtFee.Rows)
                            {
                                int due = Convert.ToInt32(row["DUE"] != DBNull.Value ? row["DUE"] : 0);
                                int sYear = Convert.ToInt32(row["SYEAR"] != DBNull.Value ? row["SYEAR"] : 0);
                                bool isScholar = string.Equals(row["schlor"]?.ToString(), "True", StringComparison.OrdinalIgnoreCase);

                                if (sYear != 4)
                                {
                                    if (due > 0)
                                    {
                                        hasFeeDueBlock = true;
                                        warningMessage = "Please Clear Fee Dues..";
                                    }
                                }
                                else if (sYear == 4 && !isScholar)
                                {
                                    if (due > 0)
                                    {
                                        hasFeeDueBlock = true;
                                        warningMessage = "Please Clear Fee Dues..";
                                    }
                                }
                                else if (sYear == 4 && isScholar)
                                {
                                    if (due > 0)
                                    {
                                        warningMessage = $"Please verify the fee due of {due} before issuing the TC for the 4th-year scholar student...";
                                    }
                                }

                                totalFeeDue += due;
                            }
                        }
                    }

                    // Step 3: Load Student TC data
                    DataTable dtStudent = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_TC_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@REGNO", regNo);
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
                            message = "Register Number not existed in student data.",
                            feeDue = totalFeeDue,
                            hasFeeDueBlock = hasFeeDueBlock,
                            warningMessage = warningMessage
                        });
                    }

                    DataRow rowStd = dtStudent.Rows[0];

                    int sYearVal = 0;
                    int.TryParse(rowStd["SYear"]?.ToString(), out sYearVal);
                    string romanYear = ToRoman(sYearVal);
                    string courseStr = rowStd["Course"]?.ToString() ?? "";
                    string branchStr = rowStd["BRANCHName"]?.ToString() ?? "";

                    bool isTcIssued = string.Equals(rowStd["tcStatus"]?.ToString(), "YES", StringComparison.OrdinalIgnoreCase);

                    var studentData = new
                    {
                        tid = isTcIssued ? rowStd["TID"]?.ToString() : "0",
                        tcNo = isTcIssued ? rowStd["TCNO"]?.ToString() : "",
                        ssNo = regNo,
                        admNo = rowStd["AdmNo"]?.ToString() ?? "",
                        studentName = rowStd["SNAME"]?.ToString() ?? "",
                        fname = rowStd["FNAME"]?.ToString() ?? "",
                        dob = FormatDateForDisplay(rowStd["DOB"]),
                        religion = rowStd["RELIGION"]?.ToString() ?? "",
                        caste = rowStd["CASTE"]?.ToString() ?? "",
                        subCaste = rowStd["SUBCASTE"]?.ToString() ?? "",
                        group = branchStr,
                        course = courseStr,
                        nationality = rowStd["NATIONALITY"]?.ToString() ?? "",
                        motherTongue = rowStd["MotherTongue"]?.ToString() ?? "",
                        dateOfAdmission = FormatDateForDisplay(rowStd["AdmissionDate"]),
                        classOfLeaving = isTcIssued ? rowStd["ClassofLeaving"]?.ToString() : $"{romanYear}-{courseStr}",
                        reasonForLeaving = isTcIssued ? rowStd["ReasonForLeaving"]?.ToString() : $"{romanYear}-{courseStr}-{branchStr}",
                        dateOfLeaving = isTcIssued ? FormatDateForDisplay(rowStd["DateOfLeaving"]) : "",
                        tcDate = isTcIssued ? FormatDateForDisplay(rowStd["TCDate"]) : DateTime.UtcNow.AddHours(5).AddMinutes(30).ToString("dd-MM-yyyy"),
                        conduct = isTcIssued ? rowStd["Conduct"]?.ToString() : "",
                        university = isTcIssued ? rowStd["University"]?.ToString() : "",
                        qualified = isTcIssued ? rowStd["Qualified"]?.ToString() : "",
                        scholar = isTcIssued ? rowStd["Scholar"]?.ToString() : "",
                        mole1 = rowStd["Mole1"]?.ToString() ?? "",
                        mole2 = rowStd["Mole2"]?.ToString() ?? "",
                        feeDue = totalFeeDue.ToString(),
                        isTcIssued = isTcIssued
                    };

                    return Ok(new
                    {
                        success = true,
                        message = "Student details loaded successfully.",
                        student = studentData,
                        calculatedFeeDue = totalFeeDue,
                        hasFeeDueBlock = hasFeeDueBlock,
                        warningMessage = warningMessage
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Save or Update TC issue details.
        /// </summary>
        [HttpPost("save")]
        public IActionResult SaveTCissues([FromBody] Tcissues request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.SSNO))
            {
                return BadRequest(new { success = false, message = "Student SSNO/Registration Number is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    string tid = string.IsNullOrWhiteSpace(request.Tid) ? "0" : request.Tid;

                    // If new record (Tid = "0"), fetch next auto TC number if TCNo is missing
                    string tcNo = request.TCNo ?? "";
                    if (tid == "0" && string.IsNullOrWhiteSpace(tcNo))
                    {
                        using (SqlCommand cmdAuto = new SqlCommand("SP_TC_AUTOTCNO_LIST", con))
                        {
                            cmdAuto.CommandType = CommandType.StoredProcedure;
                            cmdAuto.Parameters.AddWithValue("@TCNo", DBNull.Value);
                            object res = cmdAuto.ExecuteScalar();
                            if (res != null) tcNo = res.ToString() ?? "";
                        }
                    }

                    string query = @"EXEC [SP_Admin_TCissues_Save] 
                        @Tid, @SSNO, @TCNo, @DateOfAdmission, @StudentName, 
                        @Fname, @DOB, @Religion, @Caste, @SubCaste, @ClassofLeaving, 
                        @Group, @course, @FeeDue, @Nationality, @MotherTongue, @TCDate, 
                        @Conduct, @ReasonForLeaving, @DateofLeaving, @Mole1, @Mole2, @University, 
                        @ADMNO, @AcademicYear, @Scholar, @Qualified";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Tid", tid);
                        cmd.Parameters.AddWithValue("@SSNO", request.SSNO ?? "");
                        cmd.Parameters.AddWithValue("@TCNo", string.IsNullOrEmpty(tcNo) ? "0" : tcNo);
                        cmd.Parameters.AddWithValue("@DateOfAdmission", FormatDateForSql(request.DateOfAdmission));
                        cmd.Parameters.AddWithValue("@StudentName", request.StudentName ?? "");
                        cmd.Parameters.AddWithValue("@Fname", request.Fname ?? "");
                        cmd.Parameters.AddWithValue("@DOB", FormatDateForSql(request.DOB));
                        cmd.Parameters.AddWithValue("@Religion", request.Religion ?? "");
                        cmd.Parameters.AddWithValue("@Caste", request.Caste ?? "");
                        cmd.Parameters.AddWithValue("@SubCaste", request.SubCaste ?? "");
                        cmd.Parameters.AddWithValue("@ClassofLeaving", request.ClassofLeaving ?? "");
                        cmd.Parameters.AddWithValue("@Group", request.Group ?? "");
                        cmd.Parameters.AddWithValue("@course", request.Course ?? "");
                        cmd.Parameters.AddWithValue("@FeeDue", request.FeeDue ?? "0");
                        cmd.Parameters.AddWithValue("@Nationality", request.Nationality ?? "");
                        cmd.Parameters.AddWithValue("@MotherTongue", request.MotherTongue ?? "");
                        cmd.Parameters.AddWithValue("@TCDate", FormatDateForSql(request.TCDate));
                        cmd.Parameters.AddWithValue("@Conduct", request.Conduct ?? "");
                        cmd.Parameters.AddWithValue("@ReasonForLeaving", request.ReasonForLeaving ?? "");
                        cmd.Parameters.AddWithValue("@DateofLeaving", FormatDateForSql(request.DateofLeaving));
                        cmd.Parameters.AddWithValue("@Mole1", request.Mole1 ?? "");
                        cmd.Parameters.AddWithValue("@Mole2", request.Mole2 ?? "");
                        cmd.Parameters.AddWithValue("@University", request.University ?? "");
                        cmd.Parameters.AddWithValue("@ADMNO", request.ADMNO ?? "");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@Scholar", request.Scholar ?? "");
                        cmd.Parameters.AddWithValue("@Qualified", request.Qualified ?? "");

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                success = true,
                                message = tid == "0" ? "Data Saved Successfully" : "Data Updated Successfully"
                            });
                        }
                        else
                        {
                            return BadRequest(new
                            {
                                success = false,
                                message = "Unable to save TC details. Register Number may already exist or invalid data."
                            });
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
        /// Delete a TC issue record by Tid.
        /// </summary>
        [HttpDelete("delete/{tid}")]
        public IActionResult DeleteTCissue(string tid)
        {
            if (string.IsNullOrWhiteSpace(tid))
            {
                return BadRequest(new { success = false, message = "TC Issue ID (Tid) is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_Admin_TCissues_Delete", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Tid", tid);
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
        /// Load dataset required for printing TC report.
        /// </summary>
        [HttpGet("print-data")]
        public IActionResult GetTcPrintData([FromQuery] string tid, [FromQuery] string ssno)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_Adm_Print_Tc", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Tid", tid ?? "");
                        cmd.Parameters.AddWithValue("@SSNO", ssno ?? "");

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    List<Dictionary<string, object>> list = DAL.DataTableToList(dt);
                    return Ok(new { success = true, data = list });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
