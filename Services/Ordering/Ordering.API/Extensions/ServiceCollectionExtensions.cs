using System;
using System.Data.Common;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using eShop.BuildingBlocks.EventBus;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.BuildingBlocks.EventBusRabbitMQ;
using eShop.BuildingBlocks.IntegrationEventLogEF;
using eShop.BuildingBlocks.IntegrationEventLogEF.Services;
using eShop.Services.Ordering.API.Application.IntegrationEvents;
using eShop.Services.Ordering.API.Application.IntegrationEvents.EventHandling;
using eShop.Services.Ordering.API.Settings;
using eShop.Services.Ordering.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace eShop.Services.Ordering.API.Extensions {
    internal static class ServiceCollectionExtensions {
        internal static IServiceCollection AddMVC(this IServiceCollection services,
            IConfiguration configuration) {
            services.AddControllers(options => {
                options.SuppressAsyncSuffixInActionNames = true;
            });

            return services;
        }

        internal static IServiceCollection AddDBContext(this IServiceCollection services,
            IConfiguration configuration) {

            services.AddDbContext<OrderingContext>(optionsAction => {
                optionsAction.UseSqlServer(
                    connectionString: configuration.GetConnectionString("SQLServer"),
                    sqlServerOptionsAction: action => {
                        action.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name);
                        action.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        );
                    }
                );
            });

            services.AddDbContext<IntegrationEventLogContext>(optionsAction => {
                optionsAction.UseSqlServer(
                    connectionString: configuration.GetConnectionString("SQLServer"),
                    sqlServerOptionsAction: sqlServerOptionsAction => {
                        sqlServerOptionsAction.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name);
                        sqlServerOptionsAction.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        );
                    }
                );
            });

            return services;
        }

        internal static IServiceCollection AddSwagger(this IServiceCollection services,
            IConfiguration configuration) {
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo() {
                    Version = "v1",
                    Title = "OrderingService",
                    Description = "An ASP.NET Core Web API for managing customer's orders"
                });

                string xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
            });

            return services;
        }

        internal static IServiceCollection AddOptions(this IServiceCollection services,
            IConfiguration configuration) {
            services.Configure<OrderSettings>(configuration);

            return services;
        }

        internal static IServiceCollection AddEventBus(this IServiceCollection services,
            IConfiguration configuration) {

            services.AddSingleton<IRabbitMQPersistentConnection>(serviceProvider => {
                OrderSettings settings = serviceProvider.GetRequiredService<IOptions<OrderSettings>>().Value;
                ConnectionFactory connectionFactory = new ConnectionFactory() {
                    HostName = settings.EventBusConnection
                };
                ILogger<DefaultRabbitMQPersistentConnection> logger =
                    serviceProvider.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                if (!string.IsNullOrEmpty(settings.EventBusUserName)) {
                    connectionFactory.UserName = settings.EventBusUserName;
                }

                if (!string.IsNullOrEmpty(settings.EventBusPassword)) {
                    connectionFactory.Password = settings.EventBusPassword;
                }

                int retryCount = 5;
                if (!string.IsNullOrEmpty(settings.EventBusRetryCount)) {
                    retryCount = int.Parse(settings.EventBusRetryCount);
                }

                return new DefaultRabbitMQPersistentConnection(connectionFactory, logger, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionManager>();

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(serviceProvider => {
                IRabbitMQPersistentConnection connection =
                    serviceProvider.GetRequiredService<IRabbitMQPersistentConnection>();
                ILogger<EventBusRabbitMQ> logger =
                    serviceProvider.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                ILifetimeScope lifettimeScope =
                    serviceProvider.GetRequiredService<ILifetimeScope>();
                IEventBusSubscriptionManager subscriptionManager =
                    serviceProvider.GetRequiredService<IEventBusSubscriptionManager>();

                OrderSettings settings =
                    serviceProvider.GetRequiredService<IOptions<OrderSettings>>().Value;

                int retryCount = 5;
                if (!string.IsNullOrEmpty(settings.EventBusRetryCount)) {
                    retryCount = int.Parse(settings.EventBusRetryCount);
                }

                return new EventBusRabbitMQ(
                    connection,
                    logger,
                    lifettimeScope,
                    subscriptionManager,
                    settings.SubscriptionClientName,
                    retryCount
                );
            });

            return services;
        }

        internal static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services,
            IConfiguration configuration) {
            services.AddTransient<UserCheckoutAcceptedIntegrationEventHandler>();

            return services;
        }

        internal static IServiceCollection AddIntegrationEventServices(this IServiceCollection services,
            IConfiguration configuration) {
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                x => (DbConnection connection) => new IntegrationEventLogService(connection)
            );
            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

            return services;
        }

        internal static IServiceProvider AddAutofacModules(this IServiceCollection services,
            IConfiguration configuration) {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);

            return new AutofacServiceProvider(containerBuilder.Build());
        }
    }
}