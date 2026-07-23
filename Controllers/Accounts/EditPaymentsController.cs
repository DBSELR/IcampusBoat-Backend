using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using IcampusBoatBackend.Models.Accounts;

namespace IcampusBoatBackend.Controllers.Accounts
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class EditPaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EditPaymentsController(IConfiguration configuration)
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

        /// <summary>
        /// Retrieves payment head accounts for dropdown loading
        /// Parameters: 2 (<= 2, uses method params)
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Route("GetPaymentHeads")]
        public IActionResult GetPaymentHeads(string? academicYear, string? fYear)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_PAYMENT_GETPHAC", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)academicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)fYear ?? DBNull.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ReadRow(reader));
                            }
                        }
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves existing payment details by voucher number and financial year for editing
        /// Parameters: 2 (<= 2, uses method params)
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Route("GetPaymentData")]
        public IActionResult GetPaymentData(string? voucherNo, string? fYear)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_GETPAYMENT_EDITDATA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VOUCHERNO", (object?)voucherNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)fYear ?? DBNull.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ReadRow(reader));
                            }
                        }
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves distinct account numbers list for dropdown loading
        /// Parameters: 0 (<= 2, uses method params)
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAccNos")]
        public IActionResult GetAccNos()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT DISTINCT ACNO FROM TBL_ACCOUNTS ORDER BY ACNO";
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
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing payment record
        /// Parameters: 15 (> 2, uses [FromBody] EditPayments model)
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("SavePayment")]
        public IActionResult SavePayment([FromBody] EditPayments model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_PAYMENT_EDIT", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)model.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)model.FYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@VOUCHERNO", (object?)model.VoucherNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PAYMENTDATE", (object?)model.PaymentDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PAYEENAME", (object?)model.PayeeName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HEADACCOUNT", (object?)model.HeadAccount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PACCOUNT", (object?)model.PAccount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PAYMENTTYPE", (object?)model.PaymentType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PAYMENTMODE", (object?)model.PaymentMode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CHEQUENO", (object?)model.ChequeNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CCDATE", (object?)model.CcDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PURPOSE", (object?)model.Purpose ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@REMARK", (object?)model.Remark ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AMOUNT", (object?)model.Amount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CID", (object?)model.CId ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Data Updated successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}

