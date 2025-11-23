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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketByIDAsync(string id) {
            CustomerBasket basket = await this.basketRepository.GetBasketAsync(id);

            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> AddOrUpdateBasketAsync([FromBody]
            CustomerBasket customerBasket) {
            return Ok(await this.basketRepository.AddOrUpdateBasketAsync(customerBasket));
        }

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

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task DeleteBasketByIDAsync(string id) {
            await this.basketRepository.DeleteBasketAsync(id);
        }
    }
}