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
    public class HeadsMasterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public HeadsMasterController(IConfiguration configuration)
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
        [Route("CourseLoadCombo")]
        public IActionResult CourseLoadCombo()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT DISTINCT COURSECODE,COURSE FROM TBL_ADM_COURSE";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
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
        [Route("HeadsMasterList")]
        public IActionResult HeadsMasterList([FromBody] IcampusBoatBackend.Models.Fee.HeadsMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_HEADSMASTER_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSECODE", (object?)bol.Programme ?? DBNull.Value);

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
        [Route("HeadsOrder")]
        public IActionResult HeadsOrder([FromBody] IcampusBoatBackend.Models.Fee.HeadsMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT ISNULL(MAX([ORDER]),0) AS MORDER FROM TBL_FEE_HEADS WHERE ACADEMICYEAR=@AcademicYear AND COURSECODE=@Programme";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", (object?)bol.Programme ?? DBNull.Value);

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
        [Route("SaveHeadsMaster")]
        public IActionResult SaveHeadsMaster([FromBody] IcampusBoatBackend.Models.Fee.HeadsMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_HEADS_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", (object?)bol.ID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSECODE", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEETYPE", (object?)bol.FEETYPE ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ORDER", (object?)bol.ORDER ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FEENAME", (object?)bol.FEENAME ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SHORTNAME", (object?)bol.SHORTNAME ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AMOUNT", (object?)bol.AMOUNT ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACCOUNTNO", (object?)bol.ACCOUNTNO ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Fee Head saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving fee head", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteHeadsMaster")]
        public IActionResult DeleteHeadsMaster([FromBody] IcampusBoatBackend.Models.Fee.HeadsMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_HEADSMASTER_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", (object?)bol.ID ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Fee Head deleted successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting fee head", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("GetFHData")]
        public IActionResult GetFHData([FromBody] IcampusBoatBackend.Models.Fee.HeadsMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_GETFHDATA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSECODE", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SHORTNAME", (object?)bol.SHORTNAME ?? DBNull.Value);

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
        [Route("AccountNoAjax")]
        public IActionResult AccountNoAjax([FromBody] IcampusBoatBackend.Models.Fee.HeadsMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT DISTINCT ACNO FROM TBL_ACCOUNTS WHERE ACNO LIKE @ACCOUNTNO + '%'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ACCOUNTNO", (object?)bol.ACCOUNTNO ?? "");

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
        [Route("FeeTypeAjax")]
        public IActionResult FeeTypeAjax([FromBody] IcampusBoatBackend.Models.Fee.HeadsMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT DISTINCT FEETYPE FROM TBL_FEE_HEADS WHERE FEETYPE LIKE @FEETYPE + '%'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@FEETYPE", (object?)bol.FEETYPE ?? "");

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
    }
}
