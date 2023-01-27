using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Demo.Authentication.Data;
using Demo.Authentication.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Demo.Authentication.Controllers
{
    [ApiController]
    [Route("token")]
    [AllowAnonymous]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly Database _context;

        public TokenController(IConfiguration config, Database context)
        {
            _configuration = config;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(AuthDto _userData)
        {

            // todo Проверяем пароль
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                /*
                new Claim("UserId", user.UserId.ToString()),
                new Claim("DisplayName", user.DisplayName),
                new Claim("UserName", user.UserName),
                new Claim("Email", user.Email)
                */
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);

            return Ok(new TokenDto
            {
                IdToken = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
    
    public class TokenDto
    {
        public String IdToken { get; set; }    
    }
}
