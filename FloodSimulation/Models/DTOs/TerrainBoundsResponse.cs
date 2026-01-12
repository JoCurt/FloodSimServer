namespace FloodSimulation.Models.DTOs;

public class TerrainBoundsResponse
{
    public double MinX { get; set; }
    public double MinY { get; set; }
    public double MaxX { get; set; }
    public double MaxY { get; set; }
    public string CoordinateSystem { get; set; } = "EPSG:25832";
    public long RasterTileCount { get; set; }
    
    public double Width => MaxX - MinX;
    public double Height => MaxY - MinY;
    public double CenterX => (MinX + MaxX) / 2.0;
    public double CenterY => (MinY + MaxY) / 2.0;
}