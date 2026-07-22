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
    public class FormRegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public FormRegistrationController(IConfiguration configuration)
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
        [Route("LoadMenuid")]
        public IActionResult LoadMenuid()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_Load_FormReg", con) { CommandType = CommandType.StoredProcedure })
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
        [Route("LoadSmenuid")]
        public IActionResult LoadSmenuid([FromBody] IcampusBoatBackend.Models.Settings.FormRegistration bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("sp_Smenuid", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@MENUID", (object?)bol.MenuId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FORMTYPE", (object?)bol.FormType ?? DBNull.Value);

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
        [Route("SaveFormReg")]
        public IActionResult SaveFormReg([FromBody] IcampusBoatBackend.Models.Settings.FormRegistration bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("Sp_tpl_FormRegistra_Save", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@MENUID", (object?)bol.MenuId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SMENUID", (object?)bol.Smenuid ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TEXT", (object?)bol.Text ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DESCRIPTION", (object?)bol.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NAVIGATEURL", (object?)bol.NAvUrl ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FORMTYPE", (object?)bol.FormType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ISACTIVE", (object?)bol.IsActive ?? DBNull.Value);

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

        [HttpPost]
        [Route("LoadFRData")]
        public IActionResult LoadFRData([FromBody] IcampusBoatBackend.Models.Settings.FormRegistration bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("sp_FrDataList", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@MENUID", (object?)bol.MenuId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FORMTYPE", (object?)bol.FormType ?? DBNull.Value);

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
    }
}
