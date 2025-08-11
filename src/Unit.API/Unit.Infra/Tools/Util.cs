using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Unit.Application.DTOs.Response;
using Unit.Application.Util;
using Unit.Domain.Entities.Acesso;
using Unit.Domain.Entities.Util;

namespace Unit.Infra.Tools
{
    public static class Util
    {
        public static string GenerateCode(int size)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, size)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static Reply ValidateNewPassword(Usuario user)
        {
            var _rtn = new Reply();

            if (user.Pwd.ToLower().Contains(user.Usr))
            {
                _rtn.Validate = false;
                _rtn.Messages.Add("A senha não pode conter o nome do usuario");
            }

            if (user.Pwd.ToLower().Contains("password") ||
                user.Pwd.ToLower().Contains("senha"))
            {
                _rtn.Validate = false;
                _rtn.Messages.Add("A senha não pode conter palavras reservadas");
            }

            return _rtn;
        }

        public static string GenerateHashPassword(string pwd)
        {
            MD5 _md5 = MD5.Create();
            byte[] _input = Encoding.ASCII.GetBytes(pwd);
            byte[] _hash = _md5.ComputeHash(_input);
            StringBuilder _senha = new System.Text.StringBuilder();

            for (int i = 0; i < _hash.Length; i++)
            {
                _senha.Append(_hash[i].ToString("x4"));
            }

            return _senha.ToString();
        }

        public static string GenerateToken(IConfiguration config, UsuarioLoginResponse user)
        {
            #region Old
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(config.GetSection("AppSettings:Secret").Value);

            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new Claim[]
            //    {
            //        new Claim(ClaimTypes.Name, user.Usr.ToString()),
            //        new Claim(ClaimTypes.Role, String.Join(":", user.Perfis))
            //    }),
            //    Expires = DateTime.UtcNow.AddHours(3),
            //    SigningCredentials = new SigningCredentials(
            //        new SymmetricSecurityKey(key),
            //        SecurityAlgorithms.HmacSha256Signature)
            //};

            //var token = tokenHandler.CreateToken(tokenDescriptor);

            //return tokenHandler.WriteToken(token);
            #endregion

            JwtSettings _jwtSettings = new JwtSettings
            {
                SecretKey = config.GetSection("AppSettings:Secret").Value,
                Issuer = config.GetSection("AppSettings:AppName").Value,
                Audience = $"{config.GetSection("AppSettings:AppName").Value}-clients",
                ExpirationMinutes = Convert.ToInt32(config.GetSection("AppSettings:DuracaoToken").Value)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(ClaimTypes.Name, user.Usr),
            new Claim(ClaimTypes.Email, user.Email),
            //new Claim(ClaimTypes.Role, user.Role),
            new Claim("jti", Guid.NewGuid().ToString()),
            new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: credentials
            );

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }

        public static ConfigAccess GetConfigAccess(IConfiguration config)
        {
            ConfigAccess _configuration = new ConfigAccess
            {
                TamanhoSenha = config.GetSection("AppSettings:TamanhoSenha").Value != null ?
                               Convert.ToInt32(config.GetSection("AppSettings:TamanhoSenha").Value) : 6,
                LoginEmail = config.GetSection("AppSettings:LoginEmail").Value != null
                                        ? Convert.ToBoolean(config.GetSection("AppSettings:LoginEmail").Value) : false,
                ValidaEmail = config.GetSection("AppSettings:ValidaEmail").Value != null
                                        ? Convert.ToBoolean(config.GetSection("AppSettings:ValidaEmail").Value) : false,
                BloquearTentativaAcesso = config.GetSection("AppSettings:BloquearTentativaAcesso").Value != null
                                        ? Convert.ToBoolean(config.GetSection("AppSettings:BloquearTentativaAcesso").Value) : false,
                TentativaAcesso = config.GetSection("AppSettings:TentativaAcesso").Value != null
                                ? Convert.ToInt32(config.GetSection("AppSettings:TentativaAcesso").Value) : 3,
                DuracaoToken = config.GetSection("AppSettings:TentativaAcesso").Value != null
                                ? Convert.ToInt32(config.GetSection("AppSettings:DuracaoToken").Value) : 120,
                DuracaoTokenMfa = config.GetSection("AppSettings:DuracaoTokenMfa").Value != null
                                ? Convert.ToInt32(config.GetSection("AppSettings:DuracaoTokenMfa").Value) : 5,
            };

            return _configuration;
        }
    }
}
