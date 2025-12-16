using eShop.BuildingBlocks.EventBus.Extensions;
using eShop.Services.Ordering.Infrastructure.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using System.Threading;
using System.Threading.Tasks;

namespace eShop.Services.Ordering.API.Application.Commands {
    /// <summary>
    /// Provides a base implementation for handling duplicate requests and ensuring
    /// idempotent updates, in the cases where a requestID sent by the client is used
    /// to detect duplicate requests.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command handler that performs the operation
    /// if request is not duplicated.
    /// </typeparam>
    /// <typeparam name="TResponse">Return value of the innner command handler</typeparam>
    public class IdentifiedCommandHandler<TCommand, TResponse>
        : IRequestHandler<IdentifiedCommand<TCommand, TResponse>, TResponse>
        where TCommand : IRequest<TResponse> {

        private readonly IMediator mediator;
        private IRequestManager requestManager;
        private readonly ILogger<IdentifiedCommandHandler<TCommand, TResponse>> logger;

        public IdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<TCommand, TResponse>> logger) {
            this.mediator = mediator;
            this.requestManager = requestManager;
            this.logger = logger;
        }

        /// <summary>
        /// Creates the result value to return if a previous request was found
        /// </summary>
        /// <returns></returns>
        protected virtual TResponse CreateResultForDuplicateRequest() {
            return default(TResponse);
        }

        /// <summary>
        /// This method handles the command. It just ensures that no other request exists
        /// with the same ID, and if this is the case just enqueues the original inner command.
        /// </summary>
        /// <param name="request">IdentifiedCommand which contains both original command and request ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Return value of inner command or default value if request same ID was found</returns>
        public async Task<TResponse> Handle(IdentifiedCommand<TCommand, TResponse> request, CancellationToken cancellationToken) {
            bool alreadyExists = await requestManager.ExistsAsync(request.ID);
            if (alreadyExists) {
                return CreateResultForDuplicateRequest();
            } else {
                await this.requestManager.CreateRequestForCommandAsync<TCommand>(request.ID);
                try {
                    TCommand command = request.Command;
                    string comamndName = command.GetGenericTypeName();
                    string idProperty = string.Empty;
                    string commandID = string.Empty;

                    switch (command) {
                        case CreateOrderCommand createOrderCommand:
                            idProperty = nameof(createOrderCommand.UserID);
                            commandID = createOrderCommand.UserID;
                            break;
                        case CancelOrderCommand cancelOrderCommand:
                            idProperty = nameof(cancelOrderCommand.OrderNumber);
                            commandID = $"{cancelOrderCommand.OrderNumber}";
                            break;
                        default:
                            idProperty = "ID?";
                            commandID = "N/A";
                            break;
                    }

                    this.logger.LogInformation(
                        "----- Sending command {ComamndName} - {IDProperty}: {CommandID} ({@Command})}",
                        comamndName,
                        idProperty,
                        commandID,
                        command
                    );

                    // Send the embeded businesss command to mediator so it runs its related CommandHandler
                    TResponse result = await mediator.Send(command, cancellationToken);

                    logger.LogInformation(
                        "----- Command result: {@Result} - {CommandName} - {IDProperty}: {CommandID} ({@Command})",
                        result,
                        comamndName,
                        idProperty,
                        commandID,
                        command
                    );

                    return result;
                } catch {
                    return default(TResponse);
                }
            }
        }
    }
}