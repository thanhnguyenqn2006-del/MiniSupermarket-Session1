using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreProductsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "ProductName",
                value: "Sữa rửa mặt CeraVe Foaming Facial Cleanser 236ml");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                columns: new[] { "CategoryId", "Price", "ProductName" },
                values: new object[] { 1, 420000m, "Kem chống nắng Anessa Perfect UV Sunscreen SPF50+ 60g" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                columns: new[] { "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { 1, 680000m, "Toner Paula's Choice BHA Liquid Exfoliant 118ml", 25 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                columns: new[] { "CategoryId", "Price", "ProductName" },
                values: new object[] { 1, 310000m, "Kem dưỡng ẩm Neutrogena Hydro Boost Water Gel 50ml" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[,]
                {
                    { 6, "893123456006", 2, 290000m, "Son kem lì 3CE Velvet Lip Tint #Reddish", 55 },
                    { 7, "893123456007", 2, 175000m, "Kem nền Maybelline Fit Me Matte+Poreless 30ml SPF22", 60 },
                    { 8, "893123456008", 2, 145000m, "Phấn phủ kiềm dầu Innisfree No-Sebum Mineral Powder 5g", 70 },
                    { 9, "893123456009", 2, 195000m, "Mascara L'Oreal Lash Paradise Waterproof 6.4ml", 40 },
                    { 10, "893123456010", 3, 165000m, "Dầu gội bưởi Cocoon phục hồi tóc hư tổn 310ml", 60 },
                    { 11, "893123456011", 3, 89000m, "Dầu xả TRESemmé Keratin Smooth dưỡng tóc suôn mượt 340ml", 55 },
                    { 12, "893123456012", 3, 95000m, "Sữa tắm Dove Deeply Nourishing Body Wash 500ml", 80 },
                    { 13, "893123456013", 3, 135000m, "Dưỡng thể St. Ives Energizing Coconut & Coffee 200ml", 45 },
                    { 14, "893123456014", 4, 750000m, "Nước hoa Victoria's Secret Bombshell EDP 50ml", 20 },
                    { 15, "893123456015", 4, 320000m, "Xịt thơm toàn thân Bath & Body Works Japanese Cherry Blossom 236ml", 30 },
                    { 16, "893123456016", 5, 380000m, "Nước tẩy trang Bioderma Sensibio H2O Micellar Water 500ml", 45 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "ProductName",
                value: "Sữa rửa mặt CeraVe Foaming Cleanser 236ml");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                columns: new[] { "CategoryId", "Price", "ProductName" },
                values: new object[] { 2, 290000m, "Son kem lì 3CE Velvet Lip Tint" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                columns: new[] { "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[] { 3, 165000m, "Dầu gội bưởi Cocoon phục hồi tóc 310ml", 60 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                columns: new[] { "CategoryId", "Price", "ProductName" },
                values: new object[] { 5, 380000m, "Nước tẩy trang Bioderma Sensibio H2O 500ml" });
        }
    }
}
