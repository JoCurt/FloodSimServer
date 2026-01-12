using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FloodSimulation.Models.Entities;

/// <summary>
/// Entity für terrain_raster Tabelle (PostGIS Raster)
/// </summary>
[Table("terrain_raster")]
public class TerrainRaster
{
    [Key]
    [Column("rid")]
    public long Rid { get; set; }

    /// <summary>
    /// PostGIS Raster Daten als byte array
    /// </summary>
    [Column("rast", TypeName = "raster")]
    public byte[]? Rast { get; set; }

    [Column("filename")]
    [MaxLength(255)]
    public string? Filename { get; set; }
}