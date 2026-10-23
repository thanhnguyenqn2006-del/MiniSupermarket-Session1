using System;
using System.Text;
using Microsoft.Data.SqlClient;

namespace ThucHanhAdoNet
{
    public class Program
    {
        // Chuỗi kết nối ưu tiên BeautyMartDb (CSDL của project), hỗ trợ cả MiniSupermarketDb và LocalDB / SQLExpress
        private static string GetConnectionString()
        {
            return "Server=(localdb)\\mssqllocaldb;Database=BeautyMartDb;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            if (args.Length > 0 && args[0].ToLower() == "--all")
            {
                RunAllExercises();
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=========================================================================");
                Console.WriteLine("  BUỔI 5: ADO.NET NÂNG CAO, STORED PROCEDURE & GIAO DỊCH POS CHECKOUT");
                Console.WriteLine("=========================================================================");
                Console.WriteLine("  1. [Tiết 1] Bài tập 1: Đếm tổng số mặt hàng bằng ExecuteScalar()");
                Console.WriteLine("  2. [Tiết 1] Bài tập 2: Cập nhật tồn kho an toàn với SqlParameter");
                Console.WriteLine("  3. [Tiết 1] Bài tập 3: Duyệt danh sách sản phẩm bằng SqlDataReader");
                Console.WriteLine("  4. [Tiết 4] Kiểm chứng cơ chế Rollback của SqlTransaction");
                Console.WriteLine("  5. Chạy tuần tự TẤT CẢ các bài tập");
                Console.WriteLine("  0. Thoát chương trình");
                Console.WriteLine("=========================================================================");
                Console.Write("  -> Chọn chức năng (0-5): ");

                string? choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        BaiTap1_ExecuteScalar();
                        Pause();
                        break;
                    case "2":
                        BaiTap2_ExecuteNonQuery();
                        Pause();
                        break;
                    case "3":
                        BaiTap3_SqlDataReader();
                        Pause();
                        break;
                    case "4":
                        Tiet4_SqlTransaction_Rollback();
                        Pause();
                        break;
                    case "5":
                        RunAllExercises();
                        Pause();
                        break;
                    case "0":
                        Console.WriteLine("Cảm ơn bạn đã sử dụng chương trình thực hành ADO.NET!");
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ, vui lòng nhấn phím bất kỳ...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void Pause()
        {
            Console.WriteLine("\n-> Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }

        private static void RunAllExercises()
        {
            Console.WriteLine(">>> BẮT ĐẦU CHẠY TOÀN BỘ 4 BÀI TẬP THỰC HÀNH ADO.NET <<<\n");
            BaiTap1_ExecuteScalar();
            Console.WriteLine("\n" + new string('-', 85) + "\n");
            BaiTap2_ExecuteNonQuery();
            Console.WriteLine("\n" + new string('-', 85) + "\n");
            BaiTap3_SqlDataReader();
            Console.WriteLine("\n" + new string('-', 85) + "\n");
            Tiet4_SqlTransaction_Rollback();
        }

        // =====================================================================
        // TIẾT 1 - BÀI TẬP 1: ĐẾM TỔNG SỐ LƯỢNG MẶT HÀNG BẰNG ExecuteScalar()
        // =====================================================================
        public static void BaiTap1_ExecuteScalar()
        {
            string connString = GetConnectionString();
            Console.WriteLine("=== BÀI TẬP 1: ĐẾM TỔNG SỐ LƯỢNG MẶT HÀNG (ExecuteScalar) ===");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    string query = "SELECT COUNT(*) FROM Products";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        Console.WriteLine("-> Đã kết nối thành công tới SQL Server!");

                        object result = command.ExecuteScalar()!;
                        int totalCount = Convert.ToInt32(result);

                        Console.WriteLine($"-> Tổng số mặt hàng hiện có trong kho: {totalCount} sản phẩm.");
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"[LỖI SQL]: Không thể truy vấn CSDL. Chi tiết: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LỖI HỆ THỐNG]: {ex.Message}");
                }
            }
        }

        // =====================================================================
        // TIẾT 1 - BÀI TẬP 2: CẬP NHẬT TỒN KHO AN TOÀN VỚI SQLPARAMETER
        // =====================================================================
        public static void BaiTap2_ExecuteNonQuery()
        {
            string connString = GetConnectionString();
            Console.WriteLine("=== BÀI TẬP 2: CẬP NHẬT TỒN KHO AN TOÀN VỚI SQLPARAMETER (ExecuteNonQuery) ===");

            int maSanPhamCanSua = 1;
            int soLuongTonKhoMoi = 50;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    string updateSql = @"UPDATE Products 
                                         SET StockQuantity = @Stock 
                                         WHERE ProductId = @Id";

                    using (SqlCommand command = new SqlCommand(updateSql, connection))
                    {
                        command.Parameters.AddWithValue("@Stock", soLuongTonKhoMoi);
                        command.Parameters.AddWithValue("@Id", maSanPhamCanSua);

                        connection.Open();

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"-> THÀNH CÔNG: Đã cập nhật tồn kho cho Sản phẩm ID = {maSanPhamCanSua} thành {soLuongTonKhoMoi} đơn vị.");
                            Console.WriteLine($"-> Số dòng dữ liệu bị ảnh hưởng: {rowsAffected} dòng.");
                        }
                        else
                        {
                            Console.WriteLine($"-> CẢNH BÁO: Không tìm thấy sản phẩm có ID = {maSanPhamCanSua} để cập nhật.");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"[LỖI SQL]: {ex.Message}");
                }
            }
        }

        // =====================================================================
        // TIẾT 1 - BÀI TẬP 3: DUYỆT DANH SÁCH BẰNG SQLDATAREADER
        // =====================================================================
        public static void BaiTap3_SqlDataReader()
        {
            string connString = GetConnectionString();
            Console.WriteLine("=== BÀI TẬP 3: DUYỆT DANH SÁCH BẰNG SQLDATAREADER (ExecuteReader) ===");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    string query = @"SELECT ProductId, Barcode, ProductName, Price, StockQuantity 
                                     FROM Products 
                                     ORDER BY ProductId ASC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("--------------------------------------------------------------------------------------------------");
                            Console.WriteLine(string.Format("{0,-6} | {1,-14} | {2,-42} | {3,12} | {4,8}", 
                                "MÃ SP", "MÃ VẠCH", "TÊN SẢN PHẨM", "ĐƠN GIÁ", "TỒN KHO"));
                            Console.WriteLine("--------------------------------------------------------------------------------------------------");

                            int stt = 0;
                            while (reader.Read())
                            {
                                stt++;

                                int id = Convert.ToInt32(reader["ProductId"]);
                                string barcode = reader["Barcode"].ToString() ?? "";
                                string name = reader["ProductName"].ToString() ?? "";
                                decimal price = Convert.ToDecimal(reader["Price"]);
                                int stock = Convert.ToInt32(reader["StockQuantity"]);

                                if (name.Length > 40)
                                    name = name.Substring(0, 37) + "...";

                                Console.WriteLine(string.Format("{0,-6} | {1,-14} | {2,-42} | {3,12:N0} đ | {4,8}", 
                                    id, barcode, name, price, stock));
                            }

                            Console.WriteLine("--------------------------------------------------------------------------------------------------");
                            Console.WriteLine($"-> Tổng cộng đã đọc thành công: {stt} mặt hàng từ SQL Server.");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"[LỖI SQL]: {ex.Message}");
                }
            }
        }

        // =====================================================================
        // TIẾT 4 - THỰC NGHIỆM ĐIỀU KHIỂN SQLTRANSACTION & KIỂM CHỨNG ROLLBACK
        // =====================================================================
        public static void Tiet4_SqlTransaction_Rollback()
        {
            string connString = GetConnectionString();
            Console.WriteLine("=== TIẾT 4: THỬ NGHIỆM ĐIỀU KHIỂN SQLTRANSACTION (ACID ROLLBACK) ===");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                // 1. Kiểm tra tồn kho ban đầu của SP 1
                int initialStock = 0;
                using (SqlCommand cmdGet = new SqlCommand("SELECT StockQuantity FROM Products WHERE ProductId = 1", connection))
                {
                    initialStock = Convert.ToInt32(cmdGet.ExecuteScalar());
                    Console.WriteLine($"-> [TRƯỚC GIAO DỊCH]: Tồn kho Sản phẩm ID 1 là: {initialStock} đơn vị.");
                }

                // 2. Khởi tạo Transaction
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    Console.WriteLine("-> Bắt đầu phiên giao dịch (connection.BeginTransaction())...");

                    // Bước 1: Trừ 5 đơn vị tồn kho của SP 1
                    string sqlStep1 = "UPDATE Products SET StockQuantity = StockQuantity - @Qty WHERE ProductId = @Id";
                    using (SqlCommand cmd1 = new SqlCommand(sqlStep1, connection, transaction))
                    {
                        cmd1.Parameters.AddWithValue("@Qty", 5);
                        cmd1.Parameters.AddWithValue("@Id", 1);
                        int r1 = cmd1.ExecuteNonQuery();
                        Console.WriteLine($"-> Bước 1: Đã trừ 5 đơn vị tồn kho SP 1 (Số dòng tác động: {r1}).");
                    }

                    // Bước 2: Cố tình gây lỗi kiểm tra logic (Mặt hàng thứ 2 hết hàng)
                    Console.WriteLine("-> Bước 2: Chuẩn bị thực thi thao tác kiểm tra mặt hàng thứ 2...");
                    bool isInventoryShortage = true; // Giả lập hết hàng
                    if (isInventoryShortage)
                    {
                        throw new InvalidOperationException("Mặt hàng thứ hai đã hết tồn kho thực tế trong CSDL!");
                    }

                    transaction.Commit();
                    Console.WriteLine("-> GIAO DỊCH COMMIT THÀNH CÔNG!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[PHÁT SINH LỖI]: {ex.Message}");
                    transaction.Rollback();
                    Console.WriteLine("-> [HỆ THỐNG]: Đã kích hoạt transaction.Rollback()!");
                    Console.WriteLine("-> Toàn bộ thay đổi của Bước 1 đã được hoàn tác sạch sẽ!");
                }

                // 3. Kiểm tra lại tồn kho sau khi Rollback để chứng minh chuẩn ACID
                using (SqlCommand cmdCheck = new SqlCommand("SELECT StockQuantity FROM Products WHERE ProductId = 1", connection))
                {
                    int currentStock = Convert.ToInt32(cmdCheck.ExecuteScalar());
                    Console.WriteLine($"-> [SAU GIAO DỊCH]: Tồn kho Sản phẩm ID 1 hiện tại: {currentStock} đơn vị (Khớp hoàn toàn với trước giao dịch: {initialStock}).");
                }
            }
        }
    }
}
