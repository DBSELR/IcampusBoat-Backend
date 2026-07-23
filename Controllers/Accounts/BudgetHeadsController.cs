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
    public class BudgetHeadsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BudgetHeadsController(IConfiguration configuration)
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

        /// <summary>
        /// Retrieves the list of budget heads
        /// Parameters: 0 (<= 2, uses method params)
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Route("HeadsMasterList")]
        public IActionResult HeadsMasterList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_BUDGET_HEADS_LIST", con))
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

        /// <summary>
        /// Computes the next order number for a budget head in an academic year
        /// Parameters: 1 (<= 2, uses method param)
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Route("GetNextOrder")]
        public IActionResult GetNextOrder(string? academicYear)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    string query = "SELECT ISNULL(MAX([ORDER]), 0) AS MORDER FROM TBL_BUDGET_HEADS WHERE ACADEMICYEAR = @AcademicYear";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)academicYear ?? DBNull.Value);
                        var maxOrderObj = cmd.ExecuteScalar();
                        int maxOrder = (maxOrderObj != null && maxOrderObj != DBNull.Value) ? Convert.ToInt32(maxOrderObj) : 0;
                        int nextOrder = maxOrder + 1;
                        return Ok(new { nextOrder = nextOrder });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Gets specific budget head data by name and academic year
        /// Parameters: 2 (<= 2, uses method params)
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Route("GetPHData")]
        public IActionResult GetPHData(string? academicYear, string? phName)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_BUDGET_GET_DATA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BSNAME", (object?)phName ?? DBNull.Value);

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

        /// <summary>
        /// Saves or updates a budget head record
        /// Parameters: 6 (> 2, uses [FromBody] BudgetHeads model)
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("SaveHeadsMaster")]
        public IActionResult SaveHeadsMaster([FromBody] BudgetHeads model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_BUDGET_HEADS_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", string.IsNullOrEmpty(model.ID) ? "0" : model.ID);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)model.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FYEAR", (object?)model.FYEAR ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ORDER", (object?)model.ORDER ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BHNAME", (object?)model.PHNAME ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BHSNAME", (object?)model.PHSNAME ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        bool isNew = string.IsNullOrEmpty(model.ID) || model.ID == "0";
                        string message = isNew ? "Payment Head Saved Successfully" : "Payment Head Updated Successfully";

                        return Ok(new { success = true, message = message, rowsAffected = rowsAffected });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a budget head record by ID
        /// Parameters: 1 (<= 2, uses method param)
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("DeleteHeadsMaster")]
        public IActionResult DeleteHeadsMaster(string? id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_BUDGET_HEAD_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", (object?)id ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Payment Head Deleted Successfully", rowsAffected = rowsAffected });
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

