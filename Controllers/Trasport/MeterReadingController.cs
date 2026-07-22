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
    public class MeterReadingController : ControllerBase
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

        [HttpGet("load-initial")]
        public IActionResult LoadInitial()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // 1. Meter Readings List
                    DataTable dtList = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_METERREADING_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtList); }
                    }

                    // 2. Routes Dropdown
                    DataTable dtRoutes = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_ROUTNAMEVEHICLEDRIVERNAME_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtRoutes); }
                    }

                    return Ok(new
                    {
                        success = true,
                        data = new
                        {
                            readings = DAL.DataTableToList(dtList),
                            routes = DAL.DataTableToList(dtRoutes)
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
        public IActionResult GetMeterReadingList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_METERREADING_LIST", con))
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

        [HttpGet("vehicles")]
        public IActionResult GetVehiclesForRoute([FromQuery] string routeName)
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

                    // Vehicles for route
                    DataTable dtVehicles = new DataTable();
                    string vehQ = "SELECT distinct cast(VEHICLENO as varchar(50)) VEHICLENO FROM TBL_BUS_VEHICLEMASTER WHERE ROUTENAME = @RouteName";
                    using (SqlCommand cmd = new SqlCommand(vehQ, con))
                    {
                        cmd.Parameters.AddWithValue("@RouteName", routeName);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtVehicles); }
                    }

                    // Drivers list (loaded from driver master list)
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

        [HttpGet("opening-reading")]
        public IActionResult GetOpeningReading([FromQuery] string vehicleNo)
        {
            if (string.IsNullOrWhiteSpace(vehicleNo))
            {
                return BadRequest(new { success = false, message = "Vehicle No is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_VEHICLENO", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VEHICLENO", vehicleNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        return Ok(new { success = true, openingMeterReading = dt.Rows[0]["OPENINGMETERREADING"] ?? "0" });
                    }
                    return Ok(new { success = true, openingMeterReading = "0" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("save")]
        public IActionResult SaveMeterReading([FromBody] MeterReadingSaveRequest request)
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

                    // Validate closing is greater than opening
                    if (decimal.TryParse(request.ClosingMeterReading, out decimal closing) &&
                        decimal.TryParse(request.OpeningMeterReading, out decimal opening))
                    {
                        if (closing <= opening)
                        {
                            return Ok(new { success = false, message = "Enter valid meter reading. Closing meter reading must be greater than opening meter reading." });
                        }
                    }

                    bool isInsert = string.IsNullOrWhiteSpace(request.Id) || request.Id == "0";

                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_METERREADING_SAVE", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", isInsert ? 0 : request.Id);
                        cmd.Parameters.AddWithValue("@ROUTENAME", request.RouteName ?? "");
                        cmd.Parameters.AddWithValue("@VEHICLENO", request.VehicleNo ?? "");
                        cmd.Parameters.AddWithValue("@DRIVERNAME", request.DriverName ?? "");
                        cmd.Parameters.AddWithValue("@DATE", FormatDateForSql(request.Date));
                        cmd.Parameters.AddWithValue("@OPENINGMETERREADING", request.OpeningMeterReading ?? "");
                        cmd.Parameters.AddWithValue("@CLOSINGMETERREADING", request.ClosingMeterReading ?? "");
                        cmd.Parameters.AddWithValue("@REMARKS", request.Remarks ?? "");
                        cmd.Parameters.AddWithValue("@USERID", request.UserId ?? "");
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@FINANCIALYEAR", request.FinancialYear ?? "");
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
                            return BadRequest(new { success = false, message = "Unable to save meter reading." });
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
        public IActionResult DeleteMeterReading(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { success = false, message = "Meter Reading ID is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_METERREADING_DELETE", con))
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
        public IActionResult SearchMeterReadings([FromQuery] string searchName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_SEARCHBY_VEHICLENO_DRIVERNAME", con))
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
