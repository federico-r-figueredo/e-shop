using System.Net;
using eShop.Services.Catalog.API.Infrastructure.ActionResults;
using eShop.Services.Catalog.API.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Catalog.API.Infrastructure.Filters {
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

            if (context.Exception.GetType() == typeof(CatalogDomainException)) {
                ValidationProblemDetails validationProblemDetails =
                    new ValidationProblemDetails() {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };

                validationProblemDetails.Errors.Add(
                    "DomainValidations",
                    new string[] { context.Exception.Message.ToString() }
                );

                context.Result = new BadRequestObjectResult(validationProblemDetails);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            } else {
                JsonErrorResponse json = new JsonErrorResponse() {
                    Messages = new[] { "An error occurred." }
                };

                if (webHostEnvironment.IsDevelopment()) {
                    json.DeveloperMessage = context.Exception;
                }

                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        private class JsonErrorResponse {
            public string[] Messages { get; set; }
            public object DeveloperMessage { get; set; }
        }
    }
}