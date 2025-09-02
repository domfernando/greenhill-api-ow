using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using Unit.Application;
using Unit.Infra;

var builder = WebApplication.CreateBuilder(args);

// Configurar cultura para português
var culture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

#region Swagger

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Digite 'Bearer' seguido de um espaço e o token JWT"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

});

#endregion

// Application and Infra dependencies
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfra(builder.Configuration);

// Register FluentValidation (Nova forma - sem pacote depreciado)
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Configurar mensagens FluentValidation em português
//ValidatorOptions.Global.LanguageManager.Culture = culture;

//// Model validation error handling
//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.InvalidModelStateResponseFactory = context =>
//    {
//        var errors = context.ModelState
//            .Where(x => x.Value.Errors.Count > 0)
//            .ToDictionary(
//                kvp => kvp.Key,
//                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
//            );
//        return new BadRequestObjectResult(new
//        {
//            Success = false,
//            Message = "Dados inválidos",
//            Errors = errors
//        });
//    };
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJS", policy =>
    {
        policy.WithOrigins("https://localhost:3040", "http://localhost:3040","https://greenhill.ajudador.com.br", "http://greenhill.ajudador.com.br")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowNextJS");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware de localização
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = new[] { culture },
    SupportedUICultures = new[] { culture }
});

app.UseMiddleware<Unit.API.Middleware.ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();