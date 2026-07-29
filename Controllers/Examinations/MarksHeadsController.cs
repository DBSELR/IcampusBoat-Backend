using IcampusBoatBackend.Models.Examinations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace IcampusBoatBackend.Controllers.Examinations
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]

    public class MarksHeadsController : ControllerBase
    {
        /// <summary>
        /// Fetch list of all defined marks heads.
        /// </summary>
        [HttpGet]
        [Route("list")]
        public IActionResult GetList([FromQuery] MarksHeadFilterModel request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand("SP_MARKS_HEADS_LIST", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@HeadType", request.HeadType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Search", request.SearchTerm ?? (object)DBNull.Value);

                    using SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch single marks head by ID.
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand("SP_MARKS_HEADS_GETBYID", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id ?? (object)DBNull.Value);

                    using SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                return Ok(new { success = true, message = "Success", data = DAL.DataTableToList(dt) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Create or update a marks head record.
        /// </summary>
        [HttpPost]
        [Route("save")]
        public IActionResult Save([FromBody] MarksHeadItemModel request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Invalid payload." });
                }

                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_MARKS_HEADS_SAVE", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", request.Id ?? "0");
                cmd.Parameters.AddWithValue("@HeadCode", request.HeadCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HeadName", request.HeadName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ShortName", request.ShortName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HeadType", request.HeadType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaxMarks", request.MaxMarks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MinMarks", request.MinMarks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PassMarks", request.PassMarks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", request.Status ?? "Active");
                cmd.Parameters.AddWithValue("@UserId", request.UserId ?? (object)DBNull.Value);

                int rows = cmd.ExecuteNonQuery();

                if (rows <= 0)
                {
                    return BadRequest(new { success = false, message = "Failed to save marks head record." });
                }

                return Ok(new { success = true, message = "Marks head record saved successfully.", data = rows });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete marks head record by ID.
        /// </summary>
        [HttpPost]
        [Route("delete")]
        public IActionResult Delete([FromBody] MarksHeadItemModel request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Id))
                {
                    return BadRequest(new { success = false, message = "Record ID is required." });
                }

                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_MARKS_HEADS_DELETE", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", request.Id);

                int rows = cmd.ExecuteNonQuery();
                return Ok(new { success = true, message = "Marks head record deleted successfully.", data = rows });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
