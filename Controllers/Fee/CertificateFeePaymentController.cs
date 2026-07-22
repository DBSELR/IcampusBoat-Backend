using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IcampusBoatBackend.Controllers.Fee
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateFeePaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public CertificateFeePaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private Dictionary<string, object> ReadRow(SqlDataReader reader)
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var camel = char.ToLowerInvariant(name[0]) + name.Substring(1);
                row[camel] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            return row;
        }

        [HttpGet]
        [Route("LoadSYearDetails")]
        public IActionResult LoadSYearDetails()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select distinct cast(syear as varchar) syear from TBL_ADM_STUDATA";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ReadRow(reader));
                            }
                        }
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("LoadFeePayDetails")]
        public IActionResult LoadFeePayDetails()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Sp_LoadCerFeePayDetails", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ReadRow(reader));
                            }
                        }
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("GetCertificateFeePayDetails/{ssno}/{year}")]
        public IActionResult GetCertificateFeePayDetails(string ssno, string year)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Sp_CerFeePay", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@SSNO", (object?)bol.StudentSerialNo ?? DBNull.Value);
                        //cmd.Parameters.AddWithValue("@YEAR", (object?)bol.SYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SSNO", ssno);
                        cmd.Parameters.AddWithValue("@YEAR", year);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ReadRow(reader));
                            }
                        }
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("GetSSNo/{Regno}")]
        public IActionResult GetSSNo(string Regno)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_LOADREGNOTOSSNO", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", Regno);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ReadRow(reader));
                            }
                        }
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("GetStudentData/{Regno}")]
        public IActionResult GetStudentData(string Regno)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_REGNO_SSNO_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", Regno);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ReadRow(reader));
                            }
                        }
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("RepProcedureCFP/{CPFNO}")]
        public IActionResult RepProcedureCFP(string CPFNO)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("REP_CERTIFICATEFEEPAY", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CPFNO", CPFNO);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ReadRow(reader));
                            }
                        }
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("RepProcedureRegno/{SSNO}")]
        public IActionResult RepProcedureRegno(string SSNO)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SPR_ITCER", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StudentSerialNo", SSNO);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ReadRow(reader));
                            }
                        }
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("SaveCPFDetails")]
        public IActionResult SaveCPFDetails([FromBody] IcampusBoatBackend.Models.Fee.CertificateFeePayment bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_CerFeePaymentDetails", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Tid", (object?)bol.ident ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Date", (object?)bol.Date ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CFPNO", (object?)bol.CPFNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYear", (object?)bol.FYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcadamicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.StudentSerialNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SName", (object?)bol.SName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Fname", (object?)bol.Fname ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", (object?)bol.Branch ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@University", (object?)bol.University ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYear", (object?)bol.SYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TutionFee", (object?)bol.TuitionFee ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SpecialFee", (object?)bol.SpecialFee ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserID", (object?)bol.Userid ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Certificate Fee Payment saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving certificate fee payment", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("GetSSNoAjax/{SSNO}")]
        public IActionResult GetSSNoAjax(string SSNO)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT DISTINCT RegistrationNo FROM TBL_ADM_STUDATA WHERE RegistrationNo like @SSNO + '%'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@SSNO", (object?)SSNO ?? "");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ReadRow(reader));
                            }
                        }
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteCerFeepayDetails/{ident}")]
        public IActionResult DeleteCerFeepayDetails(string ident)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP__CerFeePay_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ident", (object?)ident ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Certificate Fee Payment deleted successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting certificate fee payment", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("SearchName/{name}")]
        public IActionResult SearchName(string name)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_CERTFEEPAY_SEARCH", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchName", (object?)name ?? DBNull.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ReadRow(reader));
                            }
                        }
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("GenerateCPFNO/{ACYR}")]
        public IActionResult GenerateCPFNO(string ACYR)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT MAX(DBO.VAL(CFPNO))+1 AS CFPNO FROM tbl_CerticateFeePay WHERE ACADEMICYEAR = @AcadamicYear";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@AcadamicYear", (object?)ACYR ?? DBNull.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ReadRow(reader));
                            }
                        }
                    }
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
