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
    public class FacultyMasterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public FacultyMasterController(IConfiguration configuration)
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
        [Route("GetDept")]
        public IActionResult GetDept([FromBody] IcampusBoatBackend.Models.Settings.FacultyMaster FM)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_Fclty_Dept_Load", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Dept", (object?)FM.Department ?? DBNull.Value);

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
        [Route("GetSubjects")]
        public IActionResult GetSubjects([FromBody] IcampusBoatBackend.Models.Settings.FacultyMaster FM)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_Sub_Load_fac", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Programme", (object?)FM.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)FM.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semester", (object?)FM.Semister ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmpID", (object?)FM.EmpId ?? DBNull.Value);

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
        [Route("GetAssSubjectsList")]
        public IActionResult GetAssSubjectsList([FromBody] IcampusBoatBackend.Models.Settings.FacultyMaster FM)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_Sub_Load_fac", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Programme", (object?)FM.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)FM.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semester", (object?)FM.Semister ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmpID", (object?)FM.EmpId ?? DBNull.Value);

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
        [Route("GetCourseList")]
        public IActionResult GetCourseList([FromBody] IcampusBoatBackend.Models.Settings.FacultyMaster FM)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_USERWISE_LoadCourse", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@dept", (object?)FM.Department ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)FM.AcademicYear ?? DBNull.Value);

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
        [Route("GetYearList")]
        public IActionResult GetYearList([FromBody] IcampusBoatBackend.Models.Settings.FacultyMaster FM)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_USERWISE_LoadYR", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@DEPT", (object?)FM.Department ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CourseCode", (object?)FM.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)FM.AcademicYear ?? DBNull.Value);

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
        [Route("GetEmployeeList")]
        public IActionResult GetEmployeeList([FromBody] IcampusBoatBackend.Models.Settings.FacultyMaster FM)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_USERWISE_LoadEmpid", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@EMPID", (object?)FM.EmpId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dept", (object?)FM.Department ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@fed", (object?)FM.FED ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@workMode", (object?)FM.WorkMode ?? DBNull.Value);

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
        [Route("GetFacultyList")]
        public IActionResult GetFacultyList([FromBody] IcampusBoatBackend.Models.Settings.FacultyMaster FM)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_LOAD_FACULTY", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@PROG", (object?)FM.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YR", (object?)FM.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEM", (object?)FM.Semister ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DEPT", (object?)FM.Department ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FAC", (object?)FM.Faculty ?? DBNull.Value);

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
        [Route("SaveFaculty")]
        public IActionResult SaveFaculty([FromBody] IcampusBoatBackend.Models.Settings.FacultyMaster FM)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_FACULTY_SAVE", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@ID", (object?)FM.id ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", (object?)FM.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)FM.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMISTER", (object?)FM.Semister ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DEPARTMENT", (object?)FM.Department ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUBJECT", (object?)FM.Subject ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FACULTY", (object?)FM.Faculty ?? DBNull.Value);

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
        [Route("DeleteFaculty")]
        public IActionResult DeleteFaculty([FromBody] IcampusBoatBackend.Models.Settings.FacultyMaster FM)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_FACULTY_DELETE", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@ID", (object?)FM.id ?? DBNull.Value);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            return Ok(new { message = "Success" });
                        else
                            return BadRequest(new { message = "Failed to delete record" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
