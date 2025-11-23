using System;
using Microsoft.AspNetCore.Http;

namespace eShop.Services.Basket.API.Services {
    internal class IdentityService : IIdentityService {
        private IHttpContextAccessor context;

        public IdentityService(IHttpContextAccessor context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public string GetUserIdentity() {
            return this.context.HttpContext.User.FindFirst("sub").Value;
        }
    }
}