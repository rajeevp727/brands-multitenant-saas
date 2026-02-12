using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Api.Services;

public class KeepAliveService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<KeepAliveService> _logger;
    private readonly IConfiguration _configuration;
    private const int PingIntervalMinutes = 10; // Render sleeps after 15 mins

    public KeepAliveService(IHttpClientFactory httpClientFactory, ILogger<KeepAliveService> logger, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("KeepAliveService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var url = _configuration["InternalPingUrl"] ?? "https://multitenantsaas-owbt.onrender.com/health";
                
                // Don't ping if it's localhost/dev usually, but user asked for it in general.
                // We'll proceed.
                
                using var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                
                _logger.LogInformation($"Sending Keep-Alive ping to {url}");
                
                var response = await client.GetAsync(url, stoppingToken);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Keep-Alive ping successful. Status: {StatusCode}", response.StatusCode);
                }
                else
                {
                    _logger.LogWarning("Keep-Alive ping failed. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing Keep-Alive ping.");
            }

            await Task.Delay(TimeSpan.FromMinutes(PingIntervalMinutes), stoppingToken);
        }

        _logger.LogInformation("KeepAliveService is stopping.");
    }
}
