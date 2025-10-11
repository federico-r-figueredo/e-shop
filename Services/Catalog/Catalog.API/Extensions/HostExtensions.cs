using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace eShop.Services.Catalog.API.Extensions {
    internal static class HostExtensions {
        internal static bool IsInKubernetes(this IHost host) {
            IConfiguration configuration = host.Services.GetService<IConfiguration>();
            string orchestatorType = configuration.GetValue<string>("OrchestatorType");
            return orchestatorType?.ToUpper() == "K8S";
        }

        internal static IHost MigrateDbContext<TContext>(this IHost host,
            Action<TContext, IServiceProvider> seeder) where TContext : DbContext {

            bool isUnderK8S = host.IsInKubernetes();

            using (var scope = host.Services.CreateScope()) {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                ILogger<TContext> logger = serviceProvider.GetService<ILogger<TContext>>();
                TContext context = serviceProvider.GetService<TContext>();

                try {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

                    if (isUnderK8S) {
                        InvokeSeeder(seeder, context, serviceProvider);
                    } else {
                        RetryPolicy retryPolicy = Policy.Handle<SqlException>()
                            .WaitAndRetry(new TimeSpan[] {
                                TimeSpan.FromSeconds(3),
                                TimeSpan.FromSeconds(5),
                                TimeSpan.FromSeconds(8)
                            });

                        // If the SQL Server container is not created on running `docker-compose up`,
                        // this migration can't fail for network related exceptions. The retry options
                        // for DbContext only apply to transient exceptions. Note that this is NOT
                        // applied when running some orchestrators (let the orchestrator to recreate
                        // the failing service).
                        retryPolicy.Execute(() => InvokeSeeder(seeder, context, serviceProvider));
                    }
                } catch (Exception exception) {
                    logger.LogError(exception, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                    if (isUnderK8S) {
                        throw; // Rethrow under k8s because we rely on k8s to re-run the pod.
                    }
                }
            }

            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder,
            TContext context, IServiceProvider services) where TContext : DbContext {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}