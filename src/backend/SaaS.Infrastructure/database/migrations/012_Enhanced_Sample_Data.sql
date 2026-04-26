USE MultiTenantSaaS_DB;
GO

-- ============================================================
-- Enhanced Sample Data for All Brands
-- This script adds comprehensive sample data for:
-- 1. GreenPantry (greenpantry)
-- 2. OmegaTechnologies (omega)
-- 3. BangaruKottu (bangaru)
-- ============================================================

-- Cleanup existing data
DELETE FROM OrderItems WHERE TenantId IN ('greenpantry', 'omega', 'bangaru');
DELETE FROM Orders WHERE TenantId IN ('greenpantry', 'omega', 'bangaru');
DELETE FROM Products WHERE TenantId IN ('greenpantry', 'omega', 'bangaru');
DELETE FROM Users WHERE TenantId IN ('greenpantry', 'omega', 'bangaru') AND Email NOT LIKE 'admin@%';
GO

-- ============================================================
-- GREENPANTRY - Organic Food Marketplace
-- ============================================================

-- GreenPantry Products
DECLARE @GP_TenantId NVARCHAR(50) = 'greenpantry';

INSERT INTO Products (Id, TenantId, Name, Description, Price, Category, CreatedBy, CreatedAt) VALUES
-- Fruits
(NEWID(), @GP_TenantId, 'Organic Avocado', 'Ripe Hass avocados from sustainable farms', 2.99, 'Fruits', 'System', GETUTCDATE()),
(NEWID(), @GP_TenantId, 'Organic Bananas', 'Fair-trade organic bananas', 1.49, 'Fruits', 'System', GETUTCDATE()),
(NEWID(), @GP_TenantId, 'Fresh Strawberries', 'Locally grown sweet strawberries', 4.99, 'Fruits', 'System', GETUTCDATE()),
(NEWID(), @GP_TenantId, 'Organic Apples', 'Crisp Fuji apples, pesticide-free', 3.49, 'Fruits', 'System', GETUTCDATE()),

-- Dairy
(NEWID(), @GP_TenantId, 'Farm Fresh Milk', 'Whole milk, hormone-free, 1L', 3.50, 'Dairy', 'System', GETUTCDATE()),
(NEWID(), @GP_TenantId, 'Organic Greek Yogurt', 'High protein, no added sugar, 500g', 5.99, 'Dairy', 'System', GETUTCDATE()),
(NEWID(), @GP_TenantId, 'Artisan Cheese', 'Handcrafted cheddar cheese', 8.99, 'Dairy', 'System', GETUTCDATE()),

-- Bakery
(NEWID(), @GP_TenantId, 'Almond Flour', 'Gluten-free baking essential, 500g', 12.99, 'Bakery', 'System', GETUTCDATE()),
(NEWID(), @GP_TenantId, 'Sourdough Bread', 'Artisan sourdough loaf', 6.99, 'Bakery', 'System', GETUTCDATE()),
(NEWID(), @GP_TenantId, 'Whole Wheat Bread', 'Freshly baked whole grain bread', 4.50, 'Bakery', 'System', GETUTCDATE()),

-- Spices & Pantry
(NEWID(), @GP_TenantId, 'Himalayan Pink Salt', 'Premium mineral-rich salt, 250g', 4.50, 'Spices', 'System', GETUTCDATE()),
(NEWID(), @GP_TenantId, 'Organic Turmeric', 'Premium golden turmeric powder, 100g', 6.99, 'Spices', 'System', GETUTCDATE()),
(NEWID(), @GP_TenantId, 'Extra Virgin Olive Oil', 'Cold-pressed EVOO, 500ml', 15.99, 'Pantry', 'System', GETUTCDATE()),
(NEWID(), @GP_TenantId, 'Organic Honey', 'Raw unfiltered honey, 350g', 11.99, 'Pantry', 'System', GETUTCDATE()),

-- Vegetables
(NEWID(), @GP_TenantId, 'Organic Spinach', 'Fresh baby spinach leaves, 200g', 3.99, 'Vegetables', 'System', GETUTCDATE()),
(NEWID(), @GP_TenantId, 'Cherry Tomatoes', 'Sweet organic cherry tomatoes, 300g', 4.49, 'Vegetables', 'System', GETUTCDATE());

-- GreenPantry Users
INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, CreatedAt, IsActive) 
SELECT 
    NEWID(), 
    @GP_TenantId, 
    'customer1', 
    'customer@greenpantry.com', 
    '$2a$11$8LwY8JJZqZ/ZB5zP6KXQ8.JhPDKXYdqxuQMQnH5XmZqHp5LX5YqHK', -- Pass123
    (SELECT TOP 1 Id FROM Roles WHERE TenantId = @GP_TenantId AND Name = 'Customer'),
    GETUTCDATE(),
    1
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'customer@greenpantry.com');

INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, CreatedAt, IsActive) 
SELECT 
    NEWID(), 
    @GP_TenantId, 
    'vendor1', 
    'vendor@greenpantry.com', 
    '$2a$11$8LwY8JJZqZ/ZB5zP6KXQ8.JhPDKXYdqxuQMQnH5XmZqHp5LX5YqHK', -- Pass123
    (SELECT TOP 1 Id FROM Roles WHERE TenantId = @GP_TenantId AND Name = 'Vendor'),
    GETUTCDATE(),
    1
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'vendor@greenpantry.com');

GO

-- ============================================================
-- OMEGA TECHNOLOGIES - Tech Services & Solutions
-- ============================================================

DECLARE @Omega_TenantId NVARCHAR(50) = 'omega';

INSERT INTO Products (Id, TenantId, Name, Description, Price, Category, CreatedBy, CreatedAt) VALUES
-- Cloud Services
(NEWID(), @Omega_TenantId, 'Cloud Infrastructure Audit', 'Professional security and cost audit for AWS/Azure/GCP', 1500.00, 'Cloud Services', 'System', GETUTCDATE()),
(NEWID(), @Omega_TenantId, 'Cloud Migration Service', 'End-to-end cloud migration with zero downtime', 5000.00, 'Cloud Services', 'System', GETUTCDATE()),
(NEWID(), @Omega_TenantId, 'Cloud Backup Solutions', 'Automated backup and disaster recovery', 800.00, 'Cloud Services', 'System', GETUTCDATE()),

-- Development Services
(NEWID(), @Omega_TenantId, 'React Consulting (Per Hour)', 'Expert guidance from senior frontend engineers', 120.00, 'Development', 'System', GETUTCDATE()),
(NEWID(), @Omega_TenantId, 'Full-Stack Development', 'Custom web application development', 150.00, 'Development', 'System', GETUTCDATE()),
(NEWID(), @Omega_TenantId, 'Mobile App Development', 'iOS and Android native app development', 180.00, 'Development', 'System', GETUTCDATE()),
(NEWID(), @Omega_TenantId, 'Code Review Service', 'Professional code review and optimization', 90.00, 'Development', 'System', GETUTCDATE()),

-- DevOps Services
(NEWID(), @Omega_TenantId, 'DevOps Managed Service', 'Monthly 24/7 infrastructure management', 2500.00, 'Managed Services', 'System', GETUTCDATE()),
(NEWID(), @Omega_TenantId, 'CI/CD Pipeline Setup', 'Automated deployment pipeline configuration', 1200.00, 'DevOps', 'System', GETUTCDATE()),
(NEWID(), @Omega_TenantId, 'Kubernetes Consulting', 'Container orchestration and management', 200.00, 'DevOps', 'System', GETUTCDATE()),

-- Cybersecurity
(NEWID(), @Omega_TenantId, 'Security Audit', 'Comprehensive security assessment and penetration testing', 3500.00, 'Security', 'System', GETUTCDATE()),
(NEWID(), @Omega_TenantId, 'SSL Certificate Management', 'Annual SSL certificate setup and renewal', 299.00, 'Security', 'System', GETUTCDATE()),

-- Training & Support
(NEWID(), @Omega_TenantId, 'Technical Training Session', 'Custom training for your team (per day)', 950.00, 'Training', 'System', GETUTCDATE()),
(NEWID(), @Omega_TenantId, 'Premium Support Package', 'Priority support with 2-hour response SLA', 499.00, 'Support', 'System', GETUTCDATE()),

-- SaaS Products
(NEWID(), @Omega_TenantId, 'Project Management Tool', 'Monthly subscription for project management platform', 49.99, 'SaaS', 'System', GETUTCDATE()),
(NEWID(), @Omega_TenantId, 'API Monitoring Service', 'Real-time API health monitoring and alerts', 79.99, 'SaaS', 'System', GETUTCDATE());

-- Omega Users
INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, CreatedAt, IsActive) 
SELECT 
    NEWID(), 
    @Omega_TenantId, 
    'techuser1', 
    'user@omegatech.com', 
    '$2a$11$8LwY8JJZqZ/ZB5zP6KXQ8.JhPDKXYdqxuQMQnH5XmZqHp5LX5YqHK', -- Pass123
    (SELECT TOP 1 Id FROM Roles WHERE TenantId = @Omega_TenantId AND Name = 'Customer'),
    GETUTCDATE(),
    1
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'user@omegatech.com');

INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, CreatedAt, IsActive) 
SELECT 
    NEWID(), 
    @Omega_TenantId, 
    'consultant1', 
    'consultant@omegatech.com', 
    '$2a$11$8LwY8JJZqZ/ZB5zP6KXQ8.JhPDKXYdqxuQMQnH5XmZqHp5LX5YqHK', -- Pass123
    (SELECT TOP 1 Id FROM Roles WHERE TenantId = @Omega_TenantId AND Name = 'Vendor'),
    GETUTCDATE(),
    1
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'consultant@omegatech.com');

