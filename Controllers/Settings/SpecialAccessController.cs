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
    public class SpecialAccessController : ControllerBase
    {
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
        [Route("GetERPModulesList")]
        public IActionResult GetERPModulesList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "select Distinct cast([MENUID] as varchar) [MENUID], [Text] from tbl_ERP_Main_Menu where MENUID != 1 order by [Text]";
                    using (SqlCommand cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetERPFormsList")]
        public IActionResult GetERPFormsList([FromBody] IcampusBoatBackend.Models.Settings.SpecialAccess bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ERP_FORMS", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@MENUID", (object?)bol.MenuID ?? DBNull.Value);

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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetEmployeeList")]
        public IActionResult GetEmployeeList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "SELECT DISTINCT EMPID, ISNULL(LNAME,'') +' '+ ISNULL(fNAME,'')+' '+EMPID NAME FROM TBL_EMPLOYEEDETAILS ORDER BY EMPID";
                    using (SqlCommand cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetERP_SPA_List")]
        public IActionResult GetERP_SPA_List([FromBody] IcampusBoatBackend.Models.Settings.SpecialAccess bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_SPLACCESS_DATA_LIST", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@EMPID", (object?)bol.UserGroup ?? DBNull.Value);

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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveSplAccess")]
        public IActionResult SaveSplAccess([FromBody] IcampusBoatBackend.Models.Settings.SpecialAccess bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_SPLACCESS_DATA_SAVE", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@EMPID", (object?)bol.UserGroup ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MENUID", (object?)bol.MenuID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUBMENUID", (object?)bol.SMenuID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DEPT", (object?)bol.Dept ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ISACTIVE", (object?)bol.IsActive ?? DBNull.Value);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        return Ok(new { message = "Success", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
