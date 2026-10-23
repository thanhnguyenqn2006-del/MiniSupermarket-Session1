# BUỔI 6: ENTITY FRAMEWORK CORE NÂNG CAO, PHÂN TRANG LINQ & TÍCH HỢP TOÀN DIỆN POS CLIENT - SERVER

Tài liệu hướng dẫn thực hành và kết quả kiểm thử toàn diện cho Buổi 6:
- **Tiết 1 & 2:** Xây dựng API Phân trang Server-side bằng LINQ (`.Skip()`, `.Take()`, `.CountAsync()`) kết hợp kỹ thuật Eager Loading (`.Include()`, `.AsNoTracking()`) tránh lỗi N+1 Query.
- **Tiết 3:** Kiểm thử các trường hợp phân trang, tìm kiếm từ khóa và lọc nhóm hàng trên Swagger UI; Phân tích câu lệnh T-SQL (`OFFSET x ROWS FETCH NEXT y ROWS ONLY`) được sinh ra bởi EF Core.
- **Tiết 4 & 5:** Kết nối khép kín giao diện WinForms Client (`FormProductManagement.cs`) với Web API: ComboBox nhóm hàng, bảng lưới DataGridView, các nút điều hướng chuyển trang mượt mà không bị treo RAM, và thao tác CRUD đồng bộ xuống CSDL SQL Server.
- **Bài tập mở rộng 6.1:** Endpoint `GET /api/customers/paged` phân trang danh sách khách hàng thân thiết, sắp xếp theo thứ tự giảm dần của điểm thưởng `RewardPoints`.
- **Bài tập mở rộng 6.2:** Bộ lọc sản phẩm theo nhóm hàng (`categoryId`) trên Web API và ComboBox lọc danh mục trên giao diện WinForms.

---

## 1. Danh sách tệp tin đã triển khai (Buổi 6)

