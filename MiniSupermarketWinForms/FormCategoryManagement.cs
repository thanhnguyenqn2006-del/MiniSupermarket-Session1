using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public partial class FormCategoryManagement : Form
    {
        public FormCategoryManagement()
        {
            InitializeComponent();
        }

        // =========================================================
        // PHƯƠNG THỨC CẤU HÌNH HTTPCLIENT CÓ GẮN BEARER TOKEN (BƯỚC 4)
        // =========================================================
        private HttpClient GetAuthenticatedClient()
        {
            var client = ApiClientService.CreateHttpClient();

            // Đính kèm Token vào Header theo chuẩn Bearer Authentication
            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            }

            return client;
        }

        // =========================================================
        // KHI FORM ĐƯỢC MỞ
        // =========================================================
        private async void FormCategoryManagement_Load(object sender, EventArgs e)
        {
            SetupToolStripFeatures();
            ApplyRolePermissions();
            await LoadDataAsync();
        }

        // =========================================================
        // THIẾT LẬP TOOLSTRIP: THÔNG TIN USER, TEST PHÂN QUYỀN & ĐĂNG XUẤT
        // =========================================================
        private void SetupToolStripFeatures()
        {
            toolStrip1.Items.Clear();

            var lblUser = new ToolStripLabel
            {
                Text = $"👤 Đang đăng nhập: {SessionManager.CurrentUsername} | Vai trò: {SessionManager.CurrentRole}",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            var sep1 = new ToolStripSeparator();

            var btnStaffPos = new ToolStripButton
            {
                Text = "🛒 Test POS (Cả 2 quyền)",
                ToolTipText = "Kiểm tra quyền GET /api/categories/staff-pos (Admin & Cashier)"
            };
            btnStaffPos.Click += async (s, e) => await TestStaffPosAsync();

            var btnAdminDash = new ToolStripButton
            {
                Text = "⚙️ Test Admin Dashboard (Chỉ Admin)",
                ToolTipText = "Kiểm tra quyền GET /api/categories/admin-dashboard"
            };
            btnAdminDash.Click += async (s, e) => await TestAdminDashboardAsync();

            var sep2 = new ToolStripSeparator();

            var btnLogout = new ToolStripButton
            {
                Text = "🚪 Đăng xuất",
                ForeColor = Color.DarkRed,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnLogout.Click += (s, e) => Logout();

            toolStrip1.Items.Add(lblUser);
            toolStrip1.Items.Add(sep1);
            toolStrip1.Items.Add(btnStaffPos);
            toolStrip1.Items.Add(btnAdminDash);
            toolStrip1.Items.Add(sep2);
            toolStrip1.Items.Add(btnLogout);
        }

        // =========================================================
        // PHÂN QUYỀN GIAO DIỆN THEO VAI TRÒ (ADMIN VS CASHIER)
        // =========================================================
        private void ApplyRolePermissions()
        {
            var isAdmin = string.Equals(SessionManager.CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase);

            // Chỉ Admin mới được quyền Thêm, Sửa, Xóa nhóm hàng
            btnAdd.Enabled = isAdmin;
            btnUpdate.Enabled = isAdmin;
            btnDelete.Enabled = isAdmin;

            if (!isAdmin)
            {
                this.Text = "Quản lý Danh mục Nhóm hàng - [Chế độ Xem & Tìm kiếm (Cashier)]";
            }
            else
            {
                this.Text = "Quản lý Danh mục Nhóm hàng - [Toàn quyền Quản trị (Admin)]";
            }
        }

        // =========================================================
        // KIỂM THỬ ENDPOINT STAFF-POS (ADMIN & CASHIER ĐỀU GỌI ĐƯỢC)
        // =========================================================
        private async Task TestStaffPosAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.GetAsync("categories/staff-pos");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    var msg = doc.RootElement.GetProperty("message").GetString();
                    MessageBox.Show($"[200 OK] Thành công!\nPhản hồi từ Server:\n{msg}", "Kiểm thử quyền Staff POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Lỗi HTTP {(int)response.StatusCode} {response.StatusCode}", "Kiểm thử thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // KIỂM THỬ ENDPOINT ADMIN-DASHBOARD (CHỈ ADMIN MỚI GỌI ĐƯỢC)
        // =========================================================
        private async Task TestAdminDashboardAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.GetAsync("categories/admin-dashboard");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    var msg = doc.RootElement.GetProperty("message").GetString();
                    MessageBox.Show($"[200 OK] Thành công!\nPhản hồi từ Server:\n{msg}", "Kiểm thử quyền Admin Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("[403 Forbidden] BỊ TỪ CHỐI TRUY CẬP!\nTài khoản hiện tại không có quyền Admin để truy cập Admin Dashboard.", "Phân quyền thành công", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("[401 Unauthorized] Chưa đăng nhập hoặc Token không hợp lệ!", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Lỗi HTTP {(int)response.StatusCode} {response.StatusCode}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // ĐĂNG XUẤT HỆ THỐNG
        // =========================================================
        private void Logout()
        {
            var confirm = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                SessionManager.IsLoggedOut = true;
                SessionManager.Clear();
                this.Close();
            }
        }

        // =========================================================
        // 1. TẢI DỮ LIỆU DANH SÁCH NHÓM HÀNG (GET /api/categories)
        // =========================================================
        private async Task LoadDataAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var categories = await client.GetFromJsonAsync<List<CategoryDto>>("categories");
                dgvCategories.DataSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi quyền truy cập hoặc mất kết nối: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // 2. CLICK DÒNG TRÊN DATAGRIDVIEW
        // =========================================================
        private void dgvCategories_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvCategories.Rows[e.RowIndex];

            txtId.Text = row.Cells["CategoryId"].Value?.ToString() ?? "";
            txtCategoryName.Text = row.Cells["CategoryName"].Value?.ToString() ?? "";
            txtDescription.Text = row.Cells["Description"].Value?.ToString() ?? "";
        }

        // =========================================================
        // 3. NÚT TẢI LẠI
        // =========================================================
        private async void btnLoad_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        // =========================================================
        // 4. THÊM MỚI (POST /api/categories)
        // =========================================================
        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhóm hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var client = GetAuthenticatedClient();
                var newCategory = new
                {
                    CategoryName = txtCategoryName.Text.Trim(),
                    Description = txtDescription.Text.Trim()
                };

                var response = await client.PostAsJsonAsync("categories", newCategory);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm nhóm hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                    ClearInputs();
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("[403 Forbidden] Bạn không có quyền thêm mới nhóm hàng! (Chỉ dành cho Admin)", "Bị từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("[401 Unauthorized] Phiên làm việc hết hạn hoặc chưa đăng nhập!", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Thêm thất bại!\nHTTP {(int)response.StatusCode}\n{message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // 5. CẬP NHẬT (PUT /api/categories/{id})
        // =========================================================
        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng cần cập nhật!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Tên nhóm hàng không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var client = GetAuthenticatedClient();
                var updateCategory = new
                {
                    CategoryId = id,
                    CategoryName = txtCategoryName.Text.Trim(),
                    Description = txtDescription.Text.Trim()
                };

                var response = await client.PutAsJsonAsync($"categories/{id}", updateCategory);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                    ClearInputs();
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("[403 Forbidden] Bạn không có quyền cập nhật nhóm hàng! (Chỉ dành cho Admin)", "Bị từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("[401 Unauthorized] Phiên làm việc hết hạn hoặc chưa đăng nhập!", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Cập nhật thất bại!\nHTTP {(int)response.StatusCode}\n{message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // 6. XÓA (DELETE /api/categories/{id})
        // =========================================================
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa nhóm hàng ID = {id}?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.DeleteAsync($"categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                    ClearInputs();
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("[403 Forbidden] Bạn không có quyền xóa nhóm hàng! (Chỉ dành cho Admin)", "Bị từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("[401 Unauthorized] Phiên làm việc hết hạn hoặc chưa đăng nhập!", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Xóa thất bại!\nHTTP {(int)response.StatusCode}\n{message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // 7. TÌM KIẾM (GET /api/categories/search?keyword=...)
        // =========================================================
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtKeyword.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                await LoadDataAsync();
                return;
            }

            try
            {
                using var client = GetAuthenticatedClient();
                string url = $"categories/search?keyword=" + Uri.EscapeDataString(keyword);

                var response = await client.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Yêu cầu đăng nhập để tiếp tục.", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Không thể tìm kiếm!\nHTTP {(int)response.StatusCode}\n{message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
                dgvCategories.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tìm kiếm!\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // XÓA TRẮNG Ô NHẬP LIỆU
        // =========================================================
        private void ClearInputs()
        {
            txtId.Clear();
            txtCategoryName.Clear();
            txtDescription.Clear();
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }
    }

    // =============================================================
    // DTO DÀNH CHO WINFORMS CLIENT
    // =============================================================
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}