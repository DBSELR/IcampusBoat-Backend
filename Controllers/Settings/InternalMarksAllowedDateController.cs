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
    public class InternalMarksAllowedDateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public InternalMarksAllowedDateController(IConfiguration configuration)
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
        [Route("SubjectMasterProgrammeLoad")]
        public IActionResult SubjectMasterProgrammeLoad([FromBody] IcampusBoatBackend.Models.Settings.InternalMarksAllowedDate bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_SubjectMaster_Programme_Load", con) { CommandType = CommandType.StoredProcedure })
                    {
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
        [Route("GetYearCourseWise")]
        public IActionResult GetYearCourseWise([FromBody] IcampusBoatBackend.Models.Settings.InternalMarksAllowedDate bol)
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

        [HttpGet]
        [Route("GetMidTypeMaster")]
        public IActionResult GetMidTypeMaster()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "select MidType from [tbl_midType_internaldates]";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
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

        [HttpGet]
        [Route("BindRegu")]
        public IActionResult BindRegu()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "select Distinct Regulation from tbl_Regulation";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
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
        [Route("BindInternalDates")]
        public IActionResult BindInternalDates([FromBody] IcampusBoatBackend.Models.Settings.InternalMarksAllowedDate bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("sp_GetInternalData", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@REGU", (object?)bol.Regu ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSECODE", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMESTER", (object?)bol.Semester ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACYR", (object?)bol.AcademicYear ?? DBNull.Value);

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
        [Route("BindInternalDatesflag2")]
        public IActionResult BindInternalDatesflag2([FromBody] IcampusBoatBackend.Models.Settings.InternalMarksAllowedDate bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("sp_GetInternalData_flag2", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@REGU", (object?)bol.Regu ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSECODE", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMESTER", (object?)bol.Semester ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACYR", (object?)bol.AcademicYear ?? DBNull.Value);

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
        [Route("SaveInternalDates")]
        public IActionResult SaveInternalDates([FromBody] IcampusBoatBackend.Models.Settings.InternalMarksAllowedDate bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_Save_InternalMarks_Date", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@COURSE", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMISTER", (object?)bol.Semester ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LASTDATE", (object?)bol.Date ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@USERID", (object?)bol.Userid ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DISPLAYDATE", (object?)bol.DisplayDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MIDTYPE", (object?)bol.MidType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EXAMTYPE", (object?)bol.ExamTypes ?? DBNull.Value);

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
