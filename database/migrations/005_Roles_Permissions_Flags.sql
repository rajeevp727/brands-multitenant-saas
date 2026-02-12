USE MultiTenantSaaS_DB;
GO

-- 1. Permissions & RolePermissions
CREATE TABLE Permissions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(255) NOT NULL, -- e.g., 'Products.View', 'Orders.Create'
    Module NVARCHAR(255) NOT NULL -- e.g., 'Commerce', 'Identity'
);

CREATE TABLE RolePermissions (
    RoleId UNIQUEIDENTIFIER NOT NULL,
    PermissionId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id),
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id)
);

-- 2. Feature Flags
CREATE TABLE FeatureFlags (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId NVARCHAR(255) NOT NULL,
    FeatureKey NVARCHAR(255) NOT NULL, -- e.g., 'EnableGlobalDelivery'
    IsEnabled BIT DEFAULT 0,
    INDEX IX_FeatureFlags_TenantId (TenantId)
);

-- 3. Seed Permissions
DECLARE @P1 UNIQUEIDENTIFIER = NEWID(); -- ViewProducts
DECLARE @P2 UNIQUEIDENTIFIER = NEWID(); -- CreateOrders
DECLARE @P3 UNIQUEIDENTIFIER = NEWID(); -- ManageTenants

INSERT INTO Permissions (Id, Name, Module) VALUES 
(@P1, 'Products.View', 'Commerce'),
(@P2, 'Orders.Create', 'Commerce'),
(@P3, 'Tenants.Manage', 'System');

-- 4. Map Permissions to Roles (Sample)
-- Admin gets all
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT Id, @P1 FROM Roles WHERE Name = 'Admin' UNION
SELECT Id, @P2 FROM Roles WHERE Name = 'Admin' UNION
SELECT Id, @P3 FROM Roles WHERE Name = 'Admin';

-- Customer gets View and Create
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT Id, @P1 FROM Roles WHERE Name = 'Customer' UNION
SELECT Id, @P2 FROM Roles WHERE Name = 'Customer';

-- 5. Seed Feature Flags per Brand
INSERT INTO FeatureFlags (TenantId, FeatureKey, IsEnabled) VALUES 
('greenpantry', 'EnableDeliveryTracking', 1),
('greenpantry', 'EnableWallet', 1),
('omega', 'EnableAnalytics', 1),
('bangaru', 'EnableComplianceLogs', 1),
('rajeev-pvt', 'EnableSuperAdminDashboard', 1);

GO
