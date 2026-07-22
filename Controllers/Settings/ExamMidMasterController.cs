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
    public class ExamMidMasterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ExamMidMasterController(IConfiguration configuration)
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
                    using (var cmd = new SqlCommand("SP_LOAD_GRID", con) { CommandType = CommandType.StoredProcedure })
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
        [Route("CheckMidMasterExisted")]
        public IActionResult CheckMidMasterExisted([FromBody] IcampusBoatBackend.Models.Settings.ExamMidMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_MIDMaster_Existed", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@MidType", (object?)bol.MIDType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcYr", (object?)bol.AcademicYear ?? DBNull.Value);

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
        [Route("SaveMIDMaster")]
        public IActionResult SaveMIDMaster([FromBody] IcampusBoatBackend.Models.Settings.ExamMidMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("Proc_SaveMidMaster", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Mid", (object?)bol.Sid ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MidType", (object?)bol.MIDType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MidType_Id", (object?)bol.MIDType_Id ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MidName", (object?)bol.MIDName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", (object?)bol.UserId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcYr", (object?)bol.AcademicYear ?? DBNull.Value);

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
        [Route("DeleteMidMaster")]
        public IActionResult DeleteMidMaster([FromBody] IcampusBoatBackend.Models.Settings.ExamMidMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_MIDMaster_Delete", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Mid", (object?)bol.Id ?? DBNull.Value);

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
