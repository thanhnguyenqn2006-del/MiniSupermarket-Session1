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
    /// <summary>
    /// Form quản lý Sản phẩm - Cửa hàng Mỹ phẩm &amp; Hóa phẩm Beauty Mart
    /// Tích hợp: JWT Bearer Token + EF Core via REST API (Buổi 2 + Buổi 3)
    /// </summary>
    public class FormProductManagement : Form
    {
        // ===================== UI Controls =====================
        private DataGridView dgvProducts;
        private TextBox txtProductId;
        private TextBox txtBarcode;
        private TextBox txtProductName;
        private TextBox txtPrice;
        private TextBox txtStockQuantity;
        private ComboBox cboCategoryId;
        private TextBox txtKeyword;
        private Button btnSearch;
        private Button btnLoad;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        private ToolStrip toolStrip1;
        private GroupBox grpInfo;
        private GroupBox grpList;

        // Danh sách categories để hiển thị trong ComboBox
        private List<CategoryDto> _categories = new List<CategoryDto>();

        public FormProductManagement()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Sản phẩm - Cửa hàng Beauty Mart";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F);

            // =================== ToolStrip ===================
            this.toolStrip1 = new ToolStrip { Dock = DockStyle.Top };

            // =================== Panel tìm kiếm ===================
            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(8, 8, 8, 0) };
            var lblSearch = new Label { Text = "Tìm kiếm:", Left = 10, Top = 15, Width = 70 };
            this.txtKeyword = new TextBox { Left = 85, Top = 12, Width = 280, Font = new Font("Segoe UI", 10F) };
            this.btnSearch = new Button
            {
                Text = "🔍 Tìm kiếm",
                Left = 375, Top = 10, Width = 110, Height = 30,
                BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            this.btnLoad = new Button
            {
                Text = "🔄 Tải lại",
                Left = 495, Top = 10, Width = 100, Height = 30,
                BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtKeyword, btnSearch, btnLoad });

            // =================== Group List ===================
            this.grpList = new GroupBox
            {
                Text = "Danh sách Sản phẩm (SQL Server - BeautyMartDb)",
                Left = 12, Top = 105, Width = 640, Height = 490,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.dgvProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };
            grpList.Controls.Add(dgvProducts);

            // =================== Group Info ===================
            this.grpInfo = new GroupBox
            {
                Text = "Thông tin Sản phẩm",
                Left = 665, Top = 105, Width = 400, Height = 490,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };

            int lx = 15, cx = 130, cw = 240;
            var lblId = new Label { Text = "Mã SP:", Left = lx, Top = 35, Width = 110 };
            this.txtProductId = new TextBox { Left = cx, Top = 32, Width = cw, ReadOnly = true, BackColor = Color.FromArgb(240, 240, 240) };

            var lblBarcode = new Label { Text = "Mã vạch (*):", Left = lx, Top = 72, Width = 110 };
            this.txtBarcode = new TextBox { Left = cx, Top = 69, Width = cw };

            var lblName = new Label { Text = "Tên sản phẩm (*):", Left = lx, Top = 112, Width = 110 };
            this.txtProductName = new TextBox { Left = cx, Top = 109, Width = cw, Multiline = true, Height = 50 };

            var lblPrice = new Label { Text = "Giá bán (VNĐ) (*):", Left = lx, Top = 172, Width = 110 };
            this.txtPrice = new TextBox { Text = "0", Left = cx, Top = 169, Width = cw };

            var lblStock = new Label { Text = "Tồn kho:", Left = lx, Top = 212, Width = 110 };
            this.txtStockQuantity = new TextBox { Text = "0", Left = cx, Top = 209, Width = cw };

            var lblCat = new Label { Text = "Nhóm hàng (*):", Left = lx, Top = 252, Width = 110 };
            this.cboCategoryId = new ComboBox
            {
                Left = cx, Top = 249, Width = cw,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "CategoryName",
                ValueMember = "CategoryId"
            };

            this.btnAdd = new Button
            {
                Text = "➕ Thêm mới",
                Left = lx, Top = 310, Width = 110, Height = 35,
                BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            this.btnUpdate = new Button
            {
                Text = "✏️ Cập nhật",
                Left = 135, Top = 310, Width = 110, Height = 35,
                BackColor = Color.FromArgb(255, 152, 0), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            this.btnDelete = new Button
            {
                Text = "🗑️ Xóa",
                Left = 255, Top = 310, Width = 110, Height = 35,
                BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            this.btnClear = new Button
            {
                Text = "🧹 Xóa trắng",
                Left = 135, Top = 360, Width = 110, Height = 30,
                BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };

            grpInfo.Controls.AddRange(new Control[]
            {
                lblId, txtProductId, lblBarcode, txtBarcode,
                lblName, txtProductName, lblPrice, txtPrice,
                lblStock, txtStockQuantity, lblCat, cboCategoryId,
                btnAdd, btnUpdate, btnDelete, btnClear
            });

            // =================== Add all to Form ===================
            this.Controls.Add(toolStrip1);
            this.Controls.Add(grpList);
            this.Controls.Add(grpInfo);
            this.Controls.Add(pnlSearch);

            // =================== Events ===================
            this.Load += async (s, e) =>
            {
                // Ẩn ToolStrip cũ khi nhúng vào FormMainShell (child form)
                if (!this.TopLevel) toolStrip1.Visible = false;
                else SetupToolStrip();
                ApplyRolePermissions();
                await LoadCategoriesAsync();
                await LoadDataAsync();
            };
            this.dgvProducts.CellClick += DgvProducts_CellClick;
            this.btnLoad.Click += async (s, e) => await LoadDataAsync();
            this.btnSearch.Click += async (s, e) => await SearchAsync();
            this.btnAdd.Click += async (s, e) => await AddProductAsync();
            this.btnUpdate.Click += async (s, e) => await UpdateProductAsync();
            this.btnDelete.Click += async (s, e) => await DeleteProductAsync();
            this.btnClear.Click += (s, e) => ClearInputs();
        }

        // =========================================================
        // HTTPCLIENT CÓ GẮN BEARER TOKEN
        // =========================================================
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

        // =========================================================
        // TOOLSTRIP: THÔNG TIN USER & ĐĂNG XUẤT
        // =========================================================
        private void SetupToolStrip()
        {
            toolStrip1.Items.Clear();

            var lblUser = new ToolStripLabel
            {
                Text = $"👤 {SessionManager.CurrentUsername} | Vai trò: {SessionManager.CurrentRole}",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            var sep = new ToolStripSeparator();

            var btnLogout = new ToolStripButton
            {
                Text = "🚪 Đóng cửa sổ này",
                ForeColor = Color.DarkRed,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnLogout.Click += (s, e) => this.Close();

            toolStrip1.Items.Add(lblUser);
            toolStrip1.Items.Add(sep);
            toolStrip1.Items.Add(btnLogout);
        }

        // =========================================================
        // PHÂN QUYỀN THEO VAI TRÒ
        // =========================================================
        private void ApplyRolePermissions()
        {
            bool isAdmin = string.Equals(SessionManager.CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase);
            btnAdd.Enabled = isAdmin;
            btnUpdate.Enabled = isAdmin;
            btnDelete.Enabled = isAdmin;

            this.Text = isAdmin
                ? "Quản lý Sản phẩm - [Toàn quyền Quản trị (Admin)]"
                : "Quản lý Sản phẩm - [Chế độ Xem (Cashier)]";
        }

        // =========================================================
        // TẢI DANH MỤC VÀO COMBOBOX
        // =========================================================
        private async Task LoadCategoriesAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var cats = await client.GetFromJsonAsync<List<CategoryDto>>("categories");
                if (cats != null)
                {
                    _categories = cats;
                    cboCategoryId.DataSource = _categories;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // =========================================================
        // 1. TẢI DANH SÁCH SẢN PHẨM (GET /api/products)
        // =========================================================
        private async Task LoadDataAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var products = await client.GetFromJsonAsync<List<ProductDto>>("products");
                if (products != null)
                {
                    dgvProducts.DataSource = products;
                    ConfigureGridColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureGridColumns()
        {
            if (dgvProducts.Columns["ProductId"] != null)     { dgvProducts.Columns["ProductId"].HeaderText = "Mã SP";          dgvProducts.Columns["ProductId"].Width = 55; }
            if (dgvProducts.Columns["Barcode"] != null)       { dgvProducts.Columns["Barcode"].HeaderText = "Mã vạch";          dgvProducts.Columns["Barcode"].Width = 130; }
            if (dgvProducts.Columns["ProductName"] != null)   { dgvProducts.Columns["ProductName"].HeaderText = "Tên Sản phẩm"; }
            if (dgvProducts.Columns["Price"] != null)         { dgvProducts.Columns["Price"].HeaderText = "Giá bán (VNĐ)";      dgvProducts.Columns["Price"].Width = 110; }
            if (dgvProducts.Columns["StockQuantity"] != null) { dgvProducts.Columns["StockQuantity"].HeaderText = "Tồn kho";    dgvProducts.Columns["StockQuantity"].Width = 70; }
            if (dgvProducts.Columns["CategoryId"] != null)    { dgvProducts.Columns["CategoryId"].HeaderText = "Mã nhóm";       dgvProducts.Columns["CategoryId"].Width = 70; }
            if (dgvProducts.Columns["CategoryName"] != null)  { dgvProducts.Columns["CategoryName"].HeaderText = "Nhóm hàng";   dgvProducts.Columns["CategoryName"].Width = 160; }
            // Ẩn cột Category object nếu có
            if (dgvProducts.Columns["Category"] != null) dgvProducts.Columns["Category"].Visible = false;
        }

        // =========================================================
        // 2. CLICK DÒNG TRÊN DATAGRIDVIEW
        // =========================================================
        private void DgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvProducts.Rows[e.RowIndex];

            txtProductId.Text     = row.Cells["ProductId"]?.Value?.ToString() ?? "";
            txtBarcode.Text       = row.Cells["Barcode"]?.Value?.ToString() ?? "";
            txtProductName.Text   = row.Cells["ProductName"]?.Value?.ToString() ?? "";
            txtPrice.Text         = row.Cells["Price"]?.Value?.ToString() ?? "0";
            txtStockQuantity.Text = row.Cells["StockQuantity"]?.Value?.ToString() ?? "0";

            if (int.TryParse(row.Cells["CategoryId"]?.Value?.ToString(), out int catId))
            {
                for (int i = 0; i < cboCategoryId.Items.Count; i++)
                {
                    if (cboCategoryId.Items[i] is CategoryDto cat && cat.CategoryId == catId)
                    {
                        cboCategoryId.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        // =========================================================
        // 3. TÌM KIẾM (GET /api/products/search?keyword=...)
        // =========================================================
        private async Task SearchAsync()
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
                var result = await client.GetFromJsonAsync<List<ProductDto>>(
                    "products/search?keyword=" + Uri.EscapeDataString(keyword));
                dgvProducts.DataSource = result;
                ConfigureGridColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // 4. THÊM MỚI (POST /api/products)
        // =========================================================
        private async Task AddProductAsync()
        {
            if (!ValidateInputs(out decimal price, out int stock, out int catId)) return;

            try
            {
                using var client = GetAuthenticatedClient();
                var newProduct = new
                {
                    Barcode = txtBarcode.Text.Trim(),
                    ProductName = txtProductName.Text.Trim(),
                    Price = price,
                    StockQuantity = stock,
                    CategoryId = catId
                };

                var response = await client.PostAsJsonAsync("products", newProduct);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm sản phẩm mới thành công vào SQL Server!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                    ClearInputs();
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("[403 Forbidden] Chỉ Admin mới được thêm sản phẩm!", "Bị từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        // =========================================================
        // 5. CẬP NHẬT (PUT /api/products/{id})
        // =========================================================
        private async Task UpdateProductAsync()
        {
            if (!int.TryParse(txtProductId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần cập nhật từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!ValidateInputs(out decimal price, out int stock, out int catId)) return;

            try
            {
                using var client = GetAuthenticatedClient();
                var updateProduct = new
                {
                    ProductId = id,
                    Barcode = txtBarcode.Text.Trim(),
                    ProductName = txtProductName.Text.Trim(),
                    Price = price,
                    StockQuantity = stock,
                    CategoryId = catId
                };

                var response = await client.PutAsJsonAsync($"products/{id}", updateProduct);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật sản phẩm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                    ClearInputs();
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("[403 Forbidden] Chỉ Admin mới được cập nhật sản phẩm!", "Bị từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        // =========================================================
        // 6. XÓA (DELETE /api/products/{id})
        // =========================================================
        private async Task DeleteProductAsync()
        {
            if (!int.TryParse(txtProductId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa sản phẩm ID = {id}?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.DeleteAsync($"products/{id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xóa sản phẩm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                    ClearInputs();
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("[403 Forbidden] Chỉ Admin mới được xóa sản phẩm!", "Bị từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        // =========================================================
        // VALIDATE DỮ LIỆU NHẬP
        // =========================================================
        private bool ValidateInputs(out decimal price, out int stock, out int catId)
        {
            price = 0; stock = 0; catId = 0;

            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                MessageBox.Show("Vui lòng nhập mã vạch sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!decimal.TryParse(txtPrice.Text, out price) || price < 0)
            {
                MessageBox.Show("Giá bán không hợp lệ! Vui lòng nhập số không âm.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!int.TryParse(txtStockQuantity.Text, out stock) || stock < 0)
            {
                MessageBox.Show("Số lượng tồn kho không hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cboCategoryId.SelectedItem is CategoryDto selectedCat)
            {
                catId = selectedCat.CategoryId;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // =========================================================
        // XÓA TRẮNG Ô NHẬP LIỆU
        // =========================================================
        private void ClearInputs()
        {
            txtProductId.Clear();
            txtBarcode.Clear();
            txtProductName.Clear();
            txtPrice.Text = "0";
            txtStockQuantity.Text = "0";
            if (cboCategoryId.Items.Count > 0) cboCategoryId.SelectedIndex = 0;
        }
    }

    // ===================== DTO SẢN PHẨM =====================
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto Category { get; set; }

        // Property tổng hợp để hiển thị tên nhóm hàng trực tiếp trên DataGridView
        public string CategoryName => Category?.CategoryName ?? "";
    }
}
