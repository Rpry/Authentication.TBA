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
                
                new Claim("UserId", Guid.NewGuid().ToString()),
                new Claim("Email", "userEmail"),
                new Claim("age", "2"),
                new Claim("age", "3")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signingCredentials);

            return Ok(new TokenDto
            {
                IdToken = new JwtSecurityTokenHandler().WriteToken(token),
            });
        }
    }
    
    public class TokenDto
    {
        public String IdToken { get; set; }
    }
}
