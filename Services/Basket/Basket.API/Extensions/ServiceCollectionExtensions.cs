
using System;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using eShop.BuildingBlocks.EventBus;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.BuildingBlocks.EventBusRabbitMQ;
using eShop.Services.Basket.API.Infrastructure.Repositories;
using eShop.Services.Basket.API.IntegrationEvents.Events;
using eShop.Services.Basket.API.Model;
using eShop.Services.Basket.API.Services;
using eShop.Services.Basket.API.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace eShop.Services.Basket.API.Extensions {
    internal static class ServiceCollectionExtensions {
        internal static IServiceCollection AddMVC(this IServiceCollection services,
            IConfiguration configuration) {
            services.AddControllers(options => {
                options.SuppressAsyncSuffixInActionNames = true;
            });

            return services;
        }

        internal static IServiceCollection AddRedis(this IServiceCollection services,
            IConfiguration configuration) {
            // By connecting here we are making sure that our service cannot start until
            // Redis is ready. This might slow down startup, but given that there is a
            // delay on resolving the IP address and then creating the connection it
            // seems reasonable to move that cost to Startup instead of having the first 
            // request pay the penalty.
            services.AddSingleton<ConnectionMultiplexer>(x => {
                string connectionString = configuration.GetConnectionString("Redis");
                ConfigurationOptions configurationOptions = ConfigurationOptions.Parse(connectionString, true);

                configurationOptions.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            return services;
        }

        internal static IServiceCollection AddSwagger(this IServiceCollection services,
            IConfiguration configuration) {
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo() {
                    Version = "v1",
                    Title = "BasketService",
                    Description = "An ASP.NET Core Web API for managing a customer's basket"
                });

                string xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
            });

            return services;
        }

        internal static IServiceCollection AddOptions(this IServiceCollection services,
            IConfiguration configuration) {
            services.Configure<BasketSettings>(configuration);

            return services;
        }

        internal static IServiceCollection AddRabbitMQ(this IServiceCollection services,
            IConfiguration configuration) {
            services.AddSingleton<IRabbitMQPersistentConnection>(serviceProvider => {
                BasketSettings settings =
                    serviceProvider.GetRequiredService<IOptions<BasketSettings>>().Value;
                ILogger<DefaultRabbitMQPersistentConnection> logger =
                    serviceProvider.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                ConnectionFactory factory = new ConnectionFactory() {
                    HostName = settings.EventBusConnection
                };

                if (!string.IsNullOrEmpty(settings.EventBusUserName)) {
                    factory.UserName = settings.EventBusUserName;
                }

                if (!string.IsNullOrEmpty(settings.EventBusPassword)) {
                    factory.Password = settings.EventBusPassword;
                }

                int retryCount = 5;
                if (!string.IsNullOrEmpty(settings.EventBusRetryCount)) {
                    retryCount = int.Parse(settings.EventBusRetryCount);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionManager>();

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(serviceProvider => {
                IRabbitMQPersistentConnection connection = serviceProvider.GetRequiredService<IRabbitMQPersistentConnection>();
                ILogger<EventBusRabbitMQ> logger = serviceProvider.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                ILifetimeScope lifetimeScope = serviceProvider.GetRequiredService<ILifetimeScope>();
                IEventBusSubscriptionManager subscriptionManager = serviceProvider.GetRequiredService<IEventBusSubscriptionManager>();

                BasketSettings settings = serviceProvider.GetRequiredService<IOptions<BasketSettings>>().Value;

                int retryCount = 5;
                if (!string.IsNullOrEmpty(settings.EventBusRetryCount)) {
                    retryCount = int.Parse(settings.EventBusRetryCount);
                }

                return new EventBusRabbitMQ(
                    connection,
                    logger,
                    lifetimeScope,
                    subscriptionManager,
                    settings.SubscriptionClientName,
                    retryCount
                );
            });

            return services;
        }

        internal static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services,
            IConfiguration configuration) {
            services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
            services.AddTransient<OrderStartedIntegrationEventHandler>();

            return services;
        }

        internal static IServiceCollection AddHTTPContextAccessor(this IServiceCollection services,
            IConfiguration configuration) {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }

        internal static IServiceCollection AddRepositories(this IServiceCollection services,
            IConfiguration configuration) {
            services.AddTransient<IBasketRepository, RedisBasketRepository>();

            return services;
        }

        internal static IServiceCollection AddIdentityService(this IServiceCollection services,
            IConfiguration configuration) {
            services.AddTransient<IIdentityService, IdentityService>();

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