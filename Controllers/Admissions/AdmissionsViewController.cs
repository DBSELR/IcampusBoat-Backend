using IcampusBoatBackend.Models.Admissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;

namespace IcampusBoatBackend.Controllers.Admissions
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AdmissionsViewController : ControllerBase
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

        /// <summary>
        /// Load initial data including regulations, programmes, academic years, castes, route points, and category modes.
        /// </summary>
        [HttpGet("load-initial")]
        public IActionResult LoadInitial([FromQuery] string academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // 1. Regulations
                    DataTable dtRegulations = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_LOAD_REGULATION", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtRegulations); }
                    }

                    // 2. Programmes
                    DataTable dtProgrammes = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_Programme_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtProgrammes); }
                    }

                    // 3. Academic Years
                    DataTable dtAcadYears = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_AcadamicYear_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtAcadYears); }
                    }

                    // 4. Route Names
                    DataTable dtRouteNames = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_RouteName_List", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtRouteNames); }
                    }

                    // 5. Castes
                    DataTable dtCastes = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_Caste_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtCastes); }
                    }

                    // 6. States (Static List as in legacy system)
                    List<string> statesList = new List<string>
                    {
                        "Andhra Pradesh", "Telangana", "Karnataka", "Tamil Nadu", "Maharashtra", "Kerala", "Other"
                    };

                    // 7. Categories
                    DataTable dtCategories = new DataTable();
                    string catQuery = "Sp_Load_Category_Modes @Caste='0', @Categorycode='0', @Category='0', @Academicyear=@AcYr";
                    using (SqlCommand cmd = new SqlCommand(catQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@AcYr", academicYear);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dtCategories); }
                    }

                    return Ok(new
                    {
                        success = true,
                        data = new
                        {
                            regulations = DAL.DataTableToList(dtRegulations),
                            programmes = DAL.DataTableToList(dtProgrammes),
                            academicYears = DAL.DataTableToList(dtAcadYears),
                            routeNames = DAL.DataTableToList(dtRouteNames),
                            castes = DAL.DataTableToList(dtCastes),
                            states = statesList,
                            categories = DAL.DataTableToList(dtCategories)
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("branches")]
        public IActionResult GetBranches([FromQuery] string programme, [FromQuery] string academicYear)
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
                        cmd.Parameters.AddWithValue("@Course", programme);
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
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

        [HttpGet("years")]
        public IActionResult GetYears([FromQuery] string programme, [FromQuery] string academicYear)
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
                        cmd.Parameters.AddWithValue("@Course", programme);
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
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

        [HttpGet("admission-modes")]
        public IActionResult GetAdmissionModes([FromQuery] string programme, [FromQuery] string academicYear, [FromQuery] string syear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_AdmissionMode_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", academicYear);
                        cmd.Parameters.AddWithValue("@COURSECODE", programme);
                        cmd.Parameters.AddWithValue("@SYear", syear);
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

        [HttpGet("sections")]
        public IActionResult GetSections([FromQuery] string programme, [FromQuery] string branch, [FromQuery] string syear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_SECTION", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@COURSECODE", programme);
                        cmd.Parameters.AddWithValue("@BRANCHCODE", branch);
                        cmd.Parameters.AddWithValue("@STDYEAR", syear);
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

        [HttpGet("subcastes")]
        public IActionResult GetSubCastes([FromQuery] string caste)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_SubCaste_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Caste", caste);
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

        [HttpGet("route-points")]
        public IActionResult GetRoutePoints([FromQuery] string academicYear, [FromQuery] string routeName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_RoutePoint_List", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        cmd.Parameters.AddWithValue("@RouteName", routeName);
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

        [HttpGet("bus-fee")]
        public IActionResult GetBusFee([FromQuery] string academicYear, [FromQuery] string routePoint)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_BusFee_lIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        cmd.Parameters.AddWithValue("@RoutePoint", routePoint);
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

        [HttpGet("lib-groups")]
        public IActionResult GetLibraryMemberGroups()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_LOADLIBRARYMEMBERGROUP", con))
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

        [HttpGet("reg-lib-group")]
        public IActionResult GetRegistrationLibGroup([FromQuery] string regNo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_LOADREGNOLIBRARYMEMBERGROUP", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@REGNO", regNo);
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


 

        [HttpGet("student-details/serial/{serialNo}")]
        public IActionResult GetStudentDetailsBySerial(string serialNo)
        {
            try
            {
                serialNo = WebUtility.UrlDecode(serialNo);
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_STUDENTDATA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@STUDENTSERIALNO", serialNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        return Ok(new { success = true, data = DAL.DataTableToList(dt)[0] });
                    }
                    return Ok(new { success = false, message = "No details found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("student-details/regno/{regNo}")]
        public IActionResult GetStudentDetailsByRegNo(string regNo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_RegNO", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RegistrationNo", regNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        return Ok(new { success = true, data = DAL.DataTableToList(dt)[0] });
                    }
                    return Ok(new { success = false, message = "No details found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("student-details/admno/{admNo}")]
        public IActionResult GetStudentDetailsByAdmNo(string admNo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_AdmNo", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AdmNo", admNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        return Ok(new { success = true, data = DAL.DataTableToList(dt)[0] });
                    }
                    return Ok(new { success = false, message = "No details found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("student-list")]
        public IActionResult GetStudentList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_LIST", con))
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

        [HttpGet("search-students")]
        public IActionResult SearchStudents([FromQuery] string programme, [FromQuery] string branch, [FromQuery] string syear, [FromQuery] string ssemester, [FromQuery] string section, [FromQuery] string academicYear, [FromQuery] string? searchName = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();

                    if (!string.IsNullOrWhiteSpace(searchName))
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_STUDATA_Search", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Programme", programme ?? "");
                            cmd.Parameters.AddWithValue("@Branch", branch ?? "");
                            cmd.Parameters.AddWithValue("@Year", syear ?? "0");
                            cmd.Parameters.AddWithValue("@Semister", ssemester ?? "0");
                            cmd.Parameters.AddWithValue("@Section", section ?? "");
                            cmd.Parameters.AddWithValue("@SearchName", searchName);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_LIST_Search", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Course", programme ?? "");
                            cmd.Parameters.AddWithValue("@Branch", branch ?? "");
                            cmd.Parameters.AddWithValue("@Year", syear ?? "0");
                            cmd.Parameters.AddWithValue("@Sem", ssemester ?? "0");
                            cmd.Parameters.AddWithValue("@Section", section ?? "");
                            cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? "");
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
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
