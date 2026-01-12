using FloodSimulation.Models.DTOs;
using FloodSimulation.Repositories;

namespace FloodSimulation.Services;

public class TerrainService
{
    private readonly ITerrainRasterRepository _terrainRepo;
    
    public TerrainService(ITerrainRasterRepository terrainRepo)
    {
        _terrainRepo = terrainRepo;
    }
    
    /// <summary>
    /// Gibt Höhe an einem Punkt zurück
    /// </summary>
    public async Task<ElevationResponse> GetElevationAtAsync(double x, double y)
    {
        var elevation = await _terrainRepo.GetElevationAtAsync(x, y);
        
        return new ElevationResponse
        {
            X = x,
            Y = y,
            Elevation = elevation
        };
    }
    
    /// <summary>
    /// Gibt Bounds des Terrains zurück
    /// </summary>
    public async Task<TerrainBoundsResponse?> GetTerrainBoundsAsync()
    {
        var bounds = await _terrainRepo.GetRasterBoundsAsync();
    
        if (bounds == null)
            return null;

        var tileCount = await _terrainRepo.CountRasterTilesAsync();
    
        return new TerrainBoundsResponse
        {
            MinX = bounds.Value.minX,
            MinY = bounds.Value.minY,
            MaxX = bounds.Value.maxX,
            MaxY = bounds.Value.maxY,
            RasterTileCount = tileCount
        };
    }
    
    /// <summary>
    /// Gibt allgemeine Terrain-Info zurück
    /// </summary>
    public async Task<TerrainInfoResponse> GetTerrainInfoAsync()
    {
        var tileCount = await _terrainRepo.CountRasterTilesAsync();
        var bounds = await GetTerrainBoundsAsync();
        
        return new TerrainInfoResponse
        {
            RasterTileCount = tileCount,
            Bounds = bounds,
            Status = tileCount > 0 ? "ready" : "no_data"
        };
    }
}