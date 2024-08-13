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
            string RoleName = await _roleService.GetRoleNameByIdAsync(user.IdRole);

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
        public async Task<string> GenerateBeneficiaryToken(Beneficiary beneficiary)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var NomComplet = $"{beneficiary.Nom} {beneficiary.Prenom}";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, beneficiary.Id.ToString()),
                new Claim(ClaimTypes.Name,NomComplet),
                new Claim("Has_gochap",beneficiary.Has_gochap.ToString()),
            }),
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
        public List<KeyValuePair<string, string>> ParseJwtToken(string token)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var jwtToken = tokenHandler.ReadJwtToken(token);

    var claims = jwtToken.Claims.Select(claim =>
        new KeyValuePair<string, string>(claim.Type, claim.Value)).ToList();

    return claims;
}
public bool ValidateTokenWithoutExpiration(string token)
{
    try
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenHandler = new JwtSecurityTokenHandler();

        // Paramètres de validation
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // Désactiver la validation de la date d'expiration
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        // Valider le token
        tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

        // Si le token est valide
        return true;
    }
    catch
    {
        // Si le token n'est pas valide
        return false;
    }
}
public bool ValidateTokenWithExpiration(string token)
{
    try
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenHandler = new JwtSecurityTokenHandler();

        // Paramètres de validation
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true, // Activer la validation de la date d'expiration
            ClockSkew = TimeSpan.Zero, // Désactiver la tolérance par défaut (5 minutes)
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        // Valider le token
        tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

        // Si le token est valide
        return true;
    }
    catch
    {
        // Si le token n'est pas valide ou est expiré
        return false;
    }
}


    }


}
