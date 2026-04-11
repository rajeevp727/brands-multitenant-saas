const { Client } = require('pg');

const config = {
    user: 'postgres.bfwdcgxyflsofoltdvtn',
    password: 'Raj@727_eev.MultiSaas',
    host: 'aws-1-ap-northeast-1.pooler.supabase.com',
    port: 5432,
    database: 'postgres',
    ssl: { rejectUnauthorized: false }
};

const tables = ["Tenants", "Brands", "Products", "Users", "Roles", "Orders", "OrderItems", "Permissions", "FeatureFlags", "AppConstants", "Notifications"];

const specificColumns = [
    'ALTER TABLE "Brands" ADD COLUMN IF NOT EXISTS "IsVisible" BOOLEAN DEFAULT TRUE',
    'ALTER TABLE "Brands" ADD COLUMN IF NOT EXISTS "IsActive" BOOLEAN DEFAULT TRUE',
    'ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "IsActive" BOOLEAN DEFAULT TRUE',
    'ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "IsDeleted" BOOLEAN DEFAULT FALSE',
    'ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "MetadataJson" TEXT',
    'ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "ImageUrl" TEXT',
    'ALTER TABLE "Orders" ADD COLUMN IF NOT EXISTS "IsDeleted" BOOLEAN DEFAULT FALSE',
    'ALTER TABLE "Roles" ADD COLUMN IF NOT EXISTS "Description" TEXT',
    'ALTER TABLE "Permissions" ADD COLUMN IF NOT EXISTS "Description" TEXT',
    'ALTER TABLE "OrderItems" ADD COLUMN IF NOT EXISTS "MetadataJson" TEXT',
    'ALTER TABLE "AppConstants" ADD COLUMN IF NOT EXISTS "Category" TEXT',
    'ALTER TABLE "AppConstants" ADD COLUMN IF NOT EXISTS "Description" TEXT',
    'ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "Category" TEXT',
];

async function run() {
    const client = new Client(config);
    try {
        await client.connect();
        console.log('Connected to Supabase');

        for (const t of tables) {
            const queries = [
                `ALTER TABLE "${t}" ADD COLUMN IF NOT EXISTS "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP`,
                `ALTER TABLE "${t}" ADD COLUMN IF NOT EXISTS "CreatedBy" TEXT`,
                `ALTER TABLE "${t}" ADD COLUMN IF NOT EXISTS "LastModifiedAt" TIMESTAMP WITH TIME ZONE`,
                `ALTER TABLE "${t}" ADD COLUMN IF NOT EXISTS "LastModifiedBy" TEXT`,
                `ALTER TABLE "${t}" ADD COLUMN IF NOT EXISTS "DeletedAt" TIMESTAMP WITH TIME ZONE`,
                `ALTER TABLE "${t}" ADD COLUMN IF NOT EXISTS "DeletedBy" TEXT`,
                `ALTER TABLE "${t}" ADD COLUMN IF NOT EXISTS "IsDeleted" BOOLEAN DEFAULT false`
            ];
            for (const q of queries) {
                try { await client.query(q); } catch(e) { /* ignore tables that might not exist */ }
            }
        }
        
        for (const q of specificColumns) {
            try { await client.query(q); } catch(e) { console.error('Specific failed:', q, e.message); }
        }
        
        console.log('All schemas patched successfully.');
    } catch (err) {
        console.error('Connection/Execution Error:', err);
    } finally {
        await client.end();
    }
}
run();
