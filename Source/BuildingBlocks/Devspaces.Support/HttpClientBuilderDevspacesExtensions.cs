
using Microsoft.Extensions.DependencyInjection;

namespace EShop.BuildingBlocks.Devspaces.Support {
    public static class HttpClientBuilderDevspacesExtensions {
        public static IHttpClientBuilder AddDevspacesSupprt(this IHttpClientBuilder builder) {
            builder.AddHttpMessageHandler<DevspacesMessageHandler>();
            return builder;
        }
    }
}