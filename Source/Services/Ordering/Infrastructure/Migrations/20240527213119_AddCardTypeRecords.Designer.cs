﻿// <auto-generated />
using System;
using EShop.Services.Ordering.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Migrations {
    [DbContext(typeof(OrderingContext))]
    [Migration("20240527213119_AddCardTypeRecords")]
    partial class AddCardTypeRecords {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder) {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.HasSequence("BuyersSequence", "Ordering")
                .IncrementsBy(10);

            modelBuilder.HasSequence("OrderItemSequence")
                .IncrementsBy(10);

            modelBuilder.HasSequence("OrdersSequence", "Ordering")
                .IncrementsBy(10);

            modelBuilder.HasSequence("PaymentMethodSequence", "Ordering")
                .IncrementsBy(10);

            modelBuilder.Entity("EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate.Buyer", b => {
                b.Property<int>("ID")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseHiLo(b.Property<int>("ID"), "BuyersSequence", "Ordering");

                b.Property<string>("IdentityGUID")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                b.Property<string>("Name")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("ID");

                b.HasIndex("IdentityGUID")
                    .IsUnique();

                b.ToTable("Buyers", "Ordering");
            });

            modelBuilder.Entity("EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate.CardType", b => {
                b.Property<int>("id")
                    .HasColumnType("int")
                    .HasDefaultValue(1)
                    .HasColumnName("ID");

                b.Property<string>("name")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)")
                    .HasColumnName("Name");

                b.HasKey("id");

                b.ToTable("CardTypes", "Ordering");

                b.HasData(
                    new {
                        id = 1,
                        name = "AmericanExpress"
                    },
                    new {
                        id = 2,
                        name = "Visa"
                    },
                    new {
                        id = 3,
                        name = "MasterCard"
                    });
            });

            modelBuilder.Entity("EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate.PaymentMethod", b => {
                b.Property<int>("ID")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseHiLo(b.Property<int>("ID"), "PaymentMethodSequence", "Ordering");

                b.Property<int?>("BuyerID")
                    .HasColumnType("int");

                b.Property<string>("alias")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)")
                    .HasColumnName("Alias");

                b.Property<DateOnly>("cardExpiration")
                    .HasMaxLength(200)
                    .HasColumnType("date")
                    .HasColumnName("CardExpiration");

                b.Property<string>("cardHolderName")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)")
                    .HasColumnName("CardHolderName");

                b.Property<int>("cardTypeID")
                    .HasColumnType("int")
                    .HasColumnName("CardTypeID");

                b.Property<string>("cardVerificationCode")
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnType("nvarchar(25)")
                    .HasColumnName("CardVerificationCode");

                b.Property<string>("paymentCardNumber")
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnType("nvarchar(25)")
                    .HasColumnName("PaymentCardNumber");

                b.HasKey("ID");

                b.HasIndex("BuyerID");

                b.HasIndex("cardTypeID");

                b.ToTable("PaymentMethods", "Ordering");
            });

            modelBuilder.Entity("EShop.Services.Ordering.Domain.Aggregates.OrderAggregate.Order", b => {
                b.Property<int>("ID")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseHiLo(b.Property<int>("ID"), "OrdersSequence", "Ordering");

                b.Property<int?>("buyerID")
                    .HasColumnType("int")
                    .HasColumnName("BuyerID");

                b.Property<string>("description")
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("Description");

                b.Property<DateTime>("orderDate")
                    .HasColumnType("datetime2")
                    .HasColumnName("OrderDate");

                b.Property<int>("orderStatusID")
                    .HasColumnType("int")
                    .HasColumnName("OrderStatusID");

                b.Property<int?>("paymentMethodID")
                    .HasColumnType("int")
                    .HasColumnName("PaymentMethodID");

                b.HasKey("ID");

                b.HasIndex("buyerID");

                b.HasIndex("orderStatusID");

                b.HasIndex("paymentMethodID");

                b.ToTable("Orders", "Ordering");
            });

            modelBuilder.Entity("EShop.Services.Ordering.Domain.Aggregates.OrderAggregate.OrderItem", b => {
                b.Property<int>("ID")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseHiLo(b.Property<int>("ID"), "OrderItemSequence");

                b.Property<int?>("OrderID")
                    .IsRequired()
                    .HasColumnType("int");

                b.Property<decimal>("discount")
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("Discount");

                b.Property<string>("pictureURL")
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("PictureURL");

                b.Property<int>("productID")
                    .HasColumnType("int")
                    .HasColumnName("ProductID");

                b.Property<string>("productName")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("ProductName");

                b.Property<decimal>("unitPrice")
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("UnitPrice");

                b.Property<int>("units")
                    .HasColumnType("int")
                    .HasColumnName("Units");

                b.HasKey("ID");

                b.HasIndex("OrderID");

                b.ToTable("OrderItems", "Ordering");
            });

            modelBuilder.Entity("EShop.Services.Ordering.Domain.Aggregates.OrderAggregate.OrderStatus", b => {
                b.Property<int>("id")
                    .HasColumnType("int")
                    .HasDefaultValue(1)
                    .HasColumnName("ID");

                b.Property<string>("name")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)")
                    .HasColumnName("Name");

                b.HasKey("id");

                b.ToTable("OrderStatuses", "Ordering");

                b.HasData(
                    new {
                        id = 1,
                        name = "Submitted"
                    },
                    new {
                        id = 2,
                        name = "AwaitingValidation"
                    },
                    new {
                        id = 3,
                        name = "StockConfirmed"
                    },
                    new {
                        id = 4,
                        name = "Paid"
                    },
                    new {
                        id = 5,
                        name = "Shipped"
                    },
                    new {
                        id = 6,
                        name = "Cancelled"
                    });
            });

            modelBuilder.Entity("EShop.Services.Ordering.Infrastructure.Idempotency.ClientRequest", b => {
                b.Property<Guid>("id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("ID");

                b.Property<string>("name")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("Name");

                b.Property<DateTime>("requestDateTime")
                    .HasColumnType("datetime2")
                    .HasColumnName("RequestDateTime");

                b.HasKey("id");

                b.ToTable("Requests", "Ordering");
            });

            modelBuilder.Entity("EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate.PaymentMethod", b => {
                b.HasOne("EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate.Buyer", null)
                    .WithMany("PaymentMethods")
                    .HasForeignKey("BuyerID")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate.CardType", "CardType")
                    .WithMany()
                    .HasForeignKey("cardTypeID")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("CardType");
            });

            modelBuilder.Entity("EShop.Services.Ordering.Domain.Aggregates.OrderAggregate.Order", b => {
                b.HasOne("EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate.Buyer", null)
                    .WithMany()
                    .HasForeignKey("buyerID");

                b.HasOne("EShop.Services.Ordering.Domain.Aggregates.OrderAggregate.OrderStatus", "OrderStatus")
                    .WithMany()
                    .HasForeignKey("orderStatusID")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate.PaymentMethod", null)
                    .WithMany()
                    .HasForeignKey("paymentMethodID")
                    .OnDelete(DeleteBehavior.Restrict);

                b.OwnsOne("EShop.Services.Ordering.Domain.Aggregates.OrderAggregate.Address", "Address", b1 => {
                    b1.Property<int>("OrderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseHiLo(b1.Property<int>("OrderID"), "OrdersSequence", "Ordering");

                    b1.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b1.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b1.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b1.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b1.Property<string>("ZipCode")
                        .HasColumnType("nvarchar(max)");

                    b1.HasKey("OrderID");

                    b1.ToTable("Orders", "Ordering");

                    b1.WithOwner()
                        .HasForeignKey("OrderID");
                });

                b.Navigation("Address");

                b.Navigation("OrderStatus");
            });

            modelBuilder.Entity("EShop.Services.Ordering.Domain.Aggregates.OrderAggregate.OrderItem", b => {
                b.HasOne("EShop.Services.Ordering.Domain.Aggregates.OrderAggregate.Order", null)
                    .WithMany("OrderItems")
                    .HasForeignKey("OrderID")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate.Buyer", b => {
                b.Navigation("PaymentMethods");
            });

            modelBuilder.Entity("EShop.Services.Ordering.Domain.Aggregates.OrderAggregate.Order", b => {
                b.Navigation("OrderItems");
            });
#pragma warning restore 612, 618
        }
    }
}