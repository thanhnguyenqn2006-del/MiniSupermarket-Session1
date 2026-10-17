using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    /// <summary>
    /// FormQuickReport — Báo cáo Doanh thu nhanh (Chỉ Admin)
    /// Buổi 4: Bài tập mở rộng — nhúng vào FormMainShell
    /// </summary>
    public class FormQuickReport : Form
    {
        private DateTimePicker dtpDate;
        private Button btnRunReport;
        private Panel panelCards;
        private Panel cardOrders;
        private Panel cardRevenue;
        private Panel cardBestSeller;
        private Label lblTotalOrders;
        private Label lblTotalRevenue;
        private Label lblBestSeller;
        private Label lblStatus;

        public FormQuickReport()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
        }

        private void SetupUI()
        {
            this.BackColor = Color.FromArgb(244, 245, 247);
            this.Padding   = new Padding(25);

            // ── Header toolbar ─────────────────────────────────────────
            var pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.White, Padding = new Padding(20, 0, 20, 0) };
            var pnlBorder  = new Panel { Dock = DockStyle.Bottom, Height = 2, BackColor = Color.FromArgb(230, 232, 240) };

            var lblDateLabel = new Label { Text = "Xem báo cáo ngày:", Left = 10, Top = 20, Width = 145, Font = new Font("Segoe UI", 9.5F) };
            dtpDate          = new DateTimePicker { Left = 160, Top = 17, Width = 160, Font = new Font("Segoe UI", 9.5F), Value = DateTime.Today, Format = DateTimePickerFormat.Short };

            btnRunReport = new Button
            {
                Text      = "📊  Xem Báo cáo",
                Left      = 335, Top = 13, Width = 145, Height = 38,
                BackColor = Color.FromArgb(41, 100, 180), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnRunReport.FlatAppearance.BorderSize = 0;

            lblStatus = new Label
            {
                Text      = "💡 Chọn ngày và nhấn 'Xem Báo cáo' để tải dữ liệu.",
                Left      = 500, Top = 20, Width = 500,
                ForeColor = Color.DimGray, Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            };

            pnlToolbar.Controls.AddRange(new Control[] { lblDateLabel, dtpDate, btnRunReport, lblStatus, pnlBorder });

            // ── 3 thẻ tóm tắt ─────────────────────────────────────────
            panelCards = new Panel { Dock = DockStyle.Top, Height = 170, Padding = new Padding(20, 20, 20, 0) };

            cardOrders     = CreateStatCard("📋 Tổng số Hóa đơn", Color.FromArgb(41, 100, 180));
            cardRevenue    = CreateStatCard("💰 Tổng Doanh thu", Color.FromArgb(0, 150, 80));
            cardBestSeller = CreateStatCard("🏆 Sản phẩm bán chạy", Color.FromArgb(200, 100, 0));

            lblTotalOrders  = (Label)cardOrders.Controls[0];
            lblTotalRevenue = (Label)cardRevenue.Controls[0];
            lblBestSeller   = (Label)cardBestSeller.Controls[0];

            // Bố cục 3 thẻ ngang
            cardOrders.Bounds    = new Rectangle(20, 20, 300, 130);
            cardRevenue.Bounds   = new Rectangle(340, 20, 300, 130);
            cardBestSeller.Bounds = new Rectangle(660, 20, 340, 130);

            panelCards.Controls.AddRange(new Control[] { cardOrders, cardRevenue, cardBestSeller });

            // ── Bảng chi tiết (placeholder) ───────────────────────────
            var pnlDetail = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(20), Padding = new Padding(20) };
            var lblDetailTitle = new Label
            {
                Text      = "CHI TIẾT GIAO DỊCH TRONG NGÀY",
                Dock      = DockStyle.Top,
                Height    = 40,
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 30, 48),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(5, 0, 0, 0)
            };
            
            pnlDetail.Controls.Add(lblDetailTitle);

            this.Controls.Add(pnlDetail);
            this.Controls.Add(panelCards);
            this.Controls.Add(pnlToolbar);

            btnRunReport.Click += BtnRunReport_Click;

            // Tự động tải dữ liệu khi mở form
            this.Load += (s, e) => btnRunReport.PerformClick();
        }

        // Helper tạo thẻ stat card
        private Panel CreateStatCard(string title, Color accentColor)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Padding   = new Padding(15)
            };

            // Thanh màu bên trái
            var accent = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accentColor };

            var lblCardTitle = new Label
            {
                Text      = title,
                Dock      = DockStyle.Top,
                Height    = 35,
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = accentColor,
                Padding   = new Padding(10, 8, 0, 0)
            };

            var lblValue = new Label
            {
                Text      = "---",
                Dock      = DockStyle.Fill,
                Font      = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 30, 48),
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(lblValue);
            card.Controls.Add(lblCardTitle);
            card.Controls.Add(accent);

            return card;
        }

        private async void BtnRunReport_Click(object sender, EventArgs e)
        {
            btnRunReport.Enabled = false;
            btnRunReport.Text    = "⏳ Đang tải...";
            lblStatus.Text       = "Đang kết nối API...";

            try
            {
                using var client = ApiClientService.CreateHttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);

                // Lấy tất cả sản phẩm để thống kê tồn kho
                var products = await client.GetFromJsonAsync<System.Collections.Generic.List<PosProductDto>>("products");
                var customers = await client.GetFromJsonAsync<System.Collections.Generic.List<CustLookupDto>>("customers");

                if (products != null)
                {
                    // Thống kê giả lập từ dữ liệu sản phẩm
                    int totalProducts    = products.Count;
                    decimal totalValue   = 0;
                    string bestSeller    = "---";
                    int minStock         = int.MaxValue;

                    foreach (var p in products)
                    {
                        totalValue += p.Price * p.StockQuantity;
                        if (p.StockQuantity < minStock) { minStock = p.StockQuantity; bestSeller = p.ProductName; }
                    }

                    lblTotalOrders.Text  = $"{customers?.Count ?? 0}";
                    lblTotalRevenue.Text = $"{totalValue:N0} đ";
                    lblBestSeller.Text   = bestSeller.Length > 22 ? bestSeller.Substring(0, 22) + "…" : bestSeller;

                    lblStatus.Text       = $"✅ Đã tải báo cáo ngày {dtpDate.Value:dd/MM/yyyy} — {totalProducts} sản phẩm";
                    lblStatus.ForeColor  = Color.FromArgb(0, 120, 60);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text      = "❌ Lỗi: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                btnRunReport.Enabled = true;
                btnRunReport.Text    = "📊  Xem Báo cáo";
            }
        }
    }
}
