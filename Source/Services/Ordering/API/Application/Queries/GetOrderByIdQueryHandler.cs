
using System.Threading;
using EShop.Services.Ordering.API.Application.Queries.ViewModels;
using EShop.Services.Ordering.Domain.Shared;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EShop.Services.Ordering.API.Application.Queries {
    internal class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIDQuery, GetOrderByIDResponse> {
        private string connnectionString = "Server=localhost;Initial Catalog=Ordering;User Id=admin;Password=1234;TrustServerCertificate=True;Encrypt=false;Trusted_Connection=True;";
        public async Task<Result<GetOrderByIDResponse>> Handle(GetOrderByIDQuery request, CancellationToken cancellationToken) {
            using (SqlConnection connection = new SqlConnection(this.connnectionString)) {
                connection.Open();

                var result = await connection.QueryAsync<dynamic>(
                    @"SELECT o.[ID] as OrderNumber, 
							 o.OrderDate as Date, 
							 o.Description as Description,
							 o.Address_City as City,
							 o.Address_Country as Country,
							 o.Address_State as State,
							 o.Address_Street as Street,
							 o.Address_ZipCode Zipcode,
							 os.Name as Status,
							 oi.ProductName as ProductName,
							 oi.Units as Units,
							 oi.UnitPrice as UnitPrice,
							 oi.PictureURL as PictureURL
					  FROM Ordering.Orders o
							LEFT JOIN Ordering.OrderItems oi ON o.ID = oi.OrderID
							LEFT JOIN Ordering.OrderStatuses os ON o.OrderStatusID = os.ID
					  WHERE o.ID = @orderID",
                    new { request.OrderID }
                );

                if (!result.AsList().Any()) {
                    return Result.Failure<GetOrderByIDResponse>(new Error("Order.NotFound", $"The Order with ID {request.OrderID} was not found"));
                }

                return Result.Success(MapOrderItems(result));
            }
        }

        private GetOrderByIDResponse MapOrderItems(dynamic result) {
            GetOrderByIDResponse order = new GetOrderByIDResponse(
                orderNumber: result[0].OrderNumber,
                date: result[0].Date,
                status: result[0].Status,
                description: result[0].Description,
                street: result[0].Street,
                city: result[0].City,
                zipCode: result[0].ZipCode,
                country: result[0].Country,
                orderItems: new List<OrderItemViewModel>(),
                total: 0
            );

            foreach (dynamic item in result) {
                OrderItemViewModel orderItem = new OrderItemViewModel(
                    productName: item.ProductName,
                    units: item.Units,
                    unitPrice: (double)item.UnitPrice,
                    pictureURL: item.PictureURL
                );

                order.Total += item.Units * item.UnitPrice;
                order.OrderItems.Add(orderItem);
            }

            return order;
        }
    }
}