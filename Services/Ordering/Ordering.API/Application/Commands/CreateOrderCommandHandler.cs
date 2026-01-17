using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.Services.Ordering.API.Application.DTOs;
using eShop.Services.Ordering.API.Application.IntegrationEvents;
using eShop.Services.Ordering.API.Application.IntegrationEvents.Events;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;

namespace eShop.Services.Ordering.API.Application.Commands {
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool> {
        private readonly IOrderRepository orderRepository;
        private readonly IEventBus eventBus;
        private readonly IOrderingIntegrationEventService orderingIntegrationEventService;
        private readonly ILogger<CreateOrderCommandHandler> logger;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            ILogger<CreateOrderCommandHandler> logger) {
            this.orderRepository = orderRepository;
            this.eventBus = eventBus;
            this.orderingIntegrationEventService = orderingIntegrationEventService;
            this.logger = logger;
        }

        public async Task<bool> Handle(CreateOrderCommand request,
            CancellationToken cancellationToken) {

            // Add integration event to clean the basket
            await this.orderingIntegrationEventService.AddAndSaveEventAsync(
                new OrderStartedIntegrationEvent(request.UserID)
            );

            // Add / Update the Order aggregate root
            // DDD patterns comment: Add child entities and value-objects through the Order
            // aggregate root methods and constructor so validations, invariants and business logic
            // make sure that consistency is preserved across the whole aggregate.
            Address address = new Address(
                request.Street,
                request.City,
                request.State,
                request.Country,
                request.ZipCode
            );
            Order order = new Order(
                request.UserID,
                request.UserName,
                address,
                request.CardTypeID,
                request.CardNumber,
                request.CardSecurityNumber,
                request.CardHolderName,
                request.CardExpiration
            );

            foreach (OrderItemDTO orderItemDTO in request.OrderItems) {
                order.AddOrderItem(
                    orderItemDTO.ProductID,
                    orderItemDTO.ProductName,
                    orderItemDTO.UnitPrice,
                    orderItemDTO.Discount,
                    orderItemDTO.PictureURL,
                    orderItemDTO.Units
                );
            }

            this.logger.LogInformation("----- Creating Order - Order: {@Order}", order);

            this.orderRepository.Add(order);

            return await this.orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}