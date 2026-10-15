using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using IcampusBoatBackend.Models.Settings;

namespace IcampusBoatBackend.Controllers.Settings
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class FeedBackMasterController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("theory-list")]
        public IActionResult GetTheoryList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "select * from tbl_Feedback_Master where QType='Theory' order by cast(Qorder as int) Asc";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
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
        [HttpGet("lab-list")]
        public IActionResult GetLabList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "select * from tbl_Feedback_Master where QType='Lab' order by cast(Qorder as int) Asc";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
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
        [HttpGet("check-exists")]
        public IActionResult CheckFeedbackExists([FromQuery] string? qid, [FromQuery] string? qType)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("sp_GetFeedbackdata", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Qid", qid ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@QType", qType ?? (object)DBNull.Value);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    return Ok(new { success = true, exists = dt.Rows.Count > 0, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("save")]
        public IActionResult SaveFeedBackMaster([FromBody] SaveFeedBackMasterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request body is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_FeedbackMaster_save", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", request.Id ?? "");
                        cmd.Parameters.AddWithValue("@Qorder", request.Qid ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Question", request.Question ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@userid", request.UserId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Qtype", request.QType ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new
                        {
                            success = rows > 0,
                            message = rows > 0 ? "Data Saved Successfully" : "Data Not Saved",
                            affectedRows = rows
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("delete")]
        public IActionResult DeleteFeedBackMaster([FromQuery] string? id)
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
                    using (SqlCommand cmd = new SqlCommand("SP_FeedbackMaster_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new
                        {
                            success = rows > 0,
                            message = "Record Deleted Successfully",
                            affectedRows = rows
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
