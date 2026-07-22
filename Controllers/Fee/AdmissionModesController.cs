using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IcampusBoatBackend.Controllers.Fee
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AdmissionModesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AdmissionModesController(IConfiguration configuration)
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
        [Route("Programlist")]
        public IActionResult Programlist(string academicYear)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT Coursecode,Coursecode+'-'+ Course Course FROM tbl_Adm_Course where academicyear=@AcadamicYear";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@AcadamicYear", (object?)academicYear ?? DBNull.Value);

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
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("YearsCourseWise")]
        public IActionResult YearsCourseWise(string? programme, string? academicYear)
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
                        cmd.Parameters.AddWithValue("@COURSE", (object?)programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)academicYear ?? DBNull.Value);

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
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("SaveAdmissionMode")]
        public IActionResult SaveAdmissionMode([FromBody] IcampusBoatBackend.Models.Fee.AdmissionModes bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Sp_AdmissionMode", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", (object?)bol.id ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AdmMode", (object?)bol.AdmissionMode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SchlorAmount", (object?)bol.Schamount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADAMICYEAR", (object?)bol.AcadamicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", (object?)bol.UserId ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Admission Mode saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving admission mode", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("AdmModeList")]
        public IActionResult AdmModeList([FromBody] IcampusBoatBackend.Models.Fee.AdmissionModes bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Sp_AdmissionModeList", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@acdemicyr", (object?)bol.AcadamicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSECODE", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYEAR", (object?)bol.Year ?? DBNull.Value);

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
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteAdmMode")]
        public IActionResult DeleteAdmMode(string? id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_Admission_Delete", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", (object?)id ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Admission Mode deleted successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting admission mode", error = ex.Message });
            }
        }
    }
}
