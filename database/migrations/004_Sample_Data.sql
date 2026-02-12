USE MultiTenantSaaS_DB;
GO

-- 1. Helper Variables for Guids
DECLARE @RajeevRole UNIQUEIDENTIFIER = NEWID();
DECLARE @GreenPantryRole UNIQUEIDENTIFIER = NEWID();
DECLARE @OmegaRole UNIQUEIDENTIFIER = NEWID();
DECLARE @BangaruRole UNIQUEIDENTIFIER = NEWID();

DECLARE @RajeevUser UNIQUEIDENTIFIER = NEWID();
DECLARE @GreenPantryUser UNIQUEIDENTIFIER = NEWID();
DECLARE @OmegaUser UNIQUEIDENTIFIER = NEWID();
DECLARE @BangaruUser UNIQUEIDENTIFIER = NEWID();

-- 2. Seed Roles
INSERT INTO Roles (Id, TenantId, Name) VALUES 
(@RajeevRole, 'rajeev-pvt', 'Admin'),
(@GreenPantryRole, 'greenpantry', 'Customer'),
(@OmegaRole, 'omega', 'TechnicalLead'),
(@BangaruRole, 'bangaru', 'StoreManager');

-- 3. Seed Users (Passwords are 'Pass123' for simplicity in demo)
INSERT INTO Users (Id, TenantId, Username, Email, PasswordHash, RoleId, CreatedBy) VALUES 
(@RajeevUser, 'rajeev-pvt', 'rajeev_admin', 'admin@rajeev.com', 'hashed_pass_123', @RajeevRole, 'System'),
(@GreenPantryUser, 'greenpantry', 'johndoe_gp', 'john@gp.com', 'hashed_pass_123', @GreenPantryRole, 'System'),
(@OmegaUser, 'omega', 'omega_dev', 'dev@omega.tech', 'hashed_pass_123', @OmegaRole, 'System'),
(@BangaruUser, 'bangaru', 'bangaru_staff', 'sales@bangaru.com', 'hashed_pass_123', @BangaruRole, 'System');

-- 4. Seed More Products for variety
INSERT INTO Products (Id, TenantId, Name, Description, Price, Category, CreatedBy) VALUES 
(NEWID(), 'greenpantry', 'Organic Kale', 'Fresh hydroponic kale', 3.50, 'Vegetables', 'System'),
(NEWID(), 'greenpantry', 'Almond Milk', 'Unsweetened 1L', 4.99, 'Dairy', 'System'),
(NEWID(), 'bangaru', 'Silver Anklet', 'Pure 925 Silver', 45.00, 'Silverware', 'System'),
(NEWID(), 'bangaru', 'Diamond Ring', '1 Carat Solitaire', 2500.00, 'Jewelry', 'System'),
(NEWID(), 'omega', 'API Maintenance Plan', 'Annual security updates', 499.00, 'Services', 'System'),
(NEWID(), 'rajeev-pvt', 'Corporate Laptop', 'Lending unit for staff', 1200.00, 'Hardware', 'System');

-- 5. Seed Orders & OrderItems
DECLARE @OrderId1 UNIQUEIDENTIFIER = NEWID();
DECLARE @OrderId2 UNIQUEIDENTIFIER = NEWID();

INSERT INTO Orders (Id, TenantId, UserId, TotalAmount, Status, CreatedBy) VALUES 
(@OrderId1, 'greenpantry', @GreenPantryUser, 8.49, 'Delivered', 'System'),
(@OrderId2, 'bangaru', @BangaruUser, 2500.00, 'Processing', 'System');

-- 6. Seed Notifications
INSERT INTO Notifications (Id, TenantId, UserId, Message, IsRead, CreatedBy) VALUES 
(NEWID(), 'greenpantry', @GreenPantryUser, 'Your apples have been delivered!', 1, 'System'),
(NEWID(), 'bangaru', @BangaruUser, 'New high-value order received (Diamond Ring)', 0, 'System'),
(NEWID(), 'rajeev-pvt', @RajeevUser, 'System wide security audit completed.', 0, 'System');

GO
