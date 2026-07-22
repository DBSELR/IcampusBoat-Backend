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
    public class SubjectsMasterController : ControllerBase
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
        [Route("GetProgrammeLoad")]
        public IActionResult GetProgrammeLoad([FromBody] IcampusBoatBackend.Models.Settings.SubjectMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_SubjectMaster_Programme_Load", con) { CommandType = CommandType.StoredProcedure })
                    {
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
        [Route("GetBranchLoad")]
        public IActionResult GetBranchLoad([FromBody] IcampusBoatBackend.Models.Settings.SubjectMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_SubjectMaster_Branch_Load", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Course", (object?)bol.Programme ?? DBNull.Value);
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
        [Route("GetYearList")]
        public IActionResult GetYearList([FromBody] IcampusBoatBackend.Models.Settings.SubjectMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_YEARS", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@COURSE", (object?)bol.Programme ?? DBNull.Value);
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
        [Route("GetSubjectList")]
        public IActionResult GetSubjectList([FromBody] IcampusBoatBackend.Models.Settings.SubjectMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Exam_SubjectMaster_LIST", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@PROG", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCHCODE", (object?)bol.Branch ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEM", (object?)bol.Semester ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@STREAM", (object?)bol.Stream ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicYear", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Regu", (object?)bol.Regu ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@subtype", (object?)bol.subtype ?? DBNull.Value);

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
        [Route("GetRegulationList")]
        public IActionResult GetRegulationList()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("select Distinct Regulation from tbl_Regulation", con) { CommandType = CommandType.Text })
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
        [Route("CheckElectiveCode")]
        public IActionResult CheckElectiveCode([FromBody] IcampusBoatBackend.Models.Settings.SubjectMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Sub_Load_fac_Check_Elective", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Programme", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semester", (object?)bol.Semester ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@subjectCode", (object?)bol.Elective_pcodes ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BranchCode", (object?)bol.Branch ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);

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
        [Route("CheckPaperOrder")]
        public IActionResult CheckPaperOrder([FromBody] IcampusBoatBackend.Models.Settings.SubjectMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("PROC_CHECK_PAPORDER", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@PROGRAMME", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", (object?)bol.Branch ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMESTER", (object?)bol.Semester ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@STREAM", (object?)bol.Stream ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUBJECTCODE", (object?)bol.SubjectCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ORDER", (object?)bol.Pap_Order ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Regu", (object?)bol.Regu ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@type", "");

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
        [Route("CheckSubjectCodeExits")]
        public IActionResult CheckSubjectCodeExits([FromBody] IcampusBoatBackend.Models.Settings.SubjectMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("[SP_SUBJECTCODEEXITS]", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@COURSE", (object?)bol.P_code ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", (object?)bol.B_code ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEM", (object?)bol.Semester ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUBCODE", (object?)bol.SubjectCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@REGULATION", (object?)bol.Regu ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACYR", (object?)bol.AcademicYear ?? DBNull.Value);

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
        [Route("CheckSubNameLoad")]
        public IActionResult CheckSubNameLoad([FromBody] IcampusBoatBackend.Models.Settings.SubjectMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "Select Sid,SubjectName,SessionalMaxMarks,SessionalMinMarks from tbl_Exam_SubjectMaster " +
                                   "where CourseCode=@CourseCode and BranchCode=@BranchCode and year=@Year and Semester=@Semester " +
                                   "and SubjectCode=@SubjectCode and Stream=@Stream";
                    using (SqlCommand cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.AddWithValue("@CourseCode", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BranchCode", (object?)bol.Branch ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semester", (object?)bol.Semester ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SubjectCode", (object?)bol.SubjectCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Stream", (object?)bol.Stream ?? DBNull.Value);

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
        [Route("CheckCommonSub")]
        public IActionResult CheckCommonSub([FromBody] IcampusBoatBackend.Models.Settings.SubjectMaster bol)
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "Select * from tbl_Exam_SubjectMaster where CourseCode=@CourseCode and year=@Year and Semester=@Semester " +
                                   "and SubjectCode=@SubjectCode and Stream=@Stream";
                    using (SqlCommand cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.AddWithValue("@CourseCode", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Semester", (object?)bol.Semester ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SubjectCode", (object?)bol.SubjectCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Stream", (object?)bol.Stream ?? DBNull.Value);

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
        [Route("SaveSubjectMaster")]
        public IActionResult SaveSubjectMaster([FromBody] IcampusBoatBackend.Models.Settings.SubjectMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Exam_SubjectMaster_Save", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@SID", (object?)bol.Sid ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", (object?)bol.Programme ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", (object?)bol.Branch ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@YEAR", (object?)bol.Year ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PeriodType", (object?)bol.PeroidType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEMESTER", (object?)bol.Semester ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@STREAM", (object?)bol.Stream ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUBJECTCODE", (object?)bol.SubjectCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUBJECTNAME", (object?)bol.SubjectName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ACADEMICYEAR", (object?)bol.AcademicYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FINANCIALYEAR", (object?)bol.FinancialYear ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SESSIONALMAXMARKS", (object?)bol.SessionalMaxMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SESSIONALMINMARKS", (object?)bol.SessionalMinMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AssMaxMarks", (object?)bol.AssignmentMaxMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AssMinMarks", (object?)bol.AssignmentMinMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OQMaxMarks", (object?)bol.OnlineQuizMaxmarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OQMinMarks", (object?)bol.OnlineQuizMinmarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ObjMaxMarks", (object?)bol.ObjectiveMaxmarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ObjMinMarks", (object?)bol.ObjectiveMinmarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AttMaxMarks", (object?)bol.AttendenceMaxMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AttMinMarks", (object?)bol.AttendenceMinMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DayMaxMarks", (object?)bol.DaytoDayMaxMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DayMinMarks", (object?)bol.DaytoDayMinMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@intTextMaxMarks", (object?)bol.InternalTestMaxMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@intTextMinMarks", (object?)bol.InternalTestMinMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@VivoMaxMarks", (object?)bol.ViVaVoiceMaxMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@VivoMinmarks", (object?)bol.ViVaVoiceMinMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RecordMaxMarks", (object?)bol.RecordMaxMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RecordMinMarks", (object?)bol.RecordeMinMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IS_ELEC", (object?)bol.Iselective ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ELEC_CODES", (object?)bol.Elective_pcodes ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ELEC_NAMES", (object?)bol.Elective_pNames ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PAP_ORDER", (object?)bol.Pap_Order ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ELCPN", (object?)bol.Elcnames ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IntTestMax", (object?)bol.IntTestMax ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IntTestMin", (object?)bol.IntTestMin ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReportPresentMax", (object?)bol.ReportPresentattionMax ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReportPresentMin", (object?)bol.ReportPresentattionMin ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OralTestMax", (object?)bol.OralTestMax ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OralTestMin", (object?)bol.OralTestMin ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CIE100Max", (object?)bol.CIE100Max ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CIE100Min", (object?)bol.CIE100Min ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CIE75Max", (object?)bol.CIE75Max ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CIE75Min", (object?)bol.CIE75Min ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CIE60Max", (object?)bol.CIE60Max ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CIE60Min", (object?)bol.CIE60Min ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CIE50Max", (object?)bol.CIE50Max ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CIE50Min", (object?)bol.CIE50Min ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Cie40Max", (object?)bol.CIE40Max ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CIE40Min", (object?)bol.CIE40Min ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ExtMax", (object?)bol.ExtMax ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ExtMin", (object?)bol.ExtMin ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Regu", (object?)bol.Regu ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SubjectShortname", (object?)bol.SubjectShortName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ElectiveShortName", (object?)bol.ElectiveShortName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DrawMaxMarks", (object?)bol.DrawMaxMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DrawMinMarks", (object?)bol.DrawMinMarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Status", (object?)bol.Status ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Subtype", (object?)bol.subtype ?? DBNull.Value);

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
        [Route("DeleteSubjectMaster")]
        public IActionResult DeleteSubjectMaster([FromBody] IcampusBoatBackend.Models.Settings.SubjectMaster bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("[SP_Exam_SubjectMaster_DELETE]", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@SID", (object?)bol.Sid ?? DBNull.Value);

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
                return StatusCode(500, ex.Message);
            }
        }
    }
}
