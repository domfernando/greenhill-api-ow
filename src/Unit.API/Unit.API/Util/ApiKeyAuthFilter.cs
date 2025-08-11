using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class ApiKeyAuthFilter : IAsyncActionFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeyAuthFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("API Key não encontrada");
            return;
        }

        var apiKey = _configuration.GetValue<string>("ApiKey");

        if (!apiKey.Equals(extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("Acesso não autorizado");
            return;
        }

        await next();
    }
}
