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
    public class SectionMasterController : ControllerBase
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
        [Route("GetSectionMaster")]
        public IActionResult GetSectionMaster([FromBody] IcampusBoatBackend.Models.Settings.SectionMaster sect)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Source_SectionMaster_List", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@AcademicYear", sect.AcademicYear);
                        cmd.Parameters.AddWithValue("@COURSECODE", (object?)sect.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCHCODE", (object?)sect.BranchCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SYEAR", (object?)sect.StdYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMESTER", (object?)sect.Semester ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SECTION", (object?)sect.Section ?? DBNull.Value);

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
        [Route("SaveSectionMaster")]
        public IActionResult SaveDepartmentMaster([FromBody] IcampusBoatBackend.Models.Settings.SectionMaster sect)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Source_SectionMaster_Save", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@id", sect.id);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", sect.AcademicYear);
                        cmd.Parameters.AddWithValue("@FINANCIALYEAR", sect.FinancialYear);
                        cmd.Parameters.AddWithValue("@PROGRAMME", sect.Programme);
                        cmd.Parameters.AddWithValue("@BRANCH", sect.BranchCode);
                        cmd.Parameters.AddWithValue("@STDYEAR", sect.StdYear);
                        cmd.Parameters.AddWithValue("@SECTION", sect.Section);
                        cmd.Parameters.AddWithValue("@SEMESTER", sect.Semester);
                        cmd.Parameters.AddWithValue("@MANGQUTSEATS", sect.Mangqutseats);
                        cmd.Parameters.AddWithValue("@COUNCILINGSEATS", sect.CouncilingSeats);
                        cmd.Parameters.AddWithValue("@WITHLE", sect.WithLE);
                        cmd.Parameters.AddWithValue("@TOTSEATS", sect.TotStrgth);
                        cmd.Parameters.AddWithValue("@FillSeats", sect.fillseats);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        return Ok(new { message = "Section saved successfully", rowsAffected = rowsAffected });
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
