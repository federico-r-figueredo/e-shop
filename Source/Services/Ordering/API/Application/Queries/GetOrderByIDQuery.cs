
namespace EShop.Services.Ordering.API.Application.Queries.ViewModels {
    internal class GetOrderByIDQuery : IQuery<GetOrderByIDResponse> {
        private readonly int orderID;

        public GetOrderByIDQuery(int orderID) {
            this.orderID = orderID;
        }

        public int OrderID {
            get { return this.orderID; }
        }
    }
}