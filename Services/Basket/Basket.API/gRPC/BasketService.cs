using System.Linq;
using System.Threading.Tasks;
using BasketAPI;
using eShop.Services.Basket.API.Model;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using static BasketAPI.Basket;
using BasketItem = eShop.Services.Basket.API.Model.BasketItem;

namespace eShop.Services.Basket.API.gRPC {
    public class BasketService : BasketBase {
        private readonly IBasketRepository basketRepository;
        private readonly ILogger<BasketService> logger;

        public BasketService(IBasketRepository basketRepository, ILogger<BasketService> logger) {
            this.basketRepository = basketRepository;
            this.logger = logger;
        }

        [AllowAnonymous]
        public override async Task<CustomerBasketResponse> GetBasketByID(GetBasketRequest request,
            ServerCallContext context) {
            this.logger.LogInformation(
                "Begin gRPC call from method {Method} for basket id {ID}",
                context.Method,
                request.Id
            );

            CustomerBasket basket = await this.basketRepository.GetBasketAsync(request.Id);
            if (basket != null) {
                context.Status = new Status(StatusCode.OK, $"Basket with id {request.Id} do exist");
                return MapToCustomerBasketResponse(basket);
            } else {
                context.Status = new Status(
                    StatusCode.NotFound,
                    $"Basket with id {request.Id} does not exist"
                );
            }

            return new CustomerBasketResponse();
        }

        public override async Task<CustomerBasketResponse> UpdateCustomerBasket(
            UpdateCustomerBasketRequest request, ServerCallContext context) {
            this.logger.LogInformation("Begin RPC call BasketService.UpdateBasket for buyer id {BuyerID}", request.BuyerID);

            CustomerBasket basket = MapToCustomerBasket(request);
            CustomerBasket response = await this.basketRepository.AddOrUpdateBasketAsync(basket);
            if (response == null) {
                context.Status = new Status(StatusCode.NotFound, $"Basket with buyer id {request.BuyerID} does not exist.");
                return null;
            }

            return this.MapToCustomerBasketResponse(response);
        }

        private CustomerBasketResponse MapToCustomerBasketResponse(CustomerBasket customerBasket) {
            CustomerBasketResponse response = new CustomerBasketResponse() {
                BuyerID = customerBasket.BuyerID
            };

            customerBasket.BasketItems.ForEach(x => response.Items.Add(new BasketAPI.BasketItem() {
                Id = x.ID,
                OldUnitPrice = (double)x.OldUnitPrice,
                PictureURL = x.PictureURL,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitPrice = (double)x.UnitPrice
            }));

            return response;
        }

        private CustomerBasket MapToCustomerBasket(UpdateCustomerBasketRequest request) {
            CustomerBasket response = new CustomerBasket() {
                BuyerID = request.BuyerID
            };

            request.Items.ToList().ForEach(x => response.BasketItems.Add(new BasketItem() {
                ID = x.Id,
                OldUnitPrice = (decimal)x.OldUnitPrice,
                PictureURL = x.PictureURL,
                ProductID = x.ProductID,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitPrice = (decimal)x.UnitPrice
            }));

            return response;
        }
    }
}