GO

-- ============================================================
-- BANGARU KOTTU - Luxury Jewelry
-- ============================================================

DECLARE @Bangaru_TenantId NVARCHAR(50) = 'bangaru';

INSERT INTO Products (Id, TenantId, Name, Description, Price, Category, CreatedBy, CreatedAt) VALUES
-- Gold Jewelry
(NEWID(), @Bangaru_TenantId, '24K Gold Necklace', 'Classic hand-crafted pure gold chain, 20g', 1850.00, 'Gold', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Gold Choker Set', 'Traditional South Indian bridal choker set, 35g', 3200.00, 'Gold', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Gold Bangles (Pair)', 'Elegant 22K gold bangles, 25g each', 2800.00, 'Gold', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Gold Earrings', 'Jhumka style traditional gold earrings, 8g', 720.00, 'Gold', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Gold Pendant', 'Om design 22K gold pendant with chain, 5g', 485.00, 'Gold', 'System', GETUTCDATE()),

-- Diamond Jewelry
(NEWID(), @Bangaru_TenantId, 'Diamond Stud Earrings', '0.5ct VS1 clarity certified diamonds', 3200.00, 'Diamonds', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Diamond Solitaire Ring', '1ct brilliant cut diamond engagement ring', 8500.00, 'Diamonds', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Diamond Necklace Set', 'Exquisite diamond necklace with earrings', 15000.00, 'Diamonds', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Diamond Bracelet', 'Tennis bracelet with 2ct total diamond weight', 6800.00, 'Diamonds', 'System', GETUTCDATE()),

-- Bridal Collection
(NEWID(), @Bangaru_TenantId, 'Temple Design Bangle', 'Antique finish traditional South Indian jewelry', 2400.00, 'Bridal', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Bridal Necklace Set', 'Complete bridal set with necklace, earrings, maang tikka', 12500.00, 'Bridal', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Wedding Pendant Set', 'Grand pendant with matching earrings', 4500.00, 'Bridal', 'System', GETUTCDATE()),

-- Silver Jewelry
(NEWID(), @Bangaru_TenantId, 'Silver Anklets (Pair)', '925 sterling silver anklets with ghungroo', 450.00, 'Silver', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Silver Chain', 'Oxidized silver chain with pendant, 50g', 380.00, 'Silver', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Silver Bracelet', 'Contemporary design sterling silver bracelet', 320.00, 'Silver', 'System', GETUTCDATE()),

-- Precious Stones
(NEWID(), @Bangaru_TenantId, 'Ruby Pendant', 'Natural ruby stone pendant in gold setting', 5500.00, 'Gemstones', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Emerald Ring', 'Authentic emerald ring with diamond accents', 4200.00, 'Gemstones', 'System', GETUTCDATE()),
(NEWID(), @Bangaru_TenantId, 'Sapphire Earrings', 'Blue sapphire stud earrings in white gold', 3800.00, 'Gemstones', 'System', GETUTCDATE());

-- Bangaru Users
INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, CreatedAt, IsActive) 
SELECT 
    NEWID(), 
    @Bangaru_TenantId, 
    'customer1', 
    'customer@bangarukottu.com', 
    '$2a$11$8LwY8JJZqZ/ZB5zP6KXQ8.JhPDKXYdqxuQMQnH5XmZqHp5LX5YqHK', -- Pass123
    (SELECT TOP 1 Id FROM Roles WHERE TenantId = @Bangaru_TenantId AND Name = 'Customer'),
    GETUTCDATE(),
    1
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'customer@bangarukottu.com');

INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, CreatedAt, IsActive) 
SELECT 
    NEWID(), 
    @Bangaru_TenantId, 
    'jeweler1', 
    'jeweler@bangarukottu.com', 
    '$2a$11$8LwY8JJZqZ/ZB5zP6KXQ8.JhPDKXYdqxuQMQnH5XmZqHp5LX5YqHK', -- Pass123
    (SELECT TOP 1 Id FROM Roles WHERE TenantId = @Bangaru_TenantId AND Name = 'Vendor'),
    GETUTCDATE(),
    1
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'jeweler@bangarukottu.com');

GO

-- ============================================================
-- Sample Orders for Each Brand
-- ============================================================

-- GreenPantry Sample Order
DECLARE @GP_CustomerId UNIQUEIDENTIFIER;
DECLARE @GP_ProductId1 UNIQUEIDENTIFIER;
DECLARE @GP_OrderId UNIQUEIDENTIFIER = NEWID();

