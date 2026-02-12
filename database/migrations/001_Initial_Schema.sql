IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MultiTenantSaaS_DB')
BEGIN
    CREATE DATABASE MultiTenantSaaS_DB;
END
GO

USE MultiTenantSaaS_DB;
GO

-- 1. Initial Migration: Core Tables
CREATE TABLE Tenants (
    Id NVARCHAR(255) PRIMARY KEY, -- Hostname or slug
    Name NVARCHAR(255) NOT NULL,
    Hostname NVARCHAR(255) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);

CREATE TABLE Brands (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId NVARCHAR(255) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    LogoUrl NVARCHAR(500),
    PrimaryColor NVARCHAR(50) DEFAULT '#000000',
    SecondaryColor NVARCHAR(50) DEFAULT '#ffffff',
    ConfigJson NVARCHAR(MAX) DEFAULT '{}',
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

CREATE INDEX IX_Brands_TenantId ON Brands(TenantId);

-- 2. Business Tables (with TenantId isolation)
CREATE TABLE Products (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId NVARCHAR(255) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18, 2) NOT NULL,
    Category NVARCHAR(255),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(255) DEFAULT 'System',
    LastModifiedAt DATETIME2,
    LastModifiedBy NVARCHAR(255),
    IsDeleted BIT DEFAULT 0,
    INDEX IX_Products_TenantId (TenantId)
);

-- 3. Seed Data
INSERT INTO Tenants (Id, Name, Hostname) VALUES 
('rajeev-pvt', 'Rajeev’s Pvt. Ltd', 'localhost'),
('greenpantry', 'GreenPantry', 'greenpantry.local'),
('omega', 'Omega Technologies', 'omega.local'),
('bangaru', 'BangaruKottu', 'bangaru.local');

INSERT INTO Brands (TenantId, Name, LogoUrl, PrimaryColor, SecondaryColor, ConfigJson) VALUES 
('rajeev-pvt', 'Rajeev Corporate', '/brands/rajeev.png', '#2c3e50', '#e74c3c', '{"features": ["admin", "audit"]}'),
('greenpantry', 'GreenPantry Marketplace', '/brands/greenpantry.png', '#2ecc71', '#27ae60', '{"features": ["commerce"]}'),
('omega', 'Omega Tech Services', '/brands/omega.png', '#3498db', '#2980b9', '{"features": ["subscriptions"]}'),
('bangaru', 'BangaruKottu Jewelry', '/brands/bangaru.png', '#f1c40f', '#f39c12', '{"features": ["jewelry"]}');

-- 4. Sample Queries for Isolation
-- SELECT * FROM Products WHERE TenantId = 'greenpantry';
