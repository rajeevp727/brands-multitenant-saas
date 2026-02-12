-- Connect specific sort order for brands
-- Run this in Supabase SQL Editor
-- Using quoted identifiers because table likely created as "Brands"

-- 1. Green Pantry
-- 2. Omega Tech
-- 3. Bangaru Kottu
-- 4. vajraValli
-- 5. vanaVajram
-- 6. More Brands

UPDATE "Brands" SET "SortOrder" = 1, "IsActive" = TRUE WHERE "TenantId" = 'greenpantry';
UPDATE "Brands" SET "SortOrder" = 2, "IsActive" = TRUE WHERE "TenantId" = 'omega';
UPDATE "Brands" SET "SortOrder" = 3, "IsActive" = TRUE WHERE "TenantId" = 'bangaru';
UPDATE "Brands" SET "SortOrder" = 4, "IsActive" = TRUE WHERE "TenantId" = 'vajravalli';
UPDATE "Brands" SET "SortOrder" = 5, "IsActive" = TRUE WHERE "TenantId" = 'vanavajram';

-- Hide More Brands (Rajeevstech)
UPDATE "Brands" SET "SortOrder" = 6, "IsActive" = FALSE WHERE "TenantId" = 'morebrands';

-- Ensure others are pushed to the end and default active
UPDATE "Brands" SET "SortOrder" = 999, "IsActive" = TRUE WHERE "TenantId" NOT IN ('greenpantry', 'omega', 'bangaru', 'vajravalli', 'vanavajram', 'morebrands');
