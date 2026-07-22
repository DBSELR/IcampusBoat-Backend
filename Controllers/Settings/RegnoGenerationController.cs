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
    public class RegnoGenerationController : ControllerBase
    {
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
        [Route("GetCourseList")]
        public IActionResult GetCourseList([FromBody] IcampusBoatBackend.Models.Settings.RegnoGeneration bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_COURSEBRANCH_LIST", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@STATUS", (object?)bol.Status ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSE", (object?)bol.Course ?? DBNull.Value);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetYearList")]
        public IActionResult GetYearList([FromBody] IcampusBoatBackend.Models.Settings.RegnoGeneration bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_YEARS_RegNoGen", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@COURSE", (object?)bol.Course ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetBranchList")]
        public IActionResult GetBranchList([FromBody] IcampusBoatBackend.Models.Settings.RegnoGeneration bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_Branch_LOAD", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@COURSE", (object?)bol.Course ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetSectionList")]
        public IActionResult GetSectionList([FromBody] IcampusBoatBackend.Models.Settings.RegnoGeneration bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "select distinct Section from tbl_sectionmaster where CourseCode=@Course and BranchCode=@Branch and StdYear=@StudyingYear and AcademicYear=@AcademicYear";
                    using (SqlCommand cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.AddWithValue("@Course", (object?)bol.Course ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", (object?)bol.Branch ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@StudyingYear", (object?)bol.StudyingYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetStudentData")]
        public IActionResult GetStudentData([FromBody] IcampusBoatBackend.Models.Settings.RegnoGeneration bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Get_StdData_RegGen", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CourseCode", (object?)bol.Course ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BranchCode", (object?)bol.Branch ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)bol.StudyingYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", (object?)bol.Section ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AdmType", (object?)bol.AdmnType ?? DBNull.Value);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("GenerateRegNo")]
        public IActionResult GenerateRegNo([FromBody] IcampusBoatBackend.Models.Settings.RegnoGeneration bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_REGNO_GENERATE", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@PROGRAMME", (object?)bol.Course ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", (object?)bol.Branch ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYEAR", (object?)bol.StudyingYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FORMAT", (object?)bol.RegNoFormat ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RnoFormat", (object?)bol.RnoFormat ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Reset", (object?)bol.Reset ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Section", (object?)bol.Section ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AdmType", (object?)bol.AdmnType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        return Ok(new { message = "Success", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
