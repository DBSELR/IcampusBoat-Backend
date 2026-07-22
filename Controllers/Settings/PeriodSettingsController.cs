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
    public class PeriodSettingsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public PeriodSettingsController(IConfiguration configuration)
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
        [Route("GetProgrammeList")]
        public IActionResult GetProgrammeList([FromBody] IcampusBoatBackend.Models.Settings.PeriodSettings bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_Timetable_COURSEBRANCH_LIST", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@STATUS", (object?)bol.Status ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSE", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcdYr ?? DBNull.Value);

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
        [Route("GetYearList")]
        public IActionResult GetYearList([FromBody] IcampusBoatBackend.Models.Settings.PeriodSettings bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_ADM_YEARS", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@COURSE", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);

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
        [Route("GetPeriodTimeList")]
        public IActionResult GetPeriodTimeList([FromBody] IcampusBoatBackend.Models.Settings.PeriodSettings bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("Sp_PeriodTime_List", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@COURSE", (object?)bol.Programme ?? DBNull.Value);

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
        [Route("GetSessions")]
        public IActionResult GetSessions([FromBody] IcampusBoatBackend.Models.Settings.PeriodSettings bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "select id,MorningSession,AfternoonSession,TotalPeriods from tbl_PeriodSettings " +
                                   "where Programme=@Programme and shiftno=@ShiftNo";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.AddWithValue("@Programme", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ShiftNo", (object?)bol.ShiftNo ?? DBNull.Value);

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
        [Route("SavePeriodTime")]
        public IActionResult SavePeriodTime([FromBody] IcampusBoatBackend.Models.Settings.PeriodSettings bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_PeriodSettings_Save", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@ID", (object?)bol.Id ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SHIFTNO", (object?)bol.ShiftNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MORNINGSESSION", (object?)bol.MorningSession ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@T1", (object?)bol.T1 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@T2", (object?)bol.T2 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@T3", (object?)bol.T3 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@T4", (object?)bol.T4 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@T5", (object?)bol.T5 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@T6", (object?)bol.T6 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@T7", (object?)bol.T7 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@T8", (object?)bol.T8 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@T9", (object?)bol.T9 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AFTERNOONSESSION", (object?)bol.AfternoonSession ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PSYEAR", (object?)bol.Year ?? DBNull.Value);

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
        [Route("DeletePeriodTime")]
        public IActionResult DeletePeriodTime([FromBody] IcampusBoatBackend.Models.Settings.PeriodSettings bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("Sp_PeriodTime_Delete", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@id", (object?)bol.Id ?? DBNull.Value);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Success" });
                        else
                            return BadRequest(new { message = "Failed to delete record" });
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
