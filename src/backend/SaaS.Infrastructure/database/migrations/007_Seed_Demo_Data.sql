USE MultiTenantSaaS_DB;
GO

-- Cleanup existing demo data to avoid duplicates
DELETE FROM OrderItems WHERE TenantId IN ('greenpantry', 'omega', 'bangaru');
DELETE FROM Orders WHERE TenantId IN ('greenpantry', 'omega', 'bangaru');
DELETE FROM Products WHERE TenantId IN ('greenpantry', 'omega', 'bangaru');
GO

-- 1. Seed Products for GreenPantry (Organic)
DECLARE @GP_Id NVARCHAR(50) = 'greenpantry';
INSERT INTO Products (Id, TenantId, Name, Description, Price, Category, CreatedBy) VALUES
(NEWID(), @GP_Id, 'Organic Avocado', 'Ripe Hass avocados from sustainable farms.', 2.99, 'Fruits', 'System'),
(NEWID(), @GP_Id, 'Farm Fresh Milk', 'Whole milk, hormone-free, 1L.', 3.50, 'Dairy', 'System'),
(NEWID(), @GP_Id, 'Almond Flour', 'Gluten-free baking essential.', 12.99, 'Bakery', 'System'),
(NEWID(), @GP_Id, 'Himalayan Pink Salt', 'Premium mineral-rich salt.', 4.50, 'Spices', 'System');

-- 2. Seed Products for Omega Tech (Services)
DECLARE @Omega_Id NVARCHAR(50) = 'omega';
INSERT INTO Products (Id, TenantId, Name, Description, Price, Category, CreatedBy) VALUES
(NEWID(), @Omega_Id, 'Cloud Infrastructure Audit', 'Professional security and cost audit for AWS/Azure.', 1500.00, 'Cloud', 'System'),
(NEWID(), @Omega_Id, 'React Consulting (Per Hour)', 'Expert guidance from senior frontend engineers.', 120.00, 'Development', 'System'),
(NEWID(), @Omega_Id, 'DevOps Managed Service', 'Monthly 24/7 infrastructure management.', 2500.00, 'Managed Services', 'System');

-- 3. Seed Products for BangaruKottu (Luxury)
DECLARE @Bangaru_Id NVARCHAR(50) = 'bangaru';
INSERT INTO Products (Id, TenantId, Name, Description, Price, Category, CreatedBy) VALUES
(NEWID(), @Bangaru_Id, '24K Gold Necklace', 'Classic hand-crafted pure gold chain.', 1850.00, 'Gold', 'System'),
(NEWID(), @Bangaru_Id, 'Diamond Stud Earrings', '0.5ct VS1 clarity certified diamonds.', 3200.00, 'Diamonds', 'System'),
(NEWID(), @Bangaru_Id, 'Temple Design Bangle', 'Antique finish traditional South Indian jewelry.', 2400.00, 'Bridal', 'System');
GO

-- 4. Seed Orders and Sample Activity for GreenPantry
DECLARE @GP_CustomerId UNIQUEIDENTIFIER;
SELECT TOP 1 @GP_CustomerId = Id FROM Users WHERE TenantId = 'greenpantry' AND Email = 'customer@greenpantry.com';

IF @GP_CustomerId IS NOT NULL
BEGIN
    DECLARE @OrderId UNIQUEIDENTIFIER = NEWID();
    INSERT INTO Orders (Id, TenantId, UserId, TotalAmount, Status, CreatedBy) 
    VALUES (@OrderId, 'greenpantry', @GP_CustomerId, 15.99, 'Completed', 'System');

    DECLARE @ProdId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ProdId = Id FROM Products WHERE TenantId = 'greenpantry' AND Name = 'Organic Avocado';
    
    INSERT INTO OrderItems (Id, TenantId, OrderId, ProductId, Quantity, UnitPrice)
    VALUES (NEWID(), 'greenpantry', @OrderId, @ProdId, 2, 2.99);
END
GO

-- 5. Add Feature Flags (Verified columns: Id, TenantId, FeatureKey, IsEnabled)
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'FeatureFlags')
BEGIN
    DELETE FROM FeatureFlags WHERE TenantId IN ('greenpantry', 'omega', 'bangaru');
    INSERT INTO FeatureFlags (Id, TenantId, FeatureKey, IsEnabled) VALUES
    (NEWID(), 'greenpantry', 'DeliveryTracking', 1),
    (NEWID(), 'omega', 'PremiumSupportChat', 1),
    (NEWID(), 'bangaru', 'VirtalTryOn', 0);
END
GO
