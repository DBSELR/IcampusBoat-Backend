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
    public class VehicleMasterController : ControllerBase
    {
        [HttpGet("load-initial")]
        public IActionResult LoadInitial()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // 1. Vehicle Master List
                    DataTable dtVehicles = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_VEHICLEMASTER_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtVehicles); }
                    }

                    // 2. Route Names
                    DataTable dtRoutes = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_ROUTENAMES_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtRoutes); }
                    }

                    // 3. Drivers
                    DataTable dtDrivers = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_DRIVERNAME_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtDrivers); }
                    }

                    return Ok(new
                    {
                        success = true,
                        data = new
                        {
                            vehicles = DAL.DataTableToList(dtVehicles),
                            routes = DAL.DataTableToList(dtRoutes),
                            drivers = DAL.DataTableToList(dtDrivers)
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("list")]
        public IActionResult GetVehicleList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_VEHICLEMASTER_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("check-vehicle-no")]
        public IActionResult CheckVehicleNo([FromQuery] string vehicleNo, [FromQuery] string academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string q = "SELECT * FROM TBL_BUS_VEHICLEMASTER WHERE AcademicYear=@AcYr AND VEHICLENO=@VNo";
                    using (SqlCommand cmd = new SqlCommand(q, con))
                    {
                        cmd.Parameters.AddWithValue("@AcYr", academicYear);
                        cmd.Parameters.AddWithValue("@VNo", vehicleNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        return Ok(new { success = true, exists = true, data = DAL.DataTableToList(dt)[0] });
                    }
                    return Ok(new { success = true, exists = false });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("save")]
        public IActionResult SaveVehicle([FromBody] VehicleMasterSaveRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.VehicleNo) || string.IsNullOrWhiteSpace(request.RouteName))
            {
                return BadRequest(new { success = false, message = "Vehicle No and Route Name are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // If route is enabled / insert check driver duplicates
                    bool isInsert = string.IsNullOrWhiteSpace(request.Id) || request.Id == "0";
                    if (isInsert)
                    {
                        string driverCheckQ = "SELECT COUNT(*) FROM TBL_BUS_VEHICLEMASTER WHERE DRIVERNAME = @DriverName";
                        using (SqlCommand cmdDriverCheck = new SqlCommand(driverCheckQ, con))
                        {
                            cmdDriverCheck.Parameters.AddWithValue("@DriverName", request.DriverName);
                            int driverCount = Convert.ToInt32(cmdDriverCheck.ExecuteScalar());
                            if (driverCount > 0)
                            {
                                return Ok(new { success = false, message = "Driver already Allocated For Another Bus Route..!" });
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_VEHICLEMASTER_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", isInsert ? "0" : request.Id);
                        cmd.Parameters.AddWithValue("@ROUTENAME", request.RouteName);
                        cmd.Parameters.AddWithValue("@VEHICLENO", request.VehicleNo);
                        cmd.Parameters.AddWithValue("@VEHICLEREGNO", request.VehicleRegNo);
                        cmd.Parameters.AddWithValue("@VEHICLECAPACITY", request.VehicleCapacity);
                        cmd.Parameters.AddWithValue("@DRIVERNAME", request.DriverName);
                        cmd.Parameters.AddWithValue("@UserId", request.UserId ?? "");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear);
                        cmd.Parameters.AddWithValue("@FinancialYear", request.FinancialYear);

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
                            return BadRequest(new { success = false, message = "Unable to save vehicle details." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteVehicle(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { success = false, message = "Vehicle ID is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_VEHICLEMASTER_DELETE", con))
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

        [HttpGet("search")]
        public IActionResult SearchVehicles([FromQuery] string searchName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_SEARCHBY_ROUTENAME_DRIVERNAME", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SEARCHNAME", searchName ?? "");
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
