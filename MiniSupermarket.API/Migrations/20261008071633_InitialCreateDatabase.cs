using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RewardPoints = table.Column<int>(type: "int", nullable: false),
                    MembershipRank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "Description" },
                values: new object[,]
                {
                    { 1, "Chăm sóc da & Skincare", "Sữa rửa mặt, toner, serum, kem dưỡng ẩm, kem chống nắng" },
                    { 2, "Trang điểm & Makeup", "Son môi, kem nền, phấn phủ, mascara, kẻ mắt" },
                    { 3, "Chăm sóc tóc & Cơ thể", "Dầu gội, dầu xả, sữa tắm, dưỡng thể, tẩy tế bào chết" },
                    { 4, "Nước hoa & Xịt thơm", "Nước hoa cao cấp EDP, EDT, xịt toàn thân body mist" },
                    { 5, "Hóa phẩm & Vệ sinh cá nhân", "Nước tẩy trang, bông tẩy trang, nước giặt đồ lót, dung dịch vệ sinh" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[,]
                {
                    { 1, "123 Lê Lợi, Quận 1, TP.HCM", "Nguyễn Văn A", "Vàng", "0901122334", 150 },
                    { 2, "45 Hai Bà Trưng, Hà Nội", "Trần Thị B", "Bạc", "0918877665", 50 },
                    { 3, "78 Nguyễn Huệ, Đà Nẵng", "Lê Văn C", "Chuẩn", "0983344556", 10 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "893123456001", 1, 285000m, "Sữa rửa mặt CeraVe Foaming Cleanser 236ml", 50 },
                    { 2, "893123456002", 1, 195000m, "Serum The Ordinary Niacinamide 10% + Zinc 1% 30ml", 40 },
                    { 3, "893123456003", 2, 290000m, "Son kem lì 3CE Velvet Lip Tint", 35 },
                    { 4, "893123456004", 3, 165000m, "Dầu gội bưởi Cocoon phục hồi tóc 310ml", 60 },
                    { 5, "893123456005", 5, 380000m, "Nước tẩy trang Bioderma Sensibio H2O 500ml", 45 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
