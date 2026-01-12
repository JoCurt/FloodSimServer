namespace FloodSimulation.Models.DTOs;

public class TerrainInfoResponse
{
    public long RasterTileCount { get; set; }
    public TerrainBoundsResponse? Bounds { get; set; }
    public string CoordinateSystem { get; set; } = "EPSG:25832";
    public string Status { get; set; } = "ready";
    
    public bool IsReady => Status == "ready" && RasterTileCount > 0;
}