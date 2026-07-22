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
    public class RequestSettingsController : ControllerBase
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
        [Route("GetRequestType")]
        public IActionResult GetRequestType()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_REQUEST_LIST", con) { CommandType = CommandType.StoredProcedure })
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
        [Route("GetDepartmentsList")]
        public IActionResult GetDepartmentsList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_REQ_DEPT_LIST", con) { CommandType = CommandType.StoredProcedure })
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
        [Route("GetDesignationsList")]
        public IActionResult GetDesignationsList([FromBody] IcampusBoatBackend.Models.Settings.RequestSettings bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_REQ_DESG_LIST", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@DEPTCODE", (object?)bol.Dept ?? DBNull.Value);

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
        [Route("GetEmployeeList")]
        public IActionResult GetEmployeeList([FromBody] IcampusBoatBackend.Models.Settings.RequestSettings bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_REQ_EMP_LIST_Modify", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Dept", (object?)bol.Dept ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReqTypeID", (object?)bol.ReqTypeID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DesgID", (object?)bol.DesgID ?? DBNull.Value);

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
        [Route("SaveRequestSettings")]
        public IActionResult SaveRequestSettings([FromBody] IcampusBoatBackend.Models.Settings.RequestSettings bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Req_Settings_Save", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@REQID", (object?)bol.ReqID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SENDERID", (object?)bol.SenderID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@R1ID", (object?)bol.R1ID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@R2ID", (object?)bol.R2ID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@R3ID", (object?)bol.R3ID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@R4ID", (object?)bol.R4ID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@R5ID", (object?)bol.R5ID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MENUID", (object?)bol.MENUID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SMENUID", (object?)bol.SMENUID ?? DBNull.Value);

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
        [Route("RestRequestsettings")]
        public IActionResult RestRequestsettings([FromBody] IcampusBoatBackend.Models.Settings.RequestSettings bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "DELETE FROM tbl_ERP_UM_EMP where MENUID=@MENUID AND SUBMENUID=@SMENUID";
                    using (SqlCommand cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.AddWithValue("@MENUID", (object?)bol.MENUID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SMENUID", (object?)bol.SMENUID ?? DBNull.Value);

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
        [Route("ResetRequestsettings")]
        public IActionResult ResetRequestsettings([FromBody] IcampusBoatBackend.Models.Settings.RequestSettings bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Reset_Req_Settings", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@REQID", (object?)bol.ReqID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SENDERID", (object?)bol.SenderID ?? DBNull.Value);

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
        [Route("RequestsettingsLoad")]
        public IActionResult RequestsettingsLoad([FromBody] IcampusBoatBackend.Models.Settings.RequestSettings bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Req_Settings_Load_Modified", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@REQID", (object?)bol.ReqID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DEPT", (object?)bol.Dept ?? DBNull.Value);

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
        [Route("UM_EMP_Save")]
        public IActionResult UM_EMP_Save([FromBody] IcampusBoatBackend.Models.Settings.RequestSettings bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_UM_EMP_Save", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@SENDERID", (object?)bol.SenderID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MENUID", (object?)bol.MENUID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SMENUID", (object?)bol.SMENUID ?? DBNull.Value);

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
        [Route("GetID")]
        public IActionResult GetID([FromBody] IcampusBoatBackend.Models.Settings.RequestSettings bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "select CAST(MENUID AS VARCHAR) + '_' + CAST(SMENUID AS VARCHAR) ID from tbl_erp_sub_menu where TEXT = @ReqType";
                    using (SqlCommand cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.AddWithValue("@ReqType", (object?)bol.ReqType ?? DBNull.Value);

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
    }
}
