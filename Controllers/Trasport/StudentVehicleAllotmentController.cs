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
    public class StudentVehicleAllotmentController : ControllerBase
    {
        [HttpGet("load-initial")]
        public IActionResult LoadInitial([FromQuery] string academicYear)
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

                    // 1. Allotments List
                    DataTable dtAllotments = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_STUDENTVEHICLEALLOTMENT_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", academicYear);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtAllotments); }
                    }

                    // 2. Route Names
                    DataTable dtRoutes = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_ROUTNAME_SVA_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtRoutes); }
                    }

                    return Ok(new
                    {
                        success = true,
                        data = new
                        {
                            allotments = DAL.DataTableToList(dtAllotments),
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

        [HttpGet("routes")]
        public IActionResult GetRoutes()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_ROUTNAME_SVA_LIST", con))
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

        [HttpGet("route-details")]
        public IActionResult GetRouteDetails([FromQuery] string routeName)
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

                    // 1. Vehicle Nos for Route
                    DataTable dtVehicles = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_VEHICLENO_SVA_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ROUTENAME", routeName);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtVehicles); }
                    }

                    // 2. Stop Points for Route
                    DataTable dtStops = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_STOPPOINT_SVA_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ROUTENAME", routeName);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtStops); }
                    }

                    return Ok(new
                    {
                        success = true,
                        data = new
                        {
                            vehicles = DAL.DataTableToList(dtVehicles),
                            stops = DAL.DataTableToList(dtStops)
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("vehicle-capacity")]
        public IActionResult GetVehicleCapacity([FromQuery] string vehicleNo, [FromQuery] string academicYear, [FromQuery] string allotmentId = "0")
        {
            if (string.IsNullOrWhiteSpace(vehicleNo) || string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Vehicle No and Academic Year are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_AllotAvailbily", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        cmd.Parameters.AddWithValue("@VEHICLENO", vehicleNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        int availability = Convert.ToInt32(dt.Rows[0]["AVAILBLE"]);
                        int alloted = Convert.ToInt32(dt.Rows[0]["ALLOTED"]);
                        return Ok(new
                        {
                            success = true,
                            availability = availability,
                            alloted = alloted,
                            isFull = availability <= 0
                        });
                    }

                    return Ok(new { success = false, message = "Capacity details not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("student-details/{regNo}")]
        public IActionResult GetStudentDetails(string regNo, [FromQuery] string academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // Check if already allocated
                    DataTable dtAllotCheck = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("select BusFee from tbl_adm_studata where acadamicYear=@AcYr and Registrationno=@RegNo and busfee<>''", con))
                    {
                        cmd.Parameters.AddWithValue("@AcYr", academicYear);
                        cmd.Parameters.AddWithValue("@RegNo", regNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtAllotCheck); }
                    }

                    // Get student details
                    DataTable dtDetails = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_SstdSNameclass_Stdbusallot_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StudentSerialNo", regNo);
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtDetails); }
                    }

                    if (dtDetails.Rows.Count > 0)
                    {
                        bool isAllocated = dtDetails.Rows[0]["BusStatus"]?.ToString() == "YES";
                        return Ok(new
                        {
                            success = true,
                            isAllocated = isAllocated,
                            data = DAL.DataTableToList(dtDetails)[0]
                        });
                    }

                    return Ok(new { success = false, message = "Student details not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("stop-bus-fee")]
        public IActionResult GetStopBusFee([FromQuery] string routeName, [FromQuery] string stopPoint, [FromQuery] string academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    string q = "select BUSFEE From TBL_BUS_ROUTEMASTER where ROUTENAME = @RouteName and ROUTEPOINT=@StopPoint AND AcademicYear=@AcYr";
                    using (SqlCommand cmd = new SqlCommand(q, con))
                    {
                        cmd.Parameters.AddWithValue("@RouteName", routeName);
                        cmd.Parameters.AddWithValue("@StopPoint", stopPoint);
                        cmd.Parameters.AddWithValue("@AcYr", academicYear);
                        object feeObj = cmd.ExecuteScalar();
                        return Ok(new { success = true, busFee = feeObj != null ? Convert.ToDecimal(feeObj) : 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("save")]
        public IActionResult SaveAllotment([FromBody] StudentVehicleAllotmentSaveRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.StudentSerialNo))
            {
                return BadRequest(new { success = false, message = "Student Serial No is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    bool isInsert = string.IsNullOrWhiteSpace(request.Id) || request.Id == "0";

                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_STUDENTVEHICLEALLOTMENT_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", isInsert ? "0" : request.Id);
                        cmd.Parameters.AddWithValue("@STUDENTSERIALNO", request.StudentSerialNo);
                        cmd.Parameters.AddWithValue("@BRANCHCODE", request.PresentClass ?? "");
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear);
                        cmd.Parameters.AddWithValue("@ROUTENAME", request.RouteName ?? "");
                        cmd.Parameters.AddWithValue("@STOPPOINT", request.StopPoint ?? "");
                        cmd.Parameters.AddWithValue("@SNAME", request.SName ?? "");
                        cmd.Parameters.AddWithValue("@VEHICLENO", request.VehicleNo ?? "");
                        cmd.Parameters.AddWithValue("@FROMMONTH", request.FromMonth ?? "");
                        cmd.Parameters.AddWithValue("@TOMONTH", request.ToMonth ?? "");
                        cmd.Parameters.AddWithValue("@BusFee", request.BusFee);
                        cmd.Parameters.AddWithValue("@Address", request.Address ?? "");
                        cmd.Parameters.AddWithValue("@UserId", request.UserId ?? "");
                        cmd.Parameters.AddWithValue("@FINANCIALYEAR", request.FinancialYear ?? "");
                        cmd.Parameters.AddWithValue("@COURSECODE", request.Course ?? "");
                        cmd.Parameters.AddWithValue("@FName", request.FName ?? "");
                        cmd.Parameters.AddWithValue("@SYear", request.SYear ?? "");
                        cmd.Parameters.AddWithValue("@Section", request.Section ?? "");
                        cmd.Parameters.AddWithValue("@MobileNo", request.Mobile ?? "");
                        cmd.Parameters.AddWithValue("@PaidFee", request.Paid);
                        cmd.Parameters.AddWithValue("@DueFee", request.Due);
                        cmd.Parameters.AddWithValue("@StdMobNo", request.MobileNo ?? "");
                        cmd.Parameters.AddWithValue("@BloodGrp", request.BloodGroup ?? "");
                        cmd.Parameters.AddWithValue("@Remarks", request.Remarks ?? "");

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
                            return BadRequest(new { success = false, message = "Unable to save vehicle allotment details." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("deallocate")]
        public IActionResult DeallocateStudent([FromBody] StudentVehicleAllotmentSaveRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrWhiteSpace(request.StudentSerialNo))
            {
                return BadRequest(new { success = false, message = "Allotment ID and Student Serial No are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_deallocte", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", request.Id);
                        cmd.Parameters.AddWithValue("@STUDENTSERIALNO", request.StudentSerialNo ?? "");
                        cmd.Parameters.AddWithValue("@BRANCHCODE", request.PresentClass ?? "");
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", request.AcademicYear ?? "");
                        cmd.Parameters.AddWithValue("@ROUTENAME", request.RouteName ?? "");
                        cmd.Parameters.AddWithValue("@STOPPOINT", request.StopPoint ?? "");
                        cmd.Parameters.AddWithValue("@SNAME", request.SName ?? "");
                        cmd.Parameters.AddWithValue("@VEHICLENO", request.VehicleNo ?? "");
                        cmd.Parameters.AddWithValue("@FROMMONTH", request.FromMonth ?? "");
                        cmd.Parameters.AddWithValue("@TOMONTH", request.ToMonth ?? "");
                        cmd.Parameters.AddWithValue("@BUSFEE", request.BusFee ?? "");
                        cmd.Parameters.AddWithValue("@ADDRESS", request.Address ?? "");
                        cmd.Parameters.AddWithValue("@USERID", request.UserId ?? "");
                        cmd.Parameters.AddWithValue("@FINANCIALYEAR", request.FinancialYear ?? "");
                        cmd.Parameters.AddWithValue("@COURSECODE", request.Course ?? "");
                        cmd.Parameters.AddWithValue("@FNAME", request.FName ?? "");
                        cmd.Parameters.AddWithValue("@SYEAR", request.SYear ??"");
                        cmd.Parameters.AddWithValue("@SECTION", request.Section ?? "");
                        cmd.Parameters.AddWithValue("@MOBILENO", request.Mobile ?? "");
                        cmd.Parameters.AddWithValue("@PAIDFEE", request.Paid ?? "");
                        cmd.Parameters.AddWithValue("@DUEFEE", request.Due ?? "");
                        cmd.Parameters.AddWithValue("@STDMOBNO", request.MobileNo ?? "");
                        cmd.Parameters.AddWithValue("@BLOODGRP", request.BloodGroup ?? "");
                        cmd.Parameters.AddWithValue("@REMARKS", request.Remarks ?? "");

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return Ok(new
                        {
                            success = rowsAffected > 0,
                            message = rowsAffected > 0 ? "Deallocated Successfully" : "No allotment found or deallocated."
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteAllotment(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { success = false, message = "Allotment ID is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TRANSPORT_STUDENTVEHICLEALLOTMENT_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", id);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        return Ok(new
                        {
                            success = rowsAffected > 0,
                            message = rowsAffected > 0 ? "Record Deleted Successfully" : "Already some Amount Paid...! Please clear Remaining Amount"
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
        public IActionResult SearchAllotments([FromQuery] string searchName, [FromQuery] string academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("PROC_SEARCH_STDVEH", con))
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
