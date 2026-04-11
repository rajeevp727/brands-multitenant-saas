using System;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("d:/Rajeev/Projects/MultiTenantSaaS/src/backend/SaaS.Api/appsettings.json")
    .Build();

var connString = config.GetConnectionString("DefaultConnection");
using var conn = new NpgsqlConnection(connString);
conn.Open();

Console.WriteLine("--- Tables ---");
using (var cmd = new NpgsqlCommand("SELECT table_schema, table_name FROM information_schema.tables WHERE table_schema = 'public'", conn))
using (var reader = cmd.ExecuteReader())
{
    while (reader.Read())
    {
        Console.WriteLine($"{reader.GetString(0)}.{reader.GetString(1)}");
    }
}

Console.WriteLine("\n--- Users Columns (Case Sensitive) ---");
using (var cmd = new NpgsqlCommand("SELECT column_name, data_type FROM information_schema.columns WHERE table_name = 'Users'", conn))
using (var reader = cmd.ExecuteReader())
{
    while (reader.Read())
    {
        Console.WriteLine($"{reader.GetString(0)} ({reader.GetString(1)})");
    }
}

Console.WriteLine("\n--- users Columns (Lowercase) ---");
using (var cmd = new NpgsqlCommand("SELECT column_name, data_type FROM information_schema.columns WHERE table_name = 'users'", conn))
using (var reader = cmd.ExecuteReader())
{
    while (reader.Read())
    {
        Console.WriteLine($"{reader.GetString(0)} ({reader.GetString(1)})");
    }
}
