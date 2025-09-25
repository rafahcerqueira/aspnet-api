using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using minimal_api.Domain.Entities;
using minimal_api.Infrastructure.Interfaces;

namespace minimal_api.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _key;

        public JwtService(IConfiguration configuration)
        {
            _key = configuration.GetValue<string>("Jwt") ?? "keysecret";
        }

        public string GenerateToken(Administrator admin)
        {
            if (admin is null) throw new ArgumentNullException(nameof(admin));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, admin.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, admin.Profile ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
