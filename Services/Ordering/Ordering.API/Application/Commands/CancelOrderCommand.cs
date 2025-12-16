using System.Runtime.Serialization;
using MediatR;

namespace eShop.Services.Ordering.API.Application.Commands {
    public class CancelOrderCommand : IRequest<bool> {
        public CancelOrderCommand() { }

        public CancelOrderCommand(int orderNumber) {
            this.OrderNumber = orderNumber;
        }

        [DataMember]
        public int OrderNumber { get; set; }
    }
}