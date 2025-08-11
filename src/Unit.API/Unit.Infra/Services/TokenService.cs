using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Unit.Application.DTOs.Response;
using Unit.Application.Sevices;

namespace Unit.Infra.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<string> GenerateToken(UsuarioResponse user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _configuration.GetSection("AppSettings:Secret").Value;
            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("Secret key not configured.");

            var key = Encoding.UTF8.GetBytes(secret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Usr ?? string.Empty),
            };

            if (user.Perfis != null && user.Perfis.Any())
            {
                foreach (var perfil in user.Perfis)
                {
                    claims.Add(new Claim(ClaimTypes.Role, perfil.PerfilNome ?? string.Empty));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Task.FromResult(tokenHandler.WriteToken(token));
        }
    }
}
