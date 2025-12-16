using System.Threading;
using System.Threading.Tasks;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using MediatR;

namespace eShop.Services.Ordering.API.Application.Commands {
    public class SetStockConfirmedOrderStatusCommandHandler
        : IRequestHandler<SetStockConfirmedOrderStatusCommand, bool> {
        private readonly IOrderRepository orderRepository;

        public SetStockConfirmedOrderStatusCommandHandler(IOrderRepository orderRepository) {
            this.orderRepository = orderRepository;
        }

        /// <summary>
        /// Handler which processes the command when Stock service confirms the request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(SetStockConfirmedOrderStatusCommand request,
            CancellationToken cancellationToken) {

            // Simulate a work time for confirming the stock
            await Task.Delay(10000, cancellationToken);

            Order orderToUpdate = await this.orderRepository.GetByIDAsync(request.OrderNumber);
            if (orderToUpdate == null) {
                return false;
            }

            orderToUpdate.SetStockConfirmedStatus();
            return await this.orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}