using System.Runtime.Serialization;
using MediatR;

namespace eShop.Services.Ordering.API.Application.Commands {
    public class SetStockConfirmedOrderStatusCommand : IRequest<bool> {
        public SetStockConfirmedOrderStatusCommand(int orderNumber) {
            this.OrderNumber = orderNumber;
        }

        [DataMember]
        public int OrderNumber { get; private set; }
    }
}