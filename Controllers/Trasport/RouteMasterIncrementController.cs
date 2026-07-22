using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using IcampusBoatBackend.Models.Trasport;

namespace IcampusBoatBackend.Controllers.Trasport
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class RouteMasterIncrementController : ControllerBase
    {
        // Matches RouteMasterLoad() and RouteMasterList() from DAL_RouteMaster
        [HttpGet("list")]
        public IActionResult GetRouteMasterList([FromQuery] string academicYear)
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
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_ROUTEMASTER_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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

        // Matches SaveRouteMasterIncrement() from DAL_RouteMaster
        // Handles fallback gracefully because SP_ROUTEMASTERINCREMENT is missing in the database
        [HttpPost("increment")]
        public IActionResult SaveRouteMasterIncrement([FromBody] RouteMasterIncrementSaveRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RouteName))
            {
                return BadRequest(new { success = false, message = "Route Name is required." });
            }

            decimal incrementAmount = 0;
            decimal.TryParse(request.RouteIncrement, out incrementAmount);

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    try
                    {
                        // 1. Try executing the stored procedure
                        using (SqlCommand cmd = new SqlCommand("SP_ROUTEMASTERINCREMENT", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ROUTENAME", request.RouteName);
                            cmd.Parameters.AddWithValue("@ROUTEINCREMENT", request.RouteIncrement);
                            cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            return Ok(new
                            {
                                success = true,
                                message = "Data Updated Successfully (Stored Procedure)"
                            });
                        }
                    }
                    catch (SqlException sqlEx) when (sqlEx.Number == 2812) // 2812 means Stored Procedure not found
                    {
                        // 2. Fallback: Execute direct SQL updates if Stored Procedure is missing
                        try
                        {
                            // Try updating assuming BUSFEE is a numeric column
                            string directUpdateQ = "UPDATE TBL_BUS_ROUTEMASTER SET BUSFEE = BUSFEE + @ROUTEINCREMENT WHERE ROUTENAME = @ROUTENAME AND AcademicYear = @AcademicYear";
                            using (SqlCommand cmdFallback = new SqlCommand(directUpdateQ, con))
                            {
                                cmdFallback.Parameters.AddWithValue("@ROUTENAME", request.RouteName);
                                cmdFallback.Parameters.AddWithValue
                                    ("@ROUTEINCREMENT", incrementAmount);
                                cmdFallback.Parameters.AddWithValue("@AcademicYear", request.AcademicYear);

                                int rows = cmdFallback.ExecuteNonQuery();
                                if (rows > 0)
                                {
                                    return Ok(new { success = true, message = "Data Updated Successfully (Direct SQL Numeric)" });
                                }
                            }
                        }
                        catch (SqlException)
                        {
                            // Fallback if BUSFEE is stored as a varchar string
                            string castUpdateQ = "UPDATE TBL_BUS_ROUTEMASTER SET BUSFEE = CAST(CAST(BUSFEE AS DECIMAL(18,2)) + @ROUTEINCREMENT AS VARCHAR(50)) WHERE ROUTENAME = @ROUTENAME AND AcademicYear = @AcademicYear";
                            using (SqlCommand cmdCast = new SqlCommand(castUpdateQ, con))
                            {
                                cmdCast.Parameters.AddWithValue("@ROUTENAME", request.RouteName);
                                cmdCast.Parameters.AddWithValue("@ROUTEINCREMENT", incrementAmount);
                                cmdCast.Parameters.AddWithValue("@AcademicYear", request.AcademicYear);

                                int rows = cmdCast.ExecuteNonQuery();
                                if (rows > 0)
                                {
                                    return Ok(new { success = true, message = "Data Updated Successfully (Direct SQL String Cast)" });
                                }
                            }
                        }

                        return BadRequest(new { success = false, message = "No rows were updated. Check if route name exists for this academic year." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Matches SEARCHROUTENAME_ROUTEPOINT() and SEARCH_RName_RPoint() from DAL_RouteMaster
        [HttpGet("search")]
        public IActionResult SearchRoute([FromQuery] string searchName, [FromQuery] string academicYear)
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
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_SEARCHBY_ROUTENAME_RoutePoint", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SEARCHNAME", searchName ?? "");
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