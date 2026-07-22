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
    public class TeachingLearningMethodsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public TeachingLearningMethodsController(IConfiguration configuration)
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
        [Route("LoadGridData")]
        public IActionResult LoadGridData()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_LOAD_GRID_TLM", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
        [Route("CheckTLMExisted")]
        public IActionResult CheckTLMExisted([FromBody] IcampusBoatBackend.Models.Settings.TeachingLearningMethods bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_TEACHINGLEARNINGMETHODS_EXISTED", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TLMCODE", (object?)bol.TLMCode ?? DBNull.Value);
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
        [Route("SaveTLM")]
        public IActionResult SaveTLM([FromBody] IcampusBoatBackend.Models.Settings.TeachingLearningMethods bol)
        {
            try
            {
                int rowsAffected = 0;
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("Proc_SaveTeachingLearningMethods", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TLMID", (object?)bol.Sid ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TLMCode", (object?)bol.TLMCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TLMName", (object?)bol.TLMName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", (object?)bol.UserId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcYr", (object?)bol.AcademicYear ?? DBNull.Value);

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
        [Route("DeleteTLM")]
        public IActionResult DeleteTLM([FromBody] IcampusBoatBackend.Models.Settings.TeachingLearningMethods bol)
        {
            try
            {
                int rowsAffected = 0;
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_DELETE_TLM", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TLMID", (object?)bol.Id ?? DBNull.Value);

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
