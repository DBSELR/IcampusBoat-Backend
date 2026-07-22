
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

        [HttpPost]
        [Route("Login")]
        //public IActionResult Login(string UserId, string Password)
            public IActionResult LoadAttMaxDate([FromBody] Login bol)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
                {
                    con.Open();

                    using SqlCommand sqlcmd = new SqlCommand("SP_LOGIN_CHECK", con);

                    sqlcmd.CommandType = CommandType.StoredProcedure;

                    sqlcmd.Parameters.Add("@USERID", SqlDbType.VarChar, 255).Value = bol.USERID;
                    sqlcmd.Parameters.Add("@PWD", SqlDbType.VarChar, 64).Value = bol.Password;


                    using SqlDataReader reader = sqlcmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        return Unauthorized(new
                        {
                            message = "Invalid User ID or Password"
                        });
                    }


                    string userId = reader["USERGROUP"].ToString();
                    string userName = reader["USERNAME"].ToString();
                    string userGroup = reader["SUBGROUP"].ToString();


                    var claims = new List<Claim>
                             {
                                //new Claim("UserID", UserId),
                                new Claim(ClaimTypes.Name, bol.USERID),
                                new Claim(ClaimTypes.Role, userGroup),
                                new Claim("Regno", bol.USERID),
                                new Claim("SSNO",bol.USERID),
                                new Claim("UserGroup", userGroup)
                             };


                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        signingCredentials: signIn);



                    string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

                    //var token = await _authService.GenerateJwtTokenAsync(userId, userGroup, 14400);

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

        //[HttpGet]
        //[Route("Load_Menu")]
        //public IActionResult Load_Menu(string UserId, string Password)
        //{
        //    using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
        //    {
        //        using (SqlCommand sqlcmd = new SqlCommand("SP_LOGIN_CHECK", con))
        //        {
        //            sqlcmd.CommandType = CommandType.StoredProcedure;
        //            sqlcmd.Parameters.Add("@USERID", SqlDbType.VarChar, 255).Value = UserId;
        //            sqlcmd.Parameters.Add("@PWD", SqlDbType.VarChar, 64).Value = Password;

        //            //DataTable dt = DAL.GetData_FrmSP(sqlcmd, DAL.QueryType.SP);
        //            con.Open();

        //            using SqlDataReader reader = sqlcmd.ExecuteReader();

        //            List<object> menus = new List<object>();

        //            while (reader.Read())
        //            {
        //                menus.Add(new
        //                {
        //                    MenuId = reader["MENUID"].ToString(),
        //                    MenuName = reader["MENUNAME"].ToString(),
        //                    SubGroup = reader["SUBGROUP"].ToString()
        //                });
        //            }

        //            if (menus.Count == 0)
        //            {
        //                return NotFound("No menu found.");
        //            }

        //            return Ok(menus);

        //        }
        //    }
        //}


        //[HttpGet]
        //[Route("Load_Sub_Menu")]
        //public IActionResult Load_Sub_Menu(string UserId)
        //{
        //    using (SqlConnection con = new SqlConnection(DAL.SQLConnString))
        //    {
        //        using (SqlCommand sqlcmd = new SqlCommand("[SP_LOAD_USER_SUBMENUS]", con))
        //        {
        //            sqlcmd.CommandType = CommandType.StoredProcedure;
        //            sqlcmd.Parameters.Add("@USERID", SqlDbType.VarChar, 255).Value = UserId;

        //            con.Open();

        //            using SqlDataReader reader = sqlcmd.ExecuteReader();

        //            List<object> submenus = new List<object>();

        //            while (reader.Read())
        //            {
        //                submenus.Add(new
        //                {
        //                    MenuId = reader["MENUID"].ToString(),
        //                    SMenuId = reader["SMENUID"].ToString(),
        //                    SMenuName = reader["SubMenuNam"].ToString(),
        //                    MenuName = reader["MENUNAME"].ToString(),
        //                    SubGroup = reader["SUBGROUP"].ToString(),
        //                    formtye= reader["formtype"].ToString(),
        //                    Route = reader["FORM"].ToString(),
        //                });
        //            }

        //            if (submenus.Count == 0)
        //            {
        //                return NotFound("No menu found.");
        //            }

        //            return Ok(submenus);

        //        }
        //    }
        //    }
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
                    MenuId = reader["MENUID"].ToString(),
                    SMenuId = reader["SMENUID"].ToString(),
                    SMenuName = reader["SubMenuNam"].ToString(),
                    MenuName = reader["MENUNAME"].ToString(),
                    SubGroup = reader["SUBGROUP"].ToString(),
                    FormType = reader["FORMTYPE"].ToString(),
                    Route = reader["FORM"].ToString(),
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

    public class Login
    {
        public string? USERID { get; set; }
        public string? Password { get; set; }
    }



}
