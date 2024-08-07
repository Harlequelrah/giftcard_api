using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using giftcard_api.Models;
using giftcard_api.Data;
namespace giftcard_api.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IRoleService _roleService;

        public JwtService(IConfiguration configuration, ApplicationDbContext context,IRoleService roleService)
        {
            _configuration = configuration;
            _context = context;
            _roleService = roleService;
        }
        public async Task<string> GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            string RoleName = user.IdRole==null? "UTILISATEUR" : await _roleService.GetRoleNameByIdAsync(user.IdRole);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, RoleName),
                new Claim("IsActive", user.IsActive.ToString())
            }),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public void SaveRefreshToken(User user, string refreshToken)
        {
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Durée de validité du refresh token
            user.RefreshToken = refreshToken;
            _context.SaveChanges();
        }
    }

}
