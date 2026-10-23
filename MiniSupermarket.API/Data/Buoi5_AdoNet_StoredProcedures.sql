-- =========================================================================
-- HỌC PHẦN: PHÁT TRIỂN ỨNG DỤNG VỚI .NET CORE & SQL SERVER
-- BUỔI 5: ADO.NET NÂNG CAO, STORED PROCEDURE & GIAO DỊCH POS CHECKOUT
-- Tệp kịch bản CSDL: Tạo bảng Orders, OrderDetails, Seed 15 hóa đơn mẫu,
-- và tạo 3 Stored Procedure phục vụ Báo cáo doanh thu.
-- =========================================================================

-- 1. BẢNG Orders (Hóa đơn bán hàng tổng thể)
IF OBJECT_ID('OrderDetails', 'U') IS NOT NULL DROP TABLE OrderDetails;
IF OBJECT_ID('Orders', 'U') IS NOT NULL DROP TABLE Orders;

CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    OrderCode VARCHAR(30) NOT NULL UNIQUE,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CashierUsername VARCHAR(50) NOT NULL,
    CustomerId INT NULL,
    TotalItems INT NOT NULL DEFAULT 0,
    FinalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    RewardPoints INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);

-- 2. BẢNG OrderDetails (Chi tiết các mặt hàng trong hóa đơn)
CREATE TABLE OrderDetails (
    OrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

-- 3. DỮ LIỆU MẪU 15 HÓA ĐƠN THEO ĐÚNG GIÁ TRỊ BUỔI 5:
-- Cashier05: 3 đơn - Doanh thu: 1,328,000 - Điểm: 132
-- Cashier03: 2 đơn - Doanh thu: 1,435,000 - Điểm: 143
-- Cashier01: 4 đơn - Doanh thu: 1,322,000 - Điểm: 131
-- Cashier04: 3 đơn - Doanh thu:   484,000 - Điểm:  48
-- Cashier02: 3 đơn - Doanh thu:   253,000 - Điểm:  24
-- Tổng: 15 đơn - Tổng doanh thu: 4,822,000 - Tổng điểm: 478

SET IDENTITY_INSERT Orders ON;

-- cashier01 (4 hóa đơn: 420000 + 310000 + 280000 + 312000 = 1,322,000; Điểm: 42 + 31 + 28 + 30 = 131)
INSERT INTO Orders (OrderId, OrderCode, CreatedDate, CashierUsername, CustomerId, TotalItems, FinalAmount, RewardPoints)
VALUES 
(1, 'HD-20260830-01', '2026-08-30 08:15:00', 'cashier01', 1, 1, 420000.00, 42),
(2, 'HD-20260830-02', '2026-08-30 09:30:00', 'cashier01', 2, 1, 310000.00, 31),
(3, 'HD-20260830-03', '2026-08-30 10:45:00', 'cashier01', 3, 1, 280000.00, 28),
(4, 'HD-20260830-04', '2026-08-30 11:20:00', 'cashier01', 4, 2, 312000.00, 30);

-- cashier02 (3 hóa đơn: 95000 + 78000 + 80000 = 253,000; Điểm: 9 + 7 + 8 = 24)
INSERT INTO Orders (OrderId, OrderCode, CreatedDate, CashierUsername, CustomerId, TotalItems, FinalAmount, RewardPoints)
VALUES 
(5, 'HD-20260830-05', '2026-08-30 13:10:00', 'cashier02', 5, 1, 95000.00, 9),
(6, 'HD-20260830-06', '2026-08-30 14:00:00', 'cashier02', 6, 1, 78000.00, 7),
(7, 'HD-20260830-07', '2026-08-30 15:15:00', 'cashier02', 7, 1, 80000.00, 8);

-- cashier03 (2 hóa đơn: 755000 + 680000 = 1,435,000; Điểm: 75 + 68 = 143)
INSERT INTO Orders (OrderId, OrderCode, CreatedDate, CashierUsername, CustomerId, TotalItems, FinalAmount, RewardPoints)
VALUES 
(8, 'HD-20260830-08', '2026-08-30 16:05:00', 'cashier03', 8, 2, 755000.00, 75),
(9, 'HD-20260830-09', '2026-08-30 16:50:00', 'cashier03', 9, 1, 680000.00, 68);

-- cashier04 (3 hóa đơn: 195000 + 145000 + 144000 = 484,000; Điểm: 19 + 14 + 15 = 48)
INSERT INTO Orders (OrderId, OrderCode, CreatedDate, CashierUsername, CustomerId, TotalItems, FinalAmount, RewardPoints)
VALUES 
(10, 'HD-20260830-10', '2026-08-30 17:30:00', 'cashier04', 10, 1, 195000.00, 19),
(11, 'HD-20260830-11', '2026-08-30 18:10:00', 'cashier04', 11, 1, 145000.00, 14),
(12, 'HD-20260830-12', '2026-08-30 19:00:00', 'cashier04', 12, 1, 144000.00, 15);

-- cashier05 (3 hóa đơn: 528000 + 400000 + 400000 = 1,328,000; Điểm: 52 + 40 + 40 = 132)
INSERT INTO Orders (OrderId, OrderCode, CreatedDate, CashierUsername, CustomerId, TotalItems, FinalAmount, RewardPoints)
VALUES 
(13, 'HD-20260830-13', '2026-08-30 19:45:00', 'cashier05', 13, 2, 528000.00, 52),
(14, 'HD-20260830-14', '2026-08-30 20:30:00', 'cashier05', 14, 1, 400000.00, 40),
(15, 'HD-20260830-15', '2026-08-30 21:15:00', 'cashier05', 15, 1, 400000.00, 40);

SET IDENTITY_INSERT Orders OFF;

-- Chi tiết OrderDetails tương ứng
INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, LineTotal)
VALUES
(1, 3, 1, 420000.00, 420000.00),
(2, 5, 1, 310000.00, 310000.00),
(3, 1, 1, 280000.00, 280000.00),
(4, 2, 1, 195000.00, 195000.00),
(4, 7, 1, 117000.00, 117000.00),
(5, 15, 1, 95000.00, 95000.00),
(6, 12, 1, 78000.00, 78000.00),
(7, 13, 1, 80000.00, 80000.00),
(8, 2, 1, 195000.00, 195000.00),
(8, 6, 2, 280000.00, 560000.00),
(9, 4, 1, 680000.00, 680000.00),
(10, 2, 1, 195000.00, 195000.00),
(11, 8, 1, 145000.00, 145000.00),
(12, 9, 1, 144000.00, 144000.00),
(13, 1, 1, 280000.00, 280000.00),
(13, 10, 2, 124000.00, 248000.00),
(14, 11, 2, 200000.00, 400000.00),
(15, 11, 2, 200000.00, 400000.00);

