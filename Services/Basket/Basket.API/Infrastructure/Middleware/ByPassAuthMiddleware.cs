using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace eShop.Services.Basket.API.Infrastructure.Middleware {
    internal class ByPassAuthMiddleware {
        private readonly RequestDelegate next;
        private string currentUserID;

        internal ByPassAuthMiddleware(RequestDelegate next) {
            this.next = next;
            this.currentUserID = null;
        }

        public async Task Invoke(HttpContext httpContext) {
            string resourcePath = httpContext.Request.Path;
            if (resourcePath == "/noauth") {
                string userID = httpContext.Request.Query["userid"];
                if (!string.IsNullOrEmpty(userID)) {
                    this.currentUserID = userID;
                }
                httpContext.Response.StatusCode = 200;
                httpContext.Response.ContentType = "text/string";
                await httpContext.Response.WriteAsync($"User set to {this.currentUserID}");
                return;
            }

            if (resourcePath == "/noauth/reset") {
                this.currentUserID = null;
                httpContext.Response.StatusCode = 200;
                httpContext.Response.ContentType = "text/string";
                await httpContext.Response.WriteAsync($"User set to none. Token required for protected endpoints");
                return;
            }

            string currentUserID = this.currentUserID;
            StringValues authorizationHeader = httpContext.Request.Headers["Authorization"];
            if (authorizationHeader != StringValues.Empty) {
                string header = authorizationHeader.FirstOrDefault();
                if (!string.IsNullOrEmpty(header) && header.StartsWith("Email ") && header.Length > "Email ".Length) {
                    currentUserID = header.Substring("Email ".Length);
                }
            }

            if (!string.IsNullOrEmpty(currentUserID)) {
                ClaimsIdentity claimsIdentities = new ClaimsIdentity(new[] {
                    new Claim("emails", currentUserID),
                    new Claim("name", "Test User"),
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "User"),
                    new Claim("sub", currentUserID),
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "Microsoft")
                }, "ByPassAuth");

                httpContext.User = new ClaimsPrincipal(claimsIdentities);
            }

            await this.next.Invoke(httpContext);
        }
    }
}