using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using IcampusBoatBackend.Models.Accounts;

namespace IcampusBoatBackend.Controllers.Accounts
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BudgetController(IConfiguration configuration)
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

      
        [AllowAnonymous]
        [HttpGet]
        [Route("LoadFYears")]
        public IActionResult LoadFYears()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_FYEARS", con))
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
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

       
        [AllowAnonymous]
        [HttpGet]
        [Route("BudgetList")]
        public IActionResult BudgetList(string? fYear, string? sMode)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_BUDGET_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FYEAR", string.IsNullOrEmpty(fYear) ? "NULL" : fYear);
                        cmd.Parameters.AddWithValue("@STYPE", string.IsNullOrEmpty(sMode) ? "NULL" : sMode);

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
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        
        [AllowAnonymous]
        [HttpPost]
        [Route("SaveBudget")]
        public IActionResult SaveBudget([FromBody] Budget model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_BUDGET_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", string.IsNullOrEmpty(model.ID) ? "0" : model.ID);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)model.FYEAR ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SMODE", (object?)model.SMODE ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BHSNAME", (object?)model.BHSNAME ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AMOUNT", (object?)model.AMOUNT ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@USERID", (object?)model.USERID ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Data saved successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }        
      

        
        [AllowAnonymous]
        [HttpPost]
        [Route("DeleteBudgetHead")]
        public IActionResult DeleteBudgetHead(string? id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    string query = "DELETE FROM TBL_BUDGET WHERE ID = @ID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ID", (object?)id ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Budget head deleted successfully", rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}

