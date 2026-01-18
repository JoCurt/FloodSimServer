using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FloodSimulation.Models.Entities;

[Table("water_level_measurements")]
public class WaterLevelMeasurement
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    
    [Column("station_id")]
    public Guid StationId { get; set; }
    
    [Column("timestamp")]
    public DateTime Timestamp { get; set; }
    
    [Column("water_level_cm")]
    public double WaterLevelCm { get; set; }
    
    [Column("recorded_at")]
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Property
    [ForeignKey(nameof(StationId))]
    public WaterLevelStation Station { get; set; } = null!;
}