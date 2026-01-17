using System;
using System.Net;
using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.Services.Basket.API.IntegrationEvents.Events;
using eShop.Services.Basket.API.Model;
using eShop.Services.Basket.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Basket.API.Controllers {
    /// <summary>
    /// Manages shopping basket operations for buyers.
    /// </summary>
    /// <remarks>
    /// This controller exposes endpoints to:
    /// - Retrieve a basket
    /// - Create or update a basket
    /// - Checkout a basket
    /// - Delete a basket
    ///
    /// All basket identifiers correspond to buyer IDs
    /// (e.g. <c>johndoe@example.com</c>).
    /// </remarks>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase {
        private readonly IBasketRepository basketRepository;
        private readonly IIdentityService identityService;
        private readonly IEventBus eventBus;
        private readonly ILogger<BasketController> logger;

        public BasketController(IBasketRepository basketRepository,
            IIdentityService identityService, IEventBus eventBus,
            ILogger<BasketController> logger) {
            this.basketRepository = basketRepository;
            this.identityService = identityService;
            this.eventBus = eventBus;
            this.logger = logger;
        }

        /// <summary>
        /// Retrieves the basket associated with a given buyer.
        /// </summary>
        /// <remarks>
        /// This endpoint returns the current basket for the specified buyer.
        /// If no basket exists, an empty basket will be returned.
        ///
        /// Sample request:
        ///
        ///     GET /api/basket/johndoe@example.com
        ///
        /// </remarks>
        /// <param name="id">
        /// The unique identifier of the basket to retrieve.
        /// This typically corresponds to the buyer identifier
        /// (e.g. <c>johndoe@example.com</c>).
        /// </param>
        /// <returns>
        /// Returns the <see cref="CustomerBasket"/> associated with the buyer.
        /// If the basket does not exist, a new empty basket is returned.
        /// </returns>
        /// <response code="200">
        /// Basket successfully retrieved.
        /// </response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketByIDAsync(string id) {
            CustomerBasket basket = await this.basketRepository.GetBasketAsync(id);

            return Ok(basket ?? new CustomerBasket(id));
        }

        /// <summary>
        /// Creates a new basket or updates an existing basket for a given buyer.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/basket
        ///     {
        ///       "buyerID": "johndoe@example.com",
        ///       "basketItems": [
        ///         {
        ///           "id": "b4f2e6a1-1c9e-4a8a-a2e3-91c8dcb23c11",
        ///           "productID": 101,
        ///           "productName": "Wireless Mouse",
        ///           "unitPrice": 29.99,
        ///           "oldUnitPrice": 39.99,
        ///           "quantity": 2,
        ///           "pictureURL": "https://example.com/images/wireless-mouse.jpg"
        ///         }
        ///       ]
        ///     }
        ///
        /// </remarks>
        /// <param name="customerBasket">
        /// The basket associated with a buyer, including all selected basket items.
        /// </param>
        /// <returns>
        /// Returns the created or updated <see cref="CustomerBasket"/>.
        /// </returns>
        /// <response code="200">
        /// Basket successfully created or updated.
        /// </response>
        /// <response code="400">
        /// Invalid basket payload.
        /// </response>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> AddOrUpdateBasketAsync([FromBody]
            CustomerBasket customerBasket) {
            return Ok(await this.basketRepository.AddOrUpdateBasketAsync(customerBasket));
        }

        /// <summary>
        /// Performs the checkout operation for a customer's basket.
        /// </summary>
        /// <remarks>
        /// This endpoint finalizes the basket by validating the checkout data,
        /// creating an order, and initiating the payment workflow.
        ///
        /// Sample request:
        ///
        ///     POST /api/basket/checkout
        ///     {
        ///       "street": "1234 Elm Street",
        ///       "city": "Springfield",
        ///       "state": "IL",
        ///       "zipCode": "62704",
        ///       "country": "USA",
        ///       "cardNumber": "4111111111111111",
        ///       "cardHolderName": "John Doe",
        ///       "cardExpiration": "2026-11-20T12:20:44.490Z",
        ///       "cardSecurityNumber": "123",
        ///       "cardTypeID": 1,
        ///       "buyer": "johndoe@example.com",
        ///       "requestGUID": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     }
        ///
        /// </remarks>
        /// <param name="basketCheckout">
        /// Checkout data including shipping address, payment information,
        /// buyer identity, and idempotency request identifier.
        /// </param>
        /// <param name="requestID">
        /// Optional request identifier used for idempotency.
        /// Must be a valid GUID if provided.
        /// </param>
        /// <returns>
        /// Returns <see cref="HttpStatusCode.Accepted"/> if the checkout request
        /// has been successfully accepted for processing.
        /// </returns>
        /// <response code="202">
        /// Checkout request accepted and processing has started.
        /// </response>
        /// <response code="400">
        /// Invalid checkout payload or request identifier.
        /// </response>
        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CheckoutBasketAsync(
            [FromBody] BasketCheckout basketCheckout,
            [FromHeader(Name = "x-requestid")] string requestID) {
            // TODO: Uncomment when user authorization has been implemented
            //string userID = this.identityService.GetUserIdentity();

            string userID = "johndoe@example.com";

            basketCheckout.RequestGUID =
                (Guid.TryParse(requestID, out Guid guid) && guid != Guid.Empty)
                    ? guid
                    : basketCheckout.RequestGUID;

            CustomerBasket basket = await this.basketRepository.GetBasketAsync(userID);

            if (basket == null) {
                return BadRequest();
            }

            // TODO: Uncomment when user authorization has been implemented
            //string userName = this.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;
            string userName = "johndoe";

            UserCheckoutAcceptedIntegrationEvent integrationEvent =
                new UserCheckoutAcceptedIntegrationEvent(
                    userID, userName, basketCheckout.Street, basketCheckout.City,
                    basketCheckout.State, basketCheckout.ZipCode, basketCheckout.Country,
                    basketCheckout.CardNumber, basketCheckout.CardHolderName,
                    basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber,
                    basketCheckout.CardTypeID, basketCheckout.Buyer,
                    basketCheckout.RequestGUID, basket
                );

            // Once basket is checkout, sends an integration event to Ordering.API to
            // convert Basket to Order and proceed with Order creation process.
            try {
                await this.eventBus.PublishAsync(integrationEvent);
            } catch (Exception exception) {
                this.logger.LogError(
                    exception,
                    "ERROR Publishing integration event: {IntegrationEventID} from {AppName}",
                    integrationEvent.ID,
                    Program.ApplicationName
                );
                throw;
            }

            return Accepted();
        }

        /// <summary>
        /// Deletes an existing basket associated with a buyer.
        /// </summary>
        /// <remarks>
        /// This endpoint removes the basket identified by the buyer ID.
        ///
        /// Sample request:
        ///
        ///     DELETE /api/basket/johndoe@example.com
        ///
        /// </remarks>
        /// <param name="id">
        /// The unique identifier of the basket to delete.
        /// Typically this corresponds to the buyer identifier
        /// (e.g. <c>johndoe@example.com</c>).
        /// </param>
        /// <returns>
        /// Returns <see cref="HttpStatusCode.OK"/> when the basket
        /// has been successfully deleted.
        /// </returns>
        /// <response code="200">
        /// Basket successfully deleted.
        /// </response>
        /// <response code="404">
        /// Basket not found.
        /// </response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task DeleteBasketByIDAsync(string id) {
            await this.basketRepository.DeleteBasketAsync(id);
        }
    }
}