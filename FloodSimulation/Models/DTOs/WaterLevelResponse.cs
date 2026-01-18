namespace FloodSimulation.Models.DTOs;

public class WaterLevelResponse
{
    public Guid StationId { get; set; }
    public string StationName { get; set; } = string.Empty;
    public double WaterLevelCm { get; set; }
    public double WaterLevelMeters => WaterLevelCm / 100.0;
    public DateTime Timestamp { get; set; }
    public DateTime RecordedAt { get; set; }
}

public class WaterLevelHistoryResponse
{
    public Guid StationId { get; set; }
    public string StationName { get; set; } = string.Empty;
    public List<WaterLevelDataPoint> Measurements { get; set; } = new();
}

public class WaterLevelDataPoint
{
    public DateTime Timestamp { get; set; }
    public double WaterLevelCm { get; set; }
    public double WaterLevelMeters => WaterLevelCm / 100.0;
}