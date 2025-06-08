using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web_1773.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Điện thoại" },
                    { 2, "Laptop" },
                    { 3, "Máy tính bảng" },
                    { 4, "Bàn phím" },
                    { 5, "Tai nghe" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "ImageUrls", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 1, "iPhone 15 Pro 256GB", "/images/iphone15pro.jpg", null, "iPhone 15 Pro", 29990000m },
                    { 2, 1, "Galaxy S24 256GB", "/images/galaxy-s24.jpg", null, "Samsung Galaxy S24", 22990000m },
                    { 3, 1, "Xiaomi 14 256GB", "/images/xiaomi14.jpg", null, "Xiaomi 14", 15990000m },
                    { 6, 2, "MacBook Air M3 13inch 256GB", "/images/macbook-air-m3.jpg", null, "MacBook Air M3", 32990000m },
                    { 7, 2, "Dell XPS 13 Intel i7 512GB", "/images/dell-xps13.jpg", null, "Dell XPS 13", 28990000m },
                    { 11, 3, "iPad Air 256GB WiFi", "/images/ipad-air.jpg", null, "iPad Air", 16990000m },
                    { 12, 3, "Galaxy Tab S9 256GB", "/images/galaxy-tab-s9.jpg", null, "Samsung Galaxy Tab S9", 18990000m },
                    { 16, 4, "Bàn phím wireless cao cấp", "/images/logitech-mx-keys.jpg", null, "Logitech MX Keys", 2990000m },
                    { 17, 4, "Bàn phím cơ gaming RGB", "/images/corsair-k95.jpg", null, "Corsair K95", 4990000m },
                    { 21, 5, "Tai nghe chống ồn cao cấp", "/images/sony-wh1000xm5.jpg", null, "Sony WH-1000XM5", 7990000m },
                    { 22, 5, "Tai nghe true wireless", "/images/airpods-pro.jpg", null, "Apple AirPods Pro", 6990000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