SELECT TOP 1 @GP_CustomerId = Id FROM Users WHERE TenantId = 'greenpantry' AND Email = 'customer@greenpantry.com';
SELECT TOP 1 @GP_ProductId1 = Id FROM Products WHERE TenantId = 'greenpantry' AND Name = 'Organic Avocado';

IF @GP_CustomerId IS NOT NULL AND @GP_ProductId1 IS NOT NULL
BEGIN
    INSERT INTO Orders (Id, TenantId, UserId, TotalAmount, Status, CreatedBy, CreatedAt) 
    VALUES (@GP_OrderId, 'greenpantry', @GP_CustomerId, 15.96, 'Delivered', 'System', DATEADD(day, -3, GETUTCDATE()));
    
    INSERT INTO OrderItems (Id, TenantId, OrderId, ProductId, Quantity, UnitPrice)
    VALUES (NEWID(), 'greenpantry', @GP_OrderId, @GP_ProductId1, 4, 2.99);
    
    SELECT TOP 1 @GP_ProductId1 = Id FROM Products WHERE TenantId = 'greenpantry' AND Name = 'Farm Fresh Milk';
    INSERT INTO OrderItems (Id, TenantId, OrderId, ProductId, Quantity, UnitPrice)
    VALUES (NEWID(), 'greenpantry', @GP_OrderId, @GP_ProductId1, 2, 3.50);
END

-- Omega Sample Order
DECLARE @Omega_UserId UNIQUEIDENTIFIER;
DECLARE @Omega_ProductId UNIQUEIDENTIFIER;
DECLARE @Omega_OrderId UNIQUEIDENTIFIER = NEWID();

SELECT TOP 1 @Omega_UserId = Id FROM Users WHERE TenantId = 'omega' AND Email = 'user@omegatech.com';
SELECT TOP 1 @Omega_ProductId = Id FROM Products WHERE TenantId = 'omega' AND Name = 'Cloud Infrastructure Audit';

IF @Omega_UserId IS NOT NULL AND @Omega_ProductId IS NOT NULL
BEGIN
    INSERT INTO Orders (Id, TenantId, UserId, TotalAmount, Status, CreatedBy, CreatedAt) 
    VALUES (@Omega_OrderId, 'omega', @Omega_UserId, 1500.00, 'Completed', 'System', DATEADD(day, -7, GETUTCDATE()));
    
    INSERT INTO OrderItems (Id, TenantId, OrderId, ProductId, Quantity, UnitPrice)
    VALUES (NEWID(), 'omega', @Omega_OrderId, @Omega_ProductId, 1, 1500.00);
END

-- Bangaru Sample Order
DECLARE @Bangaru_UserId UNIQUEIDENTIFIER;
DECLARE @Bangaru_ProductId UNIQUEIDENTIFIER;
DECLARE @Bangaru_OrderId UNIQUEIDENTIFIER = NEWID();

SELECT TOP 1 @Bangaru_UserId = Id FROM Users WHERE TenantId = 'bangaru' AND Email = 'customer@bangarukottu.com';
SELECT TOP 1 @Bangaru_ProductId = Id FROM Products WHERE TenantId = 'bangaru' AND Name = 'Diamond Stud Earrings';

IF @Bangaru_UserId IS NOT NULL AND @Bangaru_ProductId IS NOT NULL
BEGIN
    INSERT INTO Orders (Id, TenantId, UserId, TotalAmount, Status, CreatedBy, CreatedAt) 
    VALUES (@Bangaru_OrderId, 'bangaru', @Bangaru_UserId, 3200.00, 'Processing', 'System', DATEADD(day, -1, GETUTCDATE()));
    
    INSERT INTO OrderItems (Id, TenantId, OrderId, ProductId, Quantity, UnitPrice)
    VALUES (NEWID(), 'bangaru', @Bangaru_OrderId, @Bangaru_ProductId, 1, 3200.00);
END

GO

-- Verification Queries
PRINT '=== GreenPantry Data ===';
SELECT COUNT(*) AS ProductCount FROM Products WHERE TenantId = 'greenpantry';
SELECT COUNT(*) AS UserCount FROM Users WHERE TenantId = 'greenpantry';
SELECT COUNT(*) AS OrderCount FROM Orders WHERE TenantId = 'greenpantry';

PRINT '=== Omega Technologies Data ===';
SELECT COUNT(*) AS ProductCount FROM Products WHERE TenantId = 'omega';
SELECT COUNT(*) AS UserCount FROM Users WHERE TenantId = 'omega';
SELECT COUNT(*) AS OrderCount FROM Orders WHERE TenantId = 'omega';

PRINT '=== BangaruKottu Data ===';
SELECT COUNT(*) AS ProductCount FROM Products WHERE TenantId = 'bangaru';
SELECT COUNT(*) AS UserCount FROM Users WHERE TenantId = 'bangaru';
SELECT COUNT(*) AS OrderCount FROM Orders WHERE TenantId = 'bangaru';

GO
