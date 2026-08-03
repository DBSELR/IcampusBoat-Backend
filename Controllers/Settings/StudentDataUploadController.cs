using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

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

        //[HttpGet]
        //[Route("DownloadFields")]
        //public IActionResult DownloadFields()
        //{
        //    try
        //    {
        //        var result = new List<object>();
        //        using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("sp_DownloadFormatFeillds", con) { CommandType = CommandType.StoredProcedure })
        //            {
        //                con.Open();
        //                using (SqlDataReader reader = cmd.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        result.Add(ReadRow(reader));
        //                    }
        //                }
        //            }
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        [HttpGet]
        [Route("DownloadFields")]
        public IActionResult DownloadFields()
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DownloadFormatFeillds", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                using (var workbook = new ClosedXML.Excel.XLWorkbook())
                {
                    workbook.Worksheets.Add(dt, "STUDENT_DATA");

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);

                        return File(
                            stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "STUDENT_DATA.xlsx");
                    }
                }
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

        //[HttpPost]
        //[Route("InsertStudentData")]
        //public IActionResult InsertStudentData([FromBody] IcampusBoatBackend.Models.Settings.StudentDataUpload bol)
        //{
        //    try
        //    {
        //        DataTable dt = new DataTable();
        //        if (bol.StudentData != null && bol.StudentData.Count > 0)
        //        {
        //            var keys = new HashSet<string>();
        //            foreach (var dict in bol.StudentData)
        //            {
        //                foreach (var key in dict.Keys)
        //                {
        //                    keys.Add(key);
        //                }
        //            }
        //            foreach (var key in keys)
        //            {
        //                dt.Columns.Add(key, typeof(object));
        //            }
        //            foreach (var dict in bol.StudentData)
        //            {
        //                var row = dt.NewRow();
        //                foreach (var key in keys)
        //                {
        //                    row[key] = dict.ContainsKey(key) ? dict[key] ?? DBNull.Value : DBNull.Value;
        //                }
        //                dt.Rows.Add(row);
        //            }
        //        }

        //        using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("[SP_Import_studentData]", con) { CommandType = CommandType.StoredProcedure })
        //            {
        //                var param = cmd.Parameters.Add(new SqlParameter("@EAZYPAYDT", SqlDbType.Structured));
        //                param.Value = dt;

        //                con.Open();
        //                int rowsAffected = cmd.ExecuteNonQuery();

        //                return Ok(new { message = "Success", rowsAffected = rowsAffected });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}



        [HttpPost]
        [Route("InsertStudentData")]
        public IActionResult InsertStudentData(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Please select an Excel file.");

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                DataTable excelTable;

                using (var stream = file.OpenReadStream())
                {
                    using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        excelTable = result.Tables[0];
                    }
                }

                // Create DataTable matching SQL User Defined Table Type
                DataTable dt = new DataTable();

                dt.Columns.Add("RegistrationNo", typeof(string));
                dt.Columns.Add("STUNAME", typeof(string));
                dt.Columns.Add("FATHERNAME", typeof(string));
                dt.Columns.Add("Emailid", typeof(string));
                dt.Columns.Add("MAadharNo", typeof(string));
                dt.Columns.Add("StdMobNo", typeof(string));
                dt.Columns.Add("ParentMbNo", typeof(string));
                dt.Columns.Add("AadhaarNo", typeof(string));
                dt.Columns.Add("JnanaBhumiId", typeof(string));
                dt.Columns.Add("BusFee", typeof(string));
                dt.Columns.Add("SchAmount", typeof(string));
                dt.Columns.Add("BloodGrp", typeof(string));
                dt.Columns.Add("SpotAdmFee", typeof(string));
                dt.Columns.Add("Modeodcategory", typeof(string));
                dt.Columns.Add("ApaarID", typeof(string));

                foreach (DataRow row in excelTable.Rows)
                {
                    dt.Rows.Add(
                        row["RegistrationNo"]?.ToString(),
                        row["STUNAME"]?.ToString(),
                        row["FATHERNAME"]?.ToString(),
                        row["Emailid"]?.ToString(),
                        row["MAadharNo"]?.ToString(),
                        row["StdMobNo"]?.ToString(),
                        row["ParentMbNo"]?.ToString(),
                        row["AadhaarNo"]?.ToString(),
                        row["JnanaBhumiId"]?.ToString(),
                        row["BusFee"]?.ToString(),
                        row["SchAmount"]?.ToString(),
                        row["BloodGrp"]?.ToString(),
                        row["SpotAdmFee"]?.ToString(),
                        row["Modeodcategory"]?.ToString(),
                        row["ApaarID"]?.ToString()
                    );
                }

                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_IMPORT_STUDENTDATA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter param = cmd.Parameters.Add("@EAZYPAYDT", SqlDbType.Structured);
                        param.TypeName = "dbo.StudentDataImport";
                        param.Value = dt;

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        return Ok(new
                        {
                            message = "Excel Uploaded Successfully",
                            rowsAffected = rowsAffected
                        });
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
