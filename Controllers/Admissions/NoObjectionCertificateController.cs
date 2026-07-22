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
    public class NoObjectionCertificateController : ControllerBase
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
        /// Initial load API returning next NOC number, list of issued NOCs, and default dates.
        /// </summary>
        [HttpGet("load")]
        public IActionResult Load([FromQuery] string academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // 1. Get next auto NOC number
                    string autoNocNo = "";
                    using (SqlCommand cmd = new SqlCommand("SP_AUTUNOC_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NOCNO", "");
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            autoNocNo = result.ToString() ?? "";
                        }
                    }

                    // 2. Get NOC List
                    DataTable dtList = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_NOC_LIST", con))
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
                            autoNocNo = autoNocNo,
                            nocList = DAL.DataTableToList(dtList),
                            defaultDate = todayDate,
                            affiliatingUniversity = "JNTU,Kakinada",
                            classToStudied = academicYear
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
        /// Get next auto NOC number.
        /// </summary>
        [HttpGet("auto-nocno")]
        public IActionResult GetAutoNocNo([FromQuery] string? nocNo = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_AUTUNOC_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NocNo", nocNo ?? "");
                        object result = cmd.ExecuteScalar();
                        string generatedNo = result != null ? result.ToString() ?? "" : "";
                        return Ok(new { success = true, autoNocNo = generatedNo });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Search NOC list.
        /// </summary>
        [HttpGet("list")]
        public IActionResult GetNocList([FromQuery] string? searchName = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();

                    if (!string.IsNullOrWhiteSpace(searchName))
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_NOC_SSNO_Search", con))
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
                        using (SqlCommand cmd = new SqlCommand("SP_NOC_LIST", con))
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
        /// Fetch student details and existing NOC status by Registration/Admission Number.
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
                    using (SqlCommand cmdSSNO = new SqlCommand("SP_NOC_REGNO_SSNO_LOAD", con))
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
                        return Ok(new { success = false, message = "SSNO not exist in StudentData" });
                    }

                    // 2. Check if NOC is already issued / exists
                    DataTable dtTot = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TOT_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", ssNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtTot);
                        }
                    }

                    if (dtTot != null && dtTot.Rows.Count > 0)
                    {
                        DataRow row = dtTot.Rows[0];
                        var studentData = new
                        {
                            nocNo = row["NocNo"]?.ToString() ?? "",
                            admissionDate = FormatDateForDisplay(row["AdmissionDate"]),
                            studentName = row["StudentName"]?.ToString() ?? "",
                            fatherName = row["FatherName"]?.ToString() ?? "",
                            programme = row["CourseCode"]?.ToString() ?? "",
                            branch = row["BranchCode"]?.ToString() ?? "",
                            year = row["Year"]?.ToString() ?? "",
                            date = FormatDateForDisplay(row["Date"]),
                            fromStudentTransfe = row["FromStudentTransfe"]?.ToString() ?? "",
                            toStudentTransfe = row["ToStudentTransfe"]?.ToString() ?? "",
                            affiliatingUniversity = row["AffiliatingUniversity"]?.ToString() ?? "",
                            universityissuedtheNOC = row["UniversityissuedtheNOC"]?.ToString() ?? "",
                            totalintakeinIYear = row["TotalintakeinIYear"]?.ToString() ?? "",
                            quota = row["Quota"]?.ToString() ?? "",
                            annualtuitionfee = row["Annualtuitionfee"]?.ToString() ?? "",
                            tuitionfeeChargeble = row["tuitionfeeChargeble"]?.ToString() ?? "",
                            reasonForTransfer = row["ReasonForTransfer"]?.ToString() ?? "",
                            principal = row["Principal"]?.ToString() ?? "",
                            jAccyr = row["JAccyr"]?.ToString() ?? "",
                            dateMonthlastExamination = row["DOLExam"]?.ToString() ?? "",
                            detailsDiscontinue = row["DetailsDiscontinue"]?.ToString() ?? "",
                            seekingTransfer = row["SeekingTransfer"]?.ToString() ?? "",
                            seekingTransfer2 = row["SeekingTransferYear"]?.ToString() ?? "",
                            noOfUnfilled = row["NoOfUnfilled"]?.ToString() ?? "",
                            stydyYear = row["StudyingAcyr"]?.ToString() ?? "",
                            stydyDetails = row["StudyingDetails"]?.ToString() ?? "",
                            takenaccyr = row["Takenaccyr"]?.ToString() ?? "",
                            noofunfilledseatsaccyr = row["Noofunfilledseatsaccyr"]?.ToString() ?? "",
                            isIssued = true
                        };

                        return Ok(new
                        {
                            success = true,
                            message = "NOC already exists for this student.",
                            student = studentData,
                            ssNo = ssNo,
                            warning = "SSNO Already Exists"
                        });
                    }

                    // 3. Load default student details (first time NOC issue)
                    DataTable dtStudent = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_NOC_SSNO_LOAD", con))
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
                        return Ok(new { success = false, message = "SSNO not exist in StudentData" });
                    }

                    DataRow rowStd = dtStudent.Rows[0];

                    var defaultStudentData = new
                    {
                        admissionDate = FormatDateForDisplay(rowStd["AdmissionDate"]),
                        studentName = rowStd["SName"]?.ToString() ?? "",
                        fatherName = rowStd["FName"]?.ToString() ?? "",
                        programme = $"{rowStd["CourseCode"]}-{rowStd["CourseName"]}",
                        programmeCode = rowStd["CourseCode"]?.ToString() ?? "",
                        branch = $"{rowStd["BranchCode"]}-{rowStd["BranchName"]}",
                        branchCode = rowStd["BranchCode"]?.ToString() ?? "",
                        year = rowStd["SYear"]?.ToString() ?? "",
                        jAccyr = rowStd["JAcadamicYear"]?.ToString() ?? "",
                        isIssued = false
                    };

                    return Ok(new
                    {
                        success = true,
                        message = "Student details loaded successfully.",
                        student = defaultStudentData,
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
        /// Save or Update NOC details.
        /// </summary>
        [HttpPost("save")]
        public IActionResult SaveNoObjectionCertificate([FromBody] NocSaveRequest request)
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
                    string ssNo = "";
                    using (SqlCommand cmdSSNO = new SqlCommand("SP_NOC_REGNO_SSNO_LOAD", con))
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
                        ssNo = request.RegNo; // Fallback
                    }

                    string id = string.IsNullOrWhiteSpace(request.Id) ? "0" : request.Id;

                    // 2. Fetch auto NOC number if blank and it's a new record
                    string nocNo = request.NocNo ?? "";
                    if (id == "0" && string.IsNullOrWhiteSpace(nocNo))
                    {
                        using (SqlCommand cmdNC = new SqlCommand("SP_AUTUNOC_LOAD", con))
                        {
                            cmdNC.CommandType = CommandType.StoredProcedure;
                            cmdNC.Parameters.AddWithValue("@NocNo", "");
                            object res = cmdNC.ExecuteScalar();
                            if (res != null) nocNo = res.ToString() ?? "";
                        }
                    }

                    string query = @"EXEC SP_NOC_SAVE 
                        @id, @NocNo, @Date, @SSNO, @AdmissionDate, @StudentName, 
                        @FatherName, @Programme, @Branch, @Year, @FromStudentTransfe, 
                        @ToStudentTransfe, @TransferisSought, @AffiliatingUniversity, 
                        @UniversityissuedtheNOC, @TotalintakeinIYear, @Cellinglimit, 
                        @Quota, @Annualtuitionfee, @tuitionfeeChargeble, @ReasonForTransfer, 
                        @AcademicYear, @Principal, @JAccyr, @DateMonthlastExamination, 
                        @DetailsDiscontinue, @SeekingTransfer, @SeekingTransfer2, 
                        @NoOfUnfilled, @StydyYear, @StydyDetails, @takenaccyr, @noofunfilledseatsaccyr";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@NocNo", nocNo);
                        cmd.Parameters.AddWithValue("@Date", FormatDateForSql(request.Date));
                        cmd.Parameters.AddWithValue("@SSNO", ssNo);
                        cmd.Parameters.AddWithValue("@AdmissionDate", FormatDateForSql(request.AdmissionDate));
                        cmd.Parameters.AddWithValue("@StudentName", request.StudentName ?? "");
                        cmd.Parameters.AddWithValue("@FatherName", request.FatherName ?? "");
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? "");
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? "");
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? "0");
                        cmd.Parameters.AddWithValue("@FromStudentTransfe", request.FromStudentTransfe ?? "");
                        cmd.Parameters.AddWithValue("@ToStudentTransfe", request.ToStudentTransfe ?? "");
                        cmd.Parameters.AddWithValue("@TransferisSought", ""); // not used, empty in webforms code
                        cmd.Parameters.AddWithValue("@AffiliatingUniversity", request.AffiliatingUniversity ?? "");
                        cmd.Parameters.AddWithValue("@UniversityissuedtheNOC", request.UniversityissuedtheNOC ?? "");
                        cmd.Parameters.AddWithValue("@TotalintakeinIYear", request.TotalintakeinIYear ?? "0");
                        cmd.Parameters.AddWithValue("@Cellinglimit", ""); // not used, empty in webforms code
                        cmd.Parameters.AddWithValue("@Quota", request.Quota ?? "");
                        cmd.Parameters.AddWithValue("@Annualtuitionfee", request.Annualtuitionfee ?? "0");
                        cmd.Parameters.AddWithValue("@tuitionfeeChargeble", request.TuitionfeeChargeble ?? "0");
                        cmd.Parameters.AddWithValue("@ReasonForTransfer", request.ReasonForTransfer ?? "");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@Principal", request.Principal ?? "");
                        cmd.Parameters.AddWithValue("@JAccyr", request.JAccyr ?? "");
                        cmd.Parameters.AddWithValue("@DateMonthlastExamination", request.DateMonthlastExamination ?? "");
                        cmd.Parameters.AddWithValue("@DetailsDiscontinue", request.DetailsDiscontinue ?? "");
                        cmd.Parameters.AddWithValue("@SeekingTransfer", request.SeekingTransfer ?? "");
                        cmd.Parameters.AddWithValue("@SeekingTransfer2", request.SeekingTransfer2 ?? "");
                        cmd.Parameters.AddWithValue("@NoOfUnfilled", request.NoOfUnfilled ?? "");
                        cmd.Parameters.AddWithValue("@StydyYear", request.StydyYear ?? "");
                        cmd.Parameters.AddWithValue("@StydyDetails", request.StydyDetails ?? "");
                        cmd.Parameters.AddWithValue("@takenaccyr", request.Takenaccyr ?? "");
                        cmd.Parameters.AddWithValue("@noofunfilledseatsaccyr", request.Noofunfilledseatsaccyr ?? "");

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
                            return BadRequest(new { success = false, message = "Unable to save NOC details." });
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
        /// Delete NOC certificate record by ID.
        /// </summary>
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteNoObjectionCertificate(string id)
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
                    using (SqlCommand cmd = new SqlCommand("SP_NOC_DELETE", con))
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
        /// Get dataset required for printing No Objection Certificate.
        /// </summary>
        [HttpGet("print-data")]
        public IActionResult GetPrintData([FromQuery] string ssno)
        {
            if (string.IsNullOrWhiteSpace(ssno))
            {
                return BadRequest(new { success = false, message = "SSNO is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_Adm_Print_NoObjecttion", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", ssno);

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
