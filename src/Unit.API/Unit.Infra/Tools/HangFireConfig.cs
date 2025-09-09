//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Hangfire;
//using Hangfire.MySql;
//using Microsoft.AspNetCore.Builder;
//{
//    public static class HangFireConfig
//    {
//        public static void AddHangFireConfiguration(this IServiceCollection services, IConfiguration configuration)
//        {
//            if (services == null) throw new ArgumentNullException(nameof(services));

//            // Configuração do Hangfire usando MySQL
//            var connectionString = configuration.GetConnectionString("Default");
//            var tablePrefix = configuration["HangFire:Prefix"] ?? "Hangfire";
            
//            services.AddHangFireConfiguration(config =>
//                config.UseStorage(
//                    new MySqlStorage(
//                        connectionString,
//                        new MySqlStorageOptions
//                        {
//                            TablePrefix = tablePrefix,
//                            QueuePollInterval = TimeSpan.FromSeconds(15),
//                            JobExpirationCheckInterval = TimeSpan.FromHours(1),
//                            CountersAggregateInterval = TimeSpan.FromMinutes(5),
//                            PrepareSchemaIfNecessary = true
//                        }
//                    )
//                )
//            );
//            services.AddHangfireServer();
//        }

//        public static void UseHangFireDashboard(this IApplicationBuilder app)
//        {
//            // Adiciona o dashboard do Hangfire em /hangfire
//            app.UseHangfireDashboard("/hangfire");
//        }
//    }
//}
