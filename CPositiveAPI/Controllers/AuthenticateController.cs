using Azure;
using CPositiveAPI.Data;
using CPositiveAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CPositiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private IConfiguration _configuration;
        public readonly ApplicationDbContext Context;
        public AuthenticateController(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            Context = dbContext;
        }

        private Boolean AuthenticateUser(Users user)
        {
            using var transaction = Context.Database.BeginTransaction();
            try
            {
                var EmailId = Context.Users.Where(u => u.Username == user.Username)
                            .Select(u => u.Username)
                .FirstOrDefault();

                var Password = Context.Users.Where(u => u.Password == user.Password)
                                .Select(u => u.Password)
                                .FirstOrDefault();

                if (EmailId != null && Password != null)
                {
                    var UserId = Context.Users.Where(u => u.Username == EmailId && u.Password == Password)
                                .Select(u => u.UserId)
                                .FirstOrDefault();                 

                    return true;
                }
               
                return false;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

       


        private string GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Logins([FromBody] Login loginRequest)
        {
            IActionResult response = Unauthorized();

            // Query the database to find the user with the provided EmailId and Password
            var user = Context.Users
           .Where(u => u.Username == loginRequest.Username && u.Password == loginRequest.Password)
           .FirstOrDefault();

            if (user != null)
            {
                var token = GenerateToken();
                response = Ok(new
                {
                    token = token,
                    userId = user.UserId,
                    Data = new
                    {
                        user.EmailId,
                        user.Password
                    }
                });
            }
            return response;
        }

    
        public class Login
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

    }
}
