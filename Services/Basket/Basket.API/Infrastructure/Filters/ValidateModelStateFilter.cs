using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eShop.Services.Basket.API.Infrastructure.Filters {
    public class ValidateModelStateFilter : ActionFilterAttribute {
        public override void OnActionExecuting(ActionExecutingContext context) {
            if (context.ModelState.IsValid) {
                return;
            }

            string[] validationErrors = context.ModelState.Keys
                .SelectMany(x => context.ModelState[x].Errors)
                .Select(x => x.ErrorMessage)
                .ToArray();

            JSONErrorResponse json = new JSONErrorResponse() {
                Messages = validationErrors
            };

            context.Result = new BadRequestObjectResult(json);
        }
    }
}