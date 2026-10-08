# 🛒 MiniSupermarket System - Web API & WinForms Client

> **Môn học:** Lập trình ứng dụng .NET Core  
> **Buổi 2:** Bảo mật & Phân quyền JWT (JSON Web Token) cho Web API & Kết nối WinForms Client

---

## 📌 1. Giới thiệu & Kiến trúc
Dự án được xây dựng theo mô hình phân tầng **Client - Server**:
- **`MiniSupermarket.API` (Backend):** ASP.NET Core Web API (.NET 8.0), xác thực không trạng thái (Stateless) bằng JWT Token, phân quyền theo vai trò (`Admin`, `Cashier`).
- **`MiniSupermarketWinForms` (Frontend):** Ứng dụng desktop WinForms, gửi `Bearer Token` qua Header HTTP để truy xuất dữ liệu an toàn.

---

## 👥 2. Tài khoản thử nghiệm
| Tài khoản | Mật khẩu | Vai trò (Role) | Quyền hạn |
| :--- | :--- | :--- | :--- |
| **`admin`** | `123456` | **`Admin`** | Toàn quyền: Thêm, Sửa, Xóa danh mục, truy cập Dashboard & POS |
| **`cashier`** | `123456` | **`Cashier`** | Thu ngân: Chỉ Xem, Tìm kiếm danh mục và truy cập màn hình POS |

---

## 🔌 3. Danh sách RESTful API Endpoints

### 🔐 Xác thực (`/api/auth`)
- `POST /api/auth/login`: Đăng nhập bằng `username` và `password`, trả về JWT Token và Role.

### 📦 Danh mục sản phẩm (`/api/categories`)
| Method | Endpoint | Phân quyền | Mô tả |
| :---: | :--- | :---: | :--- |
| `GET` | `/api/categories` | Có Token | Lấy danh sách nhóm hàng |
| `GET` | `/api/categories/{id}` | Có Token | Lấy chi tiết nhóm hàng theo ID |
| `GET` | `/api/categories/search?keyword=...` | Có Token | Tìm kiếm nhóm hàng theo tên |
| `POST` | `/api/categories` | **Chỉ Admin** | Thêm nhóm hàng mới (Cashier bị chặn 403) |
| `PUT` | `/api/categories/{id}` | **Chỉ Admin** | Cập nhật nhóm hàng (Cashier bị chặn 403) |
| `DELETE`| `/api/categories/{id}` | **Chỉ Admin** | Xóa nhóm hàng (Cashier bị chặn 403) |
| `GET` | `/api/categories/staff-pos` | **Admin, Cashier** | Màn hình POS (Cả 2 đều truy cập được) |
| `GET` | `/api/categories/admin-dashboard` | **Chỉ Admin** | Màn hình quản trị (Cashier bị chặn 403) |

---

## 💻 4. Điểm mới trên WinForms Client
1. **`FormLogin`**: Màn hình đăng nhập khởi chạy đầu tiên (`Application.Run(new FormLogin())`).
2. **`SessionManager`**: Lưu trữ `JwtToken`, `CurrentRole`, `CurrentUsername` trong suốt phiên làm việc.
3. **`ApiClientService`**: Tự động đính kèm `Authorization: Bearer <token>` vào mọi yêu cầu HTTP.
4. **`FormCategoryManagement`**:
   - Tự động khóa các nút **Thêm**, **Sửa**, **Xóa** nếu đăng nhập bằng tài khoản `Cashier`.
   - Bổ sung nút kiểm tra nhanh quyền **Staff POS**, **Admin Dashboard** và nút **Đăng xuất**.

---

## 🚀 5. Hướng dẫn chạy và kiểm thử

### Khởi chạy
1. **Backend:** Chạy dự án `MiniSupermarket.API` (F5) $\rightarrow$ Swagger UI mở tại `https://localhost:7006/swagger`.
2. **Frontend:** Chạy dự án `MiniSupermarketWinForms`.

### Kiểm thử nhanh trên WinForms
- Đăng nhập `cashier` / `123456`: Các nút Thêm/Sửa/Xóa bị khóa; bấm **Test POS** thành công (200 OK); bấm **Test Admin Dashboard** bị từ chối (403 Forbidden).
- Đăng xuất và đăng nhập `admin` / `123456`: Toàn bộ các nút Thêm/Sửa/Xóa được kích hoạt; cả 2 nút kiểm tra quyền đều thành công (200 OK).
