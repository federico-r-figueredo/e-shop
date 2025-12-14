using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.API.Infrastructure.CatalogMigrations {
    /// <inheritdoc />
    public partial class Initial : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.EnsureSchema(
                name: "Catalog");

            migrationBuilder.CreateSequence(
                name: "catalog_brands_hilo",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "catalog_items_hilo",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "catalog_types_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "CatalogBrands",
                schema: "Catalog",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_CatalogBrands", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CatalogTypes",
                schema: "Catalog",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_CatalogTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CatalogItems",
                schema: "Catalog",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PictureFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CatalogTypeID = table.Column<int>(type: "int", nullable: false),
                    CatalogBrandID = table.Column<int>(type: "int", nullable: false),
                    AvailableStock = table.Column<int>(type: "int", nullable: false),
                    RestockThreshold = table.Column<int>(type: "int", nullable: false),
                    MaxStockThreshold = table.Column<int>(type: "int", nullable: false),
                    IsOnReorder = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_CatalogItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CatalogItems_CatalogBrands_CatalogBrandID",
                        column: x => x.CatalogBrandID,
                        principalSchema: "Catalog",
                        principalTable: "CatalogBrands",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogItems_CatalogTypes_CatalogTypeID",
                        column: x => x.CatalogTypeID,
                        principalSchema: "Catalog",
                        principalTable: "CatalogTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_CatalogBrandID",
                schema: "Catalog",
                table: "CatalogItems",
                column: "CatalogBrandID");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_CatalogTypeID",
                schema: "Catalog",
                table: "CatalogItems",
                column: "CatalogTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "CatalogItems",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "CatalogBrands",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "CatalogTypes",
                schema: "Catalog");

            migrationBuilder.DropSequence(
                name: "catalog_brands_hilo");

            migrationBuilder.DropSequence(
                name: "catalog_items_hilo");

            migrationBuilder.DropSequence(
                name: "catalog_types_hilo");
        }
    }
}
