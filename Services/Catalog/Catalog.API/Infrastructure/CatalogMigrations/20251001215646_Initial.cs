using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.API.Infrastructure.CatalogMigrations {
    /// <inheritdoc />
    public partial class Initial : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
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
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_CatalogBrands", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CatalogTypes",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_CatalogTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CatalogItems",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PictureFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CatalogTypeID = table.Column<int>(type: "int", nullable: false),
                    CatalogBrandID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_CatalogItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CatalogItems_CatalogBrands_CatalogBrandID",
                        column: x => x.CatalogBrandID,
                        principalTable: "CatalogBrands",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogItems_CatalogTypes_CatalogTypeID",
                        column: x => x.CatalogTypeID,
                        principalTable: "CatalogTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_CatalogBrandID",
                table: "CatalogItems",
                column: "CatalogBrandID");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_CatalogTypeID",
                table: "CatalogItems",
                column: "CatalogTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "CatalogItems");

            migrationBuilder.DropTable(
                name: "CatalogBrands");

            migrationBuilder.DropTable(
                name: "CatalogTypes");

            migrationBuilder.DropSequence(
                name: "catalog_brands_hilo");

            migrationBuilder.DropSequence(
                name: "catalog_items_hilo");

            migrationBuilder.DropSequence(
                name: "catalog_types_hilo");
        }
    }
}
