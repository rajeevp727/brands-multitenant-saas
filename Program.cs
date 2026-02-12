using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

Console.WriteLine("🚀 Starting Multi-Tenant SaaS APIs...");

var apis = new[]
{
    new { Name = "SaaS-API", Path = "src/backend/SaaS.Api" },
    new { Name = "GP-API", Path = "modules/GreenPantry/backend/GreenPantry.API" },
    new { Name = "BK-API", Path = "modules/BangaruKottu/backend/Vendor.API" }
};

var processes = new List<Process>();

foreach (var api in apis)
{
    var isMain = api.Name == "SaaS-API";
    Console.WriteLine($"[Orchestrator] Launching {api.Name} {(isMain ? "(Focus)" : "(Background)")}...");
    
    var psi = new ProcessStartInfo
    {
        FileName = "dotnet",
        Arguments = "run",
        WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), api.Path),
        UseShellExecute = isMain, 
        CreateNoWindow = !isMain
    };

    var process = Process.Start(psi);
    if (process != null) processes.Add(process);
}

Console.WriteLine("✅ APIs are launched. Waiting for SaaS API to be healthy...");

// Local Development Ports
const string SAAS_API_PORT = "5114";
const string SAAS_HEALTH_URL = $"http://localhost:{SAAS_API_PORT}/health";
const string SAAS_SWAGGER_URL = $"http://localhost:{SAAS_API_PORT}/swagger";

if (await WaitForUrl(SAAS_HEALTH_URL, 120))
{
    Console.WriteLine("🚀 SaaS API is ready. Opening Swagger...");
    Process.Start(new ProcessStartInfo(SAAS_SWAGGER_URL) { UseShellExecute = true });
}
else
{
    Console.WriteLine("❌ ERROR: SaaS API failed to become healthy within 120 seconds.");
    Console.WriteLine("Please check the terminal window labeled 'SaaS-API' for errors.");
}

Console.WriteLine("Press Ctrl+C to exit this orchestrator (note: windows will stay open).");

// Keep the orchestrator alive
Thread.Sleep(Timeout.Infinite);

async Task<bool> WaitForUrl(string url, int timeoutSeconds)
{
    using var client = new HttpClient();
    for (int i = 0; i < timeoutSeconds; i++)
    {
        try
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode) return true;
            Console.WriteLine($"[Orchestrator] SaaS API check: {response.StatusCode} (Waiting...)");
        }
        catch (Exception)
        {
            // Just swallow connection refused etc
        }
        await Task.Delay(2000);
    }
    return false;
}
