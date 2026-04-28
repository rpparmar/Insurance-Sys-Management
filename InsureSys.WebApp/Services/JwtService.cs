using InsuranceSys.Domain.Entities;
using InsuranceSys.Domain.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Insurancesys.web.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(AgencyUsersEntity user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
            var issuer = jwtSettings["Issuer"] ?? "InsureSysManagement";
            var audience = jwtSettings["Audience"] ?? "InsureSysManagement";
            var expiryMinutes = int.TryParse(jwtSettings["ExpiryMinutes"], out var mins) ? mins : 60;

            var roleName = Enum.GetName(typeof(Roles), user.Role) ?? string.Empty;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("DisplayName", user.DisplayName ?? user.Username)
            };

            if (user.AgencyId.HasValue)
                claims.Add(new Claim("AgencyId", user.AgencyId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
