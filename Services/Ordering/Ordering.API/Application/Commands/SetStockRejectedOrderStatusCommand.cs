using System.Collections.Generic;
using System.Runtime.Serialization;
using MediatR;

namespace eShop.Services.Ordering.API.Application.Commands {
    public class SetStockRejectedOrderStatusCommand : IRequest<bool> {
        public SetStockRejectedOrderStatusCommand(int orderNumber, List<int> orderStockItems) {
            this.OrderNumber = orderNumber;
            this.OrderStockItems = orderStockItems;
        }

        [DataMember]
        public int OrderNumber { get; private set; }
        [DataMember]
        public List<int> OrderStockItems { get; private set; }
    }
}