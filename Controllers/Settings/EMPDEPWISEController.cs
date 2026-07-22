using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IcampusBoatBackend.Controllers.Settings
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class EMPDEPWISEController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EMPDEPWISEController(IConfiguration configuration)
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
        [Route("LoadEmpDept")]
        public IActionResult LoadEmpDept()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "SELECT DISTINCT DEPARTMENTCODE, DEPARTMENT FROM TBL_DEPARTMENT";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        con.Open();
                        using (var reader = cmd.ExecuteReader())
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
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Deptwisedetails")]
        public IActionResult Deptwisedetails([FromBody] IcampusBoatBackend.Models.Settings.EMPDEPWISE bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("sp_Login_Permissions", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Dept", (object?)bol.Dept ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@WorkMode", (object?)bol.WorkMode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@active", (object?)bol.active ?? DBNull.Value);

                        con.Open();
                        using (var reader = cmd.ExecuteReader())
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
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("LoginStatus")]
        public IActionResult LoginStatus([FromBody] IcampusBoatBackend.Models.Settings.EMPDEPWISE bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_LOGINSTATUS", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@EMPID", (object?)bol.EmpID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@flag", (object?)bol.Flag ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@otp", (object?)bol.otp ?? DBNull.Value);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Success", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
