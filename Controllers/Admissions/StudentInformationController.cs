using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
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
        /// Fetch list of all fields/columns available in the TBL_ADM_STUDATA table.
        /// </summary>
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
        /// Fetch programmes list.
        /// </summary>
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
        /// Fetch branch list for selected programme and academic year.
        /// </summary>
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
        /// Fetch studying years list for selected programme and academic year.
        /// </summary>
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
        /// Get distinct options/values for a specific filter column in TBL_ADM_STUDATA.
        /// </summary>
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
        /// Execute custom dynamic student reports with selected columns and filters.
        /// </summary>
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

                // Whitelist and map requested columns safely to prevent SQL Injection
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

                // Build dynamic filter conditions whitelisting keys
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
                            continue; // skip unsafe/unrecognized filter keys
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
    }
}
