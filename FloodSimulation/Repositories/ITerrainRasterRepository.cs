namespace FloodSimulation.Repositories;

public interface ITerrainRasterRepository
{
    /// <summary>
    /// Gibt Höhe an einem Punkt zurück (UTM Koordinaten)
    /// </summary>
    Task<double?> GetElevationAtAsync(double x, double y);
    
    /// <summary>
    /// Gibt Bounds des Rasters zurück
    /// </summary>
    Task<(double minX, double minY, double maxX, double maxY)?> GetRasterBoundsAsync();
    
    /// <summary>
    /// Zählt Anzahl der Raster-Tiles
    /// </summary>
    Task<long> CountRasterTilesAsync();
}