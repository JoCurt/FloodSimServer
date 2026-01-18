using FloodSimulation.Data;
using FloodSimulation.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FloodSimulation.Repositories;

public class WaterLevelRepository : IWaterLevelRepository
{
    private readonly ApplicationDbContext _context;
    
    public WaterLevelRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<WaterLevelStation>> GetAllStationsAsync()
    {
        return await _context.WaterLevelStations.ToListAsync();
    }
    
    public async Task<WaterLevelStation?> GetStationByIdAsync(Guid stationId)
    {
        return await _context.WaterLevelStations
            .FirstOrDefaultAsync(s => s.Id == stationId);
    }
    
    public async Task<WaterLevelMeasurement?> GetLatestMeasurementAsync(Guid stationId)
    {
        return await _context.WaterLevelMeasurements
            .Where(m => m.StationId == stationId)
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<WaterLevelMeasurement>> GetMeasurementsSinceAsync(Guid stationId, DateTime since)
    {
        return await _context.WaterLevelMeasurements
            .Where(m => m.StationId == stationId && m.Timestamp >= since)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }
    
    public async Task SaveMeasurementsAsync(List<WaterLevelMeasurement> measurements)
    {
        if (!measurements.Any())
            return;
        
        // Nutze AddRange für bessere Performance
        await _context.WaterLevelMeasurements.AddRangeAsync(measurements);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Duplikate ignorieren (Unique Constraint)
            // Das ist OK - bedeutet wir haben die Daten schon
        }
    }
    
    public async Task DeleteMeasurementsOlderThanAsync(DateTime cutoffDate)
    {
        var oldMeasurements = await _context.WaterLevelMeasurements
            .Where(m => m.Timestamp < cutoffDate)
            .ToListAsync();
        
        if (oldMeasurements.Any())
        {
            _context.WaterLevelMeasurements.RemoveRange(oldMeasurements);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task UpdateStationLastUpdatedAsync(Guid stationId, DateTime timestamp)
    {
        var station = await _context.WaterLevelStations.FindAsync(stationId);
        if (station != null)
        {
            station.LastUpdated = timestamp;
            await _context.SaveChangesAsync();
        }
    }
}