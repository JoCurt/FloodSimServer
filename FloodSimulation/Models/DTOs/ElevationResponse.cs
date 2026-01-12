namespace FloodSimulation.Models.DTOs;

public class ElevationResponse
{
    public double X { get; set; }
    public double Y { get; set; }
    public double? Elevation { get; set; }
    public string Unit { get; set; } = "meters";
    public string CoordinateSystem { get; set; } = "EPSG:25832";
    
    public bool HasElevation => Elevation.HasValue;
}