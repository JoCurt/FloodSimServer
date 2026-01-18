using FloodSimulation.Models.Entities;
using FloodSimulation.Repositories;

namespace FloodSimulation.Services;

public class WaterLevelStartupService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WaterLevelStartupService> _logger;
    private const int HistoricalDataDays = 10;
    
    public WaterLevelStartupService(
        IServiceScopeFactory scopeFactory,
        ILogger<WaterLevelStartupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Water Level Startup Service: Loading historical data");
        
        using var scope = _scopeFactory.CreateScope();
        var pegelService = scope.ServiceProvider.GetRequiredService<PegelOnlineService>();
        var repository = scope.ServiceProvider.GetRequiredService<IWaterLevelRepository>();
        
        try
        {
            // 1. Lösche alte Daten (älter als 10 Tage)
            var cutoffDate = DateTime.UtcNow.AddDays(-HistoricalDataDays);
            _logger.LogInformation("Deleting measurements older than {CutoffDate}", cutoffDate);
            await repository.DeleteMeasurementsOlderThanAsync(cutoffDate);
            
            // 2. Lade historische Daten für alle Stationen
            var stations = await repository.GetAllStationsAsync();
            
            foreach (var station in stations)
            {
                _logger.LogInformation("Loading {Days} days of historical data for {StationName}", 
                    HistoricalDataDays, station.Name);
                
                var historicalData = await pegelService.GetHistoricalDataAsync(station.Id, HistoricalDataDays);
                
                if (historicalData.Any())
                {
                    // Konvertiere zu Measurements
                    var measurements = historicalData
                        .Select(m => new WaterLevelMeasurement
                        {
                            StationId = station.Id,
                            Timestamp = m.Timestamp, // Bereits UTC durch Fix in PegelOnlineService
                            WaterLevelCm = m.Value,
                            RecordedAt = DateTime.UtcNow // Explizit UTC!
                        })
                        .ToList();
    
                    await repository.SaveMeasurementsAsync(measurements);
    
                    _logger.LogInformation("Loaded {Count} measurements for {StationName}", 
                        measurements.Count, station.Name);
                }
                else
                {
                    _logger.LogWarning("No historical data received for {StationName}", station.Name);
                }
            }
            
            _logger.LogInformation("Historical data loading completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during historical data loading");
        }
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}