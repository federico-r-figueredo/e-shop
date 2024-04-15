using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EShop.BuildingBlocks.Devspaces.Support {
    internal class DevspacesMessageHandler : DelegatingHandler {
        private const string DEVSPACES_HEADER_NAME = "azds-route-us";
        private readonly IHttpContextAccessor httpContextAccessor;

        public DevspacesMessageHandler(IHttpContextAccessor httpContextAccessor) {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpResponseMessage, CancellationToken cancellationToken) {
            HttpRequest httpRequest = this.httpContextAccessor.HttpContext.Request;

            if (httpRequest.Headers.ContainsKey(DEVSPACES_HEADER_NAME)) {
                httpRequest.Headers.Add(DEVSPACES_HEADER_NAME, httpRequest.Headers[DEVSPACES_HEADER_NAME]);
            }

            return base.SendAsync(httpResponseMessage, cancellationToken);
        }
    }
}