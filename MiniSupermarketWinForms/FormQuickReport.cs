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

        private DataGridView dgvCashiers;

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
                Text      = "💡 Nhấn 'Xem Báo cáo' để gọi Stored Procedure từ CSDL SQL Server.",
                Left      = 500, Top = 20, Width = 550,
                ForeColor = Color.DimGray, Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            };

            pnlToolbar.Controls.AddRange(new Control[] { lblDateLabel, dtpDate, btnRunReport, lblStatus, pnlBorder });

            // ── 3 thẻ tóm tắt ─────────────────────────────────────────
            panelCards = new Panel { Dock = DockStyle.Top, Height = 170, Padding = new Padding(20, 20, 20, 0) };

            cardOrders     = CreateStatCard("📋 Tổng số Hóa đơn (Ca)", Color.FromArgb(41, 100, 180));
            cardRevenue    = CreateStatCard("💰 Doanh thu Stored Procedure", Color.FromArgb(0, 150, 80));
            cardBestSeller = CreateStatCard("🏆 Top 1 Bán chạy nhất", Color.FromArgb(200, 100, 0));

            lblTotalOrders  = (Label)cardOrders.Controls[0];
            lblTotalRevenue = (Label)cardRevenue.Controls[0];
            lblBestSeller   = (Label)cardBestSeller.Controls[0];

            // Bố cục 3 thẻ ngang
            cardOrders.Bounds     = new Rectangle(20, 20, 300, 130);
            cardRevenue.Bounds    = new Rectangle(340, 20, 300, 130);
            cardBestSeller.Bounds = new Rectangle(660, 20, 340, 130);

            panelCards.Controls.AddRange(new Control[] { cardOrders, cardRevenue, cardBestSeller });

            // ── Bảng chi tiết kết quả Stored Procedure ────────────────
            var pnlDetail = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };
            var lblDetailTitle = new Label
            {
                Text      = "📈 BÁO CÁO DOANH THU THEO TỪNG THU NGÂN (sp_GetRevenueByCashier - Buổi 5)",
                Dock      = DockStyle.Top,
                Height    = 40,
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 30, 48),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(5, 0, 0, 0)
            };

            dgvCashiers = new DataGridView
            {
                Dock                = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode       = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly            = true,
                AllowUserToAddRows  = false,
                BackgroundColor     = Color.White,
                RowHeadersVisible   = false,
                GridColor           = Color.FromArgb(230, 232, 240),
                AutoGenerateColumns = false
            };

            dgvCashiers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CashierUsername", HeaderText = "Tài khoản Thu ngân", Width = 180 });
            dgvCashiers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalOrders",     HeaderText = "Số HĐ Đã Lập",        Width = 130 });
            dgvCashiers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RevenueFormatted",HeaderText = "Tổng Doanh Thu (VNĐ)",  Width = 200 });
            dgvCashiers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PointsFormatted", HeaderText = "Điểm Thưởng Đã Tặng",   Width = 160 });

            pnlDetail.Controls.Add(dgvCashiers);
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
                Font      = new Font("Segoe UI", 16F, FontStyle.Bold),
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
            lblStatus.Text       = "Đang gọi Stored Procedure từ API...";

            try
            {
                using var client = ApiClientService.CreateHttpClient();
                if (!string.IsNullOrEmpty(SessionManager.JwtToken))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
                }

                // 1. Gọi Stored Procedure sp_GetRevenueByCashier (Buổi 5)
                var cashierResponse = await client.GetAsync("reports/revenue-by-cashier");
                if (cashierResponse.IsSuccessStatusCode)
                {
                    var jsonStr = await cashierResponse.Content.ReadAsStringAsync();
                    using var doc = System.Text.Json.JsonDocument.Parse(jsonStr);
                    if (doc.RootElement.TryGetProperty("data", out var dataArr))
                    {
                        var list = new List<CashierReportViewModel>();
                        int grandOrders = 0;
                        decimal grandRevenue = 0;

                        foreach (var el in dataArr.EnumerateArray())
                        {
                            string username = el.GetProperty("cashierUsername").GetString() ?? "";
                            int orders = el.GetProperty("totalOrders").GetInt32();
                            decimal rev = el.GetProperty("totalRevenue").GetDecimal();
                            int pts = el.GetProperty("totalPointsAwarded").GetInt32();

                            grandOrders += orders;
                            grandRevenue += rev;

                            list.Add(new CashierReportViewModel
                            {
                                CashierUsername = username,
                                TotalOrders = orders,
                                TotalRevenue = rev,
                                TotalPointsAwarded = pts
                            });
                        }

                        dgvCashiers.DataSource = list;
                        lblTotalOrders.Text = $"{grandOrders} hóa đơn";
                        lblTotalRevenue.Text = $"{grandRevenue:N0} đ";
                    }
                }

                // 2. Gọi Stored Procedure sp_GetTop5SellingProducts (Buổi 5)
                var topResponse = await client.GetAsync("reports/top-selling-products");
                if (topResponse.IsSuccessStatusCode)
                {
                    var topStr = await topResponse.Content.ReadAsStringAsync();
                    using var topDoc = System.Text.Json.JsonDocument.Parse(topStr);
                    if (topDoc.RootElement.TryGetProperty("data", out var topArr) && topArr.GetArrayLength() > 0)
                    {
                        var first = topArr[0];
                        string topName = first.GetProperty("productName").GetString() ?? "---";
                        int topQty = first.GetProperty("totalQuantitySold").GetInt32();
                        lblBestSeller.Text = $"{topName} ({topQty} cái)";
                    }
                }

                lblStatus.Text = $"✅ Dữ liệu Stored Procedure cập nhật lúc {DateTime.Now:HH:mm:ss}";
                lblStatus.ForeColor = Color.FromArgb(0, 120, 60);
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

    public class CashierReportViewModel
    {
        public string CashierUsername { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalPointsAwarded { get; set; }
        public string RevenueFormatted => $"{TotalRevenue:N0} đ";
        public string PointsFormatted => $"{TotalPointsAwarded:N0} điểm";
    }
}
