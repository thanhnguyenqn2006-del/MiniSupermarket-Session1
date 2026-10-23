using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    /// <summary>
    /// FormPOS — Quầy Bán hàng & Thu ngân (Dành cho: Cashier, Admin)
    /// Buổi 4: Nhúng động vào FormMainShell.panelMainContent
    /// </summary>
    public class FormPOS : Form
    {
        // ── Left panel (giỏ hàng) ──────────────────────────────────────
        private Panel panelLeft;
        private TextBox txtBarcode;
        private Label lblBarcodeHint;
        private DataGridView dgvCart;
        private Label lblTotalLabel;
        private Label lblTotalAmount;
        private Button btnClearCart;

        // ── Right panel (thanh toán) ───────────────────────────────────
        private Panel panelRight;
        private Label lblCustomerTitle;
        private TextBox txtCustomerPhone;
        private Label lblCustomerName;
        private Label lblCashLabel;
        private TextBox txtCashReceived;
        private Label lblChangeLabel;
        private Label lblChange;
        private Button btnCheckout;

        // ── Dữ liệu ─────────────────────────────────────────────────────
        private readonly List<CartItemDto> _cart = new List<CartItemDto>();

        public FormPOS()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
        }

        private void SetupUI()
        {
            this.Text      = "FormPOS";
            this.BackColor = Color.FromArgb(244, 245, 247);

            // ── LEFT PANEL (65%) ─────────────────────────────────────────
            panelLeft = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 840,
                BackColor = Color.White,
                Padding   = new Padding(15)
            };

            // Barcode row
            var pnlBarcode = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(0, 10, 0, 5) };
            var lblBarcode = new Label { Text = "🔍 Quét / nhập mã vạch:", Left = 0, Top = 15, Width = 175 };
            txtBarcode     = new TextBox { Left = 180, Top = 12, Width = 400, Font = new Font("Segoe UI", 11F) };
            lblBarcodeHint = new Label
            {
                Text      = "Nhấn Enter để thêm vào giỏ hàng",
                Left      = 590, Top = 15, Width = 230,
                ForeColor = Color.Gray, Font = new Font("Segoe UI", 8F, FontStyle.Italic)
            };
            pnlBarcode.Controls.AddRange(new Control[] { lblBarcode, txtBarcode, lblBarcodeHint });

            // DataGridView giỏ hàng
            dgvCart = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect           = false,
                ReadOnly              = false,
                AllowUserToAddRows    = false,
                BackgroundColor       = Color.White,
                RowHeadersVisible     = false,
                GridColor             = Color.FromArgb(230, 232, 240),
                AutoGenerateColumns   = false
            };
            SetupCartColumns();

            // Bottom row giỏ hàng
            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(0, 8, 0, 0) };
            lblTotalLabel  = new Label { Text = "TỔNG CỘNG:", Left = 0, Top = 16, Width = 120, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            lblTotalAmount = new Label
            {
                Text      = "0 đ",
                Left      = 125, Top = 10, Width = 200,
                Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 150, 80)
            };
            btnClearCart = new Button
            {
                Text      = "🗑️ Hủy giỏ hàng",
                Left      = 680, Top = 10, Width = 140, Height = 34,
                BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnClearCart.FlatAppearance.BorderSize = 0;
            pnlBottom.Controls.AddRange(new Control[] { lblTotalLabel, lblTotalAmount, btnClearCart });

            panelLeft.Controls.Add(dgvCart);
            panelLeft.Controls.Add(pnlBottom);
            panelLeft.Controls.Add(pnlBarcode);

            // ── RIGHT PANEL (35%) ─────────────────────────────────────────
            panelRight = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 251, 253),
                Padding   = new Padding(20, 15, 20, 15)
            };

            // Separator dọc
            var separator = new Panel { Dock = DockStyle.Left, Width = 2, BackColor = Color.FromArgb(230, 232, 240) };

            int ly = 15, lw = 140, cw = 180, cx = 160;

            lblCustomerTitle = new Label { Text = "👥 KHÁCH HÀNG THÂN THIẾT", Left = 20, Top = ly, Width = 320, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(70, 90, 130) };
            var lblPhone = new Label { Text = "Số điện thoại:", Left = 20, Top = ly + 30, Width = lw };
            txtCustomerPhone = new TextBox { Left = cx, Top = ly + 27, Width = cw, Font = new Font("Segoe UI", 10F) };
            lblCustomerName  = new Label { Text = "Khách vãng lai", Left = 20, Top = ly + 58, Width = 320, ForeColor = Color.Gray, Font = new Font("Segoe UI", 9F, FontStyle.Italic) };

            var divider1 = new Panel { Left = 20, Top = ly + 85, Width = 320, Height = 1, BackColor = Color.FromArgb(220, 225, 235) };

            var lblPayTitle = new Label { Text = "💳 THÔNG TIN THANH TOÁN", Left = 20, Top = ly + 100, Width = 320, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(70, 90, 130) };
            lblCashLabel    = new Label { Text = "Tiền khách đưa (đ):", Left = 20, Top = ly + 130, Width = lw + 20 };
            txtCashReceived = new TextBox { Left = cx, Top = ly + 127, Width = cw, Font = new Font("Segoe UI", 11F), Text = "0" };
            lblChangeLabel  = new Label { Text = "Tiền thừa (đ):", Left = 20, Top = ly + 165, Width = lw };
            lblChange       = new Label { Text = "0 đ", Left = cx, Top = ly + 162, Width = cw, Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.FromArgb(0, 120, 60) };

            btnCheckout = new Button
            {
                Text      = "✅  THANH TOÁN  [F9]",
                Left      = 20, Top = ly + 215, Width = 320, Height = 55,
                BackColor = Color.FromArgb(0, 150, 80), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font      = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            btnCheckout.FlatAppearance.BorderSize = 0;

            panelRight.Controls.AddRange(new Control[]
            {
                lblCustomerTitle, lblPhone, txtCustomerPhone, lblCustomerName,
                divider1, lblPayTitle,
                lblCashLabel, txtCashReceived, lblChangeLabel, lblChange,
                btnCheckout, separator
            });

            this.Controls.Add(panelRight);
            this.Controls.Add(panelLeft);

            // ── EVENTS ───────────────────────────────────────────────────
            this.KeyPreview          = true;
            this.KeyDown            += FormPOS_KeyDown;
            txtBarcode.KeyDown      += TxtBarcode_KeyDown;
            txtCashReceived.TextChanged += TxtCashReceived_TextChanged;
            btnCheckout.Click       += BtnCheckout_Click;
            btnClearCart.Click      += (s, e) => { _cart.Clear(); RefreshCartDisplay(); };
            dgvCart.CellEndEdit     += DgvCart_CellEndEdit;
            txtCustomerPhone.Leave  += async (s, e) => await LookupCustomerAsync();
        }

        private void SetupCartColumns()
        {
            dgvCart.Columns.Clear();
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductId",   HeaderText = "Mã SP",        Width = 70,  ReadOnly = true });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "Tên Sản phẩm", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPriceFmt", HeaderText = "Đơn giá",    Width = 110, ReadOnly = true });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity",    HeaderText = "SL",           Width = 55 });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalFmt",   HeaderText = "Thành tiền",   Width = 120, ReadOnly = true });

            // Nút xóa dòng
            var btnDel = new DataGridViewButtonColumn { HeaderText = "Xóa", Text = "✕", UseColumnTextForButtonValue = true, Width = 50 };
            dgvCart.Columns.Add(btnDel);
            dgvCart.CellClick += (s, e) =>
            {
                if (e.ColumnIndex == 5 && e.RowIndex >= 0)
                {
                    _cart.RemoveAt(e.RowIndex);
                    RefreshCartDisplay();
                }
            };
        }

        private System.Net.Http.HttpClient GetClient()
        {
            var client = ApiClientService.CreateHttpClient();
            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            return client;
        }

        private int? _selectedCustomerId = null;

        // ── Quét mã vạch (Sử dụng API ADO.NET Siêu Tốc Buổi 5) ─────────────
        private async void TxtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                e.SuppressKeyPress = true;
                string barcode = txtBarcode.Text.Trim();
                txtBarcode.Clear();

                try
                {
                    using var client = GetClient();
                    PosProductDto product = null;

                    // Thử gọi endpoint ADO.NET SqlDataReader siêu tốc (Buổi 5)
                    try
                    {
                        product = await client.GetFromJsonAsync<PosProductDto>($"ado-products/barcode/{barcode}");
                    }
                    catch
                    {
                        // Fallback sang endpoint products mặc định nếu có sự cố
                        product = await client.GetFromJsonAsync<PosProductDto>($"products/barcode/{barcode}");
                    }

                    if (product == null)
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm với mã vạch này!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var existing = _cart.Find(c => c.ProductId == product.ProductId);
                    if (existing != null)
                    {
                        existing.Quantity++;
                    }
                    else
                    {
                        _cart.Add(new CartItemDto
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductName,
                            UnitPrice = product.Price,
                            Quantity = 1
                        });
                    }

                    RefreshCartDisplay();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối khi tra cứu sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ── F9 = Thanh toán nhanh ─────────────────────────────────────
        private void FormPOS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9) BtnCheckout_Click(sender, EventArgs.Empty);
        }

        // ── Sửa số lượng trực tiếp trên lưới ─────────────────────────
        private void DgvCart_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3 && e.RowIndex >= 0)
            {
                if (int.TryParse(dgvCart.Rows[e.RowIndex].Cells[3].Value?.ToString(), out int qty) && qty > 0)
                    _cart[e.RowIndex].Quantity = qty;
                else
                    _cart[e.RowIndex].Quantity = 1;
                RefreshCartDisplay();
            }
        }

        // ── Tra cứu khách hàng theo SĐT ──────────────────────────────
        private async System.Threading.Tasks.Task LookupCustomerAsync()
        {
            if (string.IsNullOrWhiteSpace(txtCustomerPhone.Text))
            {
                _selectedCustomerId = null;
                lblCustomerName.Text = "Khách vãng lai";
                lblCustomerName.ForeColor = Color.Gray;
                return;
            }
            try
            {
                using var client = GetClient();
                var customers = await client.GetFromJsonAsync<List<CustLookupDto>>(
                    $"customers/search?keyword={Uri.EscapeDataString(txtCustomerPhone.Text.Trim())}");

                if (customers != null && customers.Count > 0)
                {
                    var c = customers[0];
                    _selectedCustomerId = c.CustomerId;
                    lblCustomerName.Text      = $"✅ {c.CustomerName}  |  Hạng: {c.MembershipRank}  |  Điểm: {c.RewardPoints}";
                    lblCustomerName.ForeColor = Color.FromArgb(0, 120, 60);
                }
                else
                {
                    _selectedCustomerId = null;
                    lblCustomerName.Text      = "⚠️ Không tìm thấy — Khách vãng lai";
                    lblCustomerName.ForeColor = Color.DimGray;
                }
            }
            catch
            {
                _selectedCustomerId = null;
                lblCustomerName.Text = "Khách vãng lai";
            }
        }

        // ── Tính tiền thừa ────────────────────────────────────────────
        private void TxtCashReceived_TextChanged(object sender, EventArgs e)
        {
            decimal total = _cart.Sum(x => x.TotalPrice);
            if (decimal.TryParse(txtCashReceived.Text, out decimal cash))
            {
                decimal change = cash - total;
                lblChange.Text      = change >= 0 ? $"{change:N0} đ" : "Chưa đủ tiền!";
                lblChange.ForeColor = change >= 0 ? Color.FromArgb(0, 120, 60) : Color.Red;
            }
        }

        // ── Cập nhật hiển thị giỏ hàng ───────────────────────────────
        private void RefreshCartDisplay()
        {
            dgvCart.DataSource = null;
            dgvCart.DataSource = _cart;
            decimal total       = _cart.Sum(x => x.TotalPrice);
            lblTotalAmount.Text = $"{total:N0} đ";
            TxtCashReceived_TextChanged(null, null);
        }

        // ── Thanh toán (Giao dịch ACID bảo vệ bởi SqlTransaction Buổi 5) ─
        private async void BtnCheckout_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống! Vui lòng quét sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            decimal total = _cart.Sum(x => x.TotalPrice);
            if (!decimal.TryParse(txtCashReceived.Text, out decimal cash) || cash < total)
            {
                MessageBox.Show("Tiền khách đưa chưa đủ! Vui lòng kiểm tra lại.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal change = cash - total;

            btnCheckout.Enabled = false;
            btnCheckout.Text = "⏳ ĐANG XỬ LÝ GIAO DỊCH...";

            try
            {
                // Chuẩn bị payload gửi lên API Checkout Buổi 5
                var checkoutRequest = new
                {
                    CashierUsername = !string.IsNullOrWhiteSpace(SessionManager.CurrentUsername) ? SessionManager.CurrentUsername : "cashier",
                    CustomerId = _selectedCustomerId ?? 15, // Khách vãng lai mặc định ID = 15
                    Items = _cart.Select(x => new
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice
                    }).ToList()
                };

                using var client = GetClient();
                var response = await client.PostAsJsonAsync("checkout", checkoutRequest);

                if (response.IsSuccessStatusCode)
                {
                    string orderCode = $"HD-{DateTime.Now:yyyyMMdd}-XXXX";
                    int rewardPoints = (int)(total / 10000);

                    try
                    {
                        var resJson = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                        if (resJson.TryGetProperty("orderCode", out var oc))
                            orderCode = oc.GetString() ?? orderCode;
                        if (resJson.TryGetProperty("rewardPointsAdded", out var rp))
                            rewardPoints = rp.GetInt32();
                    }
                    catch { }

                    string receipt = $"🌸 Beauty Mart — HÓA ĐƠN BÁN HÀNG\n";
                    receipt       += $"{'─',40}\n";
                    receipt       += $"Mã HĐ:   {orderCode}\n";
                    receipt       += $"Thu ngân: {SessionManager.FullName ?? SessionManager.CurrentUsername ?? "Thu ngân"}\n";
                    receipt       += $"Thời gian: {DateTime.Now:HH:mm dd/MM/yyyy}\n";
                    receipt       += $"{'─',40}\n";
                    foreach (var item in _cart)
                        receipt   += $"{item.ProductName}\n  {item.Quantity} x {item.UnitPrice:N0} đ = {item.TotalPrice:N0} đ\n";
                    receipt       += $"{'─',40}\n";
                    receipt       += $"TỔNG CỘNG:    {total:N0} đ\n";
                    receipt       += $"Tiền khách:   {cash:N0} đ\n";
                    receipt       += $"Tiền thừa:    {change:N0} đ\n";
                    receipt       += $"Điểm tích lũy: +{rewardPoints} điểm ⭐\n";
                    receipt       += $"{'─',40}\n";
                    receipt       += "Giao dịch đã được Commit an toàn vào CSDL (ACID)\n";
                    receipt       += "Cảm ơn quý khách! Hẹn gặp lại 💕";

                    MessageBox.Show(receipt, "✅ Thanh toán thành công (SqlTransaction)", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset giỏ hàng
                    _cart.Clear();
                    RefreshCartDisplay();
                    txtCashReceived.Text   = "0";
                    txtCustomerPhone.Clear();
                    _selectedCustomerId = null;
                    lblCustomerName.Text   = "Khách vãng lai";
                    lblCustomerName.ForeColor = Color.Gray;
                    txtBarcode.Focus();
                }
                else
                {
                    string errContent = await response.Content.ReadAsStringAsync();
                    string errorMsg = "Không thể thanh toán đơn hàng!";
                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(errContent);
                        if (doc.RootElement.TryGetProperty("message", out var msg))
                            errorMsg = msg.GetString() ?? errorMsg;
                    }
                    catch
                    {
                        if (!string.IsNullOrWhiteSpace(errContent)) errorMsg = errContent;
                    }

                    MessageBox.Show($"❌ GIAO DỊCH ĐÃ BỊ HUỶ (ROLLBACK AN TOÀN):\n\n{errorMsg}\n\n(Không có dữ liệu rác nào được lưu vào CSDL).",
                        "Lỗi giao dịch POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối máy chủ Web API: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCheckout.Enabled = true;
                btnCheckout.Text = "✅  THANH TOÁN  [F9]";
            }
        }
    }

    // ── DTOs cho FormPOS ──────────────────────────────────────────────
    public class PosProductDto
    {
        public int     ProductId   { get; set; }
        public string  Barcode     { get; set; } = string.Empty;
        public string  ProductName { get; set; } = string.Empty;
        public decimal Price       { get; set; }
        public int     StockQuantity { get; set; }
    }

    public class CartItemDto
    {
        public int     ProductId   { get; set; }
        public string  ProductName { get; set; } = string.Empty;
        public decimal UnitPrice   { get; set; }
        public int     Quantity    { get; set; }
        public decimal TotalPrice  => UnitPrice * Quantity;
        public string  UnitPriceFmt => $"{UnitPrice:N0}";
        public string  TotalFmt    => $"{TotalPrice:N0}";
    }

    public class CustLookupDto
    {
        public int    CustomerId     { get; set; }
        public string CustomerName   { get; set; } = string.Empty;
        public string PhoneNumber    { get; set; } = string.Empty;
        public int    RewardPoints   { get; set; }
        public string MembershipRank { get; set; } = string.Empty;
    }
}
