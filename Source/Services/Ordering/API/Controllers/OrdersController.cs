using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using EShop.Services.Ordering.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EShop.Services.Ordering.API.Application.Queries.ViewModels;
using EShop.Services.Ordering.API.Application.Models;
using EShop.Services.Ordering.API.Application.Commands;
using System.Linq;

namespace EShop.Services.Ordering.API.Controllers {
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase {
        private readonly IMediator mediator;
        private readonly ILogger<OrdersController> logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger) {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("{orderID:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetOrderByID(int orderID, CancellationToken cancellationToken) {
            GetOrderByIDQuery query = new GetOrderByIDQuery(orderID);
            Result<GetOrderByIDResponse> response = await this.mediator.Send(query, cancellationToken);

            if (response.IsFailure) {
                return NotFound(response.Error);
            }

            return Ok(response.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(string userID, string userName,
            string city, string street, string state, string country, string zipCode,
            string cardNumber, string cardHolderName, DateTime cardExpiration,
            string cardSecurityNumber, int cardTypeID, IEnumerable<BasketItem> basketItems,
            CancellationToken cancellationToken) {
            CreateOrderCommand command = new CreateOrderCommand(userID, userName, city,
                street, state, country, zipCode, cardNumber, cardHolderName, DateOnly.FromDateTime(cardExpiration),
                cardSecurityNumber, cardTypeID, basketItems.ToList());
            Result response = await this.mediator.Send(command, cancellationToken);

            if (response.IsFailure) {
                return BadRequest(response.Error);
            }

            return Ok();
        }
    }
}