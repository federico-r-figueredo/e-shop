using System;
using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EShop.Services.Ordering.Infrastructure;
using EShop.Services.Ordering.Infrastructure.Repositories;
using EShop.Services.Ordering.Domain.SeedWork;

namespace EShop.Services.Ordering.API {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddDbContext<OrderingContext>(dbContextOptions => {
                dbContextOptions.UseSqlServer(
                    Configuration.GetConnectionString("SQLServer"),
                    sqlServerOptions => {
                        sqlServerOptions.MigrationsAssembly("Infrastructure");
                        sqlServerOptions.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        );
                    }
                );
            });
            services.AddMediatR(typeof(Program).Assembly);
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBuyerRepository, BuyerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
