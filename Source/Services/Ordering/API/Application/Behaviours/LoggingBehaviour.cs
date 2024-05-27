
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using EShop.BuildingBlocks.EventBus.EventBus.Extensions;

namespace EShop.Services.Ordering.API.Application.Behaviours {
    internal class LoggingBehaviour<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger) {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
            this.logger.LogInformation($"----- Handling command {request.GetGenericTypeName()} {request}");
            TResponse response = await next();
            this.logger.LogInformation($"----- Command {request.GetGenericTypeName()} handled - response: {response}");

            return response;
        }
    }
}