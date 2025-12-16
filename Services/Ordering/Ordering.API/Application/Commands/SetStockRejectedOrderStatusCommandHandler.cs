using System.Threading;
using System.Threading.Tasks;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using eShop.Services.Ordering.Infrastructure.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Ordering.API.Application.Commands {
    // Regular command handler
    public class SetStockRejectedOrderStatusCommandHandler
        : IRequestHandler<SetStockRejectedOrderStatusCommand, bool> {
        private readonly IOrderRepository orderRepository;

        public SetStockRejectedOrderStatusCommandHandler(IOrderRepository orderRepository) {
            this.orderRepository = orderRepository;
        }

        /// <summary>
        /// Handler which processes the command when Stock service rejects the request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>bool</returns>
        public async Task<bool> Handle(SetStockRejectedOrderStatusCommand request,
            CancellationToken cancellationToken) {

            // Simulate a work time for rejecting the stock
            await Task.Delay(10000, cancellationToken);

            Order orderToUpdate = await this.orderRepository.GetByIDAsync(request.OrderNumber);
            if (orderToUpdate == null) {
                return false;
            }

            orderToUpdate.SetCancelledStatusWhenStockIsRejected(request.OrderStockItems);
            return await this.orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }

    // Use for idempotency in command process
    public class SetStockRejectedOrderStatusCommandIdentifiedHandler
        : IdentifiedCommandHandler<SetStockRejectedOrderStatusCommand, bool> {
        public SetStockRejectedOrderStatusCommandIdentifiedHandler(
            IMediator mediator, IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<SetStockRejectedOrderStatusCommand, bool>> logger)
            : base(mediator, requestManager, logger) { }

        protected override bool CreateResultForDuplicateRequest() {
            return true; // Ignore duplicate requests for processing order
        }
    }
}