USE MultiTenantSaaS_DB;
GO

-- Ensure Rajeev's contact details are correctly set across brands or as a global fallback
UPDATE Brands SET 
    Email = 'mrrajeev18@gmail.com',
    Phone = '+917032075893'
WHERE TenantId = 'rajeev-pvt';

-- Optional: Update others if they were intended to be same for demo
UPDATE Brands SET 
    Email = 'support@greenpantry.com'
WHERE TenantId = 'greenpantry' AND Email IS NULL;

UPDATE Brands SET 
    Email = 'info@omega.tech'
WHERE TenantId = 'omega' AND Email IS NULL;

UPDATE Brands SET 
    Email = 'sales@bangaru.com'
WHERE TenantId = 'bangaru' AND Email IS NULL;
GO
