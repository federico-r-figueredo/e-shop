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
using eShop.Services.Catalog.API.Controllers.gRPC;
using eShop.Services.Catalog.API.Infrastructure;
using eShop.Services.Catalog.API.IntegrationEvents;
using eShop.Services.Catalog.API.IntegrationEvents.EventHandling;
using eShop.Services.Catalog.API.IntegrationEvents.Events;
using eShop.Services.Catalog.API.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace eShop.Services.Catalog.API {
    public class Startup {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration) {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services) {
            services.AddControllers(options => {
                options.SuppressAsyncSuffixInActionNames = true;
            });
            services.AddGrpc();
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo {
                    Version = "v1",
                    Title = "Catalog Service",
                    Description = "An ASP.NET Core Web API for managing a catalog of products"
                });

                string xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
            });
            services.AddDbContext<CatalogContext>(options => {
                options.UseSqlServer(
                    configuration.GetConnectionString("SQLServer"),
                    action => {
                        action.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name);
                        action.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        );
                    }
                );
            });
            services.AddDbContext<IntegrationEventLogContext>(options => {
                options.UseSqlServer(
                    configuration.GetConnectionString("SQLServer"),
                    action => {
                        action.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name);
                        action.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        );
                    }
                );
            });
            services.Configure<CatalogSettings>(configuration);
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                x => (DbConnection connection) => new IntegrationEventLogService(connection)
            );
            services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();
            services.AddSingleton<IRabbitMQPersistentConnection>(x => {
                CatalogSettings settings = x.GetRequiredService<IOptions<CatalogSettings>>().Value;
                ILogger<DefaultRabbitMQPersistentConnection> logger =
                    x.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                ConnectionFactory factory = new ConnectionFactory() {
                    HostName = configuration["EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"])) {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"])) {
                    factory.UserName = configuration["EventBusPassword"];
                }

                int retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"])) {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
            services.AddSingleton<IEventBus, EventBusRabbitMQ>(serviceProvider => {
                IRabbitMQPersistentConnection connection = serviceProvider.GetRequiredService<IRabbitMQPersistentConnection>();
                ILifetimeScope lifetimeScope = serviceProvider.GetRequiredService<ILifetimeScope>();
                ILogger<EventBusRabbitMQ> logger = serviceProvider.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                IEventBusSubscriptionManager subscriptionManager = serviceProvider.GetRequiredService<IEventBusSubscriptionManager>();

                int retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"])) {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(
                    connection, logger, lifetimeScope, subscriptionManager,
                    configuration["SubscriptionClientName"], retryCount
                );
            });

            services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionManager>();
            services.AddTransient<ProductPriceChangedIntegrationEventHandler>();

            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);

            return new AutofacServiceProvider(containerBuilder.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options => {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapGrpcService<CatalogService>();
            });

            IEventBus eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
        }
    }
}