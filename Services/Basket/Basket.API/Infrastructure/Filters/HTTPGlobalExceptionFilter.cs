using System.Net;
using eShop.Services.Basket.API.Infrastructure.ActionResults;
using eShop.Services.Basket.API.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Basket.API.Infrastructure.Filters {
    public class HTTPGlobalExceptionFilter : IExceptionFilter {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<HTTPGlobalExceptionFilter> logger;

        public HTTPGlobalExceptionFilter(IWebHostEnvironment webHostEnvironment,
            ILogger<HTTPGlobalExceptionFilter> logger) {
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context) {
            this.logger.LogError(
                new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message
            );

            if (context.Exception.GetType() == typeof(BasketDomainException)) {
                JSONErrorResponse json = new JSONErrorResponse() {
                    Messages = new[] { context.Exception.Message }
                };

                context.Result = new BadRequestObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            } else {
                JSONErrorResponse json = new JSONErrorResponse() {
                    Messages = new[] { "An error occurred. Try it again." }
                };

                if (webHostEnvironment.IsDevelopment()) {
                    json.DeveloperMessage = context.Exception;
                }

                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            context.ExceptionHandled = true;
        }
    }
}