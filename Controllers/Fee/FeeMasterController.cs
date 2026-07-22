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
    public class FeeMasterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public FeeMasterController(IConfiguration configuration)
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
                    string query = "SELECT DISTINCT Coursecode, Course From tbl_Adm_Course";
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

        [HttpGet]
        [Route("FeePaidYear")]
        public IActionResult FeePaidYear()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select * from tbl_Feepaidyears where Isactive=1";
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

        [HttpGet]
        [Route("Caste")]
        public IActionResult Caste()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select distinct Caste from tbl_Source_CasteMaster";
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

        [HttpGet]
        [Route("FeeFineList")]
        public IActionResult FeeFineList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select * from tbl_FeeFine";
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

        [HttpGet]
        [Route("StatusList")]
        public IActionResult StatusList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_FEEMASTER_Status_LIST", con))
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
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("SaveFeeMaster")]
        public IActionResult SaveFeeMaster([FromBody] IcampusBoatBackend.Models.Fee.FeeMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_FEEMASTER_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)bol.FYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSECODE", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TERMNO", (object?)bol.TermNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCHCODE", (object?)bol.Group ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CASTE", (object?)bol.Caste ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ADMISSIONMODE", (object?)bol.AdmMode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ORDER", (object?)bol.Order ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FeeName", (object?)bol.FeeName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FeeType", (object?)bol.FeeType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Amount", (object?)bol.Amount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Feeid", (object?)bol.FeeID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FMid", (object?)bol.FMid ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Fee Master saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving fee master", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteFeeMaster")]
        public IActionResult DeleteFeeMaster([FromBody] IcampusBoatBackend.Models.Fee.FeeMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_FEEMASTER_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSECODE", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TERMNO", (object?)bol.TermNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCHCODE", (object?)bol.Group ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CASTE", (object?)bol.Caste ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Feeid", (object?)bol.FeeID ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Fee Master deleted successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting fee master", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("FeeMasterList")]
        public IActionResult FeeMasterList([FromBody] IcampusBoatBackend.Models.Fee.FeeMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEEMASTER_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSECODE", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCHCODE", (object?)bol.Group ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TERM", (object?)bol.TermNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CASTE", (object?)bol.Caste ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ADMISSIONMODE", (object?)bol.AdmMode ?? DBNull.Value);

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
        [Route("FeeMasterLoad")]
        public IActionResult FeeMasterLoad([FromBody] IcampusBoatBackend.Models.Fee.FeeMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FEE_FEEMASTER_LOAD", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FMid", (object?)bol.FMid ?? DBNull.Value);

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
        [Route("CourseBranchList")]
        public IActionResult CourseBranchList([FromBody] IcampusBoatBackend.Models.Fee.FeeMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT BranchCode, BSName From tbl_Adm_Branch where CourseCode = @Programme and AcademicYear=@AcademicYear and Branchcode not in (13) ORDER BY BranchName";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Programme", (object?)bol.Programme ?? DBNull.Value);
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
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("YearsCourseWise")]
        public IActionResult YearsCourseWise([FromBody] IcampusBoatBackend.Models.Fee.FeeMaster bol)
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
                        cmd.Parameters.AddWithValue("@COURSE", (object?)bol.Programme ?? DBNull.Value);
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
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("LoadAdmMode")]
        public IActionResult LoadAdmMode([FromBody] IcampusBoatBackend.Models.Fee.FeeMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_LOAD_ADMISSIONMODE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
        [Route("SaveFeeFine")]
        public IActionResult SaveFeeFine([FromBody] IcampusBoatBackend.Models.Fee.FeeMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_SAVE_FEEFINE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", (object?)bol.ID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Programme", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FDATE", (object?)bol.FDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TDATE", (object?)bol.TDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FINE", (object?)bol.Fine ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADAMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Fee Fine saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving fee fine", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("UpdatePaidYear")]
        public IActionResult UpdatePaidYear([FromBody] IcampusBoatBackend.Models.Fee.FeeMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdatePaidYear", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@feepaidTo", (object?)bol.PaidYear ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Paid year updated successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating paid year", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteFeeFine")]
        public IActionResult DeleteFeeFine([FromBody] IcampusBoatBackend.Models.Fee.FeeMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "delete from tbl_feefine where id=@ID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ID", (object?)bol.ID ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { message = "Fee Fine deleted successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting fee fine", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("GetFeeFineById")]
        public IActionResult GetFeeFineById([FromBody] IcampusBoatBackend.Models.Fee.FeeMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    string query = "select * from tbl_FeeFine where id=@ID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ID", (object?)bol.ID ?? DBNull.Value);

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
