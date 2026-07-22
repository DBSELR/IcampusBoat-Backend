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
    public class AttendanceMaxDatesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AttendanceMaxDatesController(IConfiguration configuration)
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
        [Route("LoadAttMaxDate")]
        public IActionResult LoadAttMaxDate([FromBody] IcampusBoatBackend.Models.Settings.AttendanceMaxDates bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ATTENDANCEMAXDATES_GRIDLOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Course", (object?)bol.Course ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEM", (object?)bol.Sem ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@year", (object?)bol.Year ?? DBNull.Value);

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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("ProgrammeList")]
        public IActionResult ProgrammeList([FromBody] IcampusBoatBackend.Models.Settings.AttendanceMaxDates bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_COURSE_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);

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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("SaveAttendanceMaxDates")]
        public IActionResult SaveAttendanceMaxDates([FromBody] IcampusBoatBackend.Models.Settings.AttendanceMaxDates bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Sp_Attendance_MaxDates", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Course", (object?)bol.Course ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Sem", (object?)bol.Sem ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FromDate", (object?)bol.FromDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToDate", (object?)bol.ToDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", (object?)bol.Userid ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Success", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("LoadStudyingYear")]
        public IActionResult LoadStudyingYear([FromBody] IcampusBoatBackend.Models.Settings.AttendanceMaxDates bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_YEARS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@COURSE", (object?)bol.Course ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);

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
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
