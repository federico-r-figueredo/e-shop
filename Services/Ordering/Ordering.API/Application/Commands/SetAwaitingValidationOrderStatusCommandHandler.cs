using System.Threading;
using System.Threading.Tasks;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using MediatR;

namespace eShop.Services.Ordering.API.Application.Commands {
    public class SetAwaitingValidationOrderStatusCommandHandler
        : IRequestHandler<SetAwaitingValidationStatusCommand, bool> {

        private IOrderRepository orderRepository;

        public SetAwaitingValidationOrderStatusCommandHandler(
            IOrderRepository orderRepository) {
            this.orderRepository = orderRepository;
        }

        /// <summary>
        /// Handler which processes the command when graceperiod has finished
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(SetAwaitingValidationStatusCommand request,
            CancellationToken cancellationToken) {
            Order orderToUpate = await this.orderRepository.GetByIDAsync(request.OrderNumber);
            if (orderToUpate == null) {
                return false;
            }

            orderToUpate.SetAwaitingValidationStatus();
            return await this.orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}