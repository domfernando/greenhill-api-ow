using Hangfire.Dashboard;
using System.Text;

namespace Unit.API.Util
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        private const string USERNAME = "admin";
        private const string PASSWORD = "Web!112916";

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Obter header de autorização
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                // Decodificar credenciais
                var encodedCredentials = authHeader["Basic ".Length..];
                var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                var credentials = decodedCredentials.Split(':', 2);

                if (credentials.Length == 2)
                {
                    var username = credentials[0];
                    var password = credentials[1];

                    // Verificar credenciais
                    if (username == USERNAME && password == PASSWORD)
                    {
                        return true;
                    }
                }
            }

            // Se não autenticado, solicitar credenciais
            httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
            httpContext.Response.StatusCode = 401;
            return false;
        }

        //public HangfireAuthorizationFilter(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        //private void SetCookie(string user, DashboardContext context)
        //{
        //    var httpContext = context.GetHttpContext();
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_configuration["HangFire:Token"]);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //        new Claim(ClaimTypes.Name, user),
        //        new Claim(ClaimTypes.Role, "Admin"),
        //        new Claim("SuperAdmin", "true") // Ensure SuperAdmin claim is present and boolean
        //    }),
        //        Expires = DateTime.UtcNow.AddHours(1),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    httpContext.Response.Cookies.Append("advice.token", tokenHandler.WriteToken(token)); // Use the same cookie name as in Authorize
        //}

        //private bool AttachUserToContext(HttpContext context, string token)
        //{
        //    try
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var key = Encoding.ASCII.GetBytes(_configuration["Hangfire:Token"]);

        //        tokenHandler.ValidateToken(token, new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            ClockSkew = TimeSpan.Zero
        //        }, out SecurityToken validatedToken);

        //        var jwtToken = (JwtSecurityToken)validatedToken;

        //        var value = jwtToken.Claims.First(x => x.Type == "SuperAdmin").Value;

        //        if (value.Length > 0)
        //        {
        //            var result = Convert.ToBoolean(value);
        //            return result;
        //        }

        //        return false;
        //    }
        //    catch { return false; }
        //}

        //public bool Authorize(DashboardContext context)
        //{
        //    var httpContext = context.GetHttpContext();

        //    if (httpContext.Request.Cookies.TryGetValue("advice.token", out var token) && !string.IsNullOrEmpty(token))
        //    {
        //        return AttachUserToContext(httpContext, token);
        //    }

        //    return false;
        //}
    }
}
