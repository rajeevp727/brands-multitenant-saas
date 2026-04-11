using System;
using System.Collections.Generic;
using Npgsql;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("d:/Rajeev/Projects/MultiTenantSaaS/src/backend/SaaS.Api/appsettings.json")
    .Build();

var connString = config.GetConnectionString("DefaultConnection");
using var conn = new NpgsqlConnection(connString);
conn.Open();

Console.WriteLine("--- All Tables in public ---");
using (var cmd = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' ORDER BY table_name", conn))
using (var reader = cmd.ExecuteReader())
{
    while (reader.Read()) Console.WriteLine(reader.GetString(0));
}

string[] targetTables = { "Users", "users" };
foreach (var tableName in targetTables)
{
    Console.WriteLine($"\n--- Columns for table: {tableName} ---");
    using (var cmd = new NpgsqlCommand("SELECT column_name FROM information_schema.columns WHERE table_name = @tableName", conn))
    {
        cmd.Parameters.AddWithValue("tableName", tableName);
        using (var reader = cmd.ExecuteReader())
        {
            int count = 0;
            while (reader.Read())
            {
                Console.Write(reader.GetString(0) + ", ");
                count++;
            }
            if (count == 0) Console.WriteLine("NOT FOUND");
            else Console.WriteLine($"\nTotal: {count}");
        }
    }
}
