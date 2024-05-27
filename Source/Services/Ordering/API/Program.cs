using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EShop.Services.Ordering.Infrastructure;

namespace EShop.Services.Ordering.API {
    public static class Program {
        public static async Task Main(string[] args) {
            IHost host = CreateHostBuilder(args).Build();

            using (IServiceScope scope = host.Services.CreateScope()) {
                OrderingContext dbContext = scope.ServiceProvider.GetRequiredService<OrderingContext>();
                await dbContext.Database.MigrateAsync();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
