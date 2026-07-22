using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using IcampusBoatBackend.Models.Trasport;

namespace IcampusBoatBackend.Controllers.Trasport
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class RouteMasterController : ControllerBase
    {
        /// <summary>
        /// Fetch list of transport routes for a given academic year.
        /// </summary>
        [HttpGet("load")]
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

        /// <summary>
        /// Get the next max route point order number for a given route name.
        /// </summary>
        [HttpGet("max-order")]
        public IActionResult GetMaxRoutePointOrderNo([FromQuery] string routeName)
        {
            if (string.IsNullOrWhiteSpace(routeName))
            {
                return BadRequest(new { success = false, message = "Route Name is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("Proc_GetMaxRPTONo", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ROUTENAME", routeName);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    int nextOrderNo = 1;
                    if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                    {
                        nextOrderNo = Convert.ToInt32(dt.Rows[0][0]) + 1;
                    }

                    return Ok(new { success = true, nextOrderNo = nextOrderNo });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Save or update route master entry.
        /// </summary>
        [HttpPost("save")]
        public IActionResult SaveRouteMaster([FromBody] RouteMasterSaveRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RouteName) || string.IsNullOrWhiteSpace(request.RoutePoint))
            {
                return BadRequest(new { success = false, message = "Route Name and Route Point are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // Check duplicate order/point if it's a new entry (Id is empty, 0, or null)
                    bool isInsert = string.IsNullOrWhiteSpace(request.Id) || request.Id == "0";

                    if (isInsert)
                    {
                        // 1. Check duplicate route point
                        string pointCheckQ = "SELECT COUNT(*) FROM TBL_BUS_ROUTEMASTER WHERE ROUTENAME = @RouteName AND ROUTEPOINT = @RoutePoint";
                        using (SqlCommand cmdCheck = new SqlCommand(pointCheckQ, con))
                        {
                            cmdCheck.Parameters.AddWithValue("@RouteName", request.RouteName);
                            cmdCheck.Parameters.AddWithValue("@RoutePoint", request.RoutePoint);
                            int pointCount = Convert.ToInt32(cmdCheck.ExecuteScalar());
                            if (pointCount > 0)
                            {
                                return Ok(new { success = false, message = "Route Point or Route Order is already existed." });
                            }
                        }

                        // 2. Check duplicate order
                        string orderCheckQ = "SELECT COUNT(*) FROM TBL_BUS_ROUTEMASTER WHERE ROUTENAME = @RouteName AND ROUTEPOINTORDERNO = @OrderNo";
                        using (SqlCommand cmdCheck = new SqlCommand(orderCheckQ, con))
                        {
                            cmdCheck.Parameters.AddWithValue("@RouteName", request.RouteName);
                            cmdCheck.Parameters.AddWithValue("@OrderNo", request.RoutePointOrderNo);
                            int orderCount = Convert.ToInt32(cmdCheck.ExecuteScalar());
                            if (orderCount > 0)
                            {
                                return Ok(new { success = false, message = "Route Point or Route Order is already existed." });
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_ROUTEMASTER_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", isInsert ? "0" : request.Id);
                        cmd.Parameters.AddWithValue("@ROUTENAME", request.RouteName);
                        cmd.Parameters.AddWithValue("@ROUTEPOINTORDERNO", request.RoutePointOrderNo);
                        cmd.Parameters.AddWithValue("@ROUTEPOINT", request.RoutePoint);
                        cmd.Parameters.AddWithValue("@BusFee", request.BusFee);
                        cmd.Parameters.AddWithValue("@UserId", request.UserId ?? "");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear);
                        cmd.Parameters.AddWithValue("@FinancialYear", request.FinancialYear);
                        cmd.Parameters.AddWithValue("@StartTime", request.StartTime ?? "");

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                success = true,
                                message = isInsert ? "Data Saved Successfully" : "Data Updated Successfully"
                            });
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Unable to save route details." });
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
        /// Delete route master entry by ID.
        /// </summary>
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteRouteMaster(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { success = false, message = "Route ID is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_ROUTEMASTER_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", id);
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
        /// Search route names or points.
        /// </summary>
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
                        cmd.Parameters.AddWithValue("@ACYR", academicYear);
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
        /// Bulk update route point orders.
        /// </summary>
        [HttpPost("update-order-bulk")]
        public IActionResult UpdateRoutePointOrdersBulk([FromBody] List<RoutePointOrderUpdateRequest> requests)
        {
            if (requests == null || requests.Count == 0)
            {
                return BadRequest(new { success = false, message = "Update list cannot be empty." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    int updatedCount = 0;
                    foreach (var req in requests)
                    {
                        if (string.IsNullOrWhiteSpace(req.Id) || string.IsNullOrWhiteSpace(req.RoutePointOrderNo))
                            continue;

                        // 1. Reset route order
                        using (SqlCommand cmdReset = new SqlCommand("SP_ROUTEID_UPDATE", con))
                        {
                            cmdReset.CommandType = CommandType.StoredProcedure;
                            cmdReset.Parameters.AddWithValue("@ID", req.Id);
                            cmdReset.Parameters.AddWithValue("@ROUTEPOINTORDERNO", req.RoutePointOrderNo);
                            cmdReset.Parameters.AddWithValue("@ROUTENAME", req.RouteName ?? "");
                            cmdReset.Parameters.AddWithValue("@ROUTEPOINT", req.RoutePoint ?? "");
                            cmdReset.Parameters.AddWithValue("@TYPE", "RESET");
                            cmdReset.ExecuteNonQuery();
                        }

                        // 2. Set new route order
                        using (SqlCommand cmdUpdate = new SqlCommand("SP_ROUTEID_UPDATE", con))
                        {
                            cmdUpdate.CommandType = CommandType.StoredProcedure;
                            cmdUpdate.Parameters.AddWithValue("@ID", req.Id);
                            cmdUpdate.Parameters.AddWithValue("@ROUTEPOINTORDERNO", req.RoutePointOrderNo);
                            cmdUpdate.Parameters.AddWithValue("@ROUTENAME", req.RouteName ?? "");
                            cmdUpdate.Parameters.AddWithValue("@ROUTEPOINT", req.RoutePoint ?? "");
                            cmdUpdate.Parameters.AddWithValue("@TYPE", ""); // empty updates order
                            
                            int res = cmdUpdate.ExecuteNonQuery();
                            if (res > 0)
                            {
                                updatedCount++;
                            }
                        }
                    }

                    return Ok(new
                    {
                        success = true,
                        message = "Route order saved successfully.",
                        updatedCount = updatedCount
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
