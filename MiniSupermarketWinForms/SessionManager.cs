namespace MiniSupermarketWinForms
{
    public static class SessionManager
    {
        // Lưu trữ JWT Token nhận từ Server
        public static string JwtToken { get; set; } = string.Empty;

        // Lưu trữ vai trò người dùng (Admin / Cashier / Warehouse) để phân quyền giao diện
        public static string CurrentRole { get; set; } = string.Empty;

        // Lưu trữ tên tài khoản người dùng đăng nhập
        public static string CurrentUsername { get; set; } = string.Empty;

        // Họ và tên đầy đủ (lấy từ API login response — Buổi 4)
        public static string FullName { get; set; } = string.Empty;

        // Lưu trữ Refresh Token nhận từ Server (nếu có)
        public static string RefreshToken { get; set; } = string.Empty;

        // Cờ đánh dấu người dùng đã thực hiện thao tác Đăng xuất
        public static bool IsLoggedOut { get; set; } = false;

        public static void Clear()
        {
            JwtToken        = string.Empty;
            CurrentRole     = string.Empty;
            CurrentUsername = string.Empty;
            FullName        = string.Empty;
            RefreshToken    = string.Empty;
            IsLoggedOut     = false;
        }
    }
}
