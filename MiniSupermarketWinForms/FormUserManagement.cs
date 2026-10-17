using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public class FormUserManagement : Form
    {
        private DataGridView dgvUsers;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnReload;

        private TextBox txtId;
        private TextBox txtUsername;
        private TextBox txtFullName;
        private ComboBox cboRole;
        private TextBox txtPassword;
        private Label lblStatusValue;
        
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnToggleStatus;
        private Button btnClear;

        private int _selectedUserId = 0;

        private readonly List<UserAccountDto> _staticAccounts = new List<UserAccountDto>
        {
            new UserAccountDto { Id=1,  Username="admin",        FullName="Quản trị viên",      Role="Admin",     IsActive=true  },
            new UserAccountDto { Id=2,  Username="cashier01",    FullName="Lê Thu Nga",         Role="Cashier",   IsActive=false },
            new UserAccountDto { Id=3,  Username="admin01",      FullName="Nguyễn Quản Trị",    Role="Admin",     IsActive=true  },
            new UserAccountDto { Id=4,  Username="admin02",      FullName="Trần Giám Đốc",      Role="Admin",     IsActive=true  },
            new UserAccountDto { Id=5,  Username="admin_backup", FullName="Đặng Hỗ Trợ",        Role="Admin",     IsActive=true  },
            new UserAccountDto { Id=6,  Username="supervisor",   FullName="Mai Giám Sát",       Role="Admin",     IsActive=true  },
            new UserAccountDto { Id=7,  Username="cashier02",    FullName="Phạm Bán Hàng",      Role="Cashier",   IsActive=true  },
            new UserAccountDto { Id=8,  Username="cashier03",    FullName="Hoàng Thu Ngân",     Role="Cashier",   IsActive=true  },
            new UserAccountDto { Id=9,  Username="cashier04",    FullName="Vũ Thị Quầy",        Role="Cashier",   IsActive=true  },
            new UserAccountDto { Id=10, Username="cashier05",    FullName="Đỗ Bán Lẻ",          Role="Cashier",   IsActive=true  },
            new UserAccountDto { Id=11, Username="cashier06",    FullName="Hồ Ca Chiều",        Role="Cashier",   IsActive=true  },
            new UserAccountDto { Id=12, Username="ware01",       FullName="Ngô Quản Kho",       Role="Warehouse", IsActive=true  },
            new UserAccountDto { Id=13, Username="ware02",       FullName="Bùi Kiểm Kê",        Role="Warehouse", IsActive=true  },
            new UserAccountDto { Id=14, Username="ware03",       FullName="Dương Thủ Kho",      Role="Warehouse", IsActive=true  },
            new UserAccountDto { Id=15, Username="ware04",       FullName="Lý Nhập Hàng",       Role="Warehouse", IsActive=true  },
            new UserAccountDto { Id=16, Username="ware05",       FullName="Trương Vận Chuyển",  Role="Warehouse", IsActive=true  },
        };

        public FormUserManagement()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent() { }

        private void SetupUI()
        {
            this.BackColor = Color.FromArgb(244, 245, 247);
            this.Font = new Font("Segoe UI", 9F);

            // --- Top Panel (Search) ---
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(15, 20, 15, 10) };
            var grpSearch = new GroupBox { Text = "Tìm kiếm", Dock = DockStyle.Fill };
            
            txtSearch = new TextBox { Left = 15, Top = 25, Width = 300 };
            txtSearch.Text = "Nhập từ khóa (tên đăng nhập hoặc họ tên)...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Nhập từ khóa (tên đăng nhập hoặc họ tên)...") { txtSearch.Text = ""; txtSearch.ForeColor = Color.Black; } };
            txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Nhập từ khóa (tên đăng nhập hoặc họ tên)..."; txtSearch.ForeColor = Color.Gray; } };
            
            btnSearch = new Button { Text = "Tìm kiếm", Left = 325, Top = 23, Width = 80, Height = 25, FlatStyle = FlatStyle.System };
            btnReload = new Button { Text = "Tải lại", Left = 415, Top = 23, Width = 80, Height = 25, FlatStyle = FlatStyle.System };
            
            grpSearch.Controls.AddRange(new Control[] { txtSearch, btnSearch, btnReload });
            pnlTop.Controls.Add(grpSearch);

            // --- Main Panel ---
            var pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 5, 15, 15) };
            
            // Right Panel (Info)
            var pnlRight = new Panel { Dock = DockStyle.Right, Width = 380, Padding = new Padding(15, 0, 0, 0) };
            var grpInfo = new GroupBox { Text = "Thông tin tài khoản", Dock = DockStyle.Fill };
            
            int ly = 30;
            int lh = 50;

            var lblId = new Label { Text = "Mã ID", Left = 20, Top = ly, Width = 300 };
            txtId = new TextBox { Left = 20, Top = ly + 20, Width = 320, ReadOnly = true, BackColor = Color.FromArgb(230, 230, 230) };
            
            ly += lh;
            var lblUser = new Label { Text = "Tên đăng nhập", Left = 20, Top = ly, Width = 300 };
            txtUsername = new TextBox { Left = 20, Top = ly + 20, Width = 320 };

            ly += lh;
            var lblFull = new Label { Text = "Họ tên", Left = 20, Top = ly, Width = 300 };
            txtFullName = new TextBox { Left = 20, Top = ly + 20, Width = 320 };

            ly += lh;
            var lblRole = new Label { Text = "Vai trò", Left = 20, Top = ly, Width = 300 };
            cboRole = new ComboBox { Left = 20, Top = ly + 20, Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
            cboRole.Items.AddRange(new string[] { "Admin", "Cashier", "Warehouse" });

            ly += lh;
            var lblPass = new Label { Text = "Mật khẩu (để trống nếu không đổi)", Left = 20, Top = ly, Width = 300 };
            txtPassword = new TextBox { Left = 20, Top = ly + 20, Width = 320, UseSystemPasswordChar = true };

            ly += 55;
            var lblStatusText = new Label { Text = "Trạng thái:", Left = 20, Top = ly, AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            lblStatusValue = new Label { Text = "—", Left = 95, Top = ly, AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };

            ly += 40;
            btnAdd = new Button { Text = "Thêm mới", Left = 20, Top = ly, Width = 80, Height = 30, FlatStyle = FlatStyle.System };
            btnUpdate = new Button { Text = "Cập nhật", Left = 110, Top = ly, Width = 80, Height = 30, FlatStyle = FlatStyle.System, Enabled = false };
            btnToggleStatus = new Button { Text = "Khóa / Mở khóa", Left = 200, Top = ly, Width = 110, Height = 30, FlatStyle = FlatStyle.System, Enabled = false };
            
            ly += 40;
            btnClear = new Button { Text = "Làm mới", Left = 20, Top = ly, Width = 80, Height = 30, FlatStyle = FlatStyle.System };

            grpInfo.Controls.AddRange(new Control[] {
                lblId, txtId,
                lblUser, txtUsername,
                lblFull, txtFullName,
                lblRole, cboRole,
                lblPass, txtPassword,
                lblStatusText, lblStatusValue,
                btnAdd, btnUpdate, btnToggleStatus, btnClear
            });

            pnlRight.Controls.Add(grpInfo);

            // Left Panel (Grid)
            var pnlLeft = new Panel { Dock = DockStyle.Fill };
            var grpList = new GroupBox { Text = "Danh sách tài khoản", Dock = DockStyle.Fill };
            
            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                GridColor = Color.LightGray,
                BorderStyle = BorderStyle.None
            };
            
            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id", HeaderText = "Mã", Width = 40 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", DataPropertyName = "Username", HeaderText = "Tên đăng nhập", Width = 120 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "FullName", DataPropertyName = "FullName", HeaderText = "Họ tên", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Role", DataPropertyName = "Role", HeaderText = "Vai trò", Width = 100 });
            dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn { Name = "IsActive", DataPropertyName = "IsActive", HeaderText = "Hoạt động", Width = 70 });
            
            grpList.Controls.Add(dgvUsers);
            pnlLeft.Controls.Add(grpList);

            pnlMain.Controls.Add(pnlLeft);
            pnlMain.Controls.Add(pnlRight);

            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlTop);

            // --- Events ---
            this.Load += (s, e) => LoadUsers();
            dgvUsers.CellClick += DgvUsers_CellClick;
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnToggleStatus.Click += BtnToggleStatus_Click;
            btnClear.Click += (s, e) => ClearInputs();
            btnSearch.Click += BtnSearch_Click;
            btnReload.Click += (s, e) => { txtSearch.Text = "Nhập từ khóa (tên đăng nhập hoặc họ tên)..."; txtSearch.ForeColor = Color.Gray; LoadUsers(); };
        }

        private void LoadUsers(string keyword = "")
        {
            dgvUsers.DataSource = null;
            var list = string.IsNullOrWhiteSpace(keyword) || keyword == "Nhập từ khóa (tên đăng nhập hoặc họ tên)..."
                ? _staticAccounts
                : _staticAccounts.Where(u => u.Username.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 || u.FullName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            
            dgvUsers.DataSource = new System.ComponentModel.BindingList<UserAccountDto>(list);

            foreach (DataGridViewRow row in dgvUsers.Rows)
            {
                if (row.Selected)
                {
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
                    row.DefaultCellStyle.SelectionForeColor = Color.White;
                }
            }
        }

        private void DgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            
            if (int.TryParse(dgvUsers.Rows[e.RowIndex].Cells["Id"].Value?.ToString(), out int id))
            {
                _selectedUserId = id;
                txtId.Text = id.ToString();
            }

            txtUsername.Text = dgvUsers.Rows[e.RowIndex].Cells["Username"]?.Value?.ToString() ?? "";
            txtFullName.Text = dgvUsers.Rows[e.RowIndex].Cells["FullName"]?.Value?.ToString() ?? "";
            cboRole.SelectedItem = dgvUsers.Rows[e.RowIndex].Cells["Role"]?.Value?.ToString() ?? "Cashier";
            
            bool isActive = (bool)(dgvUsers.Rows[e.RowIndex].Cells["IsActive"]?.Value ?? true);
            lblStatusValue.Text = isActive ? "Hoạt động" : "Đã khóa";
            lblStatusValue.ForeColor = isActive ? Color.Green : Color.Red;

            btnAdd.Enabled = false;
            btnUpdate.Enabled = true;
            btnToggleStatus.Enabled = true;
            txtUsername.ReadOnly = true;
            txtUsername.BackColor = Color.FromArgb(230, 230, 230);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Họ tên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_staticAccounts.Exists(u => u.Username.Equals(txtUsername.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int newId = _staticAccounts.Count > 0 ? _staticAccounts.Max(x => x.Id) + 1 : 1;
            var newUser = new UserAccountDto
            {
                Id = newId,
                Username = txtUsername.Text.Trim(),
                FullName = txtFullName.Text.Trim(),
                Role = cboRole.SelectedItem?.ToString() ?? "Cashier",
                IsActive = true
            };

            _staticAccounts.Add(newUser);
            LoadUsers();
            ClearInputs();
            MessageBox.Show("Thêm mới tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedUserId == 0) return;
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = _staticAccounts.Find(u => u.Id == _selectedUserId);
            if (user != null)
            {
                user.FullName = txtFullName.Text.Trim();
                user.Role = cboRole.SelectedItem?.ToString() ?? "Cashier";
                LoadUsers();
                MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnToggleStatus_Click(object sender, EventArgs e)
        {
            if (_selectedUserId == 0) return;

            var user = _staticAccounts.Find(u => u.Id == _selectedUserId);
            if (user != null)
            {
                if (user.Username.Equals(SessionManager.CurrentUsername, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Không thể khóa tài khoản đang đăng nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                user.IsActive = !user.IsActive;
                LoadUsers();
                
                // Cập nhật lại nhãn trạng thái ngay lập tức
                lblStatusValue.Text = user.IsActive ? "Hoạt động" : "Đã khóa";
                lblStatusValue.ForeColor = user.IsActive ? Color.Green : Color.Red;
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (keyword == "Nhập từ khóa (tên đăng nhập hoặc họ tên)...") keyword = "";
            LoadUsers(keyword);
        }

        private void ClearInputs()
        {
            _selectedUserId = 0;
            txtId.Clear();
            txtUsername.Clear();
            txtUsername.ReadOnly = false;
            txtUsername.BackColor = Color.White;
            txtFullName.Clear();
            txtPassword.Clear();
            cboRole.SelectedIndex = 1;
            lblStatusValue.Text = "—";
            lblStatusValue.ForeColor = Color.Black;
            
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnToggleStatus.Enabled = false;
        }
    }

    public class UserAccountDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
