using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using IcampusBoatBackend.Models.Attendance;

namespace IcampusBoatBackend.Controllers.Attendance
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class BatchesController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("programmes")]
        public IActionResult GetProgrammes([FromQuery] string? acdYr)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_Programme_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", acdYr ?? (object)DBNull.Value);
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

        [AllowAnonymous]
        [HttpGet("branches")]
        public IActionResult GetBranches([FromQuery] string? programme, [FromQuery] string? acdYr)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_Branch_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@COURSE", programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", acdYr ?? (object)DBNull.Value);
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

        [AllowAnonymous]
        [HttpGet("years")]
        public IActionResult GetYears([FromQuery] string? programme, [FromQuery] string? academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_YEARS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@COURSE", programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? (object)DBNull.Value);
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

        [AllowAnonymous]
        [HttpPost("sections")]
        public IActionResult GetSections([FromBody] BatchSectionSearchRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Section search request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = @"select DISTINCT Section from tbl_sectionmaster 
                                     where AcademicYear=@AcademicYear and CourseCode=@Programme and StdYear=@SYear and Semester=@Semester and BranchCode=@Branch";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
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

        [AllowAnonymous]
        [HttpPost("period-load")]
        public IActionResult GetBatchPeriodLoad([FromBody] BatchPeriodLoadRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Batch period load request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_Batch_PERIODLOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Shift", request.Shift ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Sem", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Stream", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Day", request.Day ?? (object)DBNull.Value);
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

        [AllowAnonymous]
        [HttpPost("subjects")]
        public IActionResult GetSubjects([FromBody] BatchSubjectsLoadRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Subject load request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_BATCH_SUB_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Shift", request.Shift ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEM", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STREAM", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DAY", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PERIODS", request.PeriodRange ?? (object)DBNull.Value);
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

        [AllowAnonymous]
        [HttpPost("lecturers")]
        public IActionResult GetLecturers([FromBody] BatchLecturerLoadRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Lecturer load request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = @"select distinct E.EmpID + '- ' + E.FName + ' (' + E.Designation + ')' as Fname, t.lecturer 
                                     from tbl_timetable t 
                                     inner join tbl_EmployeeDetails E on T.Lecturer = E.empid 
                                     where T.Shift=@Shift AND T.Programme=@Programme and T.Branch=@Branch AND T.[YEAR]=@SYear AND T.Semister=@Semester and T.stream=@Stream
                                     AND Section=@Section AND T.Day=@Day AND T.[SUB_CODE]=@Subjects AND T.FRM_TO_PERIODS=@PeriodRange And T.AcademicYear=@AcademicYear";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Shift", request.Shift ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Stream", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Day", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Subjects", request.Subjects ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PeriodRange", request.PeriodRange ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
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

        [AllowAnonymous]
        [HttpPost("students")]
        public IActionResult GetStudents([FromBody] BatchStudentsLoadRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Student list request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string spName = request.IsLecturerView ? "SP_BATCH_STUDENT_LIST_lecturer" : "SP_BATCH_STUDENT_LIST";
                    using (SqlCommand cmd = new SqlCommand(spName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEM", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEC", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STREAM", request.Stream ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DAY", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PERIODS", request.PeriodRange ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUB_CODE", request.Subjects ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FACULTYID", request.Lecturer ?? (object)DBNull.Value);
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

        [AllowAnonymous]
        [HttpPost("save-batch-students")]
        public IActionResult SaveBatchStudents([FromBody] SaveBatchStudentsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Batch save request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // Step 1: Delete existing batch student records for this slot
                    string delQuery = @"delete from tbl_Batch_Students 
                                        where FacultyID=@Lecturer and Shift=@Shift and CourseCode=@Programme and BranchCode=@Branch 
                                        and SYear=@SYear and Semister=@Semester and Section=@Section and Day=@Day 
                                        and SUB_CODE=@Subjects and Period_Range=@PeriodRange and AcadamicYear=@AcademicYear";
                    using (SqlCommand delCmd = new SqlCommand(delQuery, con))
                    {
                        delCmd.Parameters.AddWithValue("@Lecturer", request.Lecturer ?? (object)DBNull.Value);
                        delCmd.Parameters.AddWithValue("@Shift", request.Shift ?? (object)DBNull.Value);
                        delCmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        delCmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        delCmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                        delCmd.Parameters.AddWithValue("@Semester", request.Semester ?? (object)DBNull.Value);
                        delCmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        delCmd.Parameters.AddWithValue("@Day", request.Day ?? (object)DBNull.Value);
                        delCmd.Parameters.AddWithValue("@Subjects", request.Subjects ?? (object)DBNull.Value);
                        delCmd.Parameters.AddWithValue("@PeriodRange", request.PeriodRange ?? (object)DBNull.Value);
                        delCmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                        delCmd.ExecuteNonQuery();
                    }

                    // Step 2: Insert selected batch students
                    int savedCount = 0;
                    if (request.Students != null)
                    {
                        foreach (var st in request.Students)
                        {
                            if (st.Selected)
                            {
                                using (SqlCommand saveCmd = new SqlCommand("SP_BATCH_STUDENTS_SAVE", con))
                                {
                                    saveCmd.CommandType = CommandType.StoredProcedure;
                                    saveCmd.Parameters.AddWithValue("@LECTURER", request.Lecturer ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@Shift", request.Shift ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@SEMESTER", request.Semester ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@SECTION", request.Section ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@DAY", request.Day ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@SUBJECTS", request.Subjects ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@Period_Range", request.PeriodRange ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@RegNo", st.RegNo ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@SName", st.SName ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                                    saveCmd.Parameters.AddWithValue("@IsActive", "Y");

                                    savedCount += saveCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    return Ok(new { success = true, message = "Batch students saved successfully.", savedCount });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("timetable-data")]
        public IActionResult GetTimeTableData([FromBody] BatchTimeTableDataRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "TimeTable data request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Sp_Get_Edit_TimeTable", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcadamicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Course", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FromToPeriod", request.PeriodFrom ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Day", request.Day ?? (object)DBNull.Value);
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

        [AllowAnonymous]
        [HttpPost("edit-efromdate")]
        public IActionResult EditEFromDate([FromBody] EditEFromDateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Edit EFromDate request is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Sp_Edit_EFromDate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcadamicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Course", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYear", request.SYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semester ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FromToPeriod", request.PeriodFrom ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Day", request.Day ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@newEfromDate", request.NewEFromDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@userid", request.UserId ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "EFromDate updated successfully.", affectedRows = rows });
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
