using Microsoft.EntityFrameworkCore.Migrations;

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations {
    /// <inheritdoc />
    public partial class AddOrderStatusRecords : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.InsertData(
                schema: "Ordering",
                table: "OrderStatuses",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "Submitted" },
                    { 2, "AwaitingValidation" },
                    { 3, "StockConfirmed" },
                    { 4, "Paid" },
                    { 5, "Shipped" },
                    { 6, "Cancelled" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DeleteData(
                schema: "Ordering",
                table: "OrderStatuses",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "Ordering",
                table: "OrderStatuses",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "Ordering",
                table: "OrderStatuses",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "Ordering",
                table: "OrderStatuses",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "Ordering",
                table: "OrderStatuses",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "Ordering",
                table: "OrderStatuses",
                keyColumn: "ID",
                keyValue: 6);
        }
    }
}
