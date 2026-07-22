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
    public class FeedBackEmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public FeedBackEmployeeController(IConfiguration configuration)
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
        [Route("GetEmployeeDetails")]
        public IActionResult GetEmployeeDetails([FromBody] IcampusBoatBackend.Models.Settings.FeedBackEmployee bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "SELECT Empid, PreFix+''+FName+' '+LName as EmpName, Dept + '-'+Department Dept,MobileNo FROM tbl_EmployeeDetails e inner join tbl_Department d on e.Dept = d.DepartmentCode where EmpID = @Empid";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.AddWithValue("@Empid", (object?)bol.Empid ?? DBNull.Value);

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
        [Route("SaveFeedBackReg")]
        public IActionResult SaveFeedBackReg([FromBody] IcampusBoatBackend.Models.Settings.FeedBackEmployee bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("PROC_SAVEFeedBackREG", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@USERID", (object?)bol.Empid ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@USERTYPE", (object?)bol.UserType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NAME", (object?)bol.Name ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DEPARTMENT", (object?)bol.Department ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MobileNo", (object?)bol.MobileNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COMPLAINT", (object?)bol.Complaint ?? DBNull.Value);

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
