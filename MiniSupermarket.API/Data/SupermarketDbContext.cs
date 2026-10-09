using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Data
{
    // DbContext đại diện cho phiên làm việc với cơ sở dữ liệu SQL Server của Cửa hàng Mỹ phẩm & Hóa phẩm Beauty Mart
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(DbContextOptions<SupermarketDbContext> options)
            : base(options)
        {
        }

        // Khai báo các bảng dữ liệu ánh xạ từ Model
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }

        // Cấu hình dữ liệu mồi ban đầu (Data Seeding)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Data Seeding cho bảng Categories - Cửa hàng Mỹ phẩm & Hóa phẩm Beauty Mart
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Chăm sóc da & Skincare",
                    Description = "Sữa rửa mặt, toner, serum, kem dưỡng ẩm, kem chống nắng"
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Trang điểm & Makeup",
                    Description = "Son môi, kem nền, phấn phủ, mascara, kẻ mắt"
                },
                new Category
                {
                    CategoryId = 3,
                    CategoryName = "Chăm sóc tóc & Cơ thể",
                    Description = "Dầu gội, dầu xả, sữa tắm, dưỡng thể, tẩy tế bào chết"
                },
                new Category
                {
                    CategoryId = 4,
                    CategoryName = "Nước hoa & Xịt thơm",
                    Description = "Nước hoa cao cấp EDP, EDT, xịt toàn thân body mist"
                },
                new Category
                {
                    CategoryId = 5,
                    CategoryName = "Hóa phẩm & Vệ sinh cá nhân",
                    Description = "Nước tẩy trang, bông tẩy trang, nước giặt đồ lót, dung dịch vệ sinh"
                }
            );

            // 2. Data Seeding cho bảng Products mẫu - Cửa hàng Mỹ phẩm & Hóa phẩm Beauty Mart
            modelBuilder.Entity<Product>().HasData(
                // ── Nhóm 1: Chăm sóc da & Skincare (CategoryId = 1) ──────────────────
                new Product
                {
                    ProductId = 1,
                    Barcode = "893123456001",
                    ProductName = "Sữa rửa mặt CeraVe Foaming Facial Cleanser 236ml",
                    Price = 285000,
                    StockQuantity = 50,
                    CategoryId = 1
                },
                new Product
                {
                    ProductId = 2,
                    Barcode = "893123456002",
                    ProductName = "Serum The Ordinary Niacinamide 10% + Zinc 1% 30ml",
                    Price = 195000,
                    StockQuantity = 40,
                    CategoryId = 1
                },
                new Product
                {
                    ProductId = 3,
                    Barcode = "893123456003",
                    ProductName = "Kem chống nắng Anessa Perfect UV Sunscreen SPF50+ 60g",
                    Price = 420000,
                    StockQuantity = 35,
                    CategoryId = 1
                },
                new Product
                {
                    ProductId = 4,
                    Barcode = "893123456004",
                    ProductName = "Toner Paula's Choice BHA Liquid Exfoliant 118ml",
                    Price = 680000,
                    StockQuantity = 25,
                    CategoryId = 1
                },
                new Product
                {
                    ProductId = 5,
                    Barcode = "893123456005",
                    ProductName = "Kem dưỡng ẩm Neutrogena Hydro Boost Water Gel 50ml",
                    Price = 310000,
                    StockQuantity = 45,
                    CategoryId = 1
                },

                // ── Nhóm 2: Trang điểm & Makeup (CategoryId = 2) ─────────────────────
                new Product
                {
                    ProductId = 6,
                    Barcode = "893123456006",
                    ProductName = "Son kem lì 3CE Velvet Lip Tint #Reddish",
                    Price = 290000,
                    StockQuantity = 55,
                    CategoryId = 2
                },
                new Product
                {
                    ProductId = 7,
                    Barcode = "893123456007",
                    ProductName = "Kem nền Maybelline Fit Me Matte+Poreless 30ml SPF22",
                    Price = 175000,
                    StockQuantity = 60,
                    CategoryId = 2
                },
                new Product
                {
                    ProductId = 8,
                    Barcode = "893123456008",
                    ProductName = "Phấn phủ kiềm dầu Innisfree No-Sebum Mineral Powder 5g",
                    Price = 145000,
                    StockQuantity = 70,
                    CategoryId = 2
                },
                new Product
                {
                    ProductId = 9,
                    Barcode = "893123456009",
                    ProductName = "Mascara L'Oreal Lash Paradise Waterproof 6.4ml",
                    Price = 195000,
                    StockQuantity = 40,
                    CategoryId = 2
                },

                // ── Nhóm 3: Chăm sóc tóc & Cơ thể (CategoryId = 3) ──────────────────
                new Product
                {
                    ProductId = 10,
                    Barcode = "893123456010",
                    ProductName = "Dầu gội bưởi Cocoon phục hồi tóc hư tổn 310ml",
                    Price = 165000,
                    StockQuantity = 60,
                    CategoryId = 3
                },
                new Product
                {
                    ProductId = 11,
                    Barcode = "893123456011",
                    ProductName = "Dầu xả TRESemmé Keratin Smooth dưỡng tóc suôn mượt 340ml",
                    Price = 89000,
                    StockQuantity = 55,
                    CategoryId = 3
                },
                new Product
                {
                    ProductId = 12,
                    Barcode = "893123456012",
                    ProductName = "Sữa tắm Dove Deeply Nourishing Body Wash 500ml",
                    Price = 95000,
                    StockQuantity = 80,
                    CategoryId = 3
                },
                new Product
                {
                    ProductId = 13,
                    Barcode = "893123456013",
                    ProductName = "Dưỡng thể St. Ives Energizing Coconut & Coffee 200ml",
                    Price = 135000,
                    StockQuantity = 45,
                    CategoryId = 3
                },

                // ── Nhóm 4: Nước hoa & Xịt thơm (CategoryId = 4) ────────────────────
                new Product
                {
                    ProductId = 14,
                    Barcode = "893123456014",
                    ProductName = "Nước hoa Victoria's Secret Bombshell EDP 50ml",
                    Price = 750000,
                    StockQuantity = 20,
                    CategoryId = 4
                },
                new Product
                {
                    ProductId = 15,
                    Barcode = "893123456015",
                    ProductName = "Xịt thơm toàn thân Bath & Body Works Japanese Cherry Blossom 236ml",
                    Price = 320000,
                    StockQuantity = 30,
                    CategoryId = 4
                },

                // ── Nhóm 5: Hóa phẩm & Vệ sinh cá nhân (CategoryId = 5) ─────────────
                new Product
                {
                    ProductId = 16,
                    Barcode = "893123456016",
                    ProductName = "Nước tẩy trang Bioderma Sensibio H2O Micellar Water 500ml",
                    Price = 380000,
                    StockQuantity = 45,
                    CategoryId = 5
                }
            );

            // 3. Data Seeding cho bảng Customers (Khách hàng thân thiết)
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    CustomerId = 1,
                    CustomerName = "Nguyễn Văn A",
                    PhoneNumber = "0901122334",
                    Address = "123 Lê Lợi, Quận 1, TP.HCM",
                    RewardPoints = 150,
                    MembershipRank = "Vàng"
                },
                new Customer
                {
                    CustomerId = 2,
                    CustomerName = "Trần Thị B",
                    PhoneNumber = "0918877665",
                    Address = "45 Hai Bà Trưng, Hà Nội",
                    RewardPoints = 50,
                    MembershipRank = "Bạc"
                },
                new Customer
                {
                    CustomerId = 3,
                    CustomerName = "Lê Văn C",
                    PhoneNumber = "0983344556",
                    Address = "78 Nguyễn Huệ, Đà Nẵng",
                    RewardPoints = 10,
                    MembershipRank = "Chuẩn"
                },
                new Customer
                {
                    CustomerId = 4,
                    CustomerName = "Nguyễn Thị D",
                    PhoneNumber = "0901122335",
                    Address = "12 Phố Hoa, Quận 1",
                    RewardPoints = 20,
                    MembershipRank = "Chuẩn"
                },
                new Customer
                {
                    CustomerId = 5,
                    CustomerName = "Phạm Văn E",
                    PhoneNumber = "0902233445",
                    Address = "34 Đường Lý, Quận 3",
                    RewardPoints = 75,
                    MembershipRank = "Bạc"
                },
                new Customer
                {
                    CustomerId = 6,
                    CustomerName = "Lê Thị F",
                    PhoneNumber = "0903344556",
                    Address = "56 Ngõ Trường, Quận 5",
                    RewardPoints = 200,
                    MembershipRank = "Vàng"
                },
                new Customer
                {
                    CustomerId = 7,
                    CustomerName = "Trần Văn G",
                    PhoneNumber = "0904455667",
                    Address = "78 Phố Mới, Quận 7",
                    RewardPoints = 5,
                    MembershipRank = "Chuẩn"
                },
                new Customer
                {
                    CustomerId = 8,
                    CustomerName = "Hoàng Thị H",
                    PhoneNumber = "0905566778",
                    Address = "90 Đường Sông, Quận 2",
                    RewardPoints = 120,
                    MembershipRank = "Vàng"
                },
                new Customer
                {
                    CustomerId = 9,
                    CustomerName = "Phan Văn I",
                    PhoneNumber = "0906677889",
                    Address = "101 Khu 9, Quận 10",
                    RewardPoints = 60,
                    MembershipRank = "Bạc"
                },
                new Customer
                {
                    CustomerId = 10,
                    CustomerName = "Đỗ Thị K",
                    PhoneNumber = "0907788990",
                    Address = "11 Phố Cũ, Quận 4",
                    RewardPoints = 15,
                    MembershipRank = "Chuẩn"
                },
                new Customer
                {
                    CustomerId = 11,
                    CustomerName = "Vũ Văn L",
                    PhoneNumber = "0908899001",
                    Address = "22 Khu Phố, Bình Thạnh",
                    RewardPoints = 40,
                    MembershipRank = "Chuẩn"
                },
                new Customer
                {
                    CustomerId = 12,
                    CustomerName = "Bùi Thị M",
                    PhoneNumber = "0909900112",
                    Address = "33 Đại Lộ, Tân Bình",
                    RewardPoints = 95,
                    MembershipRank = "Bạc"
                },
                new Customer
                {
                    CustomerId = 13,
                    CustomerName = "Hồ Văn N",
                    PhoneNumber = "0910011223",
                    Address = "44 Ngõ Lớn, Phú Nhuận",
                    RewardPoints = 10,
                    MembershipRank = "Chuẩn"
                },
                new Customer
                {
                    CustomerId = 14,
                    CustomerName = "Đặng Thị O",
                    PhoneNumber = "0911122334",
                    Address = "55 Khu 5, Gò Vấp",
                    RewardPoints = 180,
                    MembershipRank = "Vàng"
                },
                new Customer
                {
                    CustomerId = 15,
                    CustomerName = "Lê Văn P",
                    PhoneNumber = "0912233445",
                    Address = "66 Đường 66, Thủ Đức",
                    RewardPoints = 30,
                    MembershipRank = "Chuẩn"
                }
            );
        }
    }
}
