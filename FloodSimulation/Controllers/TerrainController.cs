using Microsoft.AspNetCore.Mvc;
using FloodSimulation.Services;
using FloodSimulation.Models.DTOs;

namespace FloodSimulation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TerrainController : ControllerBase
{
    private readonly TerrainService _terrainService;
    
    public TerrainController(TerrainService terrainService)
    {
        _terrainService = terrainService;
    }
    
    /// <summary>
    /// Gibt Höhe an einem bestimmten Punkt zurück
    /// </summary>
    /// <param name="x">X-Koordinate (UTM, EPSG:25832)</param>
    /// <param name="y">Y-Koordinate (UTM, EPSG:25832)</param>
    /// <returns>Höhe in Metern</returns>
    [HttpGet("elevation")]
    [ProducesResponseType(typeof(ElevationResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ElevationResponse>> GetElevation(
        [FromQuery] double x,
        [FromQuery] double y)
    {
        var response = await _terrainService.GetElevationAtAsync(x, y);
        
        if (!response.HasElevation)
        {
            return NotFound(new
            {
                error = "No elevation data at this coordinate",
                message = "Point may be outside the raster bounds",
                coordinates = new { x, y }
            });
        }
        
        return Ok(response);
    }
    
    /// <summary>
    /// Gibt die geografischen Grenzen des verfügbaren Terrains zurück
    /// </summary>
    [HttpGet("bounds")]
    [ProducesResponseType(typeof(TerrainBoundsResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<TerrainBoundsResponse>> GetBounds()
    {
        var response = await _terrainService.GetTerrainBoundsAsync();
        
        if (response == null)
        {
            return NotFound(new { error = "No terrain data available" });
        }
        
        return Ok(response);
    }
    
    /// <summary>
    /// Gibt allgemeine Informationen über verfügbare Terrain-Daten zurück
    /// </summary>
    [HttpGet("info")]
    [ProducesResponseType(typeof(TerrainInfoResponse), 200)]
    public async Task<ActionResult<TerrainInfoResponse>> GetInfo()
    {
        var response = await _terrainService.GetTerrainInfoAsync();
        return Ok(response);
    }
}