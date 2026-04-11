using System;
using Microsoft.Extensions.Configuration;
using Npgsql;

var config = new ConfigurationBuilder()
    .AddJsonFile("D:/Rajeev/Projects/MultiTenantSaaS/src/backend/SaaS.Api/appsettings.json")
    .Build();

var cs = config.GetConnectionString("DefaultConnection");
using var conn = new NpgsqlConnection(cs);
await conn.OpenAsync();

await using var cmd = new NpgsqlCommand(@"
SELECT \"Id\", \"Email\", \"Username\", \"TenantId\", \"PasswordHash\", \"IsActive\"
FROM \"Users\"
WHERE \"Email\" = 'admin@rajeev.com'
ORDER BY \"CreatedAt\" NULLS LAST;", conn);

await using var reader = await cmd.ExecuteReaderAsync();
var found = false;
while (await reader.ReadAsync())
{
    found = true;
    Console.WriteLine($"Id={reader[0]}");
    Console.WriteLine($"Email={reader[1]}");
    Console.WriteLine($"Username={reader[2]}");
    Console.WriteLine($"TenantId={reader[3]}");
    Console.WriteLine($"PasswordHash={reader[4]}");
    Console.WriteLine($"IsActive={reader[5]}");
    Console.WriteLine("---");
}
if (!found) Console.WriteLine("NO_ADMIN_USER");
