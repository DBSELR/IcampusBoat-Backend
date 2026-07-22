using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace IcampusBoatBackend.Controllers.Settings
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class DBBackupController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DBBackupController(IConfiguration configuration)
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
        [Route("GetBackUpHistory")]
        public IActionResult GetBackUpHistory()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    using (var cmd = new SqlCommand("SP_GetDbBackupHistory", con) { CommandType = CommandType.StoredProcedure })
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
        [Route("GetLastLogInsertdDate")]
        public IActionResult GetLastLogInsertdDate()
        {
            try
            {
                var result = new List<object>();
                using (SqlConnection con = new SqlConnection(IcampusBoatBackend.DAL.SQLConnString))
                {
                    string query = "Select distinct Max(TakenDate) LogDate from Tbl_ERPDBBackUp_Log";
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
        [Route("RunBackup")]
        public IActionResult RunBackup([FromBody] IcampusBoatBackend.Models.Settings.DBBackup bol)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(IcampusBoatBackend.DAL.SQLConnString);
                string dbName = builder.InitialCatalog;
                string duplicateDbName = dbName + "_New";

                builder.InitialCatalog = "master";
                string masterConnStr = builder.ConnectionString;

                builder.InitialCatalog = duplicateDbName;
                string duplicateConnStr = builder.ConnectionString;

                string mainConnStr = IcampusBoatBackend.DAL.SQLConnString;

                // 1. Delete duplicate db if exists
                TryDeleteDuplicateDb(duplicateDbName, masterConnStr);

                // 2. Create duplicate db
                using (SqlConnection con = new SqlConnection(mainConnStr))
                {
                    using (var cmd = new SqlCommand("SP_CreateDuplicateDB", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@existingDB", dbName);
                        cmd.Parameters.AddWithValue("@NewDBName", duplicateDbName);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // 3. Drop SPs from duplicate DB
                using (SqlConnection con = new SqlConnection(duplicateConnStr))
                {
                    using (var cmd = new SqlCommand("SP_DropSpsFromDB", con) { CommandType = CommandType.StoredProcedure })
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // 4. Drop individual SPs if any remaining
                var remainingSps = new List<string>();
                using (SqlConnection con = new SqlConnection(duplicateConnStr))
                {
                    string query = "SELECT name SPName FROM dbo.sysobjects WHERE (type = 'P')";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                remainingSps.Add(reader["SPName"].ToString());
                            }
                        }
                    }
                }
                foreach (var sp in remainingSps)
                {
                    using (SqlConnection con = new SqlConnection(duplicateConnStr))
                    {
                        using (var cmd = new SqlCommand($"DROP PROCEDURE [{sp}]", con) { CommandType = CommandType.Text })
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // 5. Create backup
                string targetDir = @"D:\ERPDB-Backup";
                string dateStr = DateTime.Now.ToString("dd-MM-yyyy");
                string clientBackupFile = Path.Combine(targetDir, $"{bol.ClientbackUpPrefixName}{dateStr}.bak");
                if (System.IO.File.Exists(clientBackupFile))
                    System.IO.File.Delete(clientBackupFile);
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                using (SqlConnection con = new SqlConnection(masterConnStr))
                {
                    using (var cmd = new SqlCommand($"BACKUP DATABASE {duplicateDbName} TO DISK = '{clientBackupFile}' WITH INIT", con) { CommandType = CommandType.Text })
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // 6. Delete duplicate db
                TryDeleteDuplicateDb(duplicateDbName, masterConnStr);

                // 7. Insert DB Backup Log
                using (SqlConnection con = new SqlConnection(mainConnStr))
                {
                    using (var cmd = new SqlCommand("Sp_InsertERPDBLog", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@name", (object?)bol.EmpId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@takenDt", DateTime.Now.ToString("yyyy-MM-dd HH:mm tt"));
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // 8. Script out SPs from main DB and write to file
                var script = new StringBuilder();
                using (SqlConnection con = new SqlConnection(mainConnStr))
                {
                    string query = "SELECT OBJECT_DEFINITION(P.object_id) SP FROM sys.procedures AS P";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var spDef = reader["SP"]?.ToString();
                                if (!string.IsNullOrEmpty(spDef))
                                {
                                    script.AppendLine(bol.Separator);
                                    script.AppendLine(bol.ANSI_NULLS);
                                    script.AppendLine(bol.Go);
                                    script.AppendLine(bol.QUOTED_IDENTIFIER);
                                    script.AppendLine(bol.Go);
                                    script.AppendLine(spDef).AppendLine();
                                    script.AppendLine();
                                    script.AppendLine(bol.Go);
                                    script.AppendLine();
                                    script.AppendLine();
                                }
                            }
                        }
                    }
                }

                string scriptDir = @"E:\DBS_Backup";
                if (!Directory.Exists(scriptDir))
                    Directory.CreateDirectory(scriptDir);
                string scriptFile = Path.Combine(scriptDir, $"{bol.MailSubjectPrefixName}{dateStr}.sql");
                if (System.IO.File.Exists(scriptFile))
                    System.IO.File.Delete(scriptFile);
                System.IO.File.WriteAllText(scriptFile, script.ToString());

                // 9. Send email
                TrySendEmail(bol, scriptFile);

                return Ok(new { message = "Success", backupFile = clientBackupFile, scriptFile = scriptFile });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private void TryDeleteDuplicateDb(string duplicateDbName, string masterConnStr)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(masterConnStr))
                {
                    string query = $"ALTER DATABASE {duplicateDbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{duplicateDbName}]";
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.Text })
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Suppress exception if db does not exist or cannot be dropped
            }
        }

        private void TrySendEmail(IcampusBoatBackend.Models.Settings.DBBackup bol, string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    var dateStr = DateTime.Now.ToString("dd-MM-yyyy");
                    using var mail = new MailMessage();
                    using var smtpServer = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new System.Net.NetworkCredential(bol.FromMailid, bol.MailPswd),
                        EnableSsl = true
                    };

                    mail.From = new MailAddress(bol.FromMailid);
                    mail.To.Add(bol.ToMailid);
                    mail.Subject = bol.MailSubjectPrefixName + dateStr;
                    mail.Body = bol.MailBody;

                    var attachment = new Attachment(filePath);
                    mail.Attachments.Add(attachment);

                    smtpServer.Send(mail);
                }
            }
            catch
            {
                // Suppress email errors to allow successful response
            }
        }
    }
}
