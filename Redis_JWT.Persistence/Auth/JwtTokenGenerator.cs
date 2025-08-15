using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Redis_JWT.Application.Abstractions;
using Redis_JWT.Domain.Entities;

namespace Redis_JWT.Persistence.Auth
{
    public class JwtTokenGenerator(IConfiguration cfg) : IJwtTokenGenerator
    {
        public string GenerateToken(User user, TimeSpan? lifetime = null)
        {
            var issuer = cfg["Jwt:Issuer"]!; var audience = cfg["Jwt:Audience"]!; var secret = cfg["Jwt:Secret"]!;
            var mins = int.Parse(cfg["Jwt:ExpireMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
              new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
              new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
              new Claim(JwtRegisteredClaimNames.Email, user.Email)
    };
            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(mins), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
