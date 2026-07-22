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
    public class LeaveTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LeaveTypeController(IConfiguration configuration)
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
        [Route("GetLtypeList")]
        public IActionResult GetLtypeList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_Ltype_List", con) { CommandType = CommandType.StoredProcedure })
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
        [Route("LoadAllLtypeList")]
        public IActionResult LoadAllLtypeList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("sp_LoadAll_Ltype_List", con) { CommandType = CommandType.StoredProcedure })
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
        [Route("SaveLtype")]
        public IActionResult SaveLtype([FromBody] IcampusBoatBackend.Models.Settings.LeaveType bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_LTYPE_SAVE", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Lid", (object?)bol.Lid ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LSNAME", (object?)bol.LSNAME ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LDESC", (object?)bol.LDESC ?? DBNull.Value);

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
        [Route("CheckLtsNameDup")]
        public IActionResult CheckLtsNameDup([FromBody] IcampusBoatBackend.Models.Settings.LeaveType bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "select LID ,LDesc from tbl_LeaveType where LSName=@LSNAME";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.AddWithValue("@LSNAME", (object?)bol.LSNAME ?? DBNull.Value);

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
        [Route("LoadLSGrid")]
        public IActionResult LoadLSGrid()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "select A.LSID,B.LSNAME+'-'+LDesc Leave,A.WORKMODE,A.ALY,A.ALM,A.LYEAR,A.REMARKS,A.ALD from tbl_LeaveStructure A inner join tbl_LeaveType B on A.LID=B.LID";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
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
        [Route("SaveLStype")]
        public IActionResult SaveLStype([FromBody] IcampusBoatBackend.Models.Settings.LvStructure bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_LSTRUCTURE_SAVE", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Lsid", (object?)bol.Lsid ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lid", (object?)bol.Lid ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LYEAR", (object?)bol.Lyer ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Workmode", (object?)bol.WorkMode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ALY", (object?)bol.LvforYr ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ALM", (object?)bol.Duration ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ALD", (object?)bol.LvFrDuration ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Remarks", (object?)bol.Remark ?? DBNull.Value);

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
        [Route("DeleteLvStructure")]
        public IActionResult DeleteLvStructure([FromBody] IcampusBoatBackend.Models.Settings.LvStructure bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "delete from tbl_LeaveStructure where LSID=@Lsid";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.AddWithValue("@Lsid", (object?)bol.Lsid ?? DBNull.Value);

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
        [Route("CheckLTypeDup")]
        public IActionResult CheckLTypeDup([FromBody] IcampusBoatBackend.Models.Settings.LvStructure bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_Ltype_DUP", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@LYEAR", (object?)bol.Lyer ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LID", (object?)bol.LeaveType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@WorkMode", (object?)bol.WorkMode ?? DBNull.Value);

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
    }
}
