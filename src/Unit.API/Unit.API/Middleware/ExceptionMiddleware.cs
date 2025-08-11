using System.Text.Json;

namespace Unit.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            object response = exception switch
            {
                KeyNotFoundException => new { StatusCode = 404, exception.Message },
                FluentValidation.ValidationException validationEx => new
                {
                    StatusCode = 400,
                    Message = "Erro de validação",
                    Errors = validationEx.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                },
                _ => new { StatusCode = 500, Message = "Erro interno do servidor" }
            };

            context.Response.StatusCode = (int)response.GetType().GetProperty("StatusCode")!.GetValue(response)!;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}