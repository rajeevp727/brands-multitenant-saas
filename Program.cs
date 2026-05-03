using System.Diagnostics;
using System.Net.Http;

Console.WriteLine("Starting Multi-Tenant SaaS orchestrator...");

var root = Directory.GetCurrentDirectory();
var startedProcesses = new List<Process>();
var isShuttingDown = false;

var apiServices = new[]
{
    new Service("SaaS-API", "src/backend/SaaS.Api", "dotnet run --urls http://0.0.0.0:5114"),
    new Service("GreenPantry-API", "src/backend/modules/GreenPantry/backend/GreenPantry.API", "dotnet run --urls http://0.0.0.0:7001"),
    new Service("BangaruKottu-API", "src/backend/modules/BangaruKottu/backend/Vendor.API", "dotnet run --urls http://0.0.0.0:7002")
};

var uiServices = new[]
{
    new Service("SaaS-UI", "src/frontend", "npm run dev -- --port 5173"),
    new Service("GreenPantry-UI", "src/backend/modules/GreenPantry/frontend", "npm run dev -- --port 5174"),
    new Service("OmegaTech-UI", "src/backend/modules/OmegaTech", "npm run start"),
    new Service("BangaruKottu-UI", "src/backend/modules/BangaruKottu/frontend", "npm run dev -- --port 5176")
};

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    ShutdownChildren();
    Environment.Exit(0);
};

AppDomain.CurrentDomain.ProcessExit += (_, _) => ShutdownChildren();

foreach (var api in apiServices)
{
    StartService(api);
}

Console.WriteLine("APIs launched. Waiting for SaaS API health...");

const string saasHealthUrl = "http://localhost:5114/health";
const string saasSwaggerUrl = "http://localhost:5114/swagger";

if (!await WaitForUrl(saasHealthUrl, timeoutSeconds: 120))
{
    Console.WriteLine("SaaS API did not become healthy in time. UIs will still be started.");
}
else
{
    Console.WriteLine("SaaS API is healthy.");
    Process.Start(new ProcessStartInfo(saasSwaggerUrl) { UseShellExecute = true });
}

foreach (var ui in uiServices)
{
    StartService(ui);
}

Console.WriteLine("All brand APIs and UIs have been started.");
Console.WriteLine("Press Ctrl+C to stop all child processes.");
await Task.Delay(Timeout.Infinite);

void StartService(Service service)
{
    var workingDirectory = Path.Combine(root, service.RelativePath);
    if (!Directory.Exists(workingDirectory))
    {
        Console.WriteLine($"[Skip] {service.Name}: missing directory {workingDirectory}");
        return;
    }

    Console.WriteLine($"[Start] {service.Name}: {service.Command}");

    var processInfo = BuildShellStartInfo(service.Command, workingDirectory);
    var process = Process.Start(processInfo);
    if (process is null)
    {
        Console.WriteLine($"[Error] Failed to start {service.Name}");
        return;
    }

    startedProcesses.Add(process);
}

ProcessStartInfo BuildShellStartInfo(string command, string workingDirectory)
{
    if (OperatingSystem.IsWindows())
    {
        return new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {command}",
            WorkingDirectory = workingDirectory,
            UseShellExecute = true
        };
    }

    return new ProcessStartInfo
    {
        FileName = "/bin/bash",
        Arguments = $"-lc \"{command}\"",
        WorkingDirectory = workingDirectory,
        UseShellExecute = true
    };
}

async Task<bool> WaitForUrl(string url, int timeoutSeconds)
{
    using var client = new HttpClient();
    var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);

    while (DateTime.UtcNow < deadline)
    {
        try
        {
            using var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode) return true;
        }
        catch
        {
            // Retry while service starts.
        }

        await Task.Delay(2000);
    }

    return false;
}

void ShutdownChildren()
{
    if (isShuttingDown) return;
    isShuttingDown = true;

    Console.WriteLine("Stopping child processes...");
    foreach (var process in startedProcesses.Where(p => !p.HasExited))
    {
        try
        {
            process.Kill(entireProcessTree: true);
        }
        catch
        {
            // Ignore failures during shutdown.
        }
    }
}

record Service(string Name, string RelativePath, string Command);
