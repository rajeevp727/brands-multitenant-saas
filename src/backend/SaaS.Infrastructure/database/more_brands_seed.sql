-- 1. Insert into Tenants
INSERT INTO "Tenants" ("Id", "Name", "Hostname", "IsActive", "Identifier", "CreatedAt")
VALUES 
    ('vajravalli', 'vajraValli', 'vajravalli.com', true, 'vajravalli', NOW()),
    ('morebrands', 'More Brands', 'rajeevstech.in', true, 'morebrands', NOW())
ON CONFLICT ("Id") DO NOTHING;

-- 2. Insert into Brands
INSERT INTO "Brands" ("Id", "TenantId", "Name", "LogoUrl", "PrimaryColor", "SecondaryColor", "Slogan", "Description", "Email", "Phone", "BuiltBy", "ConfigJson")
VALUES 
(
    gen_random_uuid(), 
    'vajravalli', 
    'vajraValli', 
    '/images/vajraValli/vajraValli_logo.png', 
    '#4c1d95', 
    '#ffffff', 
    'Majestic Diamonds', 
    'Curated collection of high-clarity diamonds for global luxury brands.', 
    'info@vajravalli.com', 
    '+91 88888 99999', 
    'Vajra Tech Labs', 
    '{"domain": "vajravalli.com", "vercel": "vajra-valli", "url": "https://vajravalli.com"}'
),
(
    gen_random_uuid(), 
    'morebrands', 
    'More Brands', 
    '/images/more_brands.png', 
    '#1e293b', 
    '#64748b', 
    'Scale with Us', 
    'We''re building more amazing platforms for your business. Coming soon!', 
    'portal@rajeevstech.in', 
    '+91 91234 56789', 
    'Rajeev''s Engineering', 
    '{"status": "coming_soon"}'
)
ON CONFLICT ("TenantId") DO UPDATE SET
    "LogoUrl" = EXCLUDED."LogoUrl",
    "ConfigJson" = EXCLUDED."ConfigJson",
    "Description" = EXCLUDED."Description",
    "PrimaryColor" = EXCLUDED."PrimaryColor",
    "SecondaryColor" = EXCLUDED."SecondaryColor",
    "Slogan" = EXCLUDED."Slogan",
    "Email" = EXCLUDED."Email",
    "Phone" = EXCLUDED."Phone",
    "BuiltBy" = EXCLUDED."BuiltBy";
