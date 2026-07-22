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
    public class ReceiptConcellationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ReceiptConcellationController(IConfiguration configuration)
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

        [HttpPost]
        [Route("GetReceipt")]
        public IActionResult GetReceipt(string? receiptNo, string? academicYear)
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
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)academicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReceiptNO", (object?)receiptNo ?? DBNull.Value);

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
        [Route("CheckAdmin")]
        public IActionResult CheckAdmin(string? academicYear, string? userId)
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
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)academicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);

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

        [HttpDelete]
        [Route("DeleteReceipt")]
        public IActionResult DeleteReceipt([FromBody] IcampusBoatBackend.Models.Fee.ReceiptConcellation bol)
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
    }
}
