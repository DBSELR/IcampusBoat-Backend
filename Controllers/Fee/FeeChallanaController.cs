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
    public class FeeChallanaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public FeeChallanaController(IConfiguration configuration)
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
        [Route("GetCurrentAcyr")]
        public IActionResult GetCurrentAcyr()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select AcademicYear from tbl_AcademicYear where ISACTIVE = 'y'";
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
        [Route("FeeChallanaList")]
        public IActionResult FeeChallanaList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_FEECHALLANA_LIST", con))
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

        [HttpGet]
        [Route("GetMaxFeeRcptNo")]
        public IActionResult GetMaxFeeRcptNo()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_MAXFEERECEIPTNO", con))
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

        [HttpGet]
        [Route("FeeHeadsList")]
        public IActionResult FeeHeadsList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select FEENAME,FEETYPE,AMOUNT,IsActive from TBL_FEE_HEADS";
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
        [Route("FeeReimbursementFormat")]
        public IActionResult FeeReimbursementFormat()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_get_FEEReimbursementFormat", con))
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

        [HttpGet]
        [Route("UploadFileFields")]
        public IActionResult UploadFileFields()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_FEEREIMB_uploadFormatFeillds", con))
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
        [Route("SaveFeeChallana")]
        public IActionResult SaveFeeChallana([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_FEECHALLANA_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BCHNO", (object?)bol.BCNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RECEIPTNO", (object?)bol.ReceiptNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DATE", (object?)bol.Date ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)bol.FYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CASTE", (object?)bol.Caste ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TERM", (object?)bol.Term ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PAYMENTMODE", (object?)bol.PaymentMode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@REMARK", (object?)bol.Remark ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEEID", (object?)bol.FeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEENAME", (object?)bol.FeeName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEETYPE", (object?)bol.FeeType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEE", (object?)bol.Fee ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AMOUNT", (object?)bol.PayAmount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CID", (object?)bol.CID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DDNO", (object?)bol.DDNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DDDATE", (object?)bol.DDDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BANK", (object?)bol.Bank ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", (object?)bol.Branch ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Remarks", (object?)bol.Remark ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Fine", (object?)bol.Fine ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AddAmountID", (object?)bol.FeeIdAddAmount ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Fee Challan saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving fee challan", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("SaveRefundFee")]
        public IActionResult SaveRefundFee([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_RefundAmount_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BCHNO", (object?)bol.BCNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RECEIPTNO", (object?)bol.ReceiptNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DATE", (object?)bol.Date ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)bol.FYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CASTE", (object?)bol.Caste ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TERM", (object?)bol.Term ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PAYMENTMODE", (object?)bol.PaymentMode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@REMARK", (object?)bol.Remark ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEEID", (object?)bol.FeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEENAME", (object?)bol.FeeName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEETYPE", (object?)bol.FeeType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEE", (object?)bol.Fee ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AMOUNT", (object?)bol.PayAmount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CID", (object?)bol.CID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DDNO", (object?)bol.DDNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DDDATE", (object?)bol.DDDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BANK", (object?)bol.Bank ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", (object?)bol.Branch ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Fine", (object?)bol.Fine ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Refund fee saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving refund fee", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteFeeChallana")]
        public IActionResult DeleteFeeChallana([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_FEECHALLANA_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FID", (object?)bol.Fid ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Fee Challan deleted successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting fee challan", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("FeeChallanaLoad")]
        public IActionResult FeeChallanaLoad([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_FEECHALLANA_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Fid", (object?)bol.Fid ?? DBNull.Value);

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
        [Route("GetStudentData")]
        public IActionResult GetStudentData([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_GETSTUDENTDETAILS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);

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
        [Route("GetTerms")]
        public IActionResult GetTerms([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_LOADTERMS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);

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
        [Route("GetPaidFee")]
        public IActionResult GetPaidFee([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_LOADTERMS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.Fid ?? DBNull.Value);

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
        [Route("GetFeeDues")]
        public IActionResult GetFeeDues([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_DUE_YEARWISE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);

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
        [Route("FeeChallanaAdmissionLoad")]
        public IActionResult FeeChallanaAdmissionLoad([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_GETFEEDETAILS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TERM", (object?)bol.Term ?? DBNull.Value);

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
        [Route("GetReceiptNo")]
        public IActionResult GetReceiptNo([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT MAX(DBO.VAL(RECPTNO))+1 AS RECPTNO FROM TBL_FEE_FEECHALLAN_DETAILS WHERE FYEAR = @FYear";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@FYear", (object?)bol.FYear ?? DBNull.Value);

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
        [Route("GetSSNo")]
        public IActionResult GetSSNo([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
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
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);

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
        [Route("GetUnpaidChCount")]
        public IActionResult GetUnpaidChCount([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_UNPAIDCHALLAN", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)bol.FYear ?? DBNull.Value);

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
        [Route("GetBcNo")]
        public IActionResult GetBcNo([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_BANK_CHALLAN", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BANKCHNO", (object?)bol.BCNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);

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
        [Route("CheckBcNo")]
        public IActionResult CheckBcNo([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select COUNT(*) [COUNT] from TBL_FEE_FEECHALLAN_DETAILS WHERE BCHNO = @BCNO AND ACADEMICYEAR = @AcademicYear";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@BCNO", (object?)bol.BCNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);

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
        [Route("AjaxSSNo")]
        public IActionResult AjaxSSNo([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "Select DISTINCT RegistrationNo from TBL_ADM_STUDATA where RegistrationNo like @SSNo + '%'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@SSNo", (object?)bol.SSNo ?? "");

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
        [Route("AjaxSName")]
        public IActionResult AjaxSName([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "Select DISTINCT Rtrim(SName)+'-'+'('+Ltrim(registrationno)+')' SName from TBL_ADM_STUDATA where SName like '%' + @SNAME + '%'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@SNAME", (object?)bol.SNAME ?? "");

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
        [Route("GetFeeReceiptData")]
        public IActionResult GetFeeReceiptData([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_GETRECEIPTDATA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RECPTNO", (object?)bol.ReceiptNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)bol.FYear ?? DBNull.Value);

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
        [Route("GetFeeReceiptDataRefund")]
        public IActionResult GetFeeReceiptDataRefund([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_GETRECEIPTDATA_Refund", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RECPTNO", (object?)bol.ReceiptNO ?? DBNull.Value);

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
        [Route("GetFeeOnlineReceiptData")]
        public IActionResult GetFeeOnlineReceiptData([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SPR_FEE_ONLINERECEIPTDATA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RECPTNO", (object?)bol.ReceiptNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADAMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);

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
        [Route("GetPrintReceiptNo")]
        public IActionResult GetPrintReceiptNo([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT MAX(DBO.VAL(RECPTNO)) AS RECPTNO FROM TBL_FEE_FEECHALLAN_DETAILS WHERE FYEAR = @FYear AND SSNO=@SSNo";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@FYear", (object?)bol.FYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SSNo", (object?)bol.SSNo ?? DBNull.Value);

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
        [Route("GetStudentDataSearchName")]
        public IActionResult GetStudentDataSearchName([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_GETSTUDENTDETAILS_SEARCHNAME", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SNAME", (object?)bol.SSNo ?? DBNull.Value);

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
        [Route("GetPaidAmount")]
        public IActionResult GetPaidAmount([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select S.RECPTNO ,format(S.Createddate,'dd-MM-yyyy') as PaidDate,S.[Year],S.PAYMENTMODE,S.FEETYPE,S.Amount,S.RefundAmount,S.Remarks from TBL_FEE_FEECHALLAN_DETAILS S inner join tbl_Adm_studata A on S.SSNO = A.studentSerialNo where (A.registrationno = @SSNo Or STUDENTSERIALNO=@SSNo) and Year=@Year order by S.RECPTNO Desc";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@SSNo", (object?)bol.SSNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)bol.Year ?? DBNull.Value);

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
        [Route("GetHostelPaidAmount")]
        public IActionResult GetHostelPaidAmount([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select S.RECPTNO ,format(S.Createddate,'dd-MM-yyyy') as PaidDate,S.[Year],S.FEETYPE,S.Amount,S.Remark from TBL_HOSTEL_FEECHALLAN_DETAILS S inner join tbl_Adm_studata A on S.SSNO = A.studentSerialNo where (A.registrationno = @SSNo Or STUDENTSERIALNO=@SSNo) and Year=@Year";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@SSNo", (object?)bol.SSNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)bol.Year ?? DBNull.Value);

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
        [Route("GetHostelFeeReceiptData")]
        public IActionResult GetHostelFeeReceiptData([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SPR_FEE_HOSTELRECEIPTDATA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RECPTNO", (object?)bol.ReceiptNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);

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
        [Route("GetDate")]
        public IActionResult GetDate([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEEDATE_DIFF", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Pgrm", (object?)bol.CourseCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Academicyear", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PDATE", (object?)bol.Date ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@REGNO", (object?)bol.SSNo ?? DBNull.Value);

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
        [Route("GetStudentRegNo")]
        public IActionResult GetStudentRegNo([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT RegistrationNo from tbl_adm_studata where RegistrationNo = @SSNo or Admno = @SSNo";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@SSNo", (object?)bol.SSNo ?? DBNull.Value);

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
        [Route("GetConcessionAmount")]
        public IActionResult GetConcessionAmount([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select S.CONCNO,S.SSNO,format(S.DATE,'dd-MM-yyyy') as ConcessionDate,S.[Year],S.FEETYPE,S.FEE,S.CONCESSION,S.Remark from TBL_FEE_CONCESSION S inner join tbl_Adm_studata A on S.SSNO = A.studentSerialNo where S.[Year]=@Year and (A.registrationno = @SSNo Or STUDENTSERIALNO=@SSNo)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@SSNo", (object?)bol.SSNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)bol.Year ?? DBNull.Value);

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
        [Route("GetRefundAmount")]
        public IActionResult GetRefundAmount([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select S.ID,S.REFUNDNO,S.SSNO,format(S.DATE,'dd-MM-yyyy') as REFUNDDATE,S.[Year],S.FEETYPE,S.FEE,S.REFUNDAMT,S.Remark from TBL_FEE_RefundAmount S inner join tbl_Adm_studata A on S.SSNO = A.studentSerialNo where S.[Year]=@Year and (A.registrationno = @SSNo Or STUDENTSERIALNO=@SSNo)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@SSNo", (object?)bol.SSNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)bol.Year ?? DBNull.Value);

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
        [Route("GetReceipt")]
        public IActionResult GetReceipt([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select S.Registrationno,S.SName,F.Year,F.FEETYPE,F.FEE,F.AMOUNT,F.DATE from TBL_FEE_FEECHALLAN_DETAILS F inner join tbl_adm_studata S on F.SSNO=S.StudentSerialNo where ACADEMICYEAR=@AcademicYear and recptno=@ReceiptNO";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReceiptNO", (object?)bol.ReceiptNO ?? DBNull.Value);

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
        [Route("DeleteReceipt")]
        public IActionResult DeleteReceipt([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Proc_Delete_Receipt", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReceiptNo", (object?)bol.ReceiptNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DeletedId", (object?)bol.UserId ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Receipt deleted successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting receipt", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("CheckAdmin")]
        public IActionResult CheckAdmin([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select distinct UserGroup from tbl_Users where ACYR=@AcademicYear and UserID=@UserId";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", (object?)bol.UserId ?? DBNull.Value);

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
        [Route("AddAmount")]
        public IActionResult AddAmount([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_FEECHALLANA_AddAmt_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DATE", (object?)bol.Date ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@StudentName", (object?)bol.StudentName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Coursecode", (object?)bol.Course ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@branch", (object?)bol.Group ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CASTE", (object?)bol.Caste ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ADDAMOUNT", (object?)bol.AddAmt ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEEID", (object?)bol.FeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEENAME", (object?)bol.FeeName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEETYPE", (object?)bol.FeeType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CID", (object?)bol.CID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Remark", (object?)bol.Remark ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Add amount saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving add amount", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("FeeReimbursementSave")]
        public IActionResult FeeReimbursementSave([FromBody] IcampusBoatBackend.Models.Fee.FeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FeeReimbursement_BulkUpload", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACYR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@USERID", (object?)bol.UserId ?? DBNull.Value);

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
