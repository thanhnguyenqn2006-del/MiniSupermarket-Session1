using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[,]
                {
                    { 4, "12 Phố Hoa, Quận 1", "Nguyễn Thị D", "Chuẩn", "0901122335", 20 },
                    { 5, "34 Đường Lý, Quận 3", "Phạm Văn E", "Bạc", "0902233445", 75 },
                    { 6, "56 Ngõ Trường, Quận 5", "Lê Thị F", "Vàng", "0903344556", 200 },
                    { 7, "78 Phố Mới, Quận 7", "Trần Văn G", "Chuẩn", "0904455667", 5 },
                    { 8, "90 Đường Sông, Quận 2", "Hoàng Thị H", "Vàng", "0905566778", 120 },
                    { 9, "101 Khu 9, Quận 10", "Phan Văn I", "Bạc", "0906677889", 60 },
                    { 10, "11 Phố Cũ, Quận 4", "Đỗ Thị K", "Chuẩn", "0907788990", 15 },
                    { 11, "22 Khu Phố, Bình Thạnh", "Vũ Văn L", "Chuẩn", "0908899001", 40 },
                    { 12, "33 Đại Lộ, Tân Bình", "Bùi Thị M", "Bạc", "0909900112", 95 },
                    { 13, "44 Ngõ Lớn, Phú Nhuận", "Hồ Văn N", "Chuẩn", "0910011223", 10 },
                    { 14, "55 Khu 5, Gò Vấp", "Đặng Thị O", "Vàng", "0911122334", 180 },
                    { 15, "66 Đường 66, Thủ Đức", "Lê Văn P", "Chuẩn", "0912233445", 30 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 15);
        }
    }
}
