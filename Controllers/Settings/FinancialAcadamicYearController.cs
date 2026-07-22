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
    public class FinancialAcadamicYearController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public FinancialAcadamicYearController(IConfiguration configuration)
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
        [Route("LoadData")]
        public IActionResult LoadData()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_AcademicYear_Load", con) { CommandType = CommandType.StoredProcedure })
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
        [Route("LoadFData")]
        public IActionResult LoadFData()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_FinancialYear_Load", con) { CommandType = CommandType.StoredProcedure })
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
        [Route("SaveFinancialAcadamicYear")]
        public IActionResult SaveFinancialAcadamicYear([FromBody] IcampusBoatBackend.Models.Settings.FinancialAcadamicYear bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_AcademicYear_SAVE", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@AID", (object?)bol.AID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ISACTIVE", (object?)bol.IsActive ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AY", (object?)bol.AY ?? DBNull.Value);

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
        [Route("SaveFinancialYear")]
        public IActionResult SaveFinancialYear([FromBody] IcampusBoatBackend.Models.Settings.FinancialAcadamicYear bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_FinancialYear_SAVE", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@AID", (object?)bol.AID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FINANCIALYEAR", (object?)bol.FinancialYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ISACTIVE", (object?)bol.IsActive ?? DBNull.Value);

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
        [Route("DeleteFinancialAcademicYear")]
        public IActionResult DeleteFinancialAcademicYear([FromBody] IcampusBoatBackend.Models.Settings.FinancialAcadamicYear bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_AcademicYear_DELETE", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@AID", (object?)bol.AID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ISACTIVE", (object?)bol.IsActive ?? DBNull.Value);

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
        [Route("DeleteFinancialYear")]
        public IActionResult DeleteFinancialYear([FromBody] IcampusBoatBackend.Models.Settings.FinancialAcadamicYear bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_FinancialYear_DELETE", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@AID", (object?)bol.AID ?? DBNull.Value);

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
        [Route("ISActiveUpdate")]
        public IActionResult ISActiveUpdate([FromBody] IcampusBoatBackend.Models.Settings.FinancialAcadamicYear bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (var cmd1 = new SqlCommand("update tbl_AcademicYear set ISACTIVE='N'", con))
                    {
                        cmd1.ExecuteNonQuery();
                    }

                    using (var cmd2 = new SqlCommand("update tbl_AcademicYear set ISACTIVE='Y' where ID=@AID", con))
                    {
                        cmd2.Parameters.AddWithValue("@AID", (object?)bol.AID ?? DBNull.Value);
                        int rowsAffected = cmd2.ExecuteNonQuery();
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
        [Route("ISActiveFYRUpdate")]
        public IActionResult ISActiveFYRUpdate([FromBody] IcampusBoatBackend.Models.Settings.FinancialAcadamicYear bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (var cmd1 = new SqlCommand("update tbl_FinancialYear set ISACTIVE='N'", con))
                    {
                        cmd1.ExecuteNonQuery();
                    }

                    using (var cmd2 = new SqlCommand("update tbl_FinancialYear set ISACTIVE='Y' where ID=@AID", con))
                    {
                        cmd2.Parameters.AddWithValue("@AID", (object?)bol.AID ?? DBNull.Value);
                        int rowsAffected = cmd2.ExecuteNonQuery();
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
        [Route("CheckAcademicYearExisted")]
        public IActionResult CheckAcademicYearExisted([FromBody] IcampusBoatBackend.Models.Settings.FinancialAcadamicYear bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "select * from tbl_AcademicYear where ACADEMICYEAR=@AcademicYear";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
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
        [Route("CheckFinancialYearExisted")]
        public IActionResult CheckFinancialYearExisted([FromBody] IcampusBoatBackend.Models.Settings.FinancialAcadamicYear bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "select count(*) from tbl_financialyear where FINANCIALYEAR = @FinancialYear";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.AddWithValue("@FinancialYear", (object?)bol.FinancialYear ?? DBNull.Value);

                        con.Open();
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return Ok(new { count = count });
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
