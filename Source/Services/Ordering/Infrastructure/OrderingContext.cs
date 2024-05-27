using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dawn;
using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using EShop.Services.Ordering.Domain.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EShop.Services.Ordering.Infrastructure {
    public class OrderingContext : DbContext {
        public const string DEFAULT_SCHEMA = "Ordering";

        public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

        #region Order Aggregate

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }

        #endregion

        #region Buyer Aggregate

        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<CardType> CardTypes { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

            // TODO: Create OrderStatus if it doesn't exist based on static fields
            modelBuilder.Entity<CardType>().HasData(new CardType[] {
                new CardType(1, nameof(CardType.AmericanExpress)),
                new CardType(2, nameof(CardType.Visa)),
                new CardType(3, nameof(CardType.MasterCard))
            });

            // TODO: Create OrderStatus if it doesn't exist based on static fields
            modelBuilder.Entity<OrderStatus>().HasData(new OrderStatus[] {
                new OrderStatus(1, nameof(OrderStatus.Submitted)),
                new OrderStatus(2, nameof(OrderStatus.AwaitingValidation)),
                new OrderStatus(3, nameof(OrderStatus.StockConfirmed)),
                new OrderStatus(4, nameof(OrderStatus.Paid)),
                new OrderStatus(5, nameof(OrderStatus.Shipped)),
                new OrderStatus(6, nameof(OrderStatus.Cancelled))
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}