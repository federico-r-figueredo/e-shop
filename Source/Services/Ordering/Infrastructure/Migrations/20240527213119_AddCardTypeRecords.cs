using Microsoft.EntityFrameworkCore.Migrations;

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations {
    /// <inheritdoc />
    public partial class AddCardTypeRecords : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.InsertData(
                schema: "Ordering",
                table: "CardTypes",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "AmericanExpress" },
                    { 2, "Visa" },
                    { 3, "MasterCard" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DeleteData(
                schema: "Ordering",
                table: "CardTypes",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "Ordering",
                table: "CardTypes",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "Ordering",
                table: "CardTypes",
                keyColumn: "ID",
                keyValue: 3);
        }
    }
}
