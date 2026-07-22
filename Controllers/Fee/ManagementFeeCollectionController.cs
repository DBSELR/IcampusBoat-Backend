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
    public class ManagementFeeCollectionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ManagementFeeCollectionController(IConfiguration configuration)
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
        [Route("GetAccNos")]
        public IActionResult GetAccNos()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "Select Distinct ACNO from TBL_ACCOUNTS ORDER BY ACNO";
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

        [HttpPost]
        [Route("SaveDonationFeeChallana")]
        public IActionResult SaveDonationFeeChallana([FromBody] IcampusBoatBackend.Models.Fee.ManagementFeeCollection bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_DONATION_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.ACADEMICYEAR ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)bol.FYEAR ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DID", (object?)bol.DID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EDATE", (object?)bol.DATE ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ONDEMANDPAY", (object?)bol.ONDEMANDPAY ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AMOUNT", (object?)bol.AMOUNT ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACNO", (object?)bol.ACNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DDNo", (object?)bol.DDNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DDDate", (object?)bol.DDDATE ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankName", (object?)bol.BANKNAME ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", (object?)bol.BRANCH ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Remark", (object?)bol.REMARK ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CID", (object?)bol.CID ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Management Fee Collection saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving management fee collection", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteFeeChallana")]
        public IActionResult DeleteFeeChallana([FromBody] IcampusBoatBackend.Models.Fee.ManagementFeeCollection bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_FEECHALLANA_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FID", (object?)bol.DID ?? DBNull.Value);

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
        [Route("MFeeChallanListLoad")]
        public IActionResult MFeeChallanListLoad([FromBody] IcampusBoatBackend.Models.Fee.ManagementFeeCollection bol)
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
                        cmd.Parameters.AddWithValue("@Fid", (object?)bol.DID ?? DBNull.Value);

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
        [Route("MFeeChallanaLoad")]
        public IActionResult MFeeChallanaLoad([FromBody] IcampusBoatBackend.Models.Fee.ManagementFeeCollection bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_GETMFEEDETAILS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNO ?? DBNull.Value);

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
        public IActionResult GetReceiptNo([FromBody] IcampusBoatBackend.Models.Fee.ManagementFeeCollection bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT MAX(DBO.VAL(DID))+1 AS DID FROM TBL_FEE_DONATIONS WHERE FYEAR = @FYEAR";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)bol.FYEAR ?? DBNull.Value);

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
        public IActionResult GetSSNo([FromBody] IcampusBoatBackend.Models.Fee.ManagementFeeCollection bol)
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
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNO ?? DBNull.Value);

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
        public IActionResult AjaxSSNo([FromBody] IcampusBoatBackend.Models.Fee.ManagementFeeCollection bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "Select DISTINCT StudentSerialNo from TBL_ADM_STUDATA where StudentSerialNo like @SSNO + '%'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNO ?? "");

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
