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
    public class MiscFeeChallanaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public MiscFeeChallanaController(IConfiguration configuration)
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
                    using (SqlCommand cmd = new SqlCommand("SP_MISC_FEECHALLANA_LIST", con))
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
        public IActionResult SaveFeeChallana([FromBody] IcampusBoatBackend.Models.Fee.MiscFeeChallana bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_MISC_FEECHALLANA_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RECEIPTNO", (object?)bol.ReceiptNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DATE", (object?)bol.Date ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SSNO", (object?)bol.SSNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MISCTYPE", (object?)bol.MiscType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)bol.FYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PAYMENTMODE", (object?)bol.PaymentMode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NAME", (object?)bol.Name ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PURPOSE", (object?)bol.Purpose ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DDNO", (object?)bol.DDNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DDDATE", (object?)bol.DDDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BANK", (object?)bol.Bank ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", (object?)bol.Branch ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEEID", (object?)bol.FeeId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEENAME", (object?)bol.FeeName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEETYPE", (object?)bol.FeeType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACNO", (object?)bol.ACNO ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AMOUNT", (object?)bol.PayAmount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CID", (object?)bol.CID ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Misc Fee Challan saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving misc fee challan", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteFeeChallana")]
        public IActionResult DeleteFeeChallana([FromBody] IcampusBoatBackend.Models.Fee.MiscFeeChallana bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_MISC_FEECHALLANA_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FID", (object?)bol.Fid ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Misc Fee Challan deleted successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting misc fee challan", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("FeeChallanaLoad")]
        public IActionResult FeeChallanaLoad([FromBody] IcampusBoatBackend.Models.Fee.MiscFeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_MISC_FEECHALLANA_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FID", (object?)bol.Fid ?? DBNull.Value);

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
        public IActionResult FeeChallanaAdmissionLoad([FromBody] IcampusBoatBackend.Models.Fee.MiscFeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_MISC_GETSTUDENTDETAILS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
        [Route("FeeList")]
        public IActionResult FeeList([FromBody] IcampusBoatBackend.Models.Fee.MiscFeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT ID, FEENAME, [ORDER], FEETYPE, ACCOUNTNO, AMOUNT, ISACTIVE FROM TBL_MISC_FEE_HEADS WHERE ACADEMICYEAR=@AcademicYear";
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
        [Route("GetReceiptNo")]
        public IActionResult GetReceiptNo([FromBody] IcampusBoatBackend.Models.Fee.MiscFeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT MAX(DBO.VAL(RECPTNO))+1 AS RECPTNO FROM TBL_MISC_FEECHALLAN_DETAILS WHERE ACADEMICYEAR = @AcademicYear";
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
        [Route("GetSSNo")]
        public IActionResult GetSSNo([FromBody] IcampusBoatBackend.Models.Fee.MiscFeeChallana bol)
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
        [Route("GetMFData")]
        public IActionResult GetMFData([FromBody] IcampusBoatBackend.Models.Fee.MiscFeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SPR_MISFEERECEIPT", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RECPTNO", (object?)bol.MFID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)bol.FYear ?? DBNull.Value);
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
        [Route("GetMFID")]
        public IActionResult GetMFID([FromBody] IcampusBoatBackend.Models.Fee.MiscFeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "Select MAX(RECPTNO) RECPTNO FROM TBL_MISC_FEECHALLAN_DETAILS WHERE SSNO = @SSNo";
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
        [Route("AjaxSSNo")]
        public IActionResult AjaxSSNo([FromBody] IcampusBoatBackend.Models.Fee.MiscFeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "Select DISTINCT StudentSerialNo from TBL_ADM_STUDATA where StudentSerialNo like @SSNo + '%'";
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
        [Route("GetReprintData")]
        public IActionResult GetReprintData([FromBody] IcampusBoatBackend.Models.Fee.MiscFeeChallana bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select Max(RECPTNO)RECPTNO from TBL_MISC_FEECHALLAN_DETAILS FD inner join TBL_ADM_STUDATA S on S.StudentSerialNo = FD.SSNO where S.Registrationno = @SSNo and ACADEMICYEAR = @AcademicYear";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@SSNo", (object?)bol.SSNo ?? DBNull.Value);
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
    }
}
