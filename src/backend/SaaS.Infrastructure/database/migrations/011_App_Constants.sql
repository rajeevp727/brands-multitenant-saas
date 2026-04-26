USE MultiTenantSaaS_DB;
GO

-- 1. Create AppConstants table
CREATE TABLE AppConstants (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId NVARCHAR(50) NULL, -- NULL means global default
    ConstantKey NVARCHAR(100) NOT NULL,
    ConstantValue NVARCHAR(MAX) NOT NULL,
    Category NVARCHAR(50) DEFAULT 'General',
    UNIQUE(TenantId, ConstantKey)
);
GO

-- 2. Seed Global Constants
INSERT INTO AppConstants (TenantId, ConstantKey, ConstantValue, Category) VALUES
-- Footer
(NULL, 'FOOTER_RESOURCES', 'Resources', 'Footer'),
(NULL, 'FOOTER_PRIVACY', 'Privacy Policy', 'Footer'),
(NULL, 'FOOTER_TERMS', 'Terms of Service', 'Footer'),
(NULL, 'FOOTER_SUPPORT', 'Help & Support', 'Footer'),
(NULL, 'FOOTER_CONTACT_US', 'Contact Us', 'Footer'),
(NULL, 'FOOTER_EMAIL_LABEL', 'Email:', 'Footer'),
(NULL, 'FOOTER_PHONE_LABEL', 'Phone:', 'Footer'),
(NULL, 'FOOTER_RIGHTS_RESERVED', 'All rights reserved.', 'Footer'),
(NULL, 'FOOTER_BUILT_BY', 'Built by', 'Footer'),

-- Header
(NULL, 'NAV_HOME', 'Home', 'Navigation'),
(NULL, 'NAV_PRODUCTS', 'Products', 'Navigation'),
(NULL, 'NAV_LOGOUT', 'Logout', 'Navigation'),
(NULL, 'NAV_SIGNUP', 'Sign Up', 'Navigation'),
(NULL, 'NAV_LOGIN', 'Login', 'Navigation'),

-- Dashboard
(NULL, 'DASHBOARD_ECOSYSTEM', 'The Brand Ecosystem', 'Dashboard'),
(NULL, 'DASHBOARD_LAUNCH', 'Launch Module', 'Dashboard'),
(NULL, 'DASHBOARD_PRODUCTS', 'Available Products', 'Dashboard'),
(NULL, 'DASHBOARD_NO_PRODUCTS', 'No products found for this tenant. Visit our store soon!', 'Dashboard'),

-- Auth
(NULL, 'AUTH_SECURE_LOGIN', 'Secure Login for', 'Auth'),
(NULL, 'AUTH_NO_ACCOUNT', 'Don''t have an account?', 'Auth'),
(NULL, 'AUTH_SIGNUP_HEADER', 'Join', 'Auth'),
(NULL, 'AUTH_SIGNUP_SUB', 'Create your account to get started', 'Auth'),
(NULL, 'AUTH_PW_MISMATCH', 'Passwords do not match', 'Auth');
GO
