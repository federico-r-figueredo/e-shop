using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using eShop.BuildingBlocks.EventBus.Extensions;

namespace eShop.Services.Ordering.API.Application.Behaviours {
    internal class LoggingBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse> {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger) {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next) {
            this.logger.LogInformation(
                "----- Handling command {CommandName} ({@Command})",
                request.GetGenericTypeName(),
                request
            );

            TResponse response = await next();

            this.logger.LogInformation(
                "----- Command {CommandName} handled - response: ({@Response})",
                request.GetGenericTypeName(),
                response
            );

            return response;
        }
    }
}