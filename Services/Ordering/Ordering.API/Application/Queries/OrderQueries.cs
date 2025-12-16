using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using eShop.Services.Ordering.API.Application.ViewModels;
using Microsoft.Data.SqlClient;

namespace eShop.Services.Ordering.API.Application.Queries {
    internal class OrderQueries : IOrderQueries {
        private readonly string connectionString;

        public OrderQueries(string connectionString) {
            if (string.IsNullOrWhiteSpace(connectionString)) {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.connectionString = connectionString;
        }

        public async Task<OrderViewModel> GetOrderAsync(int id) {
            using (SqlConnection connection = new SqlConnection(this.connectionString)) {
                connection.Open();

                IEnumerable<dynamic> result = await connection.QueryAsync<dynamic>(
                    @"
                    SELECT 
                        o.ID AS OrderNumber,
                        o.OrderDate,
                        o.Description,
                        o.Address_Street AS Street,
                        o.Address_City AS City,
                        o.Address_State AS State,
                        o.Address_ZipCode AS ZipCode,
                        o.Address_Country AS Country,
                        os.Name AS Status,
                        oi.ProductName,
                        oi.Units,
                        oi.UnitPrice,
                        oi.PictureURL
                    FROM Ordering.Orders o
                    LEFT JOIN Ordering.OrderItems oi ON oi.OrderID = o.ID
                    LEFT JOIN Ordering.OrderStatuses os ON os.ID = o.OrderStatusID
                    WHERE o.ID = @ID
                    ",
                    new { ID = id }
                );

                if (result.AsList().Count == 0) {
                    throw new KeyNotFoundException();
                }

                return MapOrderItems(result);
            }
        }

        public async Task<IEnumerable<OrderSummaryViewModel>> GetOrdersFromUserAsync(Guid guid) {
            using (SqlConnection connection = new SqlConnection(this.connectionString)) {
                connection.Open();

                return await connection.QueryAsync<OrderSummaryViewModel>(
                    @"
                    SELECT 
                        o.ID AS OrderNumber,
                        o.OrderDate,
                        os.Name AS Status,
                        SUM(oi.Units * oi.UnitPrice) AS Total
                    FROM Ordering.Orders o
                    LEFT JOIN Ordering.OrderItems oi ON oi.OrderID = o.ID
                    LEFT JOIN Ordering.OrderStatuses os ON os.ID = o.OrderStatusID
                    LEFT JOIN Ordering.Buyers b ON b.ID = o.BuyerID
                    WHERE b.IdentityGUID = @GUID
                    GROUP BY o.ID, o.OrderDate, os.Name
                    ORDER BY o.ID
                    ",
                    new { GUID = guid }
                );
            }
        }

        public async Task<IEnumerable<CardTypeViewModel>> GetCardTypesAsync() {
            using (SqlConnection connection = new SqlConnection(this.connectionString)) {
                connection.Open();

                return await connection.QueryAsync<CardTypeViewModel>(
                    "SELECT * FROM Ordering.CardTypes"
                );
            }
        }

        private OrderViewModel MapOrderItems(dynamic result) {
            OrderViewModel order = new OrderViewModel {
                OrderNumber = result[0].OrderNumber,
                OrderDate = result[0].OrderDate,
                Status = result[0].Status,
                Description = result[0].Description,
                Street = result[0].Street,
                City = result[0].City,
                State = result[0].State,
                ZipCode = result[0].ZipCode,
                Country = result[0].Country,
                OrderItems = new List<OrderItemViewModel>(),
                Total = 0
            };

            foreach (dynamic item in result) {
                OrderItemViewModel orderItem = new OrderItemViewModel() {
                    ProductName = item.ProductName,
                    Units = item.Units,
                    UnitPrice = (double)item.UnitPrice,
                    PictureURL = item.PictureURL
                };

                order.Total += item.Units * item.UnitPrice;
                order.OrderItems.Add(orderItem);
            }

            return order;
        }
    }
}