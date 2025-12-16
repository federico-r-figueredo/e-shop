using System.Threading;
using System.Threading.Tasks;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using eShop.Services.Ordering.Infrastructure.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Ordering.API.Application.Commands {
    internal class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool> {
        private readonly IOrderRepository orderRepository;

        public CancelOrderCommandHandler(IOrderRepository orderRepository) {
            this.orderRepository = orderRepository;
        }

        public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken) {
            Order orderToUpate = await this.orderRepository.GetByIDAsync(request.OrderNumber);
            if (orderToUpate == null) {
                return false;
            }

            orderToUpate.SetCancelledStatus();
            return await this.orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }

    public class CancelOrderIdentifiedCommandHandler
        : IdentifiedCommandHandler<CancelOrderCommand, bool> {

        public CancelOrderIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<CancelOrderCommand, bool>> logger
        ) : base(mediator, requestManager, logger) { }

        protected override bool CreateResultForDuplicateRequest() {
            return true; // Ignore duplicate requests for processing order
        }
    }
}