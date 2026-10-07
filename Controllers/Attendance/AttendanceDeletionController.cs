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
    public class AttendanceDeletionController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("programmes")]
        public IActionResult GetProgrammes([FromQuery] string? academicYear, [FromQuery] string? date)
        {
            try
            {
                string query = $"[SP_AttDeletion_Load_Course] '{academicYear ?? ""}', '{date ?? ""}'";
                DataTable dt = DAL.GetData_FrmTxt(query);
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("branches")]
        public IActionResult GetBranches([FromBody] AttendanceDeletionBranchesRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                string query = $"[SP_AttDeletion_Load_Branch] '{request.AcademicYear ?? ""}', '{request.Date ?? ""}', '{request.Programme ?? ""}'";
                DataTable dt = DAL.GetData_FrmTxt(query);
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("years")]
        public IActionResult GetYears([FromBody] AttendanceDeletionYearsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                string query = $"[SP_AttDeletion_Load_Year] '{request.AcademicYear ?? ""}', '{request.Date ?? ""}', '{request.Programme ?? ""}', '{request.Branch ?? ""}'";
                DataTable dt = DAL.GetData_FrmTxt(query);
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("sections")]
        public IActionResult GetSections([FromBody] AttendanceDeletionSectionsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                string query = $"[SP_AttDeletion_Load_Section] '{request.AcademicYear ?? ""}', '{request.Date ?? ""}', '{request.Programme ?? ""}', '{request.Branch ?? ""}', '{request.Year ?? ""}', '{request.Semester ?? ""}'";
                DataTable dt = DAL.GetData_FrmTxt(query);
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("periods")]
        public IActionResult GetPeriods([FromBody] AttendanceDeletionPeriodsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                string query = $"[SP_AttDeletion_Load_Period] '{request.AcademicYear ?? ""}', '{request.Date ?? ""}', '{request.Programme ?? ""}', '{request.Branch ?? ""}', '{request.Year ?? ""}', '{request.Section ?? ""}', '{request.Semester ?? ""}'";
                DataTable dt = DAL.GetData_FrmTxt(query);
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("lecturers")]
        public IActionResult GetLecturers([FromBody] AttendanceDeletionLecturersRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                string query = $"[SP_AttDeletion_Load_Lecturer] '{request.AcademicYear ?? ""}', '{request.Date ?? ""}', '{request.Programme ?? ""}', '{request.Branch ?? ""}', '{request.Year ?? ""}', '{request.Section ?? ""}', '{request.Semester ?? ""}', '{request.Period ?? ""}'";
                DataTable dt = DAL.GetData_FrmTxt(query);
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("subjects")]
        public IActionResult GetSubjects([FromBody] AttendanceDeletionSubjectsRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                string query = $"SP_AttDeletion_Load_Subject '{request.AcademicYear ?? ""}', '{request.Date ?? ""}', '{request.Programme ?? ""}', '{request.Branch ?? ""}', '{request.Year ?? ""}', '{request.Section ?? ""}', '{request.Semester ?? ""}', '{request.Period ?? ""}', '{request.Lecturer ?? ""}'";
                DataTable dt = DAL.GetData_FrmTxt(query);
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("grid")]
        public IActionResult GetGrid([FromBody] AttendanceDeletionGridRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                string query = $"[SP_AttDeletion_Load_Grid] '{request.AcademicYear ?? ""}', '{request.Date ?? ""}', '{request.Programme ?? ""}', '{request.Branch ?? ""}', '{request.Year ?? ""}', '{request.Section ?? ""}', '{request.Semester ?? ""}', '{request.Period ?? ""}', '{request.Lecturer ?? ""}', '{request.Subject ?? ""}'";
                DataTable dt = DAL.GetData_FrmTxt(query);
                return Ok(new { success = true, data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("delete")]
        public IActionResult DeleteAttendance([FromBody] AttendanceDeletionDeleteRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                string query = $"[SP_AttDeletion_Delete_Attendance] '{request.AcademicYear ?? ""}', '{request.Date ?? ""}', '{request.Programme ?? ""}', '{request.Branch ?? ""}', '{request.Year ?? ""}', '{request.Section ?? ""}', '{request.Semester ?? ""}', '{request.Period ?? ""}', '{request.Lecturer ?? ""}', '{request.Subject ?? ""}', '{request.EmpId ?? ""}'";
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Attendance deleted successfully.", affectedRows = rows });
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
