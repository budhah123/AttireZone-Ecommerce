-- ============================================================
-- AttireZone Database Setup Script
-- Run this in SQL Server Management Studio (SSMS)
-- ============================================================

USE master;
GO

-- Drop database if it exists (optional - comment out for safety)
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'AttireZone')
    DROP DATABASE AttireZone;
GO

CREATE DATABASE AttireZone;
GO

USE AttireZone;
GO

-- ============================================================
-- TABLES
-- ============================================================

-- Users table storing all customer and admin accounts
CREATE TABLE Users (
    UserId              INT IDENTITY(1,1) PRIMARY KEY,
    FullName            NVARCHAR(100) NOT NULL,
    Email               NVARCHAR(150) NOT NULL UNIQUE,
    Password            NVARCHAR(256) NOT NULL, -- Should be hashed in production
    CreatedDate         DATETIME NOT NULL DEFAULT GETDATE(),
    LastModifiedDate    DATETIME DEFAULT GETDATE(),
    Role                NVARCHAR(20) NOT NULL DEFAULT 'Customer' -- 'Customer' or 'Admin'
);

-- Categories for product organization
CREATE TABLE Categories (
    CategoryId   INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    ImageUrl     NVARCHAR(300),
    IsActive     BIT NOT NULL DEFAULT 1
);

-- Products catalog
CREATE TABLE Products (
    ProductId   INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Price       DECIMAL(10,2) NOT NULL,
    StockQty    INT NOT NULL DEFAULT 0,
    ImageUrl    NVARCHAR(300),
    CategoryId  INT NOT NULL FOREIGN KEY REFERENCES Categories(CategoryId),
    IsFeatured  BIT NOT NULL DEFAULT 0,
    IsActive    BIT NOT NULL DEFAULT 1,
    CreatedAt   DATETIME NOT NULL DEFAULT GETDATE()
);

-- Customer orders
CREATE TABLE Orders (
    OrderId     INT IDENTITY(1,1) PRIMARY KEY,
    UserId      INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    OrderDate   DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(10,2) NOT NULL,
    Status      NVARCHAR(30) NOT NULL DEFAULT 'Pending',
    ShipAddress NVARCHAR(300),
    Notes       NVARCHAR(500)
);

-- Individual items in orders
CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId     INT NOT NULL FOREIGN KEY REFERENCES Orders(OrderId),
    ProductId   INT NOT NULL FOREIGN KEY REFERENCES Products(ProductId),
    Quantity    INT NOT NULL,
    UnitPrice   DECIMAL(10,2) NOT NULL
);

-- Customer reviews and feedback
CREATE TABLE Feedback (
    FeedbackId  INT IDENTITY(1,1) PRIMARY KEY,
    UserId      INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    ProductId   INT NOT NULL FOREIGN KEY REFERENCES Products(ProductId),
    Rating      INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment     NVARCHAR(1000),
    CreatedAt   DATETIME NOT NULL DEFAULT GETDATE()
);

-- Login audit log (optional - for security tracking)
CREATE TABLE LoginAuditLog (
    LogId       INT IDENTITY(1,1) PRIMARY KEY,
    UserId      INT,
    Email       NVARCHAR(150),
    LoginTime   DATETIME NOT NULL DEFAULT GETDATE(),
    Success     BIT NOT NULL,
    IPAddress   NVARCHAR(50)
);

-- ============================================================
-- INDEXES
-- ============================================================

-- Create indexes for frequently searched columns
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Orders_UserId ON Orders(UserId);
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Feedback_UserId ON Feedback(UserId);
CREATE INDEX IX_Feedback_ProductId ON Feedback(ProductId);

-- ============================================================
-- SEED DATA
-- ============================================================

-- Admin user (Password: Admin@123)
-- SHA256 hash: 0c04a3beb63e7c17c0df15c72a7f4ccedb893d58bccfecc5b1db94a78e37d3a
INSERT INTO Users (FullName, Email, Password, CreatedDate, LastModifiedDate, Role)
VALUES ('Admin User', 'admin@attirezone.com',
        '0c04a3beb63e7c17c0df15c72a7f4ccedb893d58bccfecc5b1db94a78e37d3a',
    GETDATE(), GETDATE(), 'Admin');

-- Categories
INSERT INTO Categories (CategoryName, IsActive) VALUES
('T-Shirts',    1),
('Hoodies',     1),
('Shoes',       1),
('Bags',        1),
('Watches',     1),
('Sunglasses',  1);

-- Sample Products
INSERT INTO Products (Name, Description, Price, StockQty, CategoryId, IsFeatured, IsActive) VALUES
('Classic White Tee',       'Premium 100% cotton classic fit t-shirt in timeless white',   19.99, 100, 1, 1, 1),
('Graphic Print Tee',       'Bold graphic street-style t-shirt with contemporary design',    24.99,  80, 1, 0, 1),
('Urban Pullover Hoodie',   'Heavyweight fleece pullover hoodie, perfect for casual wear',   59.99,  60, 2, 1, 1),
('Zip-Up Hoodie',           'Full-zip slim-fit hoodie with side pockets',             64.99,  45, 2, 0, 1),
('Leather Sneakers',        'Premium white leather low-top sneaker with cushioned sole',89.99,  40, 3, 1, 1),
('Running Shoes',           'Lightweight performance running shoes with breathable mesh',74.99,  55, 3, 0, 1),
('Canvas Tote Bag',         'Minimalist canvas daily carry bag with interior pocket',    34.99,  70, 4, 0, 1),
('Leather Crossbody',       'Compact premium leather crossbody bag for on-the-go style',79.99,  30, 4, 1, 1),
('Classic Steel Watch',     'Minimalist stainless steel timepiece with date window',129.99,  25, 5, 1, 1),
('Sport Chronograph',       'Multi-function sport chronograph with stopwatch features',    149.99,  20, 5, 0, 1),
('Aviator Sunglasses',      'UV400 classic aviator sunglasses with polarized lenses',     49.99,  90, 6, 1, 1),
('Wayfarer Sunglasses',     'Retro square wayfarer frames with UV protection',         44.99,  85, 6, 0, 1);

GO

-- ============================================================
-- DATABASE VERIFICATION
-- ============================================================

-- Verify tables were created
SELECT 'Users' as TableName, COUNT(*) as RecordCount FROM Users
UNION ALL
SELECT 'Categories' as TableName, COUNT(*) as RecordCount FROM Categories
UNION ALL
SELECT 'Products' as TableName, COUNT(*) as RecordCount FROM Products;

GO

PRINT '==================================================';
PRINT 'AttireZone Database Setup Complete!';
PRINT '==================================================';
PRINT 'Database: AttireZone';
PRINT 'Admin User: admin@attirezone.com';
PRINT 'Admin Password: Admin@123';
PRINT '==================================================';
GO
