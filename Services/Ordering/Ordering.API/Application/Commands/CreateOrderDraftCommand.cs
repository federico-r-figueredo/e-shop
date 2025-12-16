using MediatR;
using System.Runtime.Serialization;
using eShop.Services.Ordering.API.Application.DTOs;
using System.Collections.Generic;
using eShop.Services.Ordering.API.Application.Models;

namespace eShop.Services.Ordering.API.Application.Commands {
    [DataContract]
    public class CreateOrderDraftCommand : IRequest<OrderDraftDTO> {
        public CreateOrderDraftCommand(string buyerID, IEnumerable<BasketItem> basketItems) {
            this.BuyerID = buyerID;
            this.BasketItems = basketItems;
        }

        public string BuyerID { get; private set; }
        public IEnumerable<BasketItem> BasketItems { get; private set; }
    }
}