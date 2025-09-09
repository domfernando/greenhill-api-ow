using FluentValidation;
using Hangfire;
using Hangfire.MySql;
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
using Unit.Infra.Tools;
using Unit.API.Util;
using Microsoft.AspNetCore.SignalR;
using Unit.Infra.Services;

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

builder.Services.AddHangfireConfiguration(builder.Configuration);

// Register FluentValidation (Nova forma - sem pacote depreciado)
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

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

builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30); // tempo máximo sem ping antes de considerar cliente inativo
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);    // frequência que o servidor manda ping
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);     // tempo máximo para handshake inicial
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

app.UsehangfireConfiguration();

app.UseMiddleware<Unit.API.Middleware.ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<MySocketService>("/ws").RequireCors("AllowNextJS");

app.Run();