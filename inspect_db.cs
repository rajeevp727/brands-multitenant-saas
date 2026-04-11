using SaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) => {
        config.AddJsonFile("src/backend/SaaS.Api/appsettings.json", optional: false);
    })
    .ConfigureServices((hostContext, services) => {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

Console.WriteLine("--- Tables ---");
var tables = await db.Database.GetDbConnection().GetSchemaAsync("Tables");
foreach (DataRow row in tables.Rows) {
    Console.WriteLine($"{row["TABLE_SCHEMA"]}.{row["TABLE_NAME"]}");
}

Console.WriteLine("\n--- Users Columns ---");
var columns = await db.Database.GetDbConnection().GetSchemaAsync("Columns", new[] { null, null, "Users" });
if (columns.Rows.Count == 0) {
    columns = await db.Database.GetDbConnection().GetSchemaAsync("Columns", new[] { null, null, "users" });
}
foreach (DataRow row in columns.Rows) {
    Console.WriteLine($"{row["COLUMN_NAME"]} ({row["DATA_TYPE"]})");
}
