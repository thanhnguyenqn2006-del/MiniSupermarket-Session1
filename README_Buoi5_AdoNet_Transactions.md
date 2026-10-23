# BUỔI 5: ADO.NET NÂNG CAO, STORED PROCEDURE & GIAO DỊCH POS CHECKOUT

Tài liệu tổng hợp toàn bộ lý thuyết, hướng dẫn thực hành và kết quả kiểm thử thực tế cho Buổi 5:
- **Tiết 1:** Kiến trúc ADO.NET căn bản & 3 bài tập mẫu Console (`ExecuteScalar`, `ExecuteNonQuery` + `SqlParameter`, `SqlDataReader`).
- **Tiết 2:** API Tra cứu Barcode siêu tốc (`SqlDataReader`) & Mô hình ngắt kết nối với `DataTable` & `SqlDataAdapter` (`AdoProductController.cs`).
- **Tiết 3:** Tối ưu hóa truy vấn với Stored Procedure (`sp_GetRevenueByCashier`, `sp_GetRevenueByDateRange`, `sp_GetTop5SellingProducts`) & Báo cáo doanh thu ca bán lẻ (`ReportsController.cs`).
- **Tiết 4:** Chuẩn ACID & Kỹ thuật điều khiển giao dịch với `SqlTransaction` (Mô phỏng Rollback).
- **Tiết 5:** Web API Thanh toán POS đa bảng bọc kín trong `SqlTransaction`, Kịch bản kiểm thử Rollback khi hết kho và API Hủy đơn hoàn trả tồn kho (`CheckoutController.cs`).

---

## 1. Cấu trúc tệp tin đã triển khai

