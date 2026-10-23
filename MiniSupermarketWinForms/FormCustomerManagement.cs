using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public partial class FormCustomerManagement : Form
    {
        private DataGridView dgvCustomers;
        private TextBox txtCustomerId;
        private TextBox txtCustomerName;
        private TextBox txtPhoneNumber;
        private TextBox txtAddress;
        private TextBox txtRewardPoints;
        private ComboBox cboMembershipRank;
        private TextBox txtKeyword;
        private Button btnSearch;
        private Button btnLoad;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        private GroupBox grpInfo;
        private GroupBox grpList;

        // Phân trang Khách hàng (Bài tập mở rộng 6.1)
        private int _currentPage = 1;
        private readonly int _pageSize = 5;
        private int _totalPages = 1;
        private Button btnPrev;
        private Label lblPaging;
        private Button btnNext;

        public FormCustomerManagement()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Khách hàng thân thiết (Phân trang LINQ - Buổi 6)";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F);

            // Group Search
            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(10) };
            var lblSearch = new Label { Text = "Tìm kiếm:", Left = 15, Top = 18, Width = 70 };
            this.txtKeyword = new TextBox { Left = 90, Top = 15, Width = 280, Font = new Font("Segoe UI", 10F) };
            this.btnSearch = new Button
            {
                Text = "🔍 Tìm kiếm",
                Left = 380,
                Top = 13,
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            this.btnLoad = new Button
            {
                Text = "🔄 Tải lại",
                Left = 490,
                Top = 13,
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            pnlSearch.Controls.Add(lblSearch);
            pnlSearch.Controls.Add(txtKeyword);
            pnlSearch.Controls.Add(btnSearch);
            pnlSearch.Controls.Add(btnLoad);

            // Group List
            this.grpList = new GroupBox
            {
                Text = "Danh sách Khách hàng thân thiết (Sắp xếp theo Điểm thưởng giảm dần)",
                Left = 15,
                Top = 60,
                Width = 590,
                Height = 490,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            this.dgvCustomers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White
            };

            var pnlPaging = new Panel { Dock = DockStyle.Bottom, Height = 45 };
            btnPrev = new Button
            {
                Text = "◀ Trang trước", Left = 10, Top = 8, Width = 110, Height = 30,
                BackColor = Color.FromArgb(41, 100, 180), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
            };
            lblPaging = new Label
            {
                Text = "Trang 1 / 1", Left = 130, Top = 14, Width = 180,
                TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnNext = new Button
            {
                Text = "Trang sau ▶", Left = 320, Top = 8, Width = 110, Height = 30,
                BackColor = Color.FromArgb(41, 100, 180), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
            };
            pnlPaging.Controls.AddRange(new Control[] { btnPrev, lblPaging, btnNext });

            this.grpList.Controls.Add(dgvCustomers);
            this.grpList.Controls.Add(pnlPaging);

            // Group Info
            this.grpInfo = new GroupBox
            {
                Text = "Thông tin Khách hàng",
                Left = 620,
                Top = 60,
                Width = 350,
                Height = 460,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };

            var lblId = new Label { Text = "Mã KH:", Left = 20, Top = 35, Width = 90 };
            this.txtCustomerId = new TextBox { Left = 120, Top = 32, Width = 200, ReadOnly = true, BackColor = Color.FromArgb(240, 240, 240) };

            var lblName = new Label { Text = "Họ và tên (*):", Left = 20, Top = 75, Width = 90 };
            this.txtCustomerName = new TextBox { Left = 120, Top = 72, Width = 200 };

            var lblPhone = new Label { Text = "Số ĐT (*):", Left = 20, Top = 115, Width = 90 };
            this.txtPhoneNumber = new TextBox { Left = 120, Top = 112, Width = 200 };

            var lblAddr = new Label { Text = "Địa chỉ:", Left = 20, Top = 155, Width = 90 };
            this.txtAddress = new TextBox { Left = 120, Top = 152, Width = 200, Multiline = true, Height = 55 };

            var lblPoints = new Label { Text = "Điểm thưởng:", Left = 20, Top = 225, Width = 90 };
            this.txtRewardPoints = new TextBox { Text = "0", Left = 120, Top = 222, Width = 200 };

            var lblRank = new Label { Text = "Hạng thẻ:", Left = 20, Top = 265, Width = 90 };
            this.cboMembershipRank = new ComboBox { Left = 120, Top = 262, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            this.cboMembershipRank.Items.AddRange(new object[] { "Chuẩn", "Bạc", "Vàng", "Kim Cương" });
            this.cboMembershipRank.SelectedIndex = 0;

            this.btnAdd = new Button
            {
                Text = "➕ Thêm mới",
                Left = 20,
                Top = 320,
                Width = 95,
                Height = 35,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            this.btnUpdate = new Button
            {
                Text = "✏️ Cập nhật",
                Left = 125,
                Top = 320,
                Width = 95,
                Height = 35,
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            this.btnDelete = new Button
            {
                Text = "🗑️ Xóa",
                Left = 230,
                Top = 320,
                Width = 95,
                Height = 35,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            this.btnClear = new Button
            {
                Text = "Xóa trắng",
                Left = 125,
                Top = 370,
                Width = 95,
                Height = 30,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            grpInfo.Controls.Add(lblId);
            grpInfo.Controls.Add(txtCustomerId);
            grpInfo.Controls.Add(lblName);
            grpInfo.Controls.Add(txtCustomerName);
            grpInfo.Controls.Add(lblPhone);
            grpInfo.Controls.Add(txtPhoneNumber);
            grpInfo.Controls.Add(lblAddr);
            grpInfo.Controls.Add(txtAddress);
            grpInfo.Controls.Add(lblPoints);
            grpInfo.Controls.Add(txtRewardPoints);
            grpInfo.Controls.Add(lblRank);
            grpInfo.Controls.Add(cboMembershipRank);
            grpInfo.Controls.Add(btnAdd);
            grpInfo.Controls.Add(btnUpdate);
            grpInfo.Controls.Add(btnDelete);
            grpInfo.Controls.Add(btnClear);

            this.Controls.Add(pnlSearch);
            this.Controls.Add(grpList);
            this.Controls.Add(grpInfo);

            // Events
            this.Load += async (s, e) => await LoadDataAsync();
            this.dgvCustomers.CellClick += DgvCustomers_CellClick;
            this.btnLoad.Click += async (s, e) => { txtKeyword.Clear(); _currentPage = 1; await LoadDataAsync(); };
            this.btnSearch.Click += async (s, e) => { _currentPage = 1; await LoadDataAsync(); };
            this.txtKeyword.KeyDown += async (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; _currentPage = 1; await LoadDataAsync(); } };
            this.btnAdd.Click += async (s, e) => await AddCustomerAsync();
            this.btnUpdate.Click += async (s, e) => await UpdateCustomerAsync();
            this.btnDelete.Click += async (s, e) => await DeleteCustomerAsync();
            this.btnClear.Click += (s, e) => ClearInputs();

            // Paging events
            this.btnPrev.Click += async (s, e) =>
            {
                if (_currentPage > 1)
                {
                    _currentPage--;
                    await LoadDataAsync();
                }
            };
            this.btnNext.Click += async (s, e) =>
            {
                if (_currentPage < _totalPages)
                {
                    _currentPage++;
                    await LoadDataAsync();
                }
            };

            // Phân quyền theo vai trò (Buổi 2 + Buổi 3)
            var isAdmin = string.Equals(SessionManager.CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase);
            this.btnDelete.Enabled = isAdmin; // Chỉ Admin được quyền xóa khách hàng
        }

        private HttpClient GetAuthenticatedClient()
        {
            var client = ApiClientService.CreateHttpClient();
            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            }
            return client;
        }

        private async Task LoadDataAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                string keyword = txtKeyword.Text.Trim();
                string url = $"customers/paged?keyword={Uri.EscapeDataString(keyword)}&pageNumber={_currentPage}&pageSize={_pageSize}";

                var result = await client.GetFromJsonAsync<PagedViewResult<CustomerDto>>(url);
                if (result != null)
                {
                    _currentPage = result.CurrentPage;
                    _totalPages = result.TotalPages == 0 ? 1 : result.TotalPages;

                    dgvCustomers.DataSource = result.Items;
                    lblPaging.Text = $"Trang {_currentPage} / {_totalPages} (Tổng: {result.TotalItems} KH)";

                    btnPrev.Enabled = _currentPage > 1;
                    btnNext.Enabled = _currentPage < _totalPages;

                    ConfigureGridColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách khách hàng phân trang: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureGridColumns()
        {
            if (dgvCustomers.Columns["CustomerId"] != null) dgvCustomers.Columns["CustomerId"].HeaderText = "Mã KH";
            if (dgvCustomers.Columns["CustomerName"] != null) dgvCustomers.Columns["CustomerName"].HeaderText = "Tên Khách Hàng";
            if (dgvCustomers.Columns["PhoneNumber"] != null) dgvCustomers.Columns["PhoneNumber"].HeaderText = "Số Điện Thoại";
            if (dgvCustomers.Columns["Address"] != null) dgvCustomers.Columns["Address"].HeaderText = "Địa Chỉ";
            if (dgvCustomers.Columns["RewardPoints"] != null) dgvCustomers.Columns["RewardPoints"].HeaderText = "Điểm Thưởng (Giảm dần)";
            if (dgvCustomers.Columns["MembershipRank"] != null) dgvCustomers.Columns["MembershipRank"].HeaderText = "Hạng Thẻ";
        }

        private void DgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvCustomers.Rows.Count) return;
            var row = dgvCustomers.Rows[e.RowIndex];

            txtCustomerId.Text = row.Cells["CustomerId"].Value?.ToString() ?? "";
            txtCustomerName.Text = row.Cells["CustomerName"].Value?.ToString() ?? "";
            txtPhoneNumber.Text = row.Cells["PhoneNumber"].Value?.ToString() ?? "";
            txtAddress.Text = row.Cells["Address"].Value?.ToString() ?? "";
            txtRewardPoints.Text = row.Cells["RewardPoints"].Value?.ToString() ?? "0";

            var rank = row.Cells["MembershipRank"].Value?.ToString() ?? "Chuẩn";
            cboMembershipRank.SelectedItem = rank;
        }

        private async Task SearchAsync()
        {
            _currentPage = 1;
            await LoadDataAsync();
        }

        private async Task AddCustomerAsync()
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text) || string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên và số điện thoại khách hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int.TryParse(txtRewardPoints.Text, out int points);

            var newCust = new
            {
                CustomerName = txtCustomerName.Text.Trim(),
                PhoneNumber = txtPhoneNumber.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                RewardPoints = points,
                MembershipRank = cboMembershipRank.SelectedItem?.ToString() ?? "Chuẩn"
            };

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.PostAsJsonAsync("customers", newCust);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm khách hàng thành công vào SQL Server!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                    ClearInputs();
                }
                else
                {
                    string err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Thêm thất bại (HTTP {(int)response.StatusCode}):\n{err}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UpdateCustomerAsync()
        {
            if (!int.TryParse(txtCustomerId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần cập nhật từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text) || string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Tên và số điện thoại không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int.TryParse(txtRewardPoints.Text, out int points);

            var updateCust = new
            {
                CustomerId = id,
                CustomerName = txtCustomerName.Text.Trim(),
                PhoneNumber = txtPhoneNumber.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                RewardPoints = points,
                MembershipRank = cboMembershipRank.SelectedItem?.ToString() ?? "Chuẩn"
            };

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.PutAsJsonAsync($"customers/{id}", updateCust);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật thông tin khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                    ClearInputs();
                }
                else
                {
                    string err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Cập nhật thất bại (HTTP {(int)response.StatusCode}):\n{err}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DeleteCustomerAsync()
        {
            if (!int.TryParse(txtCustomerId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa khách hàng ID = {id}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.DeleteAsync($"customers/{id}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xóa khách hàng thành công khỏi SQL Server!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                    ClearInputs();
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("[403 Forbidden] Chỉ tài khoản Admin mới có quyền xóa khách hàng!", "Bị từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    string err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Xóa thất bại:\n{err}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtCustomerId.Clear();
            txtCustomerName.Clear();
            txtPhoneNumber.Clear();
            txtAddress.Clear();
            txtRewardPoints.Text = "0";
            cboMembershipRank.SelectedIndex = 0;
        }
    }

    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int RewardPoints { get; set; }
        public string MembershipRank { get; set; } = "Chuẩn";
    }
}
