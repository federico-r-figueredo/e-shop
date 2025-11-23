using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Services.Basket.API.Infrastructure.ActionResults {
    public class InternalServerErrorObjectResult : ObjectResult {
        public InternalServerErrorObjectResult(object value) : base(value) {
            this.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}