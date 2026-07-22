
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office.Word;
using IcampusBoatBackend;
using LMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace IcampusBoatBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login(string UserId, string Password)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    using SqlCommand sqlcmd = new SqlCommand("SP_LOGIN_CHECK", con);

                    sqlcmd.CommandType = CommandType.StoredProcedure;

                    sqlcmd.Parameters.Add("@USERID", SqlDbType.VarChar, 255).Value = UserId;
                    sqlcmd.Parameters.Add("@PWD", SqlDbType.VarChar, 64).Value = Password;


                    using SqlDataReader reader = sqlcmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        return Unauthorized(new
                        {
                            message = "Invalid User ID or Password"
                        });
                    }


                    string userId = string.IsNullOrEmpty(UserId) ? reader["USERGROUP"].ToString() : UserId;
                    string userName = reader["USERNAME"]?.ToString() ?? "";
                    string userGroup = reader["SUBGROUP"]?.ToString() ?? "";


                    var claims = new List<Claim>
                             {
                                new Claim(ClaimTypes.Name, userId),
                                new Claim(ClaimTypes.Role, userGroup),
                                new Claim("Regno", userId),
                                new Claim("SSNO", userId),
                                new Claim("UserGroup", userGroup)
                             };


                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(480),
                        signingCredentials: signIn);



                    string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

                    return Ok(new
                    {
                        token = tokenValue,
                        user = new
                        {
                            userId,
                            userName,
                            userGroup,
                            message = "Login Successful"
                        }
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [Authorize]
        [HttpGet]
        [Route("Load_Sub_Menu")]
        public IActionResult Load_Sub_Menu()
        {
            // Get the logged-in user from the JWT
            var userId = User.Identity?.Name;

            // If not found, try reading the claim directly
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.Claims
                             .FirstOrDefault(c =>
                                 c.Type == ClaimTypes.Name || c.Type == "name" || c.Type == "UserID")
                             ?.Value;
            }

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "UserId not found in JWT.",
                    Claims = User.Claims.Select(c => new
                    {
                        c.Type,
                        c.Value
                    })
                });
            }

            using SqlConnection con = new SqlConnection(DAL.SQLConnString);
            using SqlCommand sqlcmd = new SqlCommand("SP_LOAD_USER_SUBMENUS", con);

            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.AddWithValue("@USERID", userId);

            con.Open();

            using SqlDataReader reader = sqlcmd.ExecuteReader();

            List<object> submenus = new();

            while (reader.Read())
            {
                submenus.Add(new
                {
                    menuId = reader["MENUID"].ToString(),
                    sMenuId = reader["SMENUID"].ToString(),
                    sMenuName = reader["SubMenuNam"].ToString(),
                    menuName = reader["MENUNAME"].ToString(),
                    subGroup = reader["SUBGROUP"].ToString(),
                    formType = reader["FORMTYPE"].ToString(),
                    route = reader["FORM"].ToString(),
                });
            }

            return Ok(submenus);
        }

    }

        //public class LoginRequest
        //{
        //    public string userId { get; set; }
        //    public string Password { get; set; }
        //}
        public class ChangePasswordRequest
    {
        public string? USERID { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }



}
