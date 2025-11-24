using System.IO;
using eShop.Services.Basket.API.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace eShop.Services.Basket.API {
    public static class Program {
        private static readonly string @namespace = typeof(Program).Namespace;
        private static readonly string applicationName = @namespace.Substring(
            @namespace.LastIndexOf('.', @namespace.LastIndexOf('.') - 1) + 1
        );

        public static string ApplicationName {
            get { return applicationName; }
        }

        public static void Main(string[] arguments) {
            CreateWebHost(arguments).Run();
        }

        public static IWebHost CreateWebHost(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAzureKeyVault()
                .CaptureStartupErrors(false)
                .UseStartup<Startup>()
                .Build();
        }
    }
}
