
using DocumentFormat.OpenXml.Office.Word;
using LMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly IHubContext<SessionHub> _hubContext;
        public AuthController(IConfiguration configuration, IAuthService authService, IHubContext<SessionHub> hubContext)
        {
            _configuration = configuration;
            _authService = authService;
            _hubContext = hubContext;
        }

        [HttpPost("Login")]

        public async Task<ActionResult<string>> Login([FromBody] LoginRequest LoginRequest)
        {
            var connStr = _configuration.GetConnectionString("DefaultConnection");

            string UserID = "";
            string passwordHash = "";
            string USERGROUP = "";


            using (var connAuth = new SqlConnection(connStr))

            using (var cmd = new SqlCommand("[SP_LOGIN_CHECK]", connAuth))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USERID", LoginRequest.userId);
                cmd.Parameters.AddWithValue("@PWD", LoginRequest.Password);


                await connAuth.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();



                if (await reader.ReadAsync())
                {
                    UserID = reader.GetString(reader.GetOrdinal("USERGROUP"));
                    USERGROUP = reader.GetString(reader.GetOrdinal("SUBGROUP"));
                }
                else
                {
                    return Unauthorized("Invalid credentials.");
                }
            }

            //// 4) Issue token
            var token = await _authService.GenerateJwtTokenAsync(UserID, USERGROUP, 14400);


            return Ok(new
            {
                token,
                message = "Login Successful"

            });
        }

        //.....ChangePassword.....//


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var connStr = _configuration.GetConnectionString("DefaultConnection");

            string currentPassword = "";

            // Step 1: Get current password
            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_GetChangeUserPassword", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USERID", request.USERID);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        currentPassword = reader["PWord"].ToString();
                    }
                }
            }

            if (string.IsNullOrEmpty(currentPassword))
            {
                return NotFound(new { message = "User not found." });
            }


            // Step 2: Check old password
            if (request.OldPassword != currentPassword)
            {
                return Unauthorized(new { message = "Old password is incorrect." });
            }


            // Step 3: Update new password
            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("[sp_UpdateUserPassword]", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@USERID", request.USERID);
                cmd.Parameters.AddWithValue("@NewPassword", request.NewPassword);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }


            return Ok(new
            {
                message = "Password changed successfully."
            });
        }

    }



    public class LoginRequest
    {
        public string userId { get; set; }
        public string Password { get; set; }
    }
    public class ChangePasswordRequest
    {
        public string? USERID { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }



}