GO

-- =========================================================================
-- TIẾT 3: STORED PROCEDURE 1 - BÁO CÁO DOANH THU THEO THU NGÂN
-- =========================================================================
IF OBJECT_ID('sp_GetRevenueByCashier', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetRevenueByCashier;
GO

CREATE PROCEDURE sp_GetRevenueByCashier
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CashierUsername,
        COUNT(OrderId) AS TotalOrders,
        SUM(FinalAmount) AS TotalRevenue,
        SUM(RewardPoints) AS TotalPointsAwarded
    FROM Orders
    GROUP BY CashierUsername
    ORDER BY TotalRevenue DESC;
END
GO

-- =========================================================================
-- TIẾT 3: BÀI TẬP 3.1 - BÁO CÁO DOANH THU THEO KHOẢNG NGÀY (CÓ THAM SỐ)
-- =========================================================================
IF OBJECT_ID('sp_GetRevenueByDateRange', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetRevenueByDateRange;
GO

CREATE PROCEDURE sp_GetRevenueByDateRange
    @FromDate DATETIME,
    @ToDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CashierUsername,
        COUNT(OrderId) AS TotalOrders,
        SUM(FinalAmount) AS TotalRevenue,
        SUM(RewardPoints) AS TotalPointsAwarded
    FROM Orders
    WHERE CreatedDate >= @FromDate AND CreatedDate <= @ToDate
    GROUP BY CashierUsername
    ORDER BY TotalRevenue DESC;
END
GO

-- =========================================================================
-- TIẾT 3: BÀI TẬP 3.2 - TOP 5 SẢN PHẨM BÁN CHẠY NHẤT
-- =========================================================================
IF OBJECT_ID('sp_GetTop5SellingProducts', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetTop5SellingProducts;
GO

CREATE PROCEDURE sp_GetTop5SellingProducts
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 5
        p.ProductId,
        p.Barcode,
        p.ProductName,
        SUM(d.Quantity) AS TotalQuantitySold,
        SUM(d.LineTotal) AS TotalSalesRevenue
    FROM OrderDetails d
    INNER JOIN Products p ON d.ProductId = p.ProductId
    GROUP BY p.ProductId, p.Barcode, p.ProductName
    ORDER BY TotalQuantitySold DESC, TotalSalesRevenue DESC;
END
GO
