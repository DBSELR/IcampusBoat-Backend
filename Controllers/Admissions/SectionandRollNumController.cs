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
    public class SectionandRollNumController : ControllerBase
    {
        /// <summary>
        /// Initial load API returning courses and list of current section roll number allotments.
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

                    // Load Course list
                    DataTable dtCourses = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_COURSEBRANCH_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Status", "COURSE");
                        cmd.Parameters.AddWithValue("@Course", "");
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtCourses);
                        }
                    }

                    // Load Allotment List
                    DataTable dtList = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_SECTIONROLLNO_List", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtList);
                        }
                    }

                    return Ok(new
                    {
                        success = true,
                        message = "Success",
                        data = new
                        {
                            courses = DAL.DataTableToList(dtCourses),
                            sectionRollList = DAL.DataTableToList(dtList)
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
        /// Get branch list for a given course.
        /// </summary>
        [HttpGet("branches")]
        public IActionResult GetBranches([FromQuery] string courseCode)
        {
            if (string.IsNullOrWhiteSpace(courseCode))
            {
                return BadRequest(new { success = false, message = "Course code is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_COURSEBRANCH_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Status", "BRANCH");
                        cmd.Parameters.AddWithValue("@Course", courseCode);
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
        /// Get years for a given course and academic year.
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
        /// Get sections for selected course, branch, studying year, and academic year.
        /// </summary>
        [HttpGet("sections")]
        public IActionResult GetSections([FromQuery] string courseCode, [FromQuery] string branchCode, [FromQuery] string studyingYear, [FromQuery] string academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    string query = @"SELECT DISTINCT Section FROM tbl_sectionmaster 
                                     WHERE CourseCode = @Course 
                                       AND BranchCode = @Branch 
                                       AND StdYear = @StudyingYear 
                                       AND AcademicYear = @AcademicYear";

                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Course", courseCode ?? "");
                        cmd.Parameters.AddWithValue("@Branch", branchCode ?? "");
                        cmd.Parameters.AddWithValue("@StudyingYear", studyingYear ?? "");
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? "");

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
        /// Get section capacity.
        /// </summary>
        [HttpGet("capacity")]
        public IActionResult GetCapacity([FromQuery] string course, [FromQuery] string branch, [FromQuery] string studyingYear, [FromQuery] string section)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_SectionCap_Load", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Course", course ?? "");
                        cmd.Parameters.AddWithValue("@Branch", branch ?? "");
                        cmd.Parameters.AddWithValue("@StdYear", studyingYear ?? "");
                        cmd.Parameters.AddWithValue("@Section", section ?? "");

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    string capacity = "0";
                    if (dt.Rows.Count > 0)
                    {
                        capacity = dt.Rows[0]["TotSeats"]?.ToString() ?? "0";
                    }

                    return Ok(new { success = true, capacity = capacity });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get total allotted count.
        /// </summary>
        [HttpGet("allotted")]
        public IActionResult GetAllotted([FromQuery] string course, [FromQuery] string branch, [FromQuery] string studyingYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_ALLOT", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CourseCode", course ?? "");
                        cmd.Parameters.AddWithValue("@BranchCode", branch ?? "");
                        cmd.Parameters.AddWithValue("@SYear", studyingYear ?? "");
                        cmd.Parameters.AddWithValue("@RollNo", "null");

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    string allotted = "0";
                    if (dt.Rows.Count > 0)
                    {
                        allotted = dt.Rows[0]["Allot"]?.ToString() ?? "0";
                    }

                    return Ok(new { success = true, allotted = allotted });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Save or Update Section and Roll Number allotment details.
        /// </summary>
        [HttpPost("save")]
        public IActionResult SaveSectionAndRollNum([FromBody] SectionandRollNumSaveRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request payload is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    string cid = string.IsNullOrWhiteSpace(request.Cid) ? "" : request.Cid;

                    // 1. Check Exists (Only for New records)
                    if (string.IsNullOrEmpty(cid))
                    {
                        string checkQuery = @"SELECT COUNT(*) FROM tbl_ADMIN_SECTIONROLL 
                                              WHERE CourseCode = @Course 
                                                AND BranchCode = @Branch 
                                                AND StudyingYear = @StudyingYear 
                                                AND AcademicYear = @AcademicYear 
                                                AND section = @Section";
                        using (SqlCommand cmdCheck = new SqlCommand(checkQuery, con))
                        {
                            cmdCheck.Parameters.AddWithValue("@Course", request.Course);
                            cmdCheck.Parameters.AddWithValue("@Branch", request.Branch);
                            cmdCheck.Parameters.AddWithValue("@StudyingYear", request.StudyingYear);
                            cmdCheck.Parameters.AddWithValue("@AcademicYear", request.AcademicYear);
                            cmdCheck.Parameters.AddWithValue("@Section", request.Section);

                            int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                            if (count > 0)
                            {
                                return Ok(new { success = false, message = "Already Alloted..!!!" });
                            }
                        }
                    }

                    // 2. Validate Allotment capacity limit
                    int startRoll = 0, endRoll = 0, sectionCap = 0;
                    int.TryParse(request.RollNoStartFrom, out startRoll);
                    int.TryParse(request.RollNoEnd, out endRoll);
                    int.TryParse(request.SectionCapacity, out sectionCap);

                    DataTable dtAllot = new DataTable();
                    using (SqlCommand cmdAllotted = new SqlCommand("SP_GET_ALLOTTED", con))
                    {
                        cmdAllotted.CommandType = CommandType.StoredProcedure;
                        cmdAllotted.Parameters.AddWithValue("@FROM", request.RollNoStartFrom);
                        cmdAllotted.Parameters.AddWithValue("@TO", request.RollNoEnd);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmdAllotted))
                        {
                            da.Fill(dtAllot);
                        }
                    }

                    int totalAllotted = 0;
                    if (dtAllot.Rows.Count > 0 && dtAllot.Rows[0][0] != DBNull.Value)
                    {
                        totalAllotted = Convert.ToInt32(dtAllot.Rows[0][0]);
                    }

                    if (totalAllotted > sectionCap)
                    {
                        return Ok(new { success = false, message = "Allotment Exceeding Capacity..!!!" });
                    }

                    // 3. Save / Update Allotment details
                    string saveQuery = @"EXEC SP_ADMIN_SECTIONROLLNO_Save 
                                         @cid, @StudyingYear, @Course, @Branch, @SectionCapacity, 
                                         @Alloted, @RollNoStartFrom, @TO, @Section, @Stream, @AcaYear";

                    using (SqlCommand cmdSave = new SqlCommand(saveQuery, con))
                    {
                        cmdSave.Parameters.AddWithValue("@cid", cid);
                        cmdSave.Parameters.AddWithValue("@StudyingYear", request.StudyingYear);
                        cmdSave.Parameters.AddWithValue("@Course", request.Course);
                        cmdSave.Parameters.AddWithValue("@Branch", request.Branch);
                        cmdSave.Parameters.AddWithValue("@SectionCapacity", request.SectionCapacity);
                        cmdSave.Parameters.AddWithValue("@Alloted", request.Alloted);
                        cmdSave.Parameters.AddWithValue("@RollNoStartFrom", request.RollNoStartFrom);
                        cmdSave.Parameters.AddWithValue("@TO", request.RollNoEnd);
                        cmdSave.Parameters.AddWithValue("@Section", request.Section);
                        cmdSave.Parameters.AddWithValue("@Stream", request.Stream);
                        cmdSave.Parameters.AddWithValue("@AcaYear", request.AcademicYear);

                        int rowsAffected = cmdSave.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                success = true,
                                message = string.IsNullOrEmpty(cid) ? "Student Data Saved Successfully" : "Student Data Updated Successfully"
                            });
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Unable to save section allotment details." });
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
        /// Delete section roll allotment details.
        /// </summary>
        [HttpDelete("delete/{cid}")]
        public IActionResult DeleteSectionRollNum(string cid)
        {
            if (string.IsNullOrWhiteSpace(cid))
            {
                return BadRequest(new { success = false, message = "ID (cid) is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_SECTIONROLLNO_Delete", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@cid", cid);
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
