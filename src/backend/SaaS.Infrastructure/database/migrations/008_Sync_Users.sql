USE MultiTenantSaaS_DB;
GO

-- 1. Remove transactional data first due to FKs
DELETE FROM OrderItems;
DELETE FROM Invoices;
DELETE FROM Payments;
DELETE FROM Orders;
DELETE FROM Notifications;
DELETE FROM AuditLogs;
DELETE FROM Users;
GO

-- 2. Seed Users for Rajeev Pvt (Corporate)
DECLARE @AdminRoleId UNIQUEIDENTIFIER;
SELECT TOP 1 @AdminRoleId = Id FROM Roles WHERE TenantId = 'rajeev-pvt' AND Name = 'Admin';

-- If Roles are missing, recreate them (redundancy for safety)
IF @AdminRoleId IS NULL
BEGIN
    SET @AdminRoleId = NEWID();
    INSERT INTO Roles (Id, TenantId, Name) VALUES (@AdminRoleId, 'rajeev-pvt', 'Admin');
END

INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, IsActive, CreatedBy) VALUES
(NEWID(), 'rajeev-pvt', 'rajeev_admin', 'admin@rajeev.com', 'Pass123', @AdminRoleId, 1, 'System');

-- 3. Seed Users for GreenPantry
DECLARE @GP_Admin UNIQUEIDENTIFIER, @GP_Cust UNIQUEIDENTIFIER, @GP_Vend UNIQUEIDENTIFIER, @GP_Deliv UNIQUEIDENTIFIER;
SELECT TOP 1 @GP_Admin = Id FROM Roles WHERE TenantId = 'greenpantry' AND Name = 'Admin';
SELECT TOP 1 @GP_Cust = Id FROM Roles WHERE TenantId = 'greenpantry' AND Name = 'Customer';
SELECT TOP 1 @GP_Vend = Id FROM Roles WHERE TenantId = 'greenpantry' AND Name = 'Vendor';
SELECT TOP 1 @GP_Deliv = Id FROM Roles WHERE TenantId = 'greenpantry' AND Name = 'Delivery Partner';

INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, IsActive, CreatedBy) VALUES
(NEWID(), 'greenpantry', 'gp_admin', 'admin@greenpantry.com', 'Pass123', @GP_Admin, 1, 'System'),
(NEWID(), 'greenpantry', 'gp_customer', 'customer@greenpantry.com', 'Pass123', @GP_Cust, 1, 'System'),
(NEWID(), 'greenpantry', 'gp_vendor', 'vendor@greenpantry.com', 'Pass123', @GP_Vend, 1, 'System'),
(NEWID(), 'greenpantry', 'gp_delivery', 'delivery@greenpantry.com', 'Pass123', @GP_Deliv, 1, 'System');

-- 4. Seed Users for Omega
DECLARE @Omega_Admin UNIQUEIDENTIFIER, @Omega_Cust UNIQUEIDENTIFIER;
SELECT TOP 1 @Omega_Admin = Id FROM Roles WHERE TenantId = 'omega' AND Name = 'Admin';
SELECT TOP 1 @Omega_Cust = Id FROM Roles WHERE TenantId = 'omega' AND Name = 'Customer';

INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, IsActive, CreatedBy) VALUES
(NEWID(), 'omega', 'omega_admin', 'admin@omega.com', 'Pass123', @Omega_Admin, 1, 'System'),
(NEWID(), 'omega', 'omega_customer', 'customer@omega.com', 'Pass123', @Omega_Cust, 1, 'System');

-- 5. Seed Users for Bangaru
DECLARE @Bangaru_Admin UNIQUEIDENTIFIER, @Bangaru_Cust UNIQUEIDENTIFIER;
SELECT TOP 1 @Bangaru_Admin = Id FROM Roles WHERE TenantId = 'bangaru' AND Name = 'Admin';
SELECT TOP 1 @Bangaru_Cust = Id FROM Roles WHERE TenantId = 'bangaru' AND Name = 'Customer';

INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, IsActive, CreatedBy) VALUES
(NEWID(), 'bangaru', 'bangaru_admin', 'admin@bangaru.com', 'Pass123', @Bangaru_Admin, 1, 'System'),
(NEWID(), 'bangaru', 'bangaru_customer', 'customer@bangaru.com', 'Pass123', @Bangaru_Cust, 1, 'System');
GO
