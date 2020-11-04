using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using UsersAdmin.Models;
using UsersAdmin.Controllers.DTOs;

namespace UsersAdmin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AdminDbContext _DbContext;

        public AuthController(AdminDbContext DbContext) {
            _DbContext = DbContext;
        }

        [HttpPost("/login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            var identity = GetIdentity(loginDTO.Login, loginDTO.Password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }
 
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
 
            var response = new
            {
                token = encodedJwt,
                username = identity.Name
            };
 
            return Ok(response);
        }

        private ClaimsIdentity GetIdentity(string login, string password)
        {
            User user = _DbContext.User.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
                return null;

            UserRole role = _DbContext.UserRole.Find(user.Id, 1);
            if (role == null)
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
            };
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }        
    }
}
