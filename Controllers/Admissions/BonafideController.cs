using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Data;

namespace IcampusBoatBackend.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class BonafideController : ControllerBase
    {
        [HttpGet]
        [Route("load")]
        public IActionResult Load(string certificateNo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    // Get next auto number
                    string scNo = "";
                    using (SqlCommand cmd = new SqlCommand("EXEC SP_Certificate_AUTOSCNO_LIST @CertificateNO", con))
                    {
                        cmd.Parameters.AddWithValue("@CertificateNO", certificateNo ?? (object)DBNull.Value);
                        object result = cmd.ExecuteScalar();
                        if (result != null) scNo = result.ToString();
                    }

                    // Get list of certificates
                    List<object> certificates = new List<object>();

                    using (SqlCommand cmd = new SqlCommand("Bonafied_List", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                certificates.Add(new
                                {
                                    Id = reader["Id"],
                                    CertificateNo = reader["CertificateNo"],
                                    StudentName = reader["StudentName"],
                                    RegistrationNo = reader["SSNO"],
                                    Course = reader["COURSECODE"]
                                });
                            }
                        }
                    }
                    return Ok(new
                    {
                        success = true,
                        message = "Success",
                        data = new
                        {
                            nextCertificateNo = scNo,
                            certificates = certificates,
                            defaultDate = DateTime.UtcNow.AddHours(5).AddMinutes(30).ToString("dd-MM-yyyy")
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
        [Route("registration-details/{regNo}")]
        public IActionResult GetRegistrationDetails(string regNo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // Get SSNO
                    string ssNo = "";

                    using (SqlCommand cmd = new SqlCommand("SP_Bonafide_REGNO_SSNO_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", regNo);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                ssNo = reader[0].ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(ssNo))
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Registration No not existed in Studentdata"
                        });
                    }

                    // Get Student Details
                    using (SqlCommand cmd = new SqlCommand("Sp_Regno_Load", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", ssNo);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Ok(new
                                {
                                    success = true,
                                    message = "Success",
                                    data = new
                                    {
                                        ssNo = ssNo,
                                        dob = reader["DOB"] == DBNull.Value ? "" : Convert.ToDateTime(reader["DOB"]).ToString("dd-MM-yyyy"),
                                        studentName = reader["SName"].ToString(),
                                        fatherName = reader["FName"].ToString(),
                                        programme = reader["Course"].ToString(),
                                        programmeCode = reader["CourseCode"].ToString(),
                                        branch = reader["BranchName"].ToString(),
                                        branchCode = reader["BranchCode"].ToString(),
                                        year = reader["SYear"].ToString(),
                                        semester = reader["SSemester"].ToString(),
                                        academicYear = reader["AcadamicYear"].ToString(),
                                        address = reader["Address"].ToString(),
                                        courseCompletion = reader["CourseCompletion"].ToString()
                                    }
                                });
                            }
                        }
                    }

                    return Ok(new
                    {
                        success = false,
                        message = "Student details not found."
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

        [HttpPost]
        [Route("save")]
        public IActionResult Save([FromBody] BonafideSaveRequest request)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    // 1. Get internal SSNO from the registration number (DBC.SSNO = txtregno.Text)
                    string ssNo = "";
                    using (SqlCommand cmd = new SqlCommand("EXEC [SP_Bonafide_REGNO_SSNO_LOAD] @SSNO", con))
                    {
                        cmd.Parameters.AddWithValue("@SSNO", request.RegistrationNo ?? (object)DBNull.Value);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ssNo = reader[0].ToString();
                            }
                        }
                    }

                    // 2. Save Bonafide details
                    string query = "EXEC SP_Bonafide_Save @Id, @CertificateNO, @Date, @SSNO, @DOB, @StudentName, @FatherName, @Programme, @Branch, @Year, @Semister, @Purpose, @AcademicYear, @Reporttitle, @OriginalCertificate, @CourseComplete";

                    // Format dates to yyyy-MM-dd as in legacy code (Convert.ToDateTime(TxtDt.Text).ToString("yyyy-MM-dd"))
                    string formattedDate = "";
                    if (!string.IsNullOrEmpty(request.Date))
                    {
                        formattedDate = Convert.ToDateTime(request.Date).ToString("yyyy-MM-dd");
                    }
                    string formattedDob = "";
                    if (!string.IsNullOrEmpty(request.DOB))
                    {
                        formattedDob = Convert.ToDateTime(request.DOB).ToString("yyyy-MM-dd");
                    }

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", request.id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CertificateNO", request.CertificateNO ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Date", formattedDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SSNO", ssNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DOB", formattedDob ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@StudentName", request.StudentName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FatherName", request.FatherName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", request.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", request.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semister", request.Semister ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Purpose", request.Purpose ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", request.AcademicYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Reporttitle", request.Reporttitle ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@OriginalCertificate", request.OriginalCertificate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CourseComplete", request.CourseComplete ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new
                        {
                            success = true,
                            message = string.IsNullOrEmpty(request.id) ? "Data Saved Successfully" : "Data Updated Successfully",
                            data = rows
                        });
                    }
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
        [Route("edit/{id}")]
        public IActionResult Edit(string id)
        {
            using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("Bonafied_Load", con))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar)).Value = id;

                    DataTable dt = DAL.GetData_FrmSP(sqlcmd, DAL.QueryType.SP);

                    if (dt.Rows.Count > 0)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Success",
                            data = JsonConvert.SerializeObject(dt.AsEnumerable().Select(r => r.ItemArray))
                        });
                    }

                    return BadRequest(new
                    {
                        success = false,
                        message = "Record Not Found"
                    });
                }
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("EXEC Sp_Delete_Bonafied @Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id ?? (object)DBNull.Value);
                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new
                        {
                            success = true,
                            message = "Record Deleted Successfully",
                            data = rows
                        });
                    }
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

        [HttpPost]
        [Route("search")]
        public IActionResult Search([FromBody] BonafideSearchRequest request)
        {
            using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("SP_Bonafied_SSNO_Search", con))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@SearchName", SqlDbType.VarChar)).Value =
                        request.SearchName ?? (object)DBNull.Value;

                    DataTable dt = DAL.GetData_FrmSP(sqlcmd, DAL.QueryType.SP);

                    if (dt.Rows.Count > 0)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Success",
                            data = JsonConvert.SerializeObject(dt.AsEnumerable().Select(r => r.ItemArray))
                        });
                    }

                    return BadRequest(new
                    {
                        success = false,
                        message = "No Records Found"
                    });
                }
            }
        }

        [HttpGet]
        [Route("report/{id}")]
        public IActionResult Report(string id, string ssNo, string certificateNo)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DAL.SQLConnString);
                con.Open();

                using SqlCommand cmd = new SqlCommand("Sp_Adm_Print_Bc", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@SSNO", ssNo);
                cmd.Parameters.AddWithValue("@BCNo", certificateNo);

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
                        StudentName = reader["StudentName"].ToString(),
                        FatherName = reader["FatherName"].ToString(),
                        Programme = reader["course"].ToString(),
                        Branch = reader["Branchcode"].ToString(),
                        Purpose = reader["Purpose"].ToString()
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

        public class BonafideSaveRequest
        {
            public string? id { get; set; }
            public string? CertificateNO { get; set; }
            public string? Date { get; set; }
            public string? RegistrationNo { get; set; }
            public string? DOB { get; set; }
            public string? StudentName { get; set; }
            public string? FatherName { get; set; }
            public string? Programme { get; set; }
            public string? Branch { get; set; }
            public string? Year { get; set; }
            public string? Semister { get; set; }
            public string? Purpose { get; set; }
            public string? AcademicYear { get; set; }
            public string? Reporttitle { get; set; }
            public string? OriginalCertificate { get; set; }
            public string? CourseComplete { get; set; }
        }

        public class BonafideSearchRequest
        {
            public string? SearchName { get; set; }
        }

    }
}