using MediatR;
using System.Threading;
using System.Threading.Tasks;
using eShop.Services.Ordering.API.Application.DTOs;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using eShop.Services.Ordering.API.Infrastructure.Services;
using System.Linq;
using eShop.Services.Ordering.API.Extensions;
using System.Collections.Generic;

namespace eShop.Services.Ordering.API.Application.Commands {
    public class CreateOrderDraftCommandHandler
        : IRequestHandler<CreateOrderDraftCommand, OrderDraftDTO> {
        private readonly IOrderRepository orderRepository;
        private readonly IIdentityService identityService;
        private readonly IMediator mediator;

        // Using DI to inject infrastructure persistence repositories
        public CreateOrderDraftCommandHandler(IOrderRepository orderRepository,
            IIdentityService identityService, IMediator mediator) {
            this.orderRepository = orderRepository;
            this.identityService = identityService;
            this.mediator = mediator;
        }

        public Task<OrderDraftDTO> Handle(CreateOrderDraftCommand request,
            CancellationToken cancellationToken) {

            Order order = Order.NewDraft();
            IEnumerable<OrderItemDTO> orderItemDTOs = request.BasketItems.Select(
                x => x.ToOrderItemDTO()
            );
            foreach (OrderItemDTO orderItemDTO in orderItemDTOs) {
                order.AddOrderItem(
                    orderItemDTO.ProductID,
                    orderItemDTO.ProductName,
                    orderItemDTO.UnitPrice,
                    orderItemDTO.Discount,
                    orderItemDTO.PictureURL,
                    orderItemDTO.Units
                );
            }

            return Task.FromResult(OrderDraftDTO.FromOrder(order));
        }
    }
}