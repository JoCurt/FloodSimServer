using FloodSimulation.Models.Entities;

namespace FloodSimulation.Repositories;

public interface IWaterLevelRepository
{
    Task<List<WaterLevelStation>> GetAllStationsAsync();
    Task<WaterLevelStation?> GetStationByIdAsync(Guid stationId);
    Task<WaterLevelMeasurement?> GetLatestMeasurementAsync(Guid stationId);
    Task<List<WaterLevelMeasurement>> GetMeasurementsSinceAsync(Guid stationId, DateTime since);
    Task SaveMeasurementsAsync(List<WaterLevelMeasurement> measurements);
    Task DeleteMeasurementsOlderThanAsync(DateTime cutoffDate);
    Task UpdateStationLastUpdatedAsync(Guid stationId, DateTime timestamp);
}