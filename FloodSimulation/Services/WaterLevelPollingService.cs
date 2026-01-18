using FloodSimulation.Models.Entities;
using FloodSimulation.Repositories;

namespace FloodSimulation.Services;

public class WaterLevelPollingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WaterLevelPollingService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromMinutes(5);
    
    public WaterLevelPollingService(
        IServiceScopeFactory scopeFactory,
        ILogger<WaterLevelPollingService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Water Level Polling Service started");
        
        // Warte kurz beim Start (damit Server Zeit hat hochzufahren)
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PollWaterLevelsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during water level polling");
            }
            
            // Warte 5 Minuten bis zum nächsten Poll
            _logger.LogDebug("Next poll in {Minutes} minutes", _pollingInterval.TotalMinutes);
            await Task.Delay(_pollingInterval, stoppingToken);
        }
        
        _logger.LogInformation("Water Level Polling Service stopped");
    }
    
    private async Task PollWaterLevelsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var pegelService = scope.ServiceProvider.GetRequiredService<PegelOnlineService>();
        var repository = scope.ServiceProvider.GetRequiredService<IWaterLevelRepository>();
        
        _logger.LogInformation("Polling water levels from pegelonline.de");
        
        // Hole alle Stationen
        var stations = await repository.GetAllStationsAsync();
        
        foreach (var station in stations)
        {
            try
            {
                // Hole aktuellen Wasserstand
                var waterLevel = await pegelService.GetCurrentWaterLevelAsync(station.Id);
                
                if (waterLevel.HasValue)
                {
                    // Speichere Messwert
                    var measurement = new WaterLevelMeasurement
                    {
                        StationId = station.Id,
                        Timestamp = DateTime.UtcNow, // Explizit UTC!
                        WaterLevelCm = waterLevel.Value,
                        RecordedAt = DateTime.UtcNow // Auch hier UTC!
                    };
    
                    await repository.SaveMeasurementsAsync(new List<WaterLevelMeasurement> { measurement });
                    await repository.UpdateStationLastUpdatedAsync(station.Id, DateTime.UtcNow);
    
                    _logger.LogInformation("Saved water level for {StationName}: {WaterLevel} cm", 
                        station.Name, waterLevel.Value);
                }
                else
                {
                    _logger.LogWarning("No water level data received for {StationName}", station.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error polling station {StationName}", station.Name);
            }
        }
    }
}