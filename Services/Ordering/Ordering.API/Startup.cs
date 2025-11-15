using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eShop.Services.Ordering.API {
    public class Startup {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration) {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the
        // IoC container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTPS
        // request processing pipeline.
        public void Configure(IApplicationBuilder applcation, IWebHostEnvironment environment) {
            // Configure the HTTP request pipeline.
            if (environment.IsDevelopment()) {
                applcation.UseDeveloperExceptionPage();
                applcation.UseSwagger();
                applcation.UseSwaggerUI();
            }

            applcation.UseRouting();
            applcation.UseAuthorization();
            applcation.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}