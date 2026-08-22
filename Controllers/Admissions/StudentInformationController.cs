using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using IcampusBoatBackend.Models.Admissions;

namespace IcampusBoatBackend.Controllers.Admissions
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentInformationController : ControllerBase
    {
        private static readonly HashSet<string> AllowedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "STUDENTSERIALNO", "REGISTRATIONNO", "ADMISSIONNO", "ADMISSIONDATE", "DOB", "SNAME", "MODEOFADM",
            "SECTION", "ACADAMICYEAR", "AYEAR", "SYEAR", "ASEMESTER", "SSEMESTER", "SECLANG", "MEDIUM", "CASTE",
            "SUBCASTE", "GENDER", "RELIGION", "TUITIONFEE", "MISCELLANEOUSFEE", "COURSECODE", "BRANCHCODE",
            "FIRSTNAME", "LASTNAME", "MARRIEDSTATUS", "BLOODGROUP", "MOTHERTONGUE", "STUDENTAADHAAR", "MOBILENO",
            "STUDENTACNO", "STUDENTIFSCCODE", "BANKBRANCHNAME", "MOLE1", "MOLE2", "ROLLNUM", "SEC", "RATIONCARDNO",
            "PREYEAROFPASSING", "PARENTOCCUPATION", "SET_ADM_TYPE"
        };

        /// <summary>
        /// Fetch list of all fields/columns available in the TBL_ADM_STUDATA table (DAL: ADMIN_STDDATA_COLUMNS).
        /// Parameters: 0 (<= 2, uses method params)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("columns")]
        public IActionResult GetColumns()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = @"SELECT replace(col.name, ' ', '_') Name 
                                     FROM sys.columns col 
                                     JOIN sys.types typ ON col.system_type_id = typ.system_type_id AND col.user_type_id = typ.user_type_id 
                                     WHERE object_id = object_id('TBL_ADM_STUDATA') AND column_id != 1 AND column_id != 2";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch programmes list (DAL: ADMIN_STDADMIN_Programme_LIST).
        /// Parameters: 1 (<= 2, uses method param)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("programmes")]
        public IActionResult GetProgrammes([FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Academic year is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_STDDATA_Programme_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch branch list for selected programme and academic year (DAL: ADMIN_STDADMIN_Branch_LIST).
        /// Parameters: 2 (<= 2, uses method params)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("branches")]
        public IActionResult GetBranches([FromQuery] string programme, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(programme) || string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Programme and academic year are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADMIN_STDADMIN_Branch_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Course", programme);
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch studying years list for selected programme and academic year (DAL: ADMIN_STDADMIN_YEAR_LIST / Get_Year_CourseWise).
        /// Parameters: 2 (<= 2, uses method params)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("years")]
        public IActionResult GetYears([FromQuery] string programme, [FromQuery] string academicYear)
        {
            if (string.IsNullOrWhiteSpace(programme) || string.IsNullOrWhiteSpace(academicYear))
            {
                return BadRequest(new { success = false, message = "Programme and academic year are required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_YEARS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Course", programme);
                        cmd.Parameters.AddWithValue("@AcademicYear", academicYear);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get distinct options/values for a specific filter column in TBL_ADM_STUDATA (DAL: BindData_DDL_*).
        /// Parameters: 1 (<= 2, uses method param)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("filter-options")]
        public IActionResult GetFilterOptions([FromQuery] string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                return BadRequest(new { success = false, message = "ColumnName is required." });
            }

            string sanitizedColumnName = columnName.Replace(" ", "_");

            if (!AllowedColumns.Contains(sanitizedColumnName) &&
                !string.Equals(sanitizedColumnName, "Gender", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "Course", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "Branch", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "SYear", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "SSemester", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "Section", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "ModeofAdm", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "Caste", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "SubCaste", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "Religion", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "AcadamicYear", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "AYear", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "ASemester", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "ParentOccupation", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sanitizedColumnName, "SET_ADM_Type", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { success = false, message = "Invalid column name for filter options." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();

                    string selectField = sanitizedColumnName;
                    if (string.Equals(sanitizedColumnName, "Branch", StringComparison.OrdinalIgnoreCase))
                    {
                        selectField = "bsname";
                    }

                    string query = "";
                    if (string.Equals(sanitizedColumnName, "Course", StringComparison.OrdinalIgnoreCase))
                    {
                        query = @"SELECT DISTINCT c.Course AS value 
                                  FROM TBL_ADM_STUDATA s 
                                  INNER JOIN tbl_Adm_Course c ON s.CourseCode = c.CourseCode AND s.AcadamicYear = c.AcademicYear 
                                  INNER JOIN tbl_Adm_Branch b ON s.CourseCode = b.CourseCode AND s.BranchCode = b.BranchCode AND s.AcadamicYear = b.academicyear 
                                  WHERE c.Course IS NOT NULL AND c.Course <> '' ORDER BY value";
                    }
                    else if (string.Equals(sanitizedColumnName, "Branch", StringComparison.OrdinalIgnoreCase) || string.Equals(sanitizedColumnName, "bsname", StringComparison.OrdinalIgnoreCase))
                    {
                        query = @"SELECT DISTINCT b.bsname AS value 
                                  FROM TBL_ADM_STUDATA s 
                                  INNER JOIN tbl_Adm_Course c ON s.CourseCode = c.CourseCode AND s.AcadamicYear = c.AcademicYear 
                                  INNER JOIN tbl_Adm_Branch b ON s.CourseCode = b.CourseCode AND s.BranchCode = b.BranchCode AND s.AcadamicYear = b.academicyear 
                                  WHERE b.bsname IS NOT NULL AND b.bsname <> '' ORDER BY value";
                    }
                    else
                    {
                        query = $@"SELECT DISTINCT CAST({selectField} AS VARCHAR(250)) AS value 
                                  FROM TBL_ADM_STUDATA s 
                                  INNER JOIN tbl_Adm_Course c ON s.CourseCode = c.CourseCode AND s.AcadamicYear = c.AcademicYear 
                                  INNER JOIN tbl_Adm_Branch b ON s.CourseCode = b.CourseCode AND s.BranchCode = b.BranchCode AND s.AcadamicYear = b.academicyear 
                                  WHERE {selectField} IS NOT NULL ORDER BY value";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    List<string> optionsList = new List<string>();
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["value"] != DBNull.Value)
                        {
                            optionsList.Add(row["value"].ToString()!);
                        }
                    }

                    return Ok(new { success = true, data = optionsList });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch branch and year combinations for a given programme (DAL: Get_CourseWise_Branch_Year).
        /// Parameters: 1 (<= 2, uses method param)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("course-wise-branch-year")]
        public IActionResult GetCourseWiseBranchYear([FromQuery] string programme)
        {
            if (string.IsNullOrWhiteSpace(programme))
            {
                return BadRequest(new { success = false, message = "Programme is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "SELECT DISTINCT Branch, CAST(Year AS VARCHAR(5)) AS year FROM tbl_ADMIN_ApplicationSales WHERE Programme = @Programme";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Programme", programme);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Load student profile details by admission number (DAL: STUDENTADMIN_StudentData_Load).
        /// Parameters: 1 (<= 2, uses method param)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("load-by-admission-no")]
        public IActionResult GetStudentDataByAdmissionNo([FromQuery] string admissionNo)
        {
            if (string.IsNullOrWhiteSpace(admissionNo))
            {
                return BadRequest(new { success = false, message = "AdmissionNo is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "SELECT AdmissionDate, RollNo, StudentName, Programme, Branch, Section, Year, Status, Caste, SubCaste, SecondLanguage, Gender, Medium FROM tbl_Admin_StdAdmision WHERE AdmissionNo = @AdmissionNo";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AdmissionNo", admissionNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch subcastes list for selected caste (DAL: LoadComboCaste).
        /// Parameters: 1 (<= 2, uses method param)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("subcastes-by-caste")]
        public IActionResult GetSubCastesByCaste([FromQuery] string caste)
        {
            if (string.IsNullOrWhiteSpace(caste))
            {
                return BadRequest(new { success = false, message = "Caste is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    string query = "SELECT DISTINCT SubCaste FROM tbl_Admin_StdAdmision WHERE Caste = @Caste";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Caste", caste);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get download format fields (DAL: DownloadFeilds / sp_DownloadFormatFeillds).
        /// Parameters: 0 (<= 2, uses method params)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("download-format-fields")]
        public IActionResult GetDownloadFormatFields()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("sp_DownloadFormatFeillds", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get upload format fields (DAL: UploadFile / sp_uploadFormatFeillds).
        /// Parameters: 0 (<= 2, uses method params)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("upload-format-fields")]
        public IActionResult GetUploadFormatFields()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("sp_uploadFormatFeillds", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    return Ok(new { success = true, data = DAL.DataTableToList(dt) });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Execute custom dynamic student reports with selected columns and filters (DAL: BindData_DDL_WithSearch / BindData).
        /// Parameters: Complex request object (> 2 fields, uses [FromBody])
        /// </summary>
        [AllowAnonymous]
        [HttpPost("report")]
        public IActionResult GenerateReport([FromBody] StudentInformationReportRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.AcademicYear))
            {
                return BadRequest(new { success = false, message = "Academic year is required." });
            }

            if (request.Columns == null || request.Columns.Count == 0)
            {
                return BadRequest(new { success = false, message = "At least one column must be selected." });
            }

            try
            {
                List<string> selectedColumns = new List<string>();

                foreach (var col in request.Columns)
                {
                    string cleanCol = col.Trim();
                    if (!AllowedColumns.Contains(cleanCol))
                    {
                        return BadRequest(new { success = false, message = $"Column '{cleanCol}' is not recognized or allowed." });
                    }

                    if (string.Equals(cleanCol, "CourseCode", StringComparison.OrdinalIgnoreCase))
                    {
                        selectedColumns.Add("c.course AS Course");
                    }
                    else if (string.Equals(cleanCol, "BranchCode", StringComparison.OrdinalIgnoreCase))
                    {
                        selectedColumns.Add("b.bsname AS Branch");
                    }
                    else if (string.Equals(cleanCol, "AdmissionDate", StringComparison.OrdinalIgnoreCase))
                    {
                        selectedColumns.Add("FORMAT(cast(s.AdmissionDate as date), 'dd-MM-yyyy') as AdmissionDate");
                    }
                    else if (string.Equals(cleanCol, "DOB", StringComparison.OrdinalIgnoreCase))
                    {
                        selectedColumns.Add("FORMAT(cast(s.DOB as date), 'dd-MM-yyyy') as DOB");
                    }
                    else
                    {
                        selectedColumns.Add($"s.{cleanCol}");
                    }
                }

                string selectClause = string.Join(", ", selectedColumns);

                List<string> filterClauses = new List<string>();
                List<SqlParameter> sqlParams = new List<SqlParameter>();

                filterClauses.Add("s.IsActive = 1");
                filterClauses.Add("s.AcadamicYear = @AcademicYear");
                sqlParams.Add(new SqlParameter("@AcademicYear", request.AcademicYear));

                int paramIndex = 0;
                if (request.Filters != null && request.Filters.Count > 0)
                {
                    foreach (var filter in request.Filters)
                    {
                        string fieldKey = filter.Key.Trim();

                        if (!AllowedColumns.Contains(fieldKey) &&
                            !string.Equals(fieldKey, "Gender", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "Course", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "Branch", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "SYear", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "SSemester", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "Section", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "ModeofAdm", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "Caste", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "SubCaste", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "Religion", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "AYear", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "ASemester", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "ParentOccupation", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(fieldKey, "SET_ADM_Type", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        string mappedField = $"s.{fieldKey}";
                        if (string.Equals(fieldKey, "Course", StringComparison.OrdinalIgnoreCase))
                        {
                            mappedField = "c.Course";
                        }
                        else if (string.Equals(fieldKey, "Branch", StringComparison.OrdinalIgnoreCase))
                        {
                            mappedField = "b.bsname";
                        }

                        string paramName = $"@FilterVal_{paramIndex}";
                        filterClauses.Add($"{mappedField} = {paramName}");
                        sqlParams.Add(new SqlParameter(paramName, filter.Value));
                        paramIndex++;
                    }
                }

                string whereClause = string.Join(" AND ", filterClauses);

                string finalQuery = $@"SELECT {selectClause} 
                                     FROM TBL_ADM_STUDATA s 
                                     INNER JOIN tbl_Adm_Course c ON s.CourseCode = c.CourseCode AND s.AcadamicYear = c.AcademicYear 
                                     INNER JOIN tbl_Adm_Branch b ON s.CourseCode = b.CourseCode AND s.BranchCode = b.BranchCode AND s.AcadamicYear = b.academicyear 
                                     WHERE {whereClause} 
                                     ORDER BY s.RegistrationNo";

                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(finalQuery, con))
                    {
                        cmd.Parameters.AddRange(sqlParams.ToArray());
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    return Ok(new
                    {
                        success = true,
                        message = "Report generated successfully.",
                        data = DAL.DataTableToList(dt)
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Save student profile data (DAL: Savestudentdata / SP_ADM_STUDENTDATA_SAVE).
        /// Parameters: > 2 fields (uses [FromBody])
        /// </summary>
        [AllowAnonymous]
        [HttpPost("save-student-data")]
        public IActionResult SaveStudentData([FromBody] SaveStudentDataRequest model)
        {
            if (model == null)
            {
                return BadRequest(new { success = false, message = "Student data is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_ADM_STUDENTDATA_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IDENT", model.Ident ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ADMISSIONNO", model.AdmissionNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SNAME", model.SName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ADMISSIONDATE", model.AdmissionDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FIRSTNAME", model.FirstName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@LASTNAME", model.LastName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DOB", model.Dob ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@GENDER", model.Gender ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CASTE", model.Caste ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SUBCASTE", model.SubCaste ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MARRIEDSTATUS", model.MarriedStatus ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@RELIGION", model.Religion ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BLOODGROUP", model.BloodGroup ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MOTHERTONGUE", model.MotherTongue ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STUDINGYEAR", model.StudingYear ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PROGRAMME", model.Programme ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BRANCH", model.Branch ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SECLAN", model.SecLan ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MEDIUM", model.Medium ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STATUS", model.Status ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STUDENTAADHAAR", model.StudentAadhaar ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MOBILENO", model.MobileNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STUDENTACNO", model.StudentAcNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@STUDENTIFSCCODE", model.StudentIfscCode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BANKBRANCHNAME", model.BankBranchName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MOLE1", model.Mole1 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MOLE2", model.Mole2 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ROLLNUM", model.RollNum ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SEC", model.Sec ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@RATIONCARDNO", model.RationCardNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PREYEAROFPASSING", model.PreYearOfPassing ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Student data saved successfully.", affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Save parent details (DAL: SaveParentData / SP_STUDENT_PARENTDEATAILS_SAVE).
        /// Parameters: > 2 fields (uses [FromBody])
        /// </summary>
        [AllowAnonymous]
        [HttpPost("save-parent-data")]
        public IActionResult SaveParentData([FromBody] SaveParentDataRequest model)
        {
            if (model == null)
            {
                return BadRequest(new { success = false, message = "Parent data is required." });
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_STUDENT_PARENTDEATAILS_SAVE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ADMISSIONNO", model.AdmissionNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FNAME", model.FName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FAADHAARNO", model.FAadhaarNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MNAME", model.MName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MMNAME", model.MMName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MAADHAARNO", model.MAadhaarNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@GUARDIANNAME", model.GuardianName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@GUARDIANAADHAARNO", model.GuardianAadhaarNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ANNUALINCOMEGUARDIAN", model.AnnualIncomeGuardian ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@OCCUPATIONOFGUARDIAN", model.OccupationOfGuardian ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PARENTMOBILENO", model.ParentMobileNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ADDRESS", model.Address ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@VILLAGENAME", model.VillageName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@LANDLINENO", model.LandlineNo ?? (object)DBNull.Value);

                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Parent data saved successfully.", affectedRows = rows });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Execute final update for student data batch (DAL: FinalUpdationstudentdata / SP_FINALUPDATESTUDENTDATA).
        /// Parameters: 0 (<= 2, uses method params)
        /// </summary>
        [AllowAnonymous]
        [HttpPost("final-update-student-data")]
        public IActionResult FinalUpdateStudentData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_FINALUPDATESTUDENTDATA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        int rows = cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Final student data update executed successfully.", affectedRows = rows });
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


