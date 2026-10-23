/*
 * HỌC PHẦN: PHÁT TRIỂN ỨNG DỤNG VỚI .NET CORE & SQL SERVER
 * BUỔI 6: ENTITY FRAMEWORK CORE NÂNG CAO, PHÂN TRANG LINQ & TÍCH HỢP TOÀN DIỆN POS CLIENT - SERVER
 * TIẾT 4 & 5: GIAO DIỆN QUẢN LÝ SẢN PHẨM PHÂN TRANG SERVER-SIDE VÀ BỘ LỌC DANH MỤC
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    // DTO hứng dữ liệu sản phẩm kèm tên nhóm hàng (Eager Loading) từ Web API
    public class ProductItemViewDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; // Trích xuất từ Category.CategoryName qua .Include()
        public string PriceFormatted => $"{Price:N0} đ";
    }

    // DTO danh mục cho ComboBox
    public class CategoryComboDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    // DTO kết quả phân trang nhận từ Web API GET /api/products/paged
    public class PagedViewResult<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<T> Items { get; set; } = new();
    }

    public partial class FormProductManagement : Form
    {
        // ── Phân trang State ──────────────────────────────────────────
        private int _currentPage = 1;
        private readonly int _pageSize = 5; // Mặc định 5 dòng / trang theo yêu cầu giáo trình
        private int _totalPages = 1;

        // ── UI Controls: Nhập liệu ────────────────────────────────────
        private TextBox txtProductId;
        private TextBox txtBarcode;
        private TextBox txtProductName;
        private NumericUpDown nudPrice;
        private NumericUpDown nudStock;
        private ComboBox cboCategory;

        // ── UI Controls: Nút CRUD ────────────────────────────────────
        private Button btnCreate;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;

        // ── UI Controls: Tìm kiếm & Lọc ──────────────────────────────
        private TextBox txtSearch;
        private ComboBox cboFilterCategory; // Bài tập mở rộng 6.2: Bộ lọc nhóm hàng
        private Button btnSearch;
        private Button btnLoad;

        // ── UI Controls: Lưới & Phân trang ────────────────────────────
        private DataGridView dgvProducts;
        private Button btnPrev;
        private Label lblPaging;
        private Button btnNext;

        public FormProductManagement()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
        }

        private void SetupUI()
        {
            this.Text            = "Quản lý Hàng hóa & Phân trang Sản phẩm (Buổi 6 - EF Core)";
            this.Size            = new Size(1180, 680);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.Font            = new Font("Segoe UI", 9.5F);
            this.BackColor       = Color.FromArgb(244, 245, 247);

            // ==============================================================
            // 1. TOP PANEL: FORM NHẬP LIỆU & NÚT THAO TÁC CRUD (Chiều cao: 195)
            // ==============================================================
            var pnlInput = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 195,
                BackColor = Color.White,
                Padding   = new Padding(20, 15, 20, 10)
            };

            var lblHeader = new Label
            {
                Text      = "📦 QUẢN LÝ SẢN PHẨM & TỒN KHO (Eager Loading & Phân trang LINQ Server-side)",
                Dock      = DockStyle.Top,
                Height    = 28,
                Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 30, 48)
            };

            var pnlFields = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            // Hàng 1: Mã SP, Mã Vạch, Tên SP, Nhóm hàng
            var lblId = new Label { Text = "Mã SP:", Left = 10, Top = 12, Width = 60 };
            txtProductId = new TextBox { Left = 75, Top = 9, Width = 70, ReadOnly = true, BackColor = Color.FromArgb(240, 240, 240) };

            var lblBarcode = new Label { Text = "Mã vạch (*):", Left = 160, Top = 12, Width = 85 };
            txtBarcode = new TextBox { Left = 250, Top = 9, Width = 170 };

            var lblName = new Label { Text = "Tên sản phẩm (*):", Left = 435, Top = 12, Width = 115 };
            txtProductName = new TextBox { Left = 555, Top = 9, Width = 280 };

            var lblCat = new Label { Text = "Nhóm (*):", Left = 850, Top = 12, Width = 65 };
            cboCategory = new ComboBox
            {
                Left = 920, Top = 9, Width = 210,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Hàng 2: Giá bán, Tồn kho
            var lblPrice = new Label { Text = "Giá bán (VNĐ):", Left = 10, Top = 52, Width = 100 };
            nudPrice = new NumericUpDown
            {
                Left = 115, Top = 49, Width = 150,
                Maximum = 1000000000, Increment = 1000, ThousandsSeparator = true
            };

            var lblStock = new Label { Text = "Tồn kho:", Left = 285, Top = 52, Width = 65 };
            nudStock = new NumericUpDown
            {
                Left = 355, Top = 49, Width = 110,
                Maximum = 100000, Increment = 1
            };

            // Hàng 3: Các nút chức năng Thêm / Sửa / Xóa / Xóa trắng
            btnCreate = new Button
            {
                Text      = "➕ Thêm mới (btnCreate)",
                Left      = 480, Top = 45, Width = 180, Height = 34,
                BackColor = Color.FromArgb(0, 150, 80), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnCreate.FlatAppearance.BorderSize = 0;

            btnUpdate = new Button
            {
                Text      = "✏️ Cập nhật (btnUpdate)",
                Left      = 670, Top = 45, Width = 170, Height = 34,
                BackColor = Color.FromArgb(255, 140, 0), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnUpdate.FlatAppearance.BorderSize = 0;

            btnDelete = new Button
            {
                Text      = "🗑️ Xóa hàng (btnDelete)",
                Left      = 850, Top = 45, Width = 160, Height = 34,
                BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnDelete.FlatAppearance.BorderSize = 0;

            btnClear = new Button
            {
                Text      = "🧹 Xóa trắng",
                Left      = 1020, Top = 45, Width = 110, Height = 34,
                BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;

            pnlFields.Controls.AddRange(new Control[]
            {
                lblId, txtProductId, lblBarcode, txtBarcode, lblName, txtProductName, lblCat, cboCategory,
                lblPrice, nudPrice, lblStock, nudStock,
                btnCreate, btnUpdate, btnDelete, btnClear
            });

            pnlInput.Controls.Add(pnlFields);
            pnlInput.Controls.Add(lblHeader);

            // ==============================================================
            // 2. SEARCH & FILTER TOOLBAR (Chiều cao: 60)
            // ==============================================================
            var pnlSearch = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 60,
                BackColor = Color.FromArgb(235, 240, 248),
                Padding   = new Padding(20, 12, 20, 10)
            };

            var lblSearch = new Label { Text = "🔍 Tìm kiếm:", Left = 20, Top = 18, Width = 85, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            txtSearch = new TextBox { Left = 110, Top = 15, Width = 260, Font = new Font("Segoe UI", 10F) };

            var lblFilterCat = new Label { Text = "📁 Lọc nhóm hàng (B6.2):", Left = 385, Top = 18, Width = 160, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            cboFilterCategory = new ComboBox
            {
                Left = 550, Top = 15, Width = 230,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnSearch = new Button
            {
                Text      = "🔎 Tìm (btnSearch)",
                Left      = 795, Top = 13, Width = 140, Height = 34,
                BackColor = Color.FromArgb(41, 100, 180), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnSearch.FlatAppearance.BorderSize = 0;

            btnLoad = new Button
            {
                Text      = "🔄 Tải lại",
                Left      = 945, Top = 13, Width = 110, Height = 34,
                BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnLoad.FlatAppearance.BorderSize = 0;

            pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblFilterCat, cboFilterCategory, btnSearch, btnLoad });

            // ==============================================================
            // 3. BOTTOM PAGING PANEL (Chiều cao: 55)
            // ==============================================================
            var pnlPaging = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 55,
                BackColor = Color.White,
                Padding   = new Padding(20, 10, 20, 10)
            };

            btnPrev = new Button
            {
                Text      = "◀  Trang trước (btnPrev)",
                Left      = 20, Top = 10, Width = 180, Height = 35,
                BackColor = Color.FromArgb(41, 100, 180), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnPrev.FlatAppearance.BorderSize = 0;

            lblPaging = new Label
            {
                Text      = "Trang 1 / 1 (Tổng: 0 SP)",
                Left      = 215, Top = 16, Width = 280,
                Font      = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnNext = new Button
            {
                Text      = "Trang sau ▶  (btnNext)",
                Left      = 510, Top = 10, Width = 180, Height = 35,
                BackColor = Color.FromArgb(41, 100, 180), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnNext.FlatAppearance.BorderSize = 0;

            var lblHint = new Label
            {
                Text      = "💡 Mỗi trang hiển thị 5 sản phẩm — Trích xuất từ máy chủ qua LINQ .Skip().Take()",
                Left      = 710, Top = 17, Width = 440,
                ForeColor = Color.DimGray, Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            };

            pnlPaging.Controls.AddRange(new Control[] { btnPrev, lblPaging, btnNext, lblHint });

            // ==============================================================
            // 4. MAIN DATAGRIDVIEW PANEL (Fill)
            // ==============================================================
            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 10, 20, 0), BackColor = Color.FromArgb(244, 245, 247) };

            dgvProducts = new DataGridView
            {
                Dock                = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode       = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect         = false,
                ReadOnly            = true,
                AllowUserToAddRows  = false,
                BackgroundColor     = Color.White,
                RowHeadersVisible   = false,
                GridColor           = Color.FromArgb(230, 232, 240),
                AutoGenerateColumns = false
            };
            SetupGridColumns();

            pnlGrid.Controls.Add(dgvProducts);

            // Add all to Form
            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlPaging);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlInput);

            // ==============================================================
            // 5. ATTACH EVENTS
            // ==============================================================
            this.Load += FormProductManagement_Load;
            btnPrev.Click += btnPrev_Click;
            btnNext.Click += btnNext_Click;
            btnSearch.Click += btnSearch_Click;
            btnLoad.Click += async (s, e) => { txtSearch.Clear(); if (cboFilterCategory.Items.Count > 0) cboFilterCategory.SelectedIndex = 0; _currentPage = 1; await LoadProductsPagedAsync(); };
            txtSearch.KeyDown += async (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; btnSearch_Click(s, e); } };
            cboFilterCategory.SelectedIndexChanged += async (s, e) => { _currentPage = 1; await LoadProductsPagedAsync(); };
            btnCreate.Click += btnCreate_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += (s, e) => ClearInputs();
            dgvProducts.CellClick += DgvProducts_CellClick;
        }

        private void SetupGridColumns()
        {
            dgvProducts.Columns.Clear();
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductId",      HeaderText = "Mã SP",        Width = 70 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Barcode",        HeaderText = "Mã Vạch",      Width = 140 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName",    HeaderText = "Tên Sản phẩm", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PriceFormatted", HeaderText = "Đơn Giá",     Width = 120 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "StockQuantity",  HeaderText = "Tồn Kho",     Width = 90 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName",   HeaderText = "Nhóm Hàng (Include)", Width = 180 });
        }

        // =========================================================================
        // SỰ KIỆN TẢI DỮ LIỆU BAN ĐẦU
        // =========================================================================
        private async void FormProductManagement_Load(object sender, EventArgs e)
        {
            await LoadCategoriesComboAsync(); // Nạp nhóm hàng vào ComboBox
            await LoadProductsPagedAsync();   // Nạp bảng sản phẩm phân trang
        }

        // 1. Tải danh sách nhóm hàng lên ComboBox (Nhập liệu & Bộ lọc)
        private async Task LoadCategoriesComboAsync()
        {
            try
            {
                var categories = await ApiClientService.Client
                    .GetFromJsonAsync<List<CategoryComboDto>>("categories");

                if (categories != null)
                {
                    // ComboBox nhập liệu trong Form
                    cboCategory.DataSource = new List<CategoryComboDto>(categories);
                    cboCategory.DisplayMember = "CategoryName";
                    cboCategory.ValueMember = "CategoryId";

                    // ComboBox bộ lọc nhóm hàng (Bài tập mở rộng 6.2)
                    var filterList = new List<CategoryComboDto>
                    {
                        new CategoryComboDto { CategoryId = 0, CategoryName = "--- Tất cả nhóm hàng ---" }
                    };
                    filterList.AddRange(categories);
                    cboFilterCategory.DataSource = filterList;
                    cboFilterCategory.DisplayMember = "CategoryName";
                    cboFilterCategory.ValueMember = "CategoryId";
                    cboFilterCategory.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. Gọi API phân trang và hiển thị dữ liệu lên DataGridView (LINQ Server-side)
        private async Task LoadProductsPagedAsync()
        {
            try
            {
                string search = txtSearch.Text.Trim();
                int filterCatId = 0;
                if (cboFilterCategory.SelectedValue != null && int.TryParse(cboFilterCategory.SelectedValue.ToString(), out int cId))
                {
                    filterCatId = cId;
                }

                // Xây dựng URL phân trang kèm tham số tìm kiếm và bộ lọc nhóm hàng (Bài 6.2)
                string url = $"products/paged?keyword={Uri.EscapeDataString(search)}&pageNumber={_currentPage}&pageSize={_pageSize}";
                if (filterCatId > 0)
                {
                    url += $"&categoryId={filterCatId}";
                }

                var result = await ApiClientService.Client.GetFromJsonAsync<PagedViewResult<ProductItemViewDto>>(url);
                if (result != null)
                {
                    _currentPage = result.CurrentPage;
                    _totalPages = result.TotalPages == 0 ? 1 : result.TotalPages;

                    // Hiển thị dữ liệu lên bảng
                    dgvProducts.DataSource = result.Items;

                    // Cập nhật nhãn phân trang
                    lblPaging.Text = $"Trang {_currentPage} / {_totalPages} (Tổng: {result.TotalItems} SP)";

                    // Bật / tắt nút điều hướng phân trang
                    btnPrev.Enabled = _currentPage > 1;
                    btnNext.Enabled = _currentPage < _totalPages;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu sản phẩm phân trang: " + ex.Message, "Lỗi kết nối API",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 3. Sự kiện chuyển trang (Trang trước & Trang sau)
        private async void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await LoadProductsPagedAsync();
            }
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                await LoadProductsPagedAsync();
            }
        }

        // 4. Sự kiện tìm kiếm
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            _currentPage = 1; // Khi tìm kiếm từ khóa mới, luôn quay về trang 1
            await LoadProductsPagedAsync();
        }

        // 5. Thêm mới sản phẩm qua API
        private async void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                MessageBox.Show("Vui lòng nhập mã vạch sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBarcode.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProductName.Focus();
                return;
            }

            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newProd = new
            {
                Barcode       = txtBarcode.Text.Trim(),
                ProductName   = txtProductName.Text.Trim(),
                Price         = nudPrice.Value,
                StockQuantity = (int)nudStock.Value,
                CategoryId    = Convert.ToInt32(cboCategory.SelectedValue)
            };

            try
            {
                var res = await ApiClientService.Client.PostAsJsonAsync("products", newProd);
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("✅ Thêm mới sản phẩm thành công vào CSDL!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadProductsPagedAsync();
                }
                else
                {
                    string err = await res.Content.ReadAsStringAsync();
                    MessageBox.Show($"Thêm thất bại ({res.StatusCode}): {err}", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối khi thêm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 6. Cập nhật sản phẩm qua API
        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductId.Text) || !int.TryParse(txtProductId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm từ bảng để cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var updateProd = new
            {
                ProductId     = id,
                Barcode       = txtBarcode.Text.Trim(),
                ProductName   = txtProductName.Text.Trim(),
                Price         = nudPrice.Value,
                StockQuantity = (int)nudStock.Value,
                CategoryId    = Convert.ToInt32(cboCategory.SelectedValue)
            };

            try
            {
                var res = await ApiClientService.Client.PutAsJsonAsync($"products/{id}", updateProd);
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("✅ Cập nhật sản phẩm thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadProductsPagedAsync();
                }
                else
                {
                    string err = await res.Content.ReadAsStringAsync();
                    MessageBox.Show($"Cập nhật thất bại: {err}", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 7. Xóa sản phẩm qua API
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductId.Text) || !int.TryParse(txtProductId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa trên bảng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string name = txtProductName.Text;
            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm #{id} - '{name}' khỏi CSDL?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    var res = await ApiClientService.Client.DeleteAsync($"products/{id}");
                    if (res.IsSuccessStatusCode)
                    {
                        MessageBox.Show("✅ Đã xóa sản phẩm thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputs();
                        await LoadProductsPagedAsync();
                    }
                    else
                    {
                        string err = await res.Content.ReadAsStringAsync();
                        MessageBox.Show($"Xóa thất bại: {err}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 8. Đổ dữ liệu từ dòng được chọn lên các ô nhập liệu
        private void DgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvProducts.Rows[e.RowIndex].DataBoundItem is ProductItemViewDto item)
            {
                txtProductId.Text   = item.ProductId.ToString();
                txtBarcode.Text     = item.Barcode;
                txtProductName.Text = item.ProductName;
                nudPrice.Value      = item.Price >= nudPrice.Minimum && item.Price <= nudPrice.Maximum ? item.Price : 0;
                nudStock.Value      = item.StockQuantity >= nudStock.Minimum && item.StockQuantity <= nudStock.Maximum ? item.StockQuantity : 0;

                if (item.CategoryId > 0)
                {
                    cboCategory.SelectedValue = item.CategoryId;
                }
            }
        }

        private void ClearInputs()
        {
            txtProductId.Clear();
            txtBarcode.Clear();
            txtProductName.Clear();
            nudPrice.Value = 0;
            nudStock.Value = 0;
            if (cboCategory.Items.Count > 0) cboCategory.SelectedIndex = 0;
            txtBarcode.Focus();
        }
    }
}
