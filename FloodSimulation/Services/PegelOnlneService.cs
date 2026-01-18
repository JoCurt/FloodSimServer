using System.Text.Json;
using FloodSimulation.Models.DTOs;

namespace FloodSimulation.Services;

public class PegelOnlineService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PegelOnlineService> _logger;
    private const string BaseUrl = "https://www.pegelonline.wsv.de/webservices/rest-api/v2";
    
    public PegelOnlineService(HttpClient httpClient, ILogger<PegelOnlineService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    /// <summary>
    /// Holt aktuellen Wasserstand von einer Station
    /// </summary>
    public async Task<double?> GetCurrentWaterLevelAsync(Guid stationId)
    {
        try
        {
            // API-Endpoint: /stations/{uuid}/W/currentmeasurement.json
            var url = $"{BaseUrl}/stations/{stationId}/W/currentmeasurement.json";
            
            _logger.LogDebug("Fetching current water level from {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch water level for station {StationId}: {StatusCode}", 
                    stationId, response.StatusCode);
                return null;
            }
            
            var json = await response.Content.ReadAsStringAsync();
            var measurement = JsonSerializer.Deserialize<PegelOnlineMeasurement>(json, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return measurement?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current water level for station {StationId}", stationId);
            return null;
        }
    }
    
    /// <summary>
    /// Holt historische Daten (letzte N Tage)
    /// </summary>
    public async Task<List<PegelOnlineMeasurement>> GetHistoricalDataAsync(Guid stationId, int days)
    {
        try
        {
            var url = $"{BaseUrl}/stations/{stationId}/W/measurements.json?start=P{days}D";
        
            _logger.LogInformation("Fetching historical data ({Days} days) from {Url}", days, url);
        
            var response = await _httpClient.GetAsync(url);
        
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch historical data for station {StationId}: {StatusCode}", 
                    stationId, response.StatusCode);
                return new List<PegelOnlineMeasurement>();
            }
        
            var json = await response.Content.ReadAsStringAsync();
            var measurements = JsonSerializer.Deserialize<List<PegelOnlineMeasurement>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
            // NEU: Konvertiere alle Timestamps zu UTC
            if (measurements != null)
            {
                foreach (var m in measurements)
                {
                    // Stelle sicher dass Timestamp UTC ist
                    if (m.Timestamp.Kind == DateTimeKind.Local)
                    {
                        m.Timestamp = m.Timestamp.ToUniversalTime();
                    }
                    else if (m.Timestamp.Kind == DateTimeKind.Unspecified)
                    {
                        // Behandle als UTC wenn nicht spezifiziert
                        m.Timestamp = DateTime.SpecifyKind(m.Timestamp, DateTimeKind.Utc);
                    }
                }
            }
        
            return measurements ?? new List<PegelOnlineMeasurement>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching historical data for station {StationId}", stationId);
            return new List<PegelOnlineMeasurement>();
        }
    }
}