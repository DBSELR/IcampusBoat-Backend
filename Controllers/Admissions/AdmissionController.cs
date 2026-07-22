using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Net;

namespace IcampusBoatBackend.Controllers.Admissions
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AdmissionController : ControllerBase
    {
 

    [HttpGet]
    [Route("load")]
    public IActionResult Load([FromQuery] string ssno, [FromQuery] string academicYear)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
            {
                con.Open();

                // Auto ID
                string autoId = "";
                using (SqlCommand cmd = new SqlCommand("SP_autoid", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SSNO", ssno ?? (object)DBNull.Value);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        autoId = result.ToString();
                }

                // Castes
                DataTable castes;
                using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_Caste_LIST"))
                {
                    castes = DAL.GetData_FrmSP(cmd, DAL.QueryType.SP);
                }

                // Library Groups
                DataTable libraryGroups;
                using (SqlCommand cmd = new SqlCommand("SP_ADMIN_LOADLIBRARYMEMBERGROUP"))
                {
                    libraryGroups = DAL.GetData_FrmSP(cmd, DAL.QueryType.SP);
                }

                // Sections
                DataTable sections;
                using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_Section_LIST"))
                {
                    sections = DAL.GetData_FrmSP(cmd, DAL.QueryType.SP);
                }

                // Statuses
                DataTable statuses;
                using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_Status_LIST"))
                {
                    statuses = DAL.GetData_FrmSP(cmd, DAL.QueryType.SP);
                }

                // Regulations
                DataTable regulations;
                using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_LOAD_REGULATION"))
                {
                    regulations = DAL.GetData_FrmSP(cmd, DAL.QueryType.SP);
                }

                // Programmes
                DataTable programmes;
                using (SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_Programme_LIST"))
                {
                    cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? (object)DBNull.Value);
                    programmes = DAL.GetData_FrmSP(cmd, DAL.QueryType.SP);
                }

                // Academic Years
                DataTable academicYears;
                using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_AcadamicYear_LIST"))
                {
                    academicYears = DAL.GetData_FrmSP(cmd, DAL.QueryType.SP);
                }

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = new
                    {
                        autoId,
                        castes = JsonConvert.SerializeObject(castes),
                        libraryGroups = JsonConvert.SerializeObject(libraryGroups),
                        sections = JsonConvert.SerializeObject(sections),
                        statuses = JsonConvert.SerializeObject(statuses),
                        regulations = JsonConvert.SerializeObject(regulations),
                        programmes = JsonConvert.SerializeObject(programmes),
                        academicYears = JsonConvert.SerializeObject(academicYears)
                    }
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }


    [HttpGet]
        [Route("programmes")]
        public IActionResult GetProgrammes(string academicYear)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_Programme_LIST", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("branches")]
        public IActionResult GetBranches(string programme, string academicYear)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_Branch_LIST", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@COURSE", programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("subcastes")]
        public IActionResult GetSubCastes(string caste)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_SubCaste_LIST", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Caste", caste ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        //[HttpGet]
        //[Route("tuition-fee")]
        //public IActionResult GetTuitionFee(string feeAdmType, string programme, string academicYear)
        //{
        //    try
        //    {
        //        using SqlConnection con = new SqlConnection(DAL.SQLConnString);
        //        con.Open();

        //        using SqlCommand cmd = new SqlCommand("LOAD_TUTIONFEE_AMT", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@FeeAdmType", feeAdmType ?? (object)DBNull.Value);
        //        cmd.Parameters.AddWithValue("@Programme", programme ?? (object)DBNull.Value);
        //        cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? (object)DBNull.Value);

        //        DataTable dt = new DataTable();
        //        using SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(dt);

        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Success",
        //            data = DAL.DataTableToList(dt)
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            success = false,
        //            message = ex.Message
        //        });
        //    }
        //}

        //[HttpPost]
        //[Route("scholarship-amount")]
        //public IActionResult GetScholarshipAmount([FromBody] StudentAdminRequest request)
        //{
        //    try
        //    {
        //        using SqlConnection con = new SqlConnection(DAL.SQLConnString);
        //        con.Open();

        //        using SqlCommand cmd = new SqlCommand("SP_ADMIN_SCHAMOUNT_LIST", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@ModeofAdm", request.ModeofAdm ?? (object)DBNull.Value);
        //        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
        //        cmd.Parameters.AddWithValue("@AYear", request.AYear ?? (object)DBNull.Value);
        //        cmd.Parameters.AddWithValue("@AcademicYear", request.AcadamicYear ?? (object)DBNull.Value);

        //        DataTable dt = new DataTable();
        //        using SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(dt);

        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Success",
        //            data = DAL.DataTableToList(dt)
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            success = false,
        //            message = ex.Message
        //        });
        //    }
        //}

        [HttpGet]
        [Route("route-points")]
        public IActionResult GetRoutePoints(string academicYear, string routeName)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_RoutePoint_List", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RouteName", routeName ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("bus-fee")]
        public IActionResult GetBusFee(string academicYear, string routePoint)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_ADMIN_BusFee_lIST", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AcademicYear", academicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RoutePoint", routePoint ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        //[HttpGet]
        //[Route("regulation-batches")]
        //public IActionResult GetRegulationBatches(string regulation)
        //{
        //    try
        //    {
        //        using SqlConnection con = new SqlConnection(DAL.SQLConnString);
        //        con.Open();

        //        using SqlCommand cmd = new SqlCommand("SP_LOADBATCH", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@Regulation", regulation ?? (object)DBNull.Value);

        //        DataTable dt = new DataTable();
        //        using SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(dt);

        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Success",
        //            data = DAL.DataTableToList(dt)
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            success = false,
        //            message = ex.Message
        //        });
        //    }
        //}

        [HttpGet]
        [Route("student-details")]
        public IActionResult GetStudentDetails([FromQuery] string studentSerialNo)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_GET_STUDENTDATA", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@STUDENTSERIALNO", studentSerialNo ?? (object)DBNull.Value);

                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No Records Found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = new
                    {
                        StudentSerialNo = reader["StudentSerialNo"]?.ToString(),
                        StudentName = reader["SName"]?.ToString(),
                        FatherName = reader["FName"]?.ToString(),
                        MotherName = reader["MName"]?.ToString(),
                        Programme = reader["CourseCode"]?.ToString(),
                        Branch = reader["Branchcode"]?.ToString(),
                        Section = reader["Section"]?.ToString(),
                        AdmissionNo = reader["AdmNo"]?.ToString(),
                        RegistrationNo = reader["RegistrationNo"]?.ToString(),
                        DOB = reader["DOB"]?.ToString(),
                        Gender = reader["Gender"]?.ToString(),
                        MobileNo = reader["StdMobNo"]?.ToString(),
                        ParentMobileNo = reader["ParentMbNo"]?.ToString(),
                        Address = reader["Address"]?.ToString()
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        //[HttpGet]
        //[Route("validate-regno/{regNo}")]
        //public IActionResult ValidateRegNo(string regNo)
        //{
        //    try
        //    {
        //        using SqlConnection con = new SqlConnection(DAL.SQLConnString);
        //        con.Open();

        //        DataTable dt = new DataTable();
        //        using (SqlCommand cmd = new SqlCommand("SP_CHECHREGNO", con))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@RegistrationNo", regNo ?? (object)DBNull.Value);
        //            using SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            da.Fill(dt);
        //        }

        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Success",
        //            data = DAL.DataTableToList(dt)
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            success = false,
        //            message = ex.Message
        //        });
        //    }
        //}

        [HttpGet]
        [Route("validate-admno/{admNo}")]
        public IActionResult ValidateAdmNo(string admNo)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand("SP_GET_AdmNo_Count", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AdmNo", admNo ?? (object)DBNull.Value);
                    using SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = DAL.DataTableToList(dt)

                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("save")]
        [HttpPost]
        public IActionResult Save([FromBody] StudentAdminRequest request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_SAVE", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IDENT", request.Ident ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@STUDENTSERIALNO", request.StudentSerialNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AdmNo", request.AdmNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@REGISTRATIONNO", request.RegistrationNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ADMISSIONDATE", request.AdmissionDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DOB", request.DOB ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SNAME", request.SName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MODEOFADM", request.ModeofAdm ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PROGRAMME", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BRANCH", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SECTION", request.Section ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AYEAR", request.AYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SYEAR", request.SYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ACADAMICYEAR", request.AcadamicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@JACADAMICYEAR", request.JAcadamicYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ASEMESTER", request.ASemester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSEMESTER", request.SSemester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CASTE", request.Caste ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SUBCASTE", request.SubCaste ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GENDER", request.Gender ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NATIONALITY", request.Nationality ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RELIGION", request.Religion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BLOODGRP", request.BloodGrp ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PH", request.PH ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TUITIONFEE", request.TuitionFee ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MISCELLANEOUSFEE", request.Miscellaneousfee ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SCHAMOUNT", request.SchAmount ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BHFEE", request.BHFee ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LHFEE", request.LHFee ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BUSFEE", request.BusFee ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DONATION", request.Donation ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RANK", request.Rank ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HALLTICKETNO", request.HallTicketNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSCSCHOOLNAME", request.SSCSchoolName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSCMARKSPERCENTAGE", request.SSCMarksPercentage ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LASTATTENDEDCOLLEGENAME", request.LastAttendedCollegeName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GROUPSUBJECTSMARKSPERCENTAGE", request.GroupSubjectsMarksPercentage ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AGGREGATE", request.Aggregate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MYPASSING", request.MYPassing ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FNAME", request.FName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PARENTOCCUPATION", request.ParentOccupation ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@INCOME", request.Income ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MNAME", request.MName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ADDRESS", request.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PARENTMBNO", request.ParentMbNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@STDMOBNO", request.StdMobNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AADHAARNO", request.AadhaarNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RATIONCARDNO", request.RationcardNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EMAILID", request.Emailid ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@USERID", request.UserId ?? (object)DBNull.Value);
                //cmd.Parameters.AddWithValue("@STATUS", request.STATUS ?? (object)DBNull.Value);
                string status = request.STATUS;

                if (string.IsNullOrWhiteSpace(status))
                {
                    if (!string.IsNullOrWhiteSpace(request.StudentSerialNo) &&
                        !string.IsNullOrWhiteSpace(request.RegistrationNo))
                    {
                        status = "UPDATE";
                    }
                    else
                    {
                        status = "INSERT";
                    }
                }

                cmd.Parameters.AddWithValue("@STATUS", status);

                cmd.Parameters.AddWithValue("@SSC_HALLTICKETNO", request.SSC_HallTicketNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSC_BOARD", request.SSC_Board ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSC_Studied", request.SSCStudied ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSC_AGGREGATE", request.SSC_Aggregate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSC_MYPASSING", request.SSC_MYPassing ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@INT_COLLEGENAME", request.Int_CollegeName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@INT_MARKSPERC", request.Int_MarksPerc ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@INT_HALLTICKENO", request.Int_HallTicketNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@INT_BOARD", request.Int_Board ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@INT_AGGREGATE", request.Int_Aggregate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@INT_MYPASSING", request.Int_MYPassing ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UG_COLLEGENAME", request.UG_CollegeName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UG_MARKSPERC", request.UG_MarksPerc ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UG_HALLTICKETNO", request.UG_HallTicketNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UG_UNIVERSITY", request.UG_University ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UG_AGGREGATE", request.UG_Aggregate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UG_MYPASSING", request.UG_MYPassing ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@ADMISSION_TYPE", request.Fee_admType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Isactive", request.Isactive);
                cmd.Parameters.AddWithValue("@Reason", request.Reason ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Date", request.Date ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AStatus", request.AStatus ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SET_ADM_Type", request.SET ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HallTicket", request.HallTicket ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SETRank", request.SETRank ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BranchRank", request.BranchRank ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Mole1", request.Mole1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Mole2", request.Mole2 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@States", request.States ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Category", request.Category ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RoutePoint", request.RoutePoint ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ParentMbNo2", request.ParentMbNo2 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IncomeCNo", request.ICNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MotherTongue", request.MotherTongue ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Maths", request.Maths ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Physics", request.Physics ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Chemistry", request.Chemistry ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SpotAdmFee", request.SpotAdmFee ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LE", request.LE);
                cmd.Parameters.AddWithValue("@Fac_Child", request.Fac_Child);
                cmd.Parameters.AddWithValue("@JnanaBhumiId", request.JnanaBhumiId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Regulation", request.Regulation ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MAadharNo", request.MAadharNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UGCourse", request.UgCourse ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LIBRARYMEMBERGROUP", request.LIBRARYMEMBERGROUP ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SCHLOR", request.SCHLOR);
                cmd.Parameters.AddWithValue("@MODEOFCTGY", request.ModeofCtgy ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AllottedQuota", request.AllottedQuota ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NSP", request.NSP);
                cmd.Parameters.AddWithValue("@Apaar", request.APAAR ?? (object)DBNull.Value);
                int rows = cmd.ExecuteNonQuery();

                if (rows <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = request.STATUS == "INSERT"
                            ? "Data not saved."
                            : "No record updated."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = request.STATUS == "INSERT"
                        ? "Data Saved Successfully"
                        : "Data Updated Successfully"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("edit/{studentSerialNo}")]
        public IActionResult Edit(string studentSerialNo)
        {
            try
            {
                studentSerialNo = WebUtility.UrlDecode(studentSerialNo);
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_LOAD", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentSerialNo", studentSerialNo ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No Records Found"
                    });
                }

                return Ok(studentSerialNo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpDelete]
        [Route("{studentSerialNo}")]
        public IActionResult Delete(string studentSerialNo)
        {
            try
            {
                studentSerialNo = WebUtility.UrlDecode(studentSerialNo);
                Console.WriteLine(studentSerialNo);
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_DELETE", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentSerialNo", studentSerialNo ?? (object)DBNull.Value);

                int rows = cmd.ExecuteNonQuery();

                return Ok(new
                {
                    Received = studentSerialNo,
                    success = true,
                    message = "Record Deleted Successfully",
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("search")]
        public IActionResult Search([FromBody] StudentAdminRequest request)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_LIST_Search", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Course", request.Programme ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", request.SYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Sem", request.SSemester ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Section", request.Section ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AcademicYear", request.AcadamicYear ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("report/{studentSerialNo}")]
        public IActionResult Report(string studentSerialNo)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_GET_STUDENTDATA", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentSerialNo", studentSerialNo ?? (object)DBNull.Value);

                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No Records Found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = new
                    {
                        StudentSerialNo = reader["StudentSerialNo"]?.ToString(),
                        StudentName = reader["SName"]?.ToString(),
                        FatherName = reader["FName"]?.ToString(),
                        MotherName = reader["MName"]?.ToString(),
                        Programme = reader["Programme"]?.ToString(),
                        Branch = reader["Branch"]?.ToString(),
                        Section = reader["Section"]?.ToString(),
                        AdmissionNo = reader["AdmNo"]?.ToString(),
                        RegistrationNo = reader["RegistrationNo"]?.ToString(),
                        DOB = reader["DOB"]?.ToString(),
                        Gender = reader["Gender"]?.ToString(),
                        MobileNo = reader["StdMobNo"]?.ToString(),
                        ParentMobileNo = reader["ParentMbNo"]?.ToString(),
                        Address = reader["Address"]?.ToString()
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("autocomplete/occupation")]
        public IActionResult AutocompleteOccupation(string prefixText)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_POCCUPATION_AJAX", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PrefixText", prefixText ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("autocomplete/school-name")]
        public IActionResult AutocompleteSchoolName(string prefixText)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_SCHOOLNAME_AJAX", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PrefixText", prefixText ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("autocomplete/inter-college")]
        public IActionResult AutocompleteInterCollege(string prefixText)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_INTERCLGNAME_AJAX", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PrefixText", prefixText ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("autocomplete/student-name")]
        public IActionResult AutocompleteStudentName(string searchName)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("SP_AJAX_SNAME", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SearchName", searchName ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = DAL.DataTableToList(dt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }

    public class StudentAdminRequest
    {
        public string? Ident { get; set; }
        public string? StudentSerialNo { get; set; }
        public string? AdmNo { get; set; }
        public string? RegistrationNo { get; set; }
        public string? AdmissionDate { get; set; }
        public string? DOB { get; set; }
        public string? SName { get; set; }
        public string? ModeofAdm { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Section { get; set; }
        public string? AYear { get; set; }
        public string? SYear { get; set; }
        public string? AcadamicYear { get; set; }
        public string? JAcadamicYear { get; set; }
        public string? ASemester { get; set; }
        public string? SSemester { get; set; }
        public string? Caste { get; set; }
        public string? SubCaste { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? Religion { get; set; }
        public string? BloodGrp { get; set; }
        public string? PH { get; set; }
        public string? TuitionFee { get; set; }
        public string? Miscellaneousfee { get; set; }
        public string? SchAmount { get; set; }
        public string? BHFee { get; set; }
        public string? LHFee { get; set; }
        public string? BusFee { get; set; }
        public string? Donation { get; set; }
        public string? Rank { get; set; }
        public string? HallTicketNo { get; set; }
        public string? SSCSchoolName { get; set; }
        public string? SSCMarksPercentage { get; set; }
        public string? LastAttendedCollegeName { get; set; }
        public string? GroupSubjectsMarksPercentage { get; set; }
        public string? Aggregate { get; set; }
        public string? MYPassing { get; set; }
        public string? FName { get; set; }
        public string? ParentOccupation { get; set; }
        public string? Income { get; set; }
        public string? MName { get; set; }
        public string? Address { get; set; }
        public string? ParentMbNo { get; set; }
        public string? StdMobNo { get; set; }
        public string? AadhaarNo { get; set; }
        public string? RationcardNo { get; set; }
        public string? Emailid { get; set; }
        public string? UserId { get; set; }
        public string? STATUS { get; set; }
        public string? SSC_HallTicketNo { get; set; }
        public string? SSC_Board { get; set; }
        public string? SSCStudied { get; set; }
        public string? SSC_Aggregate { get; set; }
        public string? SSC_MYPassing { get; set; }
        public string? Int_CollegeName { get; set; }
        public string? Int_MarksPerc { get; set; }
        public string? Int_HallTicketNo { get; set; }
        public string? Int_Board { get; set; }
        public string? Int_Aggregate { get; set; }
        public string? Int_MYPassing { get; set; }
        public string? UG_CollegeName { get; set; }
        public string? UG_MarksPerc { get; set; }
        public string? UG_HallTicketNo { get; set; }
        public string? UG_University { get; set; }
        public string? UG_Aggregate { get; set; }
        public string? UG_MYPassing { get; set; }
        public string? Fee_admType { get; set; }
        public bool Isactive { get; set; }
        public string? Reason { get; set; }
        public string? Date { get; set; }
        public string? AStatus { get; set; }
        public string? SET { get; set; }
        public string? HallTicket { get; set; }
        public string? SETRank { get; set; }
        public string? BranchRank { get; set; }
        public string? Mole1 { get; set; }
        public string? Mole2 { get; set; }
        public string? States { get; set; }
        public string? Category { get; set; }
        public string? RoutePoint { get; set; }
        public string? ParentMbNo2 { get; set; }
        public string? ICNo { get; set; }
        public string? MotherTongue { get; set; }
        public string? Maths { get; set; }
        public string? Physics { get; set; }
        public string? Chemistry { get; set; }
        public string? SpotAdmFee { get; set; }
        public bool LE { get; set; }
        public bool Fac_Child { get; set; }
        public string? JnanaBhumiId { get; set; }
        public string? Regulation { get; set; }
        public string? MAadharNo { get; set; }
        public string? UgCourse { get; set; }
        public string? LIBRARYMEMBERGROUP { get; set; }
        public bool SCHLOR { get; set; }
        public string? ModeofCtgy { get; set; }
        public string? AllottedQuota { get; set; }
        public bool NSP { get; set; }
        public string? APAAR { get; set; }
        public string? SearchName { get; set; }
    }
}
