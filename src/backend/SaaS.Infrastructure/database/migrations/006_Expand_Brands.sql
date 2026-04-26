USE MultiTenantSaaS_DB;
GO

-- 1. Expand Brands table
ALTER TABLE Brands ADD Slogan NVARCHAR(MAX) NULL;
ALTER TABLE Brands ADD Description NVARCHAR(MAX) NULL;
ALTER TABLE Brands ADD Email NVARCHAR(255) NULL;
ALTER TABLE Brands ADD Phone NVARCHAR(50) NULL;
ALTER TABLE Brands ADD PrivacyPolicy NVARCHAR(MAX) NULL;
ALTER TABLE Brands ADD TermsOfService NVARCHAR(MAX) NULL;
GO

-- 2. Update existing brands with dynamic data
UPDATE Brands SET 
    Slogan = 'The Corporate Holding Group',
    Description = 'Managing a diverse ecosystem of industry-leading brands. From organic commerce to high-end jewelry and technical excellence.',
    Email = 'mrrajeev18@gmail.com',
    Phone = '+917032075893',
    PrivacyPolicy = '# Privacy Policy for Rajeev’s Pvt. Ltd\n\nYour privacy is important to us...',
    TermsOfService = '# Terms of Service\n\nBy using our platform, you agree to these terms...'
WHERE TenantId = 'rajeev-pvt';

UPDATE Brands SET 
    Slogan = 'Freshness Delivered to Your Doorstep',
    Description = 'Your premier destination for organic groceries and farm-fresh products. B2C and B2B commerce simplified.',
    Email = 'support@greenpantry.com',
    Phone = '+919999999991',
    PrivacyPolicy = '# GreenPantry Privacy Policy\n\nWe care about your greens and your data.',
    TermsOfService = '# GreenPantry Terms\n\nOrganic rules apply.'
WHERE TenantId = 'greenpantry';

UPDATE Brands SET 
    Slogan = 'Accelerating Digital Transformation',
    Description = 'Cutting-edge SaaS solutions, technical consulting, and managed cloud infrastructure for the modern enterprise.',
    Email = 'info@omega.tech',
    Phone = '+919999999992',
    PrivacyPolicy = '# Omega Tech Privacy Policy\n\nData is our DNA.',
    TermsOfService = '# Omega Tech Terms\n\nStandard software terms apply.'
WHERE TenantId = 'omega';

UPDATE Brands SET 
    Slogan = 'Heritage in Every Glimmer',
    Description = 'Exquisite gold and diamond jewelry. A legacy of trust, purity, and craftsmanship spanning generations.',
    Email = 'sales@bangaru.com',
    Phone = '+919999999993',
    PrivacyPolicy = '# BangaruKottu Privacy Policy\n\nYour valuables are safe with us.',
    TermsOfService = '# BangaruKottu Terms\n\nPurity is guaranteed.'
WHERE TenantId = 'bangaru';
GO

-- 3. Seed Users for all roles in all brands
-- Roles: Admin, Customer, Vendor, Delivery Partner
-- Passwords set to 'Pass123'

-- Helper to get Role IDs
DECLARE @AdminRoleId UNIQUEIDENTIFIER, @CustomerRoleId UNIQUEIDENTIFIER, @VendorRoleId UNIQUEIDENTIFIER, @DeliveryRoleId UNIQUEIDENTIFIER;

-- Rajeev Pvt Users
SELECT TOP 1 @AdminRoleId = Id FROM Roles WHERE TenantId = 'rajeev-pvt' AND Name = 'Admin';
INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, IsActive, CreatedBy) VALUES
(NEWID(), 'rajeev-pvt', 'rajeev_admin', 'admin@rajeev.com', 'Pass123', @AdminRoleId, 1, 'System');

-- GreenPantry Users
SELECT TOP 1 @AdminRoleId = Id FROM Roles WHERE TenantId = 'greenpantry' AND Name = 'Admin';
SELECT TOP 1 @CustomerRoleId = Id FROM Roles WHERE TenantId = 'greenpantry' AND Name = 'Customer';
SELECT TOP 1 @VendorRoleId = Id FROM Roles WHERE TenantId = 'greenpantry' AND Name = 'Vendor';
SELECT TOP 1 @DeliveryRoleId = Id FROM Roles WHERE TenantId = 'greenpantry' AND Name = 'Delivery Partner';

INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, IsActive, CreatedBy) VALUES
(NEWID(), 'greenpantry', 'gp_admin', 'admin@greenpantry.com', 'Pass123', @AdminRoleId, 1, 'System'),
(NEWID(), 'greenpantry', 'gp_customer', 'customer@greenpantry.com', 'Pass123', @CustomerRoleId, 1, 'System'),
(NEWID(), 'greenpantry', 'gp_vendor', 'vendor@greenpantry.com', 'Pass123', @VendorRoleId, 1, 'System'),
(NEWID(), 'greenpantry', 'gp_delivery', 'delivery@greenpantry.com', 'Pass123', @DeliveryRoleId, 1, 'System');

-- Omega Users
SELECT TOP 1 @AdminRoleId = Id FROM Roles WHERE TenantId = 'omega' AND Name = 'Admin';
SELECT TOP 1 @CustomerRoleId = Id FROM Roles WHERE TenantId = 'omega' AND Name = 'Customer';

INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, IsActive, CreatedBy) VALUES
(NEWID(), 'omega', 'omega_admin', 'admin@omega.com', 'Pass123', @AdminRoleId, 1, 'System'),
(NEWID(), 'omega', 'omega_customer', 'customer@omega.com', 'Pass123', @CustomerRoleId, 1, 'System');

-- Bangaru Users
SELECT TOP 1 @AdminRoleId = Id FROM Roles WHERE TenantId = 'bangaru' AND Name = 'Admin';
SELECT TOP 1 @CustomerRoleId = Id FROM Roles WHERE TenantId = 'bangaru' AND Name = 'Customer';

INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, IsActive, CreatedBy) VALUES
(NEWID(), 'bangaru', 'bangaru_admin', 'admin@bangaru.com', 'Pass123', @AdminRoleId, 1, 'System'),
(NEWID(), 'bangaru', 'bangaru_customer', 'customer@bangaru.com', 'Pass123', @CustomerRoleId, 1, 'System');
GO
