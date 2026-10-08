using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public partial class FormLogin : Form
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;
        private Label lblTitle;
        private Label lblUser;
        private Label lblPass;
        private Label lblHint;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                TextAlign = ContentAlignment.MiddleCenter,
                Left = 20,
                Top = 15,
                Width = 360,
                Height = 30
            };

            this.lblUser = new Label
            {
                Text = "Tài khoản:",
                Font = new Font("Segoe UI", 9.5f),
                Left = 30,
                Top = 60,
                Width = 90,
                Height = 25
            };

            this.txtUser = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Left = 125,
                Top = 57,
                Width = 230
            };

            this.lblPass = new Label
            {
                Text = "Mật khẩu:",
                Font = new Font("Segoe UI", 9.5f),
                Left = 30,
                Top = 98,
                Width = 90,
                Height = 25
            };

            this.txtPass = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Left = 125,
                Top = 95,
                Width = 230,
                UseSystemPasswordChar = true
            };

            this.btnLogin = new Button
            {
                Text = "Đăng nhập hệ thống",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Left = 125,
                Top = 135,
                Width = 230,
                Height = 36,
                Cursor = Cursors.Hand
            };

            this.lblHint = new Label
            {
                Text = "Tài khoản thử nghiệm: admin/123456 (Admin) | cashier/123456 (Cashier)",
                Font = new Font("Segoe UI", 8.25f, FontStyle.Italic),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleCenter,
                Left = 10,
                Top = 180,
                Width = 380,
                Height = 22
            };

            this.btnLogin.Click += btnLogin_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUser);
            this.Controls.Add(txtUser);
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPass);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lblHint);

            this.Text = "Đăng nhập - MiniSupermarket";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(400, 215);
            this.AcceptButton = btnLogin;
        }

        // Sự kiện khi người dùng bấm nút Đăng nhập
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text.Trim();
            string password = txtPass.Text.Trim();

            // Kiểm tra ràng buộc cơ bản phía Client
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnLogin.Enabled = false;
                btnLogin.Text = "Đang xác thực...";

                using (var client = ApiClientService.CreateHttpClient())
                {
                    // Đóng gói dữ liệu gửi lên endpoint POST /api/auth/login
                    var loginData = new { Username = username, Password = password };
                    var response = await client.PostAsJsonAsync("auth/login", loginData);

                    if (response.IsSuccessStatusCode)
                    {
                        // Đọc chuỗi JSON trả về từ Server khi đăng nhập thành công
                        var jsonString = await response.Content.ReadAsStringAsync();
                        using (var doc = JsonDocument.Parse(jsonString))
                        {
                            // Trích xuất Token và Role lưu vào lớp tĩnh SessionManager dùng chung toàn ứng dụng
                            SessionManager.JwtToken = doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
                            SessionManager.CurrentRole = doc.RootElement.GetProperty("role").GetString() ?? string.Empty;
                            SessionManager.CurrentUsername = username;

                            if (doc.RootElement.TryGetProperty("refreshToken", out var rt))
                            {
                                SessionManager.RefreshToken = rt.GetString() ?? string.Empty;
                            }
                        }

                        MessageBox.Show($"Đăng nhập thành công với quyền: {SessionManager.CurrentRole}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Mở Form quản lý chính (FormCategoryManagement) và ẩn Form đăng nhập đi
                        SessionManager.IsLoggedOut = false;
                        FormCategoryManagement mainForm = new FormCategoryManagement();
                        this.Hide();
                        mainForm.ShowDialog();

                        // Nếu người dùng chọn Đăng xuất từ mainForm, hiển thị lại FormLogin
                        if (SessionManager.IsLoggedOut)
                        {
                            txtPass.Clear();
                            this.Show();
                        }
                        else
                        {
                            this.Close(); // Đóng hẳn ứng dụng khi form chính tắt
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối đến Server: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Đăng nhập hệ thống";
            }
        }
    }
}
