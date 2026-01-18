using System.Text.Json.Serialization;

namespace FloodSimulation.Models.DTOs;

/// <summary>
/// Response von pegelonline.de API für Messwerte
/// </summary>
public class PegelOnlineMeasurement
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    
    [JsonPropertyName("value")]
    public double Value { get; set; }
}

/// <summary>
/// Response für Timeseries (historische Daten)
/// </summary>
public class PegelOnlineTimeseries
{
    [JsonPropertyName("currentMeasurement")]
    public PegelOnlineMeasurement? CurrentMeasurement { get; set; }
}