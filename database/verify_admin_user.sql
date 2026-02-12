-- Quick verification query for production database
-- Run this first to check if admin user exists

SELECT 
    u."Id",
    u."Username",
    u."Email",
    u."PasswordHash",
    u."TenantId",
    u."IsActive",
    r."Name" as "RoleName",
    u."CreatedAt", 
    u."CreatedBy"
FROM "Users" u
LEFT JOIN "Roles" r ON u."RoleId" = r."Id"
WHERE u."Email" = 'admin@rajeev.com';

-- If this returns no rows, the admin user doesn't exist
-- If it returns a row, check:
--   - TenantId should be 'rajeev-pvt'
--   - IsActive should be true
--   - PasswordHash should be 'Pass123'
--   - RoleName should be 'Admin'
