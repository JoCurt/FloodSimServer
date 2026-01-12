using Microsoft.AspNetCore.Mvc;
using FloodSimulation.Data;
using FloodSimulation.Repositories; // NEU
using Microsoft.EntityFrameworkCore;

namespace FloodSimulation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITerrainRasterRepository _terrainRepo; // NEU
    
    // Constructor erweitert
    public TestController(
        ApplicationDbContext context,
        ITerrainRasterRepository terrainRepo) // NEU
    {
        _context = context;
        _terrainRepo = terrainRepo; // NEU
    }
    
    /// <summary>
    /// Einfacher Health Check
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "Server is running!",
            timestamp = DateTime.UtcNow
        });
    }
    
    /// <summary>
    /// Test DB-Verbindung
    /// </summary>
    [HttpGet("db")]
    public async Task<IActionResult> TestDatabase()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                return StatusCode(500, new { error = "Cannot connect to database" });
            }
            
            var tileCount = await _context.TerrainRasters.CountAsync();
            
            return Ok(new
            {
                status = "Database connection OK!",
                rasterTiles = tileCount,
                connectionString = "Connected to flood_sim_test_db"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message,
                stackTrace = ex.StackTrace
            });
        }
    }
    
    /// <summary>
    /// NEU: Test Repository - Bounds
    /// </summary>
    [HttpGet("repository/bounds")]
    public async Task<IActionResult> TestRepositoryBounds()
    {
        try
        {
            var bounds = await _terrainRepo.GetRasterBoundsAsync();
            
            if (bounds == null)
            {
                return NotFound(new { error = "No raster data found" });
            }
            
            return Ok(new
            {
                status = "Repository works!",
                bounds = new
                {
                    minX = bounds.Value.minX,
                    minY = bounds.Value.minY,
                    maxX = bounds.Value.maxX,
                    maxY = bounds.Value.maxY
                },
                width = bounds.Value.maxX - bounds.Value.minX,
                height = bounds.Value.maxY - bounds.Value.minY
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message,
                stackTrace = ex.StackTrace
            });
        }
    }
    
    /// <summary>
    /// NEU: Test Repository - Elevation an einem Punkt
    /// </summary>
    [HttpGet("repository/elevation")]
    public async Task<IActionResult> TestRepositoryElevation(
        [FromQuery] double? x = null,
        [FromQuery] double? y = null)
    {
        try
        {
            // Standard-Koordinaten: Mitte deines Rasters
            double testX = x ?? 324500;
            double testY = y ?? 5508500;
            
            var elevation = await _terrainRepo.GetElevationAtAsync(testX, testY);
            
            return Ok(new
            {
                status = "Repository works!",
                coordinates = new { x = testX, y = testY },
                elevation = elevation,
                unit = "meters",
                hasData = elevation.HasValue
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message,
                stackTrace = ex.StackTrace
            });
        }
    }
    
    /// <summary>
    /// NEU: Test Repository - Tile Count
    /// </summary>
    [HttpGet("repository/tiles")]
    public async Task<IActionResult> TestRepositoryTiles()
    {
        try
        {
            var count = await _terrainRepo.CountRasterTilesAsync();
            
            return Ok(new
            {
                status = "Repository works!",
                tileCount = count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message,
                stackTrace = ex.StackTrace
            });
        }
    }
}