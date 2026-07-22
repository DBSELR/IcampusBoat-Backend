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
    public class UserGroupController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UserGroupController(IConfiguration configuration)
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
        [Route("GetEmployeesList")]
        public IActionResult GetEmployeesList([FromBody] IcampusBoatBackend.Models.Settings.UserGroup bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "Select EMPID, Prefix +FName + ' (' + Designation + ')' as EMPNAME, USERGROUP FROM tbl_EmployeeDetails";
                    if (!string.IsNullOrEmpty(bol.Dept) && bol.Dept != "0")
                    {
                        query += " where Dept = @Dept";
                    }
                    query += " order by EmpID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        if (!string.IsNullOrEmpty(bol.Dept) && bol.Dept != "0")
                        {
                            cmd.Parameters.AddWithValue("@Dept", bol.Dept);
                        }

                        con.Open();
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
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetERPDepartmentsList")]
        public IActionResult GetERPDepartmentsList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "select Distinct DepartmentCode, [Description] from tbl_Department order by DepartmentCode";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();
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
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetUserGroups")]
        public IActionResult GetUserGroups()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "SELECT DISTINCT UserGroup FROM tbl_ERP_UserMenu";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();
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
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("UpdateUGDataEmp")]
        public IActionResult UpdateUGDataEmp([FromBody] IcampusBoatBackend.Models.Settings.UserGroup bol)
        {
            try
            {
                int rowsAffected = 0;
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ERP_UM_EMP_UPDATE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@USERGROUP", (object?)bol.UserGroupVal ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EMPID", (object?)bol.EmpID ?? DBNull.Value);

                        con.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
                return Ok(new { message = "Success", rowsAffected = rowsAffected });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
