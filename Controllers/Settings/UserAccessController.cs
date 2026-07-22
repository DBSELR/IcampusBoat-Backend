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
    public class UserAccessController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UserAccessController(IConfiguration configuration)
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
        [Route("GetERPFormsList")]
        public IActionResult GetERPFormsList([FromBody] IcampusBoatBackend.Models.Settings.UserAccess bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ERP_FORMS_Access", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MENUID", (object?)bol.MenuID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UG", (object?)bol.UserGroup ?? DBNull.Value);
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
        [Route("GetERPModulesList")]
        public IActionResult GetERPModulesList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "select Distinct cast([MENUID] as varchar) [MENUID], [Text] from tbl_ERP_Main_Menu where MENUID != 1 order by [Text]";
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
        [Route("GetUserGroupsSettings")]
        public IActionResult GetUserGroupsSettings()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "SELECT DISTINCT UserGroup UserID FROM tbl_ERP_UserMenu";
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
        [Route("GetUserGroupsNotIN")]
        public IActionResult GetUserGroupsNotIN([FromBody] IcampusBoatBackend.Models.Settings.UserAccess bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "SELECT DISTINCT UserGroup FROM tbl_ERP_UserMenu where UserGroup not in(@UserGroup)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@UserGroup", (object?)bol.UserGroup ?? DBNull.Value);
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
        [Route("GetUserGroupMenuLoad")]
        public IActionResult GetUserGroupMenuLoad([FromBody] IcampusBoatBackend.Models.Settings.UserAccess bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ERP_UserGroup_Menu_Load", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", (object?)bol.UserGroup ?? DBNull.Value);
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
        [Route("CheckUserGroupDel")]
        public IActionResult CheckUserGroupDel([FromBody] IcampusBoatBackend.Models.Settings.UserAccess bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_UG_CHECK", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", (object?)bol.UserGroup ?? DBNull.Value);
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
        [Route("DeleteUserForms")]
        public IActionResult DeleteUserForms([FromBody] IcampusBoatBackend.Models.Settings.UserAccess bol)
        {
            try
            {
                int rowsAffected = 0;
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ERP_USERS_MENU_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@USERGROUP", (object?)bol.UserGroup ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CHUG", (object?)bol.ChUG ?? DBNull.Value);
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

        [HttpPost]
        [Route("DeleteUserGroup")]
        public IActionResult DeleteUserGroup([FromBody] IcampusBoatBackend.Models.Settings.UserAccess bol)
        {
            try
            {
                int rowsAffected = 0;
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "delete from tbl_ERP_UserMenu where usergroup = @UserGroup";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@UserGroup", (object?)bol.UserGroup ?? DBNull.Value);
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

        [HttpPost]
        [Route("SaveUserForms")]
        public IActionResult SaveUserForms([FromBody] IcampusBoatBackend.Models.Settings.UserAccess bol)
        {
            try
            {
                int rowsAffected = 0;
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ERP_USERS_MENU_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@USERGROUP", (object?)bol.UserGroup ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MENUID", (object?)bol.MenuID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SMENUID", (object?)bol.SMenuID ?? DBNull.Value);
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

        [HttpPost]
        [Route("UpdateUGDataEmp")]
        public IActionResult UpdateUGDataEmp([FromBody] IcampusBoatBackend.Models.Settings.UserAccess bol)
        {
            try
            {
                int rowsAffected = 0;
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ERP_UM_EMP_UPDATE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@USERGROUP", (object?)bol.UserGroup ?? DBNull.Value);
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
