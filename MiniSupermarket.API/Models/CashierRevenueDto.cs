/*
 * HỌC PHẦN: PHÁT TRIỂN ỨNG DỤNG VỚI .NET CORE & SQL SERVER
 * BUỔI 5: ADO.NET NÂNG CAO, STORED PROCEDURE & GIAO DỊCH POS CHECKOUT
 * TIẾT 3: TỐI ƯU TRUY VẤN VỚI STORED PROCEDURE & BÁO CÁO DOANH THU CA BÁN LẺ
 * Mo ta: DTO hung du lieu ket qua thong ke tu Stored Procedure
 */
namespace MiniSupermarket.API.Models
{
    public class CashierRevenueDto
    {
        // Ten dang nhap cua thu ngan chiu trach nhiem ca ban
        public string CashierUsername { get; set; } = string.Empty;

        // Tong so luong don hang thu ngan da xu ly thanh cong
        public int TotalOrders { get; set; }

        // Tong so tien thu ve cho sieu thi (VND)
        public decimal TotalRevenue { get; set; }

        // Tong diem thuong da tich luy cho khach hang
        public int TotalPointsAwarded { get; set; }
    }

    // DTO hung ket qua Top san pham ban chay (Bai tap 3.2)
    public class TopSellingProductDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalSalesRevenue { get; set; }
    }
}
