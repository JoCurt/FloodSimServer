using Microsoft.AspNetCore.Mvc;
using FloodSimulation.Repositories;
using FloodSimulation.Models.DTOs;
using FloodSimulation.Models.Entities;

namespace FloodSimulation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WaterLevelController : ControllerBase
{
    private readonly IWaterLevelRepository _repository;
    
    public WaterLevelController(IWaterLevelRepository repository)
    {
        _repository = repository;
    }
    
    /// <summary>
    /// Gibt alle Pegel-Stationen zurück
    /// </summary>
    [HttpGet("stations")]
    public async Task<ActionResult<List<WaterLevelStation>>> GetStations()
    {
        var stations = await _repository.GetAllStationsAsync();
        return Ok(stations);
    }
    
    /// <summary>
    /// Gibt aktuellsten Wasserstand für eine Station zurück
    /// </summary>
    [HttpGet("current/{stationId}")]
    public async Task<ActionResult<WaterLevelResponse>> GetCurrent(Guid stationId)
    {
        var station = await _repository.GetStationByIdAsync(stationId);
        if (station == null)
        {
            return NotFound(new { error = "Station not found" });
        }
        
        var measurement = await _repository.GetLatestMeasurementAsync(stationId);
        if (measurement == null)
        {
            return NotFound(new { error = "No measurements available for this station" });
        }
        
        var response = new WaterLevelResponse
        {
            StationId = station.Id,
            StationName = station.Name,
            WaterLevelCm = measurement.WaterLevelCm,
            Timestamp = measurement.Timestamp,
            RecordedAt = measurement.RecordedAt
        };
        
        return Ok(response);
    }
    
    /// <summary>
    /// Gibt historische Daten für eine Station zurück
    /// </summary>
    [HttpGet("history/{stationId}")]
    public async Task<ActionResult<WaterLevelHistoryResponse>> GetHistory(
        Guid stationId,
        [FromQuery] int days = 10)
    {
        var station = await _repository.GetStationByIdAsync(stationId);
        if (station == null)
        {
            return NotFound(new { error = "Station not found" });
        }
        
        var since = DateTime.UtcNow.AddDays(-days);
        var measurements = await _repository.GetMeasurementsSinceAsync(stationId, since);
        
        var response = new WaterLevelHistoryResponse
        {
            StationId = station.Id,
            StationName = station.Name,
            Measurements = measurements.Select(m => new WaterLevelDataPoint
            {
                Timestamp = m.Timestamp,
                WaterLevelCm = m.WaterLevelCm
            }).ToList()
        };
        
        return Ok(response);
    }
    
    /// <summary>
    /// Gibt aktuelle Wasserstände aller Stationen zurück
    /// </summary>
    [HttpGet("current-all")]
    public async Task<ActionResult<List<WaterLevelResponse>>> GetCurrentAll()
    {
        var stations = await _repository.GetAllStationsAsync();
        var responses = new List<WaterLevelResponse>();
        
        foreach (var station in stations)
        {
            var measurement = await _repository.GetLatestMeasurementAsync(station.Id);
            if (measurement != null)
            {
                responses.Add(new WaterLevelResponse
                {
                    StationId = station.Id,
                    StationName = station.Name,
                    WaterLevelCm = measurement.WaterLevelCm,
                    Timestamp = measurement.Timestamp,
                    RecordedAt = measurement.RecordedAt
                });
            }
        }
        
        return Ok(responses);
    }
}