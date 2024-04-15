
using Microsoft.Extensions.DependencyInjection;

namespace EShop.BuildingBlocks.Devspaces.Support {
    public static class ServiceCollectionDevspacesExtensions {
        public static IServiceCollection AddDevspaces(this IServiceCollection services) {
            services.AddTransient<DevspacesMessageHandler>();
            return services;
        }
    }
}