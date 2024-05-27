using System.Threading;
using System.Threading.Tasks;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using EShop.Services.Ordering.Domain.SeedWork;
using EShop.Services.Ordering.Domain.Shared;
using Microsoft.Extensions.Logging;
using MediatR;
using System;

namespace EShop.Services.Ordering.API.Application.Commands {
    internal class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand> {
        private readonly IOrderRepository orderRepository;
        private readonly IMediator mediator;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<CreateOrderCommandHandler> logger;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IMediator mediator,
            IUnitOfWork unitOfWork, ILogger<CreateOrderCommandHandler> logger) {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> Handle(CreateOrderCommand command, CancellationToken cancellationToken) {
            try {
                // Add / Update the Order AggregateRoot
                // DDD patterns comment: Add child entities and value-objects through the Order aggregate root
                // methods and constructor so validations, invariants and business logic make sure that
                // consistency is preserved across the whole aggregate.
                Address address = new Address(
                    command.Street,
                    command.City,
                    command.State,
                    command.Country,
                    command.ZipCode
                );
                Order order = new Order(
                    command.UserID,
                    command.UserName,
                    address,
                    command.CardTypeID,
                    command.CardNumber,
                    command.CardSecurityNumber,
                    command.CardHolderName,
                    command.CardExpiration
                );

                foreach (OrderItemDTO orderItem in command.OrderItems) {
                    order.AddOrUpdateOrderItem(
                        orderItem.ProductID,
                        orderItem.ProductName,
                        orderItem.UnitPrice,
                        orderItem.Discount,
                        orderItem.PictureURL,
                        orderItem.Units
                    );
                }

                this.logger.LogInformation($"----- Creating Order - Order: {order}");

                this.orderRepository.Add(order);

                bool result = await unitOfWork.SaveEntitiesAsync(cancellationToken);

                // if (!result) {
                //     return Result.Failure(new Error("CreateOrder.StatePersistenceFailed", $"New Order's state persistence was unsuccessful."));
                // }
            } catch (Exception ex) {
                return Result.Failure(new Error("CreateOrder.Failed", ex.Message));
            }

            return Result.Success();
        }
    }
}