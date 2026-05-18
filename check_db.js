const { Client } = require('pg');

const client = new Client({
  connectionString: 'postgres://postgres.bfwdcgxyflsofoltdvtn:Raj@727_eev.MultiSaas@aws-1-ap-northeast-1.pooler.supabase.com:5432/postgres',
  ssl: { rejectUnauthorized: false }
});

async function run() {
  await client.connect();
  const res = await client.query('SELECT "Id", "Name" FROM greenpantry."Restaurants"');
  console.log('Restaurants found:', res.rows.length);
  
  for (const r of res.rows) {
    const rId = r.Id;
    console.log(`Inserting menu for ${r.Name} (${rId})`);
    
    // Check if items exist
    const itemsRes = await client.query('SELECT count(*) FROM greenpantry."MenuItems" WHERE "RestaurantId" = $1 AND "IsDeleted" = false', [rId]);
    if (parseInt(itemsRes.rows[0].count) > 0) {
      console.log('Items already exist for', r.Name);
      continue;
    }
    
    await client.query(`
      INSERT INTO greenpantry."MenuItems" (
        "Id", "RestaurantId", "Name", "Description", "Price", "ImageUrl", "Category", 
        "IsVegetarian", "IsVegan", "IsGlutenFree", "IsSpicy", "SpiceLevel", 
        "PreparationTime", "IsAvailable", "StockQuantity", "CreatedAt", "UpdatedAt", "CreatedBy", "IsDeleted",
        "Allergens", "Ingredients", "Variants", "Tags"
      ) VALUES 
      (gen_random_uuid(), $1, 'Special Biryani', 'Delicious traditional biryani', 350.00, 'https://images.unsplash.com/photo-1563379091339-03246963d4d4?w=400', 'Main Course', false, false, false, true, 2, 30, true, 100, NOW(), NOW(), 'system', false, ARRAY[]::text[], ARRAY[]::text[], '[]'::jsonb, ARRAY[]::text[]),
      (gen_random_uuid(), $1, 'Ghee Roast Dosa', 'Crispy dosa roasted in pure ghee', 150.00, 'https://images.unsplash.com/photo-1551218808-94e220e084d2?w=400', 'Breakfast', true, false, true, false, 0, 15, true, 100, NOW(), NOW(), 'system', false, ARRAY[]::text[], ARRAY[]::text[], '[]'::jsonb, ARRAY[]::text[]),
      (gen_random_uuid(), $1, 'Paneer Tikka', 'Spicy grilled paneer with vegetables', 250.00, 'https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=400', 'Starters', true, false, true, true, 2, 20, true, 100, NOW(), NOW(), 'system', false, ARRAY[]::text[], ARRAY[]::text[], '[]'::jsonb, ARRAY[]::text[]),
      (gen_random_uuid(), $1, 'Filter Coffee', 'Authentic South Indian filter coffee', 60.00, 'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=400', 'Beverages', true, false, true, false, 0, 5, true, -1, NOW(), NOW(), 'system', false, ARRAY[]::text[], ARRAY[]::text[], '[]'::jsonb, ARRAY[]::text[])
    `, [rId]);
  }
  console.log('Done inserting menu items.');
  await client.end();
}

run().catch(console.error);
