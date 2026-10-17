using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    /// <summary>
    /// FormMainShell — Khung giao diện chính Single-Form Architecture
    /// Sidebar (230px) + TopHeader (60px) + MainContent (Fill)
    /// Buổi 4: Role-Based Access Control (Admin / Cashier / Warehouse)
    /// </summary>
    public class FormMainShell : Form
    {
        // ===================== UI Controls =====================
        private Panel panelSidebar;
        private Panel panelTopHeader;
        private Panel panelMainContent;
        private Panel panelUserFooter;

        private Label lblLogo;
        private Label lblTitle;
        private Label lblUserInfo;

        private Button btnPOS;
        private Button btnCategory;
        private Button btnProduct;
        private Button btnCustomer;
        private Button btnReports;
        private Button btnUserManage;
        private Button btnLogout;

        // Form con đang hiển thị
        private Form _activeForm = null;

        public FormMainShell()
        {
            InitializeComponent();
            SetupUI();
        }

        // Hàm rỗng để Visual Studio Designer không bị crash khi cố parse code tự viết
        private void InitializeComponent()
        {
        }

        private void SetupUI()
        {
            this.Text            = "Hệ thống Quản lý Bán lẻ & Tồn kho — Cửa hàng Mỹ phẩm Beauty Mart";
            this.Size            = new Size(1280, 720);
            this.MinimumSize     = new Size(1024, 600);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.Font            = new Font("Segoe UI", 9F);
            this.BackColor       = Color.FromArgb(244, 245, 247);

            // ============================================================
            //  SIDEBAR (bên trái, 230px, xanh đen đậm)
            // ============================================================
            panelSidebar = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 230,
                BackColor = Color.FromArgb(24, 30, 48)
            };

            // Logo MiniMart
            lblLogo = new Label
            {
                Text      = "🌸 Beauty Mart",
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Dock      = DockStyle.Top,
                Height    = 70,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(15, 20, 35)
            };

            // Nút sidebar helper
            btnPOS        = CreateSidebarButton("🛒  Bán hàng (POS)");
            btnCategory   = CreateSidebarButton("📁  Quản lý Danh mục");
            btnProduct    = CreateSidebarButton("📦  Quản lý Sản phẩm");
            btnCustomer   = CreateSidebarButton("👥  Quản lý Khách hàng");
            btnReports    = CreateSidebarButton("📊  Báo cáo Doanh thu");
            btnUserManage = CreateSidebarButton("🛡  Quản trị Tài khoản");

            // Footer sidebar — Đăng xuất
            panelUserFooter = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 55,
                BackColor = Color.FromArgb(15, 20, 35)
            };

            btnLogout = new Button
            {
                Text      = "🚪  Đăng xuất",
                Dock      = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(255, 100, 100),
                BackColor = Color.Transparent,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor    = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(20, 0, 0, 0)
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 80, 80);
            panelUserFooter.Controls.Add(btnLogout);

            // Thêm vào Sidebar theo thứ tự (Top → Bottom)
            panelSidebar.Controls.Add(panelUserFooter);
            panelSidebar.Controls.Add(btnUserManage);
            panelSidebar.Controls.Add(btnReports);
            panelSidebar.Controls.Add(btnCustomer);
            panelSidebar.Controls.Add(btnProduct);
            panelSidebar.Controls.Add(btnCategory);
            panelSidebar.Controls.Add(btnPOS);
            panelSidebar.Controls.Add(lblLogo);

            // ============================================================
            //  TOP HEADER (60px, trắng — breadcrumb + user info)
            // ============================================================
            panelTopHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 60,
                BackColor = Color.White,
                Padding   = new Padding(20, 0, 20, 0)
            };

            lblTitle = new Label
            {
                Text      = "BÀN LÀM VIỆC HỆ THỐNG",
                Font      = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 30, 48),
                Dock      = DockStyle.Left,
                Width     = 450,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblUserInfo = new Label
            {
                Text      = "Đang tải...",
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(80, 90, 110),
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight
            };

            // Đường kẻ dưới header
            var headerBorder = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 2,
                BackColor = Color.FromArgb(230, 232, 240)
            };

            panelTopHeader.Controls.Add(lblUserInfo);
            panelTopHeader.Controls.Add(lblTitle);
            panelTopHeader.Controls.Add(headerBorder);

            // ============================================================
            //  MAIN CONTENT (Fill — vùng nhúng form con động)
            // ============================================================
            panelMainContent = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(244, 245, 247),
                Padding   = new Padding(0)
            };

            // ============================================================
            //  ADD ALL TO FORM
            // ============================================================
            this.Controls.Add(panelMainContent);
            this.Controls.Add(panelTopHeader);
            this.Controls.Add(panelSidebar);

            // ============================================================
            //  EVENTS
            // ============================================================
            this.Load         += FormMainShell_Load;
            btnPOS.Click      += btnPOS_Click;
            btnCategory.Click += btnCategory_Click;
            btnProduct.Click  += btnProduct_Click;
            btnCustomer.Click += btnCustomer_Click;
            btnReports.Click  += btnReports_Click;
            btnUserManage.Click += btnUserManage_Click;
            btnLogout.Click   += btnLogout_Click;
        }

        // ==============================================================
        //  HELPER: Tạo nút sidebar đồng nhất
        // ==============================================================
        private Button CreateSidebarButton(string text)
        {
            var btn = new Button
            {
                Text      = text,
                Dock      = DockStyle.Top,
                Height    = 50,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(180, 190, 210),
                BackColor = Color.Transparent,
                Font      = new Font("Segoe UI", 9.5F),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(20, 0, 0, 0),
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(38, 48, 75);
            return btn;
        }

        // ==============================================================
        //  FORM LOAD — Khởi tạo phiên làm việc
        // ==============================================================
        private void FormMainShell_Load(object sender, EventArgs e)
        {
            // Hiển thị thông tin phiên đăng nhập trên header
            lblUserInfo.Text = $"👤  {SessionManager.FullName}  |  Vai trò: [{SessionManager.CurrentRole}]  |  {DateTime.Now:HH:mm dd/MM/yyyy}";

            // Áp dụng phân quyền hiển thị sidebar
            ApplyRolePermissions(SessionManager.CurrentRole);

            // Mở màn hình mặc định theo vai trò
            OpenDefaultScreenByRole(SessionManager.CurrentRole);
        }

        // ==============================================================
        //  PHÂN QUYỀN RBAC — Ẩn/Hiện nút sidebar theo Role
        // ==============================================================
        private void ApplyRolePermissions(string role)
        {
            switch (role?.ToUpper())
            {
                case "ADMIN":
                    // Toàn quyền — hiển thị tất cả
                    btnPOS.Visible        = true;
                    btnCategory.Visible   = true;
                    btnProduct.Visible    = true;
                    btnCustomer.Visible   = true;
                    btnReports.Visible    = true;
                    btnUserManage.Visible = true;
                    break;

                case "CASHIER":
                    // Thu ngân — chỉ POS và Khách hàng
                    btnPOS.Visible        = true;
                    btnCustomer.Visible   = true;
                    btnCategory.Visible   = false;
                    btnProduct.Visible    = false;
                    btnReports.Visible    = false;
                    btnUserManage.Visible = false;
                    break;

                case "WAREHOUSE":
                    // Thủ kho — Danh mục và Sản phẩm
                    btnPOS.Visible        = false;
                    btnCategory.Visible   = true;
                    btnProduct.Visible    = true;
                    btnCustomer.Visible   = false;
                    btnReports.Visible    = false;
                    btnUserManage.Visible = false;
                    break;

                default:
                    MessageBox.Show("Tài khoản chưa được cấp quyền hạn hợp lệ!",
                        "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    break;
            }
        }

        // ==============================================================
        //  NHÚNG ĐỘNG FORM CON VÀO MAIN CONTENT PANEL
        // ==============================================================
        private void OpenChildForm(Form childForm, string screenTitle, Button senderButton)
        {
            if (_activeForm != null)
            {
                _activeForm.Close();
                _activeForm = null;
            }

            HighlightActiveButton(senderButton);

            _activeForm                  = childForm;
            childForm.TopLevel           = false;
            childForm.FormBorderStyle    = FormBorderStyle.None;
            childForm.Dock               = DockStyle.Fill;

            panelMainContent.Controls.Clear();
            panelMainContent.Controls.Add(childForm);
            panelMainContent.Tag = childForm;

            lblTitle.Text = screenTitle;
            childForm.BringToFront();
            childForm.Show();
        }

        // Làm nổi bật nút menu đang được chọn
        private void HighlightActiveButton(Button activeButton)
        {
            Button[] navButtons = { btnPOS, btnCategory, btnProduct, btnCustomer, btnReports, btnUserManage };
            foreach (var btn in navButtons)
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.FromArgb(180, 190, 210);
                btn.Font      = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            }

            // Nổi bật nút đang active
            activeButton.BackColor = Color.FromArgb(41, 100, 180);
            activeButton.ForeColor = Color.White;
            activeButton.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        }

        // Mở màn hình mặc định theo vai trò
        private void OpenDefaultScreenByRole(string role)
        {
            switch (role?.ToUpper())
            {
                case "ADMIN":
                    OpenChildForm(new FormCategoryManagement(), "QUẢN LÝ DANH MỤC NHÓM HÀNG", btnCategory);
                    break;
                case "CASHIER":
                    OpenChildForm(new FormCustomerManagement(), "QUẢN LÝ KHÁCH HÀNG & TÍCH ĐIỂM", btnCustomer);
                    break;
                case "WAREHOUSE":
                    OpenChildForm(new FormProductManagement(), "QUẢN LÝ SẢN PHẨM & HÀNG TỒN KHO", btnProduct);
                    break;
            }
        }

        // ==============================================================
        //  SỰ KIỆN CLICK SIDEBAR
        // ==============================================================
        private void btnPOS_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormPOS(), "HỆ THỐNG QUẦY BÁN HÀNG & THU NGÂN (POS)", btnPOS);
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormCategoryManagement(), "QUẢN LÝ DANH MỤC NHÓM HÀNG", btnCategory);
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormProductManagement(), "QUẢN LÝ THÔNG TIN SẢN PHẨM & HÀNG TỒN KHO", btnProduct);
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormCustomerManagement(), "QUẢN LÝ KHÁCH HÀNG THÂN THIẾT & TÍCH ĐIỂM", btnCustomer);
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            // Lớp bảo vệ thứ hai — chặn truy cập nếu không phải Admin
            if (!string.Equals(SessionManager.CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Bạn không có quyền xem dữ liệu tài chính của siêu thị!",
                    "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            OpenChildForm(new FormQuickReport(), "BÁO CÁO DOANH THU & HIỆU SUẤT CA BÁN", btnReports);
        }

        private void btnUserManage_Click(object sender, EventArgs e)
        {
            if (!string.Equals(SessionManager.CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Chỉ Quản trị viên (Admin) mới được quản lý tài khoản!",
                    "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            OpenChildForm(new FormUserManagement(), "QUẢN TRỊ TÀI KHOẢN VÀ PHÂN QUYỀN HỆ THỐNG", btnUserManage);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất phiên làm việc?",
                "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                // Xóa session
                SessionManager.JwtToken        = string.Empty;
                SessionManager.CurrentUsername = string.Empty;
                SessionManager.CurrentRole     = string.Empty;
                SessionManager.FullName        = string.Empty;
                SessionManager.IsLoggedOut     = true;

                this.Close();
            }
        }
    }
}
