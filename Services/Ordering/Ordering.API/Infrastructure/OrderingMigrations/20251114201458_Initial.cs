using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Infrastructure.OrderingMigrations {
    /// <inheritdoc />
    public partial class Initial : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.EnsureSchema(
                name: "Ordering");

            migrationBuilder.EnsureSchema(
                name: "Idempotency");

            migrationBuilder.CreateSequence(
                name: "buyer_hilo",
                schema: "Ordering",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "order_hilo",
                schema: "Ordering",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "orderitems_hilo",
                schema: "Ordering",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "payment_hilo",
                schema: "Ordering",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Buyers",
                schema: "Ordering",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false),
                    IdentityGUID = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Buyers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CardTypes",
                schema: "Ordering",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_CardTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ClientRequests",
                schema: "Idempotency",
                columns: table => new {
                    GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_ClientRequests", x => x.GUID);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                schema: "Ordering",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_OrderStatuses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                schema: "Ordering",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false),
                    CardTypeID = table.Column<int>(type: "int", nullable: false),
                    BuyerID = table.Column<int>(type: "int", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CardHolderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", maxLength: 25, nullable: false),
                    SecurityNumber = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_PaymentMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_Buyers_BuyerID",
                        column: x => x.BuyerID,
                        principalSchema: "Ordering",
                        principalTable: "Buyers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_CardTypes_CardTypeID",
                        column: x => x.CardTypeID,
                        principalSchema: "Ordering",
                        principalTable: "CardTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "Ordering",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Address_Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderStatusID = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuyerID = table.Column<int>(type: "int", nullable: true),
                    isDraft = table.Column<bool>(type: "bit", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethodID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Orders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Orders_Buyers_BuyerID",
                        column: x => x.BuyerID,
                        principalSchema: "Ordering",
                        principalTable: "Buyers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Orders_OrderStatuses_OrderStatusID",
                        column: x => x.OrderStatusID,
                        principalSchema: "Ordering",
                        principalTable: "OrderStatuses",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Orders_PaymentMethods_PaymentMethodID",
                        column: x => x.PaymentMethodID,
                        principalSchema: "Ordering",
                        principalTable: "PaymentMethods",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "Ordering",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PictureURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Units = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_OrderItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderID",
                        column: x => x.OrderID,
                        principalSchema: "Ordering",
                        principalTable: "Orders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buyers_IdentityGUID",
                schema: "Ordering",
                table: "Buyers",
                column: "IdentityGUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderID",
                schema: "Ordering",
                table: "OrderItems",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BuyerID",
                schema: "Ordering",
                table: "Orders",
                column: "BuyerID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatusID",
                schema: "Ordering",
                table: "Orders",
                column: "OrderStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodID",
                schema: "Ordering",
                table: "Orders",
                column: "PaymentMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_BuyerID",
                schema: "Ordering",
                table: "PaymentMethods",
                column: "BuyerID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_CardTypeID",
                schema: "Ordering",
                table: "PaymentMethods",
                column: "CardTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "ClientRequests",
                schema: "Idempotency");

            migrationBuilder.DropTable(
                name: "OrderItems",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "OrderStatuses",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "PaymentMethods",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "Buyers",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "CardTypes",
                schema: "Ordering");

            migrationBuilder.DropSequence(
                name: "buyer_hilo",
                schema: "Ordering");

            migrationBuilder.DropSequence(
                name: "order_hilo",
                schema: "Ordering");

            migrationBuilder.DropSequence(
                name: "orderitems_hilo",
                schema: "Ordering");

            migrationBuilder.DropSequence(
                name: "payment_hilo",
                schema: "Ordering");
        }
    }
}
