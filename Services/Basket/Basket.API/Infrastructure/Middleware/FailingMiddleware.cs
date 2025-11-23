using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using eShop.Services.Basket.API.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Basket.API.Infrastructure.Middleware {
    internal class FailingMiddleware {
        private readonly RequestDelegate next;
        private bool mustFail;
        private readonly FailingOptions options;
        private readonly ILogger<FailingMiddleware> logger;

        public FailingMiddleware(RequestDelegate next, FailingOptions options,
            ILogger<FailingMiddleware> logger) {
            this.next = next;
            this.mustFail = false;
            this.options = options;
            this.logger = logger;
        }

        internal async Task Invoke(HttpContext httpContext) {
            string resourcePath = httpContext.Request.Path;
            if (resourcePath.Equals(this.options.ConfigPath, StringComparison.OrdinalIgnoreCase)) {
                await this.ProcessConfigRequest(httpContext);
                return;
            }

            if (this.MustFail(httpContext)) {
                this.logger.LogInformation("Response for path {Path} will fail", resourcePath);
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "text/plain";
                await httpContext.Response.WriteAsync("Failed due to FailingMiddleware enabled.");
                return;
            }

            await this.next.Invoke(httpContext);
        }

        private async Task ProcessConfigRequest(HttpContext httpContext) {
            bool isEnabled = httpContext.Request.Query.Keys.Any(x => x == "enable");
            bool isDisabled = httpContext.Request.Query.Keys.Any(x => x == "disable");

            if (isEnabled && isDisabled) {
                throw new ArgumentException("Must use enable or disable query string values, but not both");
            }

            if (isDisabled) {
                this.mustFail = false;
                await SendOkResponse(httpContext, "FailingMiddleware disabled. Further requests will be processed.");
                return;
            }

            if (isEnabled) {
                this.mustFail = true;
                await SendOkResponse(httpContext, "FailingMiddleware enabled. Further request will return HTTP 500");
                return;
            }

            // If reach here, that means that no valid parameter has been passed. Just output status.
            await SendOkResponse(httpContext, string.Format("FailingMiddleware is {0}", this.mustFail ? "enabled" : "disabled"));
        }

        private async Task SendOkResponse(HttpContext httpContext, string message) {
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            httpContext.Response.ContentType = "text/plain";
            await httpContext.Response.WriteAsync(message);
        }

        private bool MustFail(HttpContext httpContext) {
            string resourcePath = httpContext.Request.Path.Value;

            if (this.options.NotFilteredPaths.Any(
                x => x.Equals(resourcePath, StringComparison.InvariantCultureIgnoreCase)
            )) {
                return false;
            }

            return this.mustFail &&
                (this.options.EndpointPaths.Any(x => x == resourcePath) ||
                 this.options.EndpointPaths.Count == 0);
        }
    }
}