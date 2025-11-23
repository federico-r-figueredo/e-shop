using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace eShop.Services.Basket.API {
    public static class Program {
        private static readonly string @namespace = typeof(Program).Namespace;
        private static readonly string applicationName = @namespace.Substring(
            @namespace.LastIndexOf('.', @namespace.LastIndexOf('.') - 1) + 1
        );

        public static string ApplicationName {
            get { return applicationName; }
        }

        public static void Main(string[] args) {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}
