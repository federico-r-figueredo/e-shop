using System.Threading;
using System.Threading.Tasks;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using MediatR;

namespace eShop.Services.Ordering.API.Application.Commands {
    public class SetPaidOrderStatusCommandHandler
        : IRequestHandler<SetPaidOrderStatusCommand, bool> {

        private readonly IOrderRepository orderRepository;

        public SetPaidOrderStatusCommandHandler(IOrderRepository orderRepository) {
            this.orderRepository = orderRepository;
        }

        /// <summary>
        /// Handler which processes the command when Shipment service confirms the payment
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>bool</returns>
        public async Task<bool> Handle(SetPaidOrderStatusCommand request,
            CancellationToken cancellationToken) {

            // Simulat a work time for validating payment
            await Task.Delay(10000, cancellationToken);

            Order orderToUpdate = await this.orderRepository.GetByIDAsync(request.OrderNumber);
            if (orderToUpdate == null) {
                return false;
            }

            orderToUpdate.SetPaidStatus();
            return await this.orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}