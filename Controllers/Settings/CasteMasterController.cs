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
    public class CasteMasterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public CasteMasterController(IConfiguration configuration)
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
        [Route("GetCasteMaster")]
        public IActionResult GetCasteMaster()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Source_CastMaster_List ", con))
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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("SaveCaste")]
        public IActionResult SaveCaste([FromBody] IcampusBoatBackend.Models.Settings.CasteMaster CM)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Source_CasteMaster_Save ", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CASTEID", CM.Casteid);
                        cmd.Parameters.AddWithValue("@CASTE", CM.Caste);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", CM.AcademicYear);
                        cmd.Parameters.AddWithValue("@FINANCIALYEAR", CM.FinancialYear);

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

        [HttpGet]
        [Route("GetSubCasteMaster")]
        public IActionResult GetSubCasteMaster()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Source_SubCasteMaster_List ", con))
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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("SaveSubCaste")]
        public IActionResult SaveSubCaste([FromBody] IcampusBoatBackend.Models.Settings.CasteMaster CM)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_Source_SubCasteMaster_Save ", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SubCasteid", CM.SubCasteid);
                        cmd.Parameters.AddWithValue("@Caste", CM.Caste);
                        cmd.Parameters.AddWithValue("@SubCasteCode", CM.SubCasteCode);
                        cmd.Parameters.AddWithValue("@SubCaste", CM.SubCaste);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", CM.AcademicYear);
                        cmd.Parameters.AddWithValue("@FINANCIALYEAR", CM.FinancialYear);

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

        [HttpGet]
        [Route("GetLoadCaste")]
        public IActionResult GetLoadCaste()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("select Caste from tbl_Source_CasteMaster order by Caste", con))
                    {
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
        [Route("SubCasteCodeChecking")]
        public IActionResult SubCasteCodeChecking([FromBody] IcampusBoatBackend.Models.Settings.CasteMaster CM)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("select * from tbl_Source_SubCasteMaster where AcademicYear=@ACADEMICYEAR and SubcasteCode = @SubCasteCode ", con))
                    {
                        cmd.Parameters.AddWithValue("@SubCasteid", CM.SubCasteid);
                        cmd.Parameters.AddWithValue("@Caste", CM.Caste);
                        cmd.Parameters.AddWithValue("@SubCasteCode", CM.SubCasteCode);
                        cmd.Parameters.AddWithValue("@SubCaste", CM.SubCaste);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", CM.AcademicYear);
                        cmd.Parameters.AddWithValue("@FINANCIALYEAR", CM.FinancialYear);

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
