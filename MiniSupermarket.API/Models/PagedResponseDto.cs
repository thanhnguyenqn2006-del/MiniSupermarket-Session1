/*
 * HỌC PHẦN: PHÁT TRIỂN ỨNG DỤNG VỚI .NET CORE & SQL SERVER
 * BUỔI 6: ENTITY FRAMEWORK CORE NÂNG CAO, PHÂN TRANG LINQ & TÍCH HỢP TOÀN DIỆN POS CLIENT - SERVER
 * TIẾT 1 & 2: DTO PHÂN TRANG GENERIC VÀ CHI TIẾT SẢN PHẨM / KHÁCH HÀNG
 */
using System.Collections.Generic;

namespace MiniSupermarket.API.Models
{
    // DTO chi tiết từng sản phẩm trả về kèm tên nhóm hàng (Eager Loading)
    public class ProductItemDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; // Lấy từ bảng Categories qua Eager Loading .Include()
    }

    // DTO phân trang cho Khách hàng thân thiết (Bài tập mở rộng 6.1)
    public class CustomerItemDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int RewardPoints { get; set; }
        public string MembershipRank { get; set; } = string.Empty;
    }

    // DTO bao bọc kết quả phân trang tổng quát (Generic Server-side Pagination)
    public class PagedResult<T>
    {
        public int CurrentPage { get; set; }       // Trang hiện tại (1, 2, 3...)
        public int PageSize { get; set; }          // Số phần tử trên mỗi trang (ví dụ: 5 dòng)
        public int TotalItems { get; set; }         // Tổng số phần tử thỏa mãn điều kiện lọc
        public int TotalPages { get; set; }         // Tổng số trang = Ceiling(TotalItems / PageSize)
        public List<T> Items { get; set; } = new(); // Danh sách dữ liệu của trang hiện hành
    }
}
