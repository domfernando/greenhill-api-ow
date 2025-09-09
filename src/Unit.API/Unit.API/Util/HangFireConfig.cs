using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Unit.API.Util
{
    public static class CronExpressions
    {
        public const string TODO_DIA_1_AS_1H = "0 1 1 * *";
        public const string TODO_DIA_5_AS_8H = "0 8 5 * *";
        public const string TODO_DIA_8_AS_8H = "0 8 8 * *";
        public const string TODO_DIA_10_AS_8H = "0 8 10 * *";
    }
    public static class HangFireConfig
    {
        public static IServiceCollection AddHangfireConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHangFire(configuration);

            return services;
        }
        private static void AddHangFire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseStorage(new MySqlStorage(configuration.GetConnectionString("Default"), new MySqlStorageOptions
                    {
                        TablesPrefix = configuration["HangFire:TablePrefix"],
                        PrepareSchemaIfNecessary = true
                    })));

            services.AddHangfireServer();
        }

        public static void UsehangfireConfiguration(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            var jobOptions = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")
            };

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            //app.UseHangfireDashboard("/hangfire");

            //app.UseHangfireDashboard("/hangfire", new DashboardOptions
            //{
            //    Authorization = new[] { new HangfireAuthorizationFilter(app.ApplicationServices.GetRequiredService<IConfiguration>()) }
            //});
        }

        private static void Jobs(RecurringJobOptions jobOptions)
        {
            void RegistrarJob<T>(string id, Expression<Action<T>> metodo, string cronExpression)
            {
                RecurringJob.RemoveIfExists(id);
                RecurringJob.AddOrUpdate<T>(id, metodo, cronExpression, jobOptions);
            }

            #region Jobs

            //RegistrarJob<IQueueService>(
            //    "ProcessarFilaEnvioEmail",
            //    service => service.ProcessarFilaEnvioEmail(),
            //    "*/5 * * * *"
            //);

            #endregion
        }
    }
}