| STT | Đường dẫn tệp tin | Mục đích & Nghiệp vụ |
|:---:|---|---|
| 1 | [PagedResponseDto.cs](file:///C:/MiniSupermarket.API/MiniSupermarket.API/Models/PagedResponseDto.cs) | DTO Generic `PagedResult<T>`, DTO sản phẩm kèm nhóm hàng `ProductItemDto`, DTO khách hàng `CustomerItemDto` |
| 2 | [ProductsController.cs](file:///C:/MiniSupermarket.API/MiniSupermarket.API/Controllers/ProductsController.cs) | Endpoint `GET /api/products/paged`: Phân trang LINQ, tìm kiếm theo tên/mã vạch và lọc theo `categoryId` |
| 3 | [CustomersController.cs](file:///C:/MiniSupermarket.API/MiniSupermarket.API/Controllers/CustomersController.cs) | Endpoint `GET /api/customers/paged`: Phân trang khách hàng, sắp xếp giảm dần theo `RewardPoints` (Bài 6.1) |
| 4 | [ApiClientService.cs](file:///C:/MiniSupermarket.API/MiniSupermarketWinForms/ApiClientService.cs) | Cung cấp `ApiClientService.Client`, tự động đính kèm `Bearer JWT Token` và xử lý kết nối SSL |
| 5 | [FormProductManagement.cs](file:///C:/MiniSupermarket.API/MiniSupermarketWinForms/FormProductManagement.cs) | Giao diện quản lý hàng hóa: ComboBox nhóm hàng, bộ lọc nhóm hàng (Bài 6.2), DataGridView phân trang và nút CRUD |
| 6 | [FormCustomerManagement.cs](file:///C:/MiniSupermarket.API/MiniSupermarketWinForms/FormCustomerManagement.cs) | Giao diện khách hàng thân thiết: Tích hợp phân trang LINQ theo điểm thưởng giảm dần |

---

## 2. Danh sách Endpoints Web API (Buổi 6)

| Phương thức | Endpoint URI | Tham số Query | Mô tả chức năng |
|:---:|---|---|---|
| `GET` | `/api/products/paged` | `keyword`, `categoryId`, `pageNumber`, `pageSize` | Phân trang sản phẩm LINQ Server-side, nạp Category qua `.Include()`, hỗ trợ tìm kiếm và lọc |
| `GET` | `/api/customers/paged` | `keyword`, `pageNumber`, `pageSize` | Phân trang khách hàng sắp xếp giảm dần theo `RewardPoints` (Bài 6.1) |
| `POST` | `/api/products` | Body: JSON Product | Thêm mới sản phẩm vào CSDL SQL Server |
| `PUT` | `/api/products/{id}` | Body: JSON Product | Cập nhật thông tin sản phẩm theo ID |
| `DELETE` | `/api/products/{id}` | Path: `id` | Xóa sản phẩm khỏi CSDL (yêu cầu quyền Admin) |

---

## 3. Phân tích câu lệnh T-SQL do EF Core sinh ra (Tiết 3)

Khi Client gọi:
```http
GET /api/products/paged?pageNumber=1&pageSize=5
```
LINQ truy vấn:
```csharp
var items = await _context.Products
    .Include(p => p.Category)
    .AsNoTracking()
    .OrderBy(p => p.ProductId)
    .Skip(0)
    .Take(5)
    .ToListAsync();
```
EF Core tự động biên dịch sang câu lệnh chuẩn của SQL Server:
```sql
SELECT [p].[ProductId], [p].[Barcode], [p].[ProductName], [p].[Price], 
       [p].[StockQuantity], [p].[CategoryId], [c].[CategoryName]
FROM [Products] AS [p]
LEFT JOIN [Categories] AS [c] ON [p].[CategoryId] = [c].[CategoryId]
ORDER BY [p].[ProductId]
OFFSET 0 ROWS FETCH NEXT 5 ROWS ONLY;
```
> **Ý nghĩa kỹ thuật**:
> - Mệnh đề `OFFSET (pageNumber - 1) * pageSize ROWS FETCH NEXT pageSize ROWS ONLY` giúp SQL Server chỉ đọc và truyền tải đúng 5 bản ghi qua đường truyền mạng, không kéo toàn bộ bảng về RAM của máy trạm.
> - Kỹ thuật Eager Loading `LEFT JOIN [Categories]` lấy sẵn `CategoryName` trong 1 truy vấn duy nhất, triệt tiêu 100% lỗi kinh điển **N+1 Query Problem**.

---

## 4. Hướng dẫn chạy thử nghiệm & Đối soát

### Cách 1: Kiểm thử Backend qua Swagger UI
1. Chạy Backend Web API:
   ```powershell
   dotnet run --project MiniSupermarket.API/MiniSupermarket.API.csproj
   ```
2. Mở trình duyệt truy cập: `https://localhost:7006/swagger` (hoặc `http://localhost:5097/swagger`).
3. Mở mục **`GET /api/Products/paged`**:
   - Nhập `pageNumber = 1`, `pageSize = 5`, bấm **Execute** $\rightarrow$ Kết quả trả về 5 sản phẩm đầu tiên, `totalItems: 16`, `totalPages: 4`.
   - Nhập `keyword = "kem"`, bấm **Execute** $\rightarrow$ Bảng lọc chính xác các mặt hàng kem chống nắng/dưỡng ẩm và tính lại `totalPages`.
   - Nhập `categoryId = 1`, bấm **Execute** $\rightarrow$ Chỉ trả về các sản phẩm thuộc nhóm 1.
4. Mở mục **`GET /api/Customers/paged`**:
   - Nhập `pageNumber = 1`, `pageSize = 5`, bấm **Execute** $\rightarrow$ Danh sách khách hàng được sắp xếp theo `RewardPoints` từ cao xuống thấp.

### Cách 2: Kiểm thử Frontend qua WinForms Client
1. Trong Visual Studio, chọn project khởi động là `MiniSupermarketWinForms` $\rightarrow$ Nhấn **F5**.
2. Đăng nhập tài khoản `admin` / `123456`.
3. Bấm vào menu sidebar **"📦 Quản lý Sản phẩm"**:
   - Lưới `dgvProducts` hiển thị đúng 5 sản phẩm của trang 1 kèm nhãn: `Trang 1 / 4 (Tổng: 16 SP)`.
   - Bấm nút **"Trang sau ▶"**: Dữ liệu nạp trang 2 tức thì, nút Trang trước được kích hoạt.
   - Chọn một nhóm hàng trong ComboBox **"📁 Lọc nhóm hàng (B6.2)"**: Bảng tự động tải lại chỉ các mặt hàng thuộc nhóm đã chọn.
   - Click chọn 1 dòng: Dữ liệu tự động đổ lên form nhập liệu. Sửa giá và bấm **"✏️ Cập nhật"** $\rightarrow$ Thông báo thành công và CSDL được cập nhật.

---

## 5. Kết quả nghiệm thu (Checklist Rubric Đạt Chuẩn)

- [x] **Tiêu chí 1 (Backend Paging):** Gọi `GET /api/products/paged?pageNumber=1&pageSize=5` trên Swagger trả về kết quả 200 OK kèm cấu trúc `currentPage`, `totalItems`, `totalPages`, `items`.
- [x] **Tiêu chí 2 (Eager Loading):** Dữ liệu trả về có trường `categoryName` đầy đủ nhờ `.Include(p => p.Category)`, không bị null hay vòng lặp tham chiếu JSON.
- [x] **Tiêu chí 3 (Tìm kiếm & Lọc):** Nhập từ khóa tìm kiếm (VD: "CeraVe") hoặc lọc theo `categoryId` thì bảng lọc đúng và tự động tính lại tổng số trang.
- [x] **Tiêu chí 4 (WinForms Paging):** Bấm nút `◀ Trang trước` và `Trang sau ▶` trên WinForms, lưới `dgvProducts` chuyển đổi mượt mà, không đơ lag hay tốn RAM.
- [x] **Tiêu chí 5 (CRUD Hoàn chỉnh):** Thao tác Thêm, Sửa, Xóa từ màn hình WinForms được cập nhật trực tiếp xuống CSDL SQL Server `BeautyMartDb`.
- [x] **Bài tập mở rộng 6.1:** Đã hoàn thành Endpoint và giao diện phân trang `Customers` sắp xếp theo `RewardPoints` giảm dần.
- [x] **Bài tập mở rộng 6.2:** Đã tích hợp ComboBox lọc theo `categoryId` trên cả Backend API và Form quản lý WinForms.
