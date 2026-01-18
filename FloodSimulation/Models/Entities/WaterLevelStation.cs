using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FloodSimulation.Models.Entities;

[Table("water_level_stations")]
public class WaterLevelStation
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Column("water_body")]
    [MaxLength(255)]
    public string? WaterBody { get; set; }
    
    [Column("latitude")]
    public double? Latitude { get; set; }
    
    [Column("longitude")]
    public double? Longitude { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("last_updated")]
    public DateTime? LastUpdated { get; set; }
    
    // Navigation Property
    public ICollection<WaterLevelMeasurement> Measurements { get; set; } = new List<WaterLevelMeasurement>();
}