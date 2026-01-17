using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using FluentValidation;
using eShop.BuildingBlocks.EventBus.Extensions;
using eShop.Services.Ordering.Domain.Exceptions;

namespace eShop.Services.Ordering.API.Application.Behaviours {
    internal class ValidatorBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse> {

        private readonly IValidator<TRequest>[] validators;
        private readonly ILogger<ValidatorBehaviour<TRequest, TResponse>> logger;

        public ValidatorBehaviour(IValidator<TRequest>[] validators,
            ILogger<ValidatorBehaviour<TRequest, TResponse>> logger) {
            this.validators = validators;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next) {

            string typeName = request.GetGenericTypeName();

            this.logger.LogInformation("----- Validating command {CommandType}", typeName);

            var failures = this.validators
                .Select(validator => validator.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Any()) {
                this.logger.LogWarning(
                    "Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}",
                    typeName,
                    request,
                    failures
                );

                throw new OrderingDomainException(
                    $"Command validation errors for type {typeof(TRequest).Name}",
                    new ValidationException("Validation exception", failures)
                );
            }

            return await next();
        }
    }
}