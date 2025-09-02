using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Reflection;
using Unit.Application.Base;
using Unit.Application.Services;
using Unit.Application.Sevices;
using Unit.Application.Util;
using Unit.Infra.Persistence.Context;
using Unit.Infra.Repositories;
using Unit.Infra.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Unit.Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
        {
            AddPersistence(services, configuration);

            services.AddHttpClient<IHttpClient, HttpClientService>();
            services.AddScoped<IEvolutionService, EvolutionService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IPerfilService, PerfilService>();
            services.AddScoped<IServicoService, ServicoService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IQueueService, QueueService>();
            services.AddScoped<IMensagemService, MensagemService>();
            services.AddScoped<IPapelService, PapelService>();
            services.AddScoped<IPessoaService, PessoaService>();
            services.AddScoped<IEnderecoService, EnderecoService>();
            services.AddScoped<ICongService, CongService>();
            services.AddScoped<IPubService, PubService>();
            services.AddScoped<IGrupoService, GrupoService>();
            services.AddScoped<IViaCepService, ViaCepService>();
            services.AddScoped<IOradorTemaService, OradorTemaService>();
            services.AddScoped<ITemaService, TemaService>();
            services.AddScoped<IArranjoService, ArranjoService>();
            services.AddScoped<IDiscursoService, DiscursoService>();
            services.AddScoped<IEventoService, EventoService>();
            services.AddScoped<IRelatorioService, RelatorioService>();

            AddAuthorization(services, configuration);

            return services;
        }

        private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationAssembly = typeof(ApplicationId)
                                    .GetTypeInfo().Assembly.GetName().Name;
            var serverVersion = new MariaDbServerVersion(new Version(10, 3, 37));
            services.AddDbContext<DatabaseContext>(opt =>
            {
                opt.UseMySql(configuration.GetConnectionString("Default"),
                             serverVersion,
                             sql => sql.MigrationsAssembly("Unit.API").EnableRetryOnFailure())
                   .LogTo(Console.WriteLine, LogLevel.Information)
                   .EnableDetailedErrors();
            });

            services.AddScoped<DatabaseContext, DatabaseContext>();
        }

        private static void AddAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings
            {
                SecretKey = configuration.GetSection("AppSettings:Secret").Value,
                Issuer = configuration.GetSection("AppSettings:AppName").Value,
                Audience = $"{configuration.GetSection("AppSettings:AppName").Value}-clients",
                ExpirationMinutes = Convert.ToInt32(configuration.GetSection("AppSettings:DuracaoToken").Value)
            };

            services.AddSingleton(jwtSettings);

            // Configurar autenticação JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ClockSkew = TimeSpan.Zero // Remove delay padrão de 5 minutos
                    };
                });

            services.AddAuthorization();
        }
    }
}
