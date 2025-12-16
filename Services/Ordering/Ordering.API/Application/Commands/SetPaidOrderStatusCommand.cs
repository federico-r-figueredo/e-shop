using System.Runtime.Serialization;
using MediatR;

namespace eShop.Services.Ordering.API.Application.Commands {
    public class SetPaidOrderStatusCommand : IRequest<bool> {

        public SetPaidOrderStatusCommand(int orderNumber) {
            this.OrderNumber = orderNumber;
        }

        [DataMember]
        public int OrderNumber { get; private set; }
    }
}