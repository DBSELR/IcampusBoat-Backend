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
    public class EditFeeChallanaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EditFeeChallanaController(IConfiguration configuration)
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

        [HttpPost]
        [Route("SaveFeeChallana")]
        public IActionResult SaveFeeChallana([FromBody] IcampusBoatBackend.Models.Fee.EditFeeChallana bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_EDITFEECHALLANA_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RECEIPTNO", (object?)bol.ReceiptNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DATE", (object?)bol.Date ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)bol.FYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CASTE", (object?)bol.Caste ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TERM", (object?)bol.Term ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@REMARK", (object?)bol.Remark ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEEID", (object?)bol.FeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEENAME", (object?)bol.FeeName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEETYPE", (object?)bol.FeeType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEE", (object?)bol.Fee ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OLDAMOUNT", (object?)bol.OldFee ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NEWAMOUNT", (object?)bol.NewFee ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CID", (object?)bol.CID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ID", (object?)bol.ID ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Edit Fee Challan saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving edited fee challan", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteFeeChallana")]
        public IActionResult DeleteFeeChallana([FromBody] IcampusBoatBackend.Models.Fee.EditFeeChallana bol)
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
        public IActionResult FeeChallanaLoad([FromBody] IcampusBoatBackend.Models.Fee.EditFeeChallana bol)
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
        [Route("FeeChallanaAdmissionLoad")]
        public IActionResult FeeChallanaAdmissionLoad([FromBody] IcampusBoatBackend.Models.Fee.EditFeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_EDITFEEDETAILS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RECEIPTNO", (object?)bol.ReceiptNO ?? DBNull.Value);
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
        [Route("GetReceiptNo")]
        public IActionResult GetReceiptNo([FromBody] IcampusBoatBackend.Models.Fee.EditFeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT MAX(DBO.VAL(RECPTNO))+1 AS RECPTNO FROM TBL_FEE_FEECHALLAN_DETAILS WHERE ACADEMICYEAR = @AcademicYear";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
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
        [Route("UpdatePaymentMode")]
        public IActionResult UpdatePaymentMode([FromBody] IcampusBoatBackend.Models.Fee.EditFeeChallana bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_UpdatePaymentMode", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RECEIPTNO", (object?)bol.ReceiptNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)bol.FYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PAYMENTMODE", (object?)bol.PaymentMode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CID", (object?)bol.CID ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Payment mode updated successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating payment mode", error = ex.Message });
            }
        }
    }
}