| STT | Đường dẫn tệp tin | Mục đích & Nội dung |
|---|---|---|
| 1 | [Buoi5_AdoNet_StoredProcedures.sql](file:///c:/MiniSupermarket.API/MiniSupermarket.API/Data/Buoi5_AdoNet_StoredProcedures.sql) | Script tạo bảng `Orders`, `OrderDetails`, seed 15 đơn hàng chuẩn và 3 Stored Procedures |
| 2 | [ThucHanhAdoNet/Program.cs](file:///c:/MiniSupermarket.API/ThucHanhAdoNet/Program.cs) | Ứng dụng Console thực hành trọn vẹn Tiết 1 (Bài 1, 2, 3) và Tiết 4 (Mô phỏng Rollback ACID) |
| 3 | [AdoProductController.cs](file:///c:/MiniSupermarket.API/MiniSupermarket.API/Controllers/AdoProductController.cs) | Controller ADO.NET thuần: Tra cứu Barcode nhanh (<100ms), Cache ngoại tuyến qua DataTable, Lọc tồn kho & Đếm theo danh mục |
| 4 | [CashierRevenueDto.cs](file:///c:/MiniSupermarket.API/MiniSupermarket.API/Models/CashierRevenueDto.cs) | DTO hứng dữ liệu Stored Procedure báo cáo doanh thu & Top 5 sản phẩm bán chạy |
| 5 | [ReportsController.cs](file:///c:/MiniSupermarket.API/MiniSupermarket.API/Controllers/ReportsController.cs) | Controller gọi Stored Procedure tổng hợp doanh thu ca làm việc của từng thu ngân |
| 6 | [CheckoutDtos.cs](file:///c:/MiniSupermarket.API/MiniSupermarket.API/Models/CheckoutDtos.cs) | DTO giỏ hàng POS thanh toán (`CartItemCheckoutDto`, `CheckoutRequestDto`) |
| 7 | [CheckoutController.cs](file:///c:/MiniSupermarket.API/MiniSupermarket.API/Controllers/CheckoutController.cs) | API Thanh toán POS và API Hủy hóa đơn bọc toàn bộ chuỗi thao tác trong `SqlTransaction` |

---

## 2. Danh sách Endpoints Web API (Buổi 5)

| Phương thức | Endpoint URI | Mô tả nghiệp vụ | Tiết |
|---|---|---|---|
| `GET` | `/api/ado-products/barcode/{barcode}` | Tra cứu sản phẩm theo mã vạch bằng `SqlDataReader` (Forward-only, cực nhanh) | Tiết 2 |
| `GET` | `/api/ado-products/disconnected-cache` | Tải toàn bộ danh sách sản phẩm lưu đệm bằng `DataTable` & `SqlDataAdapter` | Tiết 2 |
| `GET` | `/api/ado-products/filter-stock?minStock=50` | Lọc sản phẩm có tồn kho thấp `<= minStock` cảnh báo nhập kho | Tiết 2 (Bài 2.1) |
| `GET` | `/api/ado-products/count-by-category` | Thống kê số lượng mặt hàng theo từng danh mục qua `DataTable` | Tiết 2 (Bài 2.2) |
| `GET` | `/api/reports/revenue-by-cashier` | Gọi Stored Procedure `sp_GetRevenueByCashier` xuất báo cáo doanh thu theo thu ngân | Tiết 3 |
| `GET` | `/api/reports/revenue-by-date-range?fromDate=...&toDate=...` | Gọi Stored Procedure `sp_GetRevenueByDateRange` thống kê theo khoảng ngày | Tiết 3 (Bài 3.1) |
| `GET` | `/api/reports/top-selling-products` | Gọi Stored Procedure `sp_GetTop5SellingProducts` lấy top 5 sản phẩm bán chạy nhất | Tiết 3 (Bài 3.2) |
| `POST` | `/api/checkout` | Thanh toán đơn hàng POS đa bảng: Insert Orders $\rightarrow$ OrderDetails $\rightarrow$ Trừ tồn kho $\rightarrow$ Tích điểm khách hàng. Bọc kín trong `SqlTransaction` | Tiết 5 |
| `DELETE` | `/api/checkout/cancel/{orderId}` | Hủy hóa đơn: Hoàn trả tồn kho $\rightarrow$ Trừ điểm khách $\rightarrow$ Xóa OrderDetails & Orders. Bọc trong `SqlTransaction` | Tiết 5 (Bài tập mở rộng) |

---

## 3. Hướng dẫn chạy thử nghiệm & Đối soát

### Cách 1: Chạy Console kiểm chứng ADO.NET & Transaction (Tiết 1 & Tiết 4)
```powershell
# Chạy tất cả 4 bài tập tự động:
dotnet run --project ThucHanhAdoNet/ThucHanhAdoNet.csproj -- --all

# Hoặc chạy menu tương tác:
dotnet run --project ThucHanhAdoNet/ThucHanhAdoNet.csproj
```

### Cách 2: Chạy Web API và kiểm thử qua Swagger UI
1. Nhấn `F5` hoặc chạy lệnh:
   ```powershell
   dotnet run --project MiniSupermarket.API/MiniSupermarket.API.csproj
   ```
2. Mở trình duyệt truy cập: `http://localhost:5097/swagger` (hoặc `https://localhost:7006/swagger`).
3. Thực thi các Endpoint tương ứng trong các nhóm `AdoProduct`, `Reports`, `Checkout`.

---

## 4. Kết quả nghiệm thu (Checklist Rubric Đạt Chuẩn)

- [x] **Tiêu chuẩn 1 (Tiết 1):** Khối `using` giải phóng kết nối tự động, `SqlParameter` triệt tiêu 100% SQL Injection, làm chủ `ExecuteScalar`, `ExecuteNonQuery`, `ExecuteReader`.
- [x] **Tiêu chuẩn 2 (Tiết 2):** Endpoint `GET /api/ado-products/barcode/{barcode}` phản hồi siêu tốc dưới 100ms bằng `SqlDataReader`. Mô hình ngoại tuyến `DataTable` tự động ngắt kết nối an toàn.
- [x] **Tiêu chuẩn 3 (Tiết 3):** Stored Procedure `sp_GetRevenueByCashier` trả về chính xác 5 thu ngân với tổng doanh thu 4,822,000 VND và 478 điểm thưởng.
- [x] **Tiêu chuẩn 4 (Tiết 4):** Trình bày và kiểm chứng cơ chế ACID; `transaction.Rollback()` khôi phục sạch sẽ dữ liệu khi phát sinh Exception.
- [x] **Tiêu chuẩn 5 (Tiết 5):** Endpoint `POST /api/checkout` thực hiện liên hoàn 4 thao tác, trả về mã hóa đơn tự động và điểm tích lũy.
- [x] **Tiêu chuẩn 6 (Tiết 5):** Khi đặt mua vượt quá số lượng tồn kho (VD: 999 đơn vị), API ném mã lỗi 400 và tự động Rollback, không phát sinh hóa đơn rác hay làm lệch tồn kho.
- [x] **Tiêu chuẩn 7 (Tiết 5):** Endpoint `DELETE /api/checkout/cancel/{orderId}` hoàn trả đúng số lượng tồn kho và khấu trừ điểm thưởng chính xác.
