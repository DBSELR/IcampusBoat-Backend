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
    public class BranchMasterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public BranchMasterController(IConfiguration configuration)
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

        [HttpPost]
        [Route("GetBranchMaster")]
        public IActionResult GetBranchMaster([FromBody] IcampusBoatBackend.Models.Settings.BranchMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_BRANCH_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetDepartmentList")]
        public IActionResult GetDepartmentList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_Dept_LIST", con))
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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("SaveBranchMaster")]
        public IActionResult SaveBranchMaster([FromBody] IcampusBoatBackend.Models.Settings.BranchMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // 1. Save Branch Master
                            using (var cmd = new SqlCommand("SP_ADM_BRANCH_SAVE", con, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@BID", (object?)bol.BID ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@COURSE", (object?)bol.COURSE ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@Department", (object?)bol.Department ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@BRANCHCODE", (object?)bol.BRANCHCODE ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@BRANCHShortNAME", (object?)bol.BRANCHShortNAME ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@BranchName", (object?)bol.BranchName ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@FinancialYear", (object?)bol.FinancialYear ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@Fed", (object?)bol.Fed ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }

                            // 2. Delete Branch Dept
                            using (var cmdDel = new SqlCommand("delete from tbl_Adm_Branch_Dept where bid = @BID", con, transaction))
                            {
                                cmdDel.Parameters.AddWithValue("@BID", (object?)bol.BID ?? DBNull.Value);
                                cmdDel.ExecuteNonQuery();
                            }

                            // 3. Get MaxYR
                            int maxYR = 0;
                            using (var cmdMax = new SqlCommand("Select [YEAR] from tbl_Adm_Course where AcademicYear=@AcademicYear And coursecode=@coursecode", con, transaction))
                            {
                                cmdMax.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                                cmdMax.Parameters.AddWithValue("@coursecode", (object?)bol.COURSE ?? DBNull.Value);
                                var maxResult = cmdMax.ExecuteScalar();
                                if (maxResult != null && maxResult != DBNull.Value)
                                {
                                    maxYR = Convert.ToInt32(maxResult);
                                }
                            }

                            // 4. Save Branch Dept in a loop
                            for (int i = 1; i <= maxYR; i++)
                            {
                                if (bol.Fed != "0" && i == 1)
                                {
                                    using (var cmdDep1 = new SqlCommand("SP_ADM_BRANCH_Dep_SAVE", con, transaction))
                                    {
                                        cmdDep1.CommandType = CommandType.StoredProcedure;
                                        cmdDep1.Parameters.AddWithValue("@BID", (object?)bol.BID ?? DBNull.Value);
                                        cmdDep1.Parameters.AddWithValue("@COURSE", (object?)bol.COURSE ?? DBNull.Value);
                                        cmdDep1.Parameters.AddWithValue("@Department", (object?)bol.Fed ?? DBNull.Value);
                                        cmdDep1.Parameters.AddWithValue("@BRANCHCODE", (object?)bol.BRANCHCODE ?? DBNull.Value);
                                        cmdDep1.Parameters.AddWithValue("@BRANCHShortNAME", (object?)bol.BRANCHShortNAME ?? DBNull.Value);
                                        cmdDep1.Parameters.AddWithValue("@BranchName", (object?)bol.BranchName ?? DBNull.Value);
                                        cmdDep1.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                                        cmdDep1.Parameters.AddWithValue("@FinancialYear", (object?)bol.FinancialYear ?? DBNull.Value);
                                        cmdDep1.Parameters.AddWithValue("@YR", i);
                                        cmdDep1.ExecuteNonQuery();
                                    }
                                }

                                using (var cmdDep2 = new SqlCommand("SP_ADM_BRANCH_Dep_SAVE", con, transaction))
                                {
                                    cmdDep2.CommandType = CommandType.StoredProcedure;
                                    cmdDep2.Parameters.AddWithValue("@BID", (object?)bol.BID ?? DBNull.Value);
                                    cmdDep2.Parameters.AddWithValue("@COURSE", (object?)bol.COURSE ?? DBNull.Value);
                                    cmdDep2.Parameters.AddWithValue("@Department", (object?)bol.Department ?? DBNull.Value);
                                    cmdDep2.Parameters.AddWithValue("@BRANCHCODE", (object?)bol.BRANCHCODE ?? DBNull.Value);
                                    cmdDep2.Parameters.AddWithValue("@BRANCHShortNAME", (object?)bol.BRANCHShortNAME ?? DBNull.Value);
                                    cmdDep2.Parameters.AddWithValue("@BranchName", (object?)bol.BranchName ?? DBNull.Value);
                                    cmdDep2.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                                    cmdDep2.Parameters.AddWithValue("@FinancialYear", (object?)bol.FinancialYear ?? DBNull.Value);
                                    cmdDep2.Parameters.AddWithValue("@YR", i);
                                    cmdDep2.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            return Ok(new { message = "Success" });
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return StatusCode(500, new { message = ex.Message });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("DeleteBranchMaster")]
        public IActionResult DeleteBranchMaster([FromBody] IcampusBoatBackend.Models.Settings.BranchMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_BRANCH_DELETE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BID", (object?)bol.BID ?? DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            return Ok(new { message = "Success" });
                        else
                            return BadRequest(new { message = "Failed" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("CheckBranchCode")]
        public IActionResult CheckBranchCode([FromBody] IcampusBoatBackend.Models.Settings.BranchMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Select * from tbl_Adm_Branch where AcademicYear=@AcademicYear And coursecode=@COURSE and Dept=@Department and BranchCode = @BRANCHCODE", con))
                    {
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@COURSE", (object?)bol.COURSE ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Department", (object?)bol.Department ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCHCODE", (object?)bol.BRANCHCODE ?? DBNull.Value);

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
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
