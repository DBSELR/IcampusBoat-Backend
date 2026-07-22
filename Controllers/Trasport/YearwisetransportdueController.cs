using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace IcampusBoatBackend.Controllers.Trasport
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class YearwisetransportdueController : ControllerBase
    {
        [HttpGet("due-details/{regNo}")]
        public IActionResult GetYearwiseTransportDue(string regNo)
        {
            if (string.IsNullOrWhiteSpace(regNo))
            {
                return BadRequest(new { success = false, message = "Registration No is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SPR_TRANSPORT_DUEDETAILS_YEARWISE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", regNo);
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
