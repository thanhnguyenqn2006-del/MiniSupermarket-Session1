using System;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    internal static class Program
    {
        /// <summary>
        /// Điểm khởi chạy chính của ứng dụng WinForms.
        /// Buổi 2: Đặt FormLogin làm form khởi chạy đầu tiên của ứng dụng.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Thay đổi Form khởi chạy đầu tiên là FormLogin thay vì FormCategoryManagement
            Application.Run(new FormLogin());
        }
    }
}
