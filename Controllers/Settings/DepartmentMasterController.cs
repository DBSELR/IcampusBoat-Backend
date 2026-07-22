using DocumentFormat.OpenXml.Office.Word;
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
    public class DepartmentMasterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DepartmentMasterController(IConfiguration configuration)
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
        [Route("GetDepartmentMaster")]
        public IActionResult GetDepartmentMaster()
        {

            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    var result = new List<object>();
                    using (SqlCommand cmd = new SqlCommand("Sp_Department_List", con))
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
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("SaveDepartmentMaster")]
        public IActionResult SaveDepartmentMaster([FromBody] IcampusBoatBackend.Models.Settings.DepartmentMaster dept)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_DEPARTMENT_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", dept.Id);
                        cmd.Parameters.AddWithValue("@DepartmentCode", dept.DepartmentCode);
                        cmd.Parameters.AddWithValue("@Department", dept.Department);
                        cmd.Parameters.AddWithValue("@Departmenttype", dept.DepartmentType);
                        cmd.Parameters.AddWithValue("@Description", dept.Description);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Department saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving department", error = ex.Message });
            }
        }
    }
}
