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
    public class StudentDataUploadController : ControllerBase
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

        [HttpGet]
        [Route("DownloadFields")]
        public IActionResult DownloadFields()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DownloadFormatFeillds", con) { CommandType = CommandType.StoredProcedure })
                    {
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

        [HttpGet]
        [Route("UploadFile")]
        public IActionResult UploadFile()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_uploadFormatFeillds", con) { CommandType = CommandType.StoredProcedure })
                    {
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
        [Route("FinalUpdationStudentData")]
        public IActionResult FinalUpdationStudentData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_FINALUPDATESTUDENTDATA", con) { CommandType = CommandType.StoredProcedure })
                    {
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

        [HttpPost]
        [Route("InsertStudentData")]
        public IActionResult InsertStudentData([FromBody] IcampusBoatBackend.Models.Settings.StudentDataUpload bol)
        {
            try
            {
                DataTable dt = new DataTable();
                if (bol.StudentData != null && bol.StudentData.Count > 0)
                {
                    var keys = new HashSet<string>();
                    foreach (var dict in bol.StudentData)
                    {
                        foreach (var key in dict.Keys)
                        {
                            keys.Add(key);
                        }
                    }
                    foreach (var key in keys)
                    {
                        dt.Columns.Add(key, typeof(object));
                    }
                    foreach (var dict in bol.StudentData)
                    {
                        var row = dt.NewRow();
                        foreach (var key in keys)
                        {
                            row[key] = dict.ContainsKey(key) ? dict[key] ?? DBNull.Value : DBNull.Value;
                        }
                        dt.Rows.Add(row);
                    }
                }

                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("[SP_Import_studentData]", con) { CommandType = CommandType.StoredProcedure })
                    {
                        var param = cmd.Parameters.Add(new SqlParameter("@EAZYPAYDT", SqlDbType.Structured));
                        param.Value = dt;

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
