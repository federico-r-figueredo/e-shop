using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MediatR;
using eShop.BuildingBlocks.EventBus.Extensions;
using eShop.Services.Ordering.API.Application.IntegrationEvents;
using eShop.Services.Ordering.Infrastructure;

namespace eShop.Services.Ordering.API.Application.Behaviours {
    internal class TransactionBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse> {
        private readonly OrderingContext dbContext;
        private readonly IOrderingIntegrationEventService orderingIntegrationEventService;
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> logger;

        public TransactionBehaviour(OrderingContext dbContext,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger) {
            this.dbContext = dbContext ?? throw new ArgumentException(nameof(OrderingContext));
            this.orderingIntegrationEventService = orderingIntegrationEventService
                ?? throw new ArgumentException(nameof(OrderingIntegrationEventService));
            this.logger = logger ?? throw new ArgumentException(nameof(ILogger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next) {

            TResponse response = default(TResponse);
            string typeName = request.GetGenericTypeName();

            try {
                if (this.dbContext.HasActiveTransaction) {
                    return await next();
                }

                IExecutionStrategy executionStrategy = this.dbContext.Database.CreateExecutionStrategy();

                await executionStrategy.ExecuteAsync(async () => {
                    Guid transactionID;

                    using (IDbContextTransaction transaction = await this.dbContext.BeginTransactionAsync()) {
                        this.logger.LogInformation(
                            "----- Begin transaction {TransactionID} for {CommandName} ({@Command})",
                            transaction.TransactionId,
                            typeName,
                            request
                        );

                        response = await next();

                        this.logger.LogInformation(
                            "----- Commit transaction {TransactionID} for {CommandName}",
                            transaction.TransactionId,
                            typeName
                        );

                        await this.dbContext.CommitTransactionAsync(transaction);

                        transactionID = transaction.TransactionId;
                    }

                    await this.orderingIntegrationEventService.PublishEventsThroughEventBusAsync(transactionID);
                });

                return response;

            } catch (Exception exception) {
                this.logger.LogError(
                    exception,
                    "ERROR Handling transaction for {CommandName} ({@Command})",
                    typeName,
                    request
                );

                throw;
            }
        }
    }
}