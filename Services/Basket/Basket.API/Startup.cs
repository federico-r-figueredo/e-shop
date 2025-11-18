using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eShop.Services.Basket.API {
    public class Startup {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration) {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services
        // to the IoC container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime.
        // Use this method to configure the HTTP
        // request processing pipeline.
        public void Configure(IApplicationBuilder builder, IWebHostEnvironment environment) {
            if (environment.IsDevelopment()) {
                builder.UseDeveloperExceptionPage();
                builder.UseSwagger();
                builder.UseSwaggerUI();
            }

            builder.UseRouting();
            builder.UseAuthorization();
            builder.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}