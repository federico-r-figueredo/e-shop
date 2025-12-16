using System.Threading;
using System.Threading.Tasks;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using eShop.Services.Ordering.Infrastructure.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Ordering.API.Application.Commands {
    // Regular command handler
    public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, bool> {
        private readonly IOrderRepository orderRepository;

        public ShipOrderCommandHandler(IOrderRepository orderRepository) {
            this.orderRepository = orderRepository;
        }

        /// <summary>
        /// Handler which processes the command when administrator executes ship order
        /// from app
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>false</returns>
        public async Task<bool> Handle(ShipOrderCommand request, CancellationToken cancellationToken) {
            Order orderToUpdate = await this.orderRepository.GetByIDAsync(request.OrderNumber);
            if (orderToUpdate == null) {
                return false;
            }

            orderToUpdate.SetShippedStatus();
            return await this.orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }

    // Use for idempotency in command process
    public class ShipOrderIdentifiedCommandHandler
        : IdentifiedCommandHandler<ShipOrderCommand, bool> {
        public ShipOrderIdentifiedCommandHandler(IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<ShipOrderCommand, bool>> logger)
            : base(mediator, requestManager, logger) {
        }

        protected override bool CreateResultForDuplicateRequest() {
            return true; // Ignore duplicate requests for processing order
        }
    }
}