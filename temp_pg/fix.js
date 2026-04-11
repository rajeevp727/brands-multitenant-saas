const { Client } = require('pg');

const config = {
    user: 'postgres.bfwdcgxyflsofoltdvtn',
    password: 'Raj@727_eev.MultiSaas',
    host: 'aws-1-ap-northeast-1.pooler.supabase.com',
    port: 5432,
    database: 'postgres',
    ssl: { rejectUnauthorized: false }
};

async function run() {
    const client = new Client(config);
    try {
        await client.connect();
        console.log('Connected to Supabase');

        const res = await client.query("SELECT column_name FROM information_schema.columns WHERE table_name = 'Users'");
        console.log('Current columns in Users table:', res.rows.map(r => r.column_name).join(', '));

        const queries = [
            'ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP',
            'ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "CreatedBy" TEXT',
            'ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "LastModifiedAt" TIMESTAMP WITH TIME ZONE',
            'ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "LastModifiedBy" TEXT',
            'ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "DeletedAt" TIMESTAMP WITH TIME ZONE',
            'ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "DeletedBy" TEXT',
            'ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "IsDeleted" BOOLEAN DEFAULT false',
            'ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "RefreshToken" TEXT',
            'ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "RefreshTokenExpiryTime" TIMESTAMP WITH TIME ZONE'
        ];

        for (const q of queries) {
            try { 
                await client.query(q); 
                console.log('Successfully ran:', q);
            } catch(e) { 
                console.error('Failed on query:', q, e.message); 
            }
        }

        console.log('Database schema patched successfully. Run node node script.');
    } catch (err) {
        console.error('Connection/Execution Error:', err);
    } finally {
        await client.end();
    }
}

run();
