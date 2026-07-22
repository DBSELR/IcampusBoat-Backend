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
    public class ProgrammeMasterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ProgrammeMasterController(IConfiguration configuration)
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
        [Route("GetProgrammeMaster")]
        public IActionResult GetProgrammeMaster([FromBody] IcampusBoatBackend.Models.Settings.ProgrammeMaster prog)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_ADM_COURSE_LIST", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)prog.AcademicYear ?? DBNull.Value);

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
        [Route("SaveProgrammeMaster")]
        public IActionResult SaveProgrammeMaster([FromBody] IcampusBoatBackend.Models.Settings.ProgrammeMaster prog)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_ADM_COURSE_SAVE", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@CID", (object?)prog.CID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSECODE", (object?)prog.COURSECODE ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSE", (object?)prog.COURSE ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DEGREE", (object?)prog.DEGREE ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)prog.YEAR ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)prog.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FinancialYear", (object?)prog.FinancialYear ?? DBNull.Value);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Success" });
                        else
                            return BadRequest(new { message = "Failed" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteProgrammeMaster")]
        public IActionResult DeleteProgrammeMaster([FromBody] IcampusBoatBackend.Models.Settings.ProgrammeMaster prog)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_ADM_COURSE_DELETE", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@CID", (object?)prog.CID ?? DBNull.Value);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Success" });
                        else
                            return BadRequest(new { message = "Failed" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("CheckCourseCode")]
        public IActionResult CheckCourseCode([FromBody] IcampusBoatBackend.Models.Settings.ProgrammeMaster prog)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("select * from tbl_Adm_Course where AcademicYear=@AcademicYear And CourseCode=@CourseCode", con))
                    {
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)prog.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CourseCode", (object?)prog.COURSECODE ?? DBNull.Value);

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
