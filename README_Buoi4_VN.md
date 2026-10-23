README — Buổi 4 (Refresh JWT, Revoke và Tích hợp Client)

Mục tiêu
-------
Tài liệu Buổi 4: cơ chế xoay vòng refresh token, các endpoint revoke, lưu refresh token vào cơ sở dữ liệu (EF Core), và tích hợp/kiểm thử từ WinForms client.

Nội dung chính
-------------
- Xoay vòng Refresh Token: endpoint /api/Auth/refresh phát một refresh token mới và vô hiệu hóa token cũ.
- Lưu Refresh Token: lưu refresh token trong bảng RefreshTokens trên cơ sở dữ liệu bằng EF Core.
- Các endpoint revoke:
  - POST /api/Auth/revoke        — thu hồi một refresh token (public)
  - POST /api/Auth/revoke-user   — (Admin) thu hồi tất cả token của một người dùng
  - POST /api/Auth/revoke-all    — (Admin) thu hồi toàn bộ refresh token
- Bảo mật: giữ access token thời gian ngắn (ví dụ 15 phút), luôn xoay refresh token, kiểm tra tồn tại và thời hạn của refresh token khi refresh.
- Tích hợp WinForms:
  - SessionManager lưu accessToken, refreshToken và role.
  - ApiClient tự động gắn header Bearer và thử refresh khi gặp 401.
  - Nếu refresh thất bại, fallback sang yêu cầu đăng nhập lại.
- Validation & Filters: factories trả lỗi ModelState tùy chỉnh, ValidateModelAttribute, LogActionFilter, ExecutionTimeFilter.

Hướng dẫn kiểm thử nhanh
-------------------------
- Đăng nhập (lấy access + refresh):
  curl -k -X POST "https://localhost:7006/api/Auth/login" -H "Content-Type: application/json" -d '{"username":"admin","password":"123456"}'

- Refresh (xoay token):
  curl -k -X POST "https://localhost:7006/api/Auth/refresh" -H "Content-Type: application/json" -d '{"refreshToken":"<refreshToken>"}'

- Revoke single:
  curl -k -X POST "https://localhost:7006/api/Auth/revoke" -H "Content-Type: application/json" -d '{"refreshToken":"<refreshToken>"}'

- Revoke-user (Admin):
  curl -k -X POST "https://localhost:7006/api/Auth/revoke-user" -H "Authorization: Bearer <accessToken>" -H "Content-Type: application/json" -d '{"username":"cashier"}'

Ghi chú & xử lý sự cố
--------------------
- Mọi thay đổi model hoặc seed cần tạo migration mới và chạy update-database.
- Nếu file exe bị khoá khi build/run thì dừng tiến trình trước khi chạy migration hoặc build.
- Kiểm tra connection string trong MiniSupermarket.API/appsettings.json để chắc cập nhật đúng database.

Cải tiến đề xuất
----------------
- Lưu refresh token dạng hash thay vì plaintext trong DB.
- Với client web, cân nhắc lưu refresh token trong HttpOnly cookie.
- Thêm unit/integration tests cho AuthController và vòng đời token.


