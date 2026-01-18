using Microsoft.EntityFrameworkCore;
using FloodSimulation.Models.Entities;

namespace FloodSimulation.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TerrainRaster> TerrainRasters { get; set; }
    
    // WaterLevel Data
    public DbSet<WaterLevelStation> WaterLevelStations { get; set; }
    public DbSet<WaterLevelMeasurement> WaterLevelMeasurements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostGIS Extensions
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.HasPostgresExtension("postgis_raster");

        // TerrainRaster Config
        modelBuilder.Entity<TerrainRaster>(entity =>
        {
            entity.HasKey(e => e.Rid);
            entity.ToTable("terrain_raster");
        });
        
        // WaterLevelStation Config
        modelBuilder.Entity<WaterLevelStation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("water_level_stations");
            
            entity.HasMany(e => e.Measurements)
                .WithOne(e => e.Station)
                .HasForeignKey(e => e.StationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // WaterLevelMeasurement Config
        modelBuilder.Entity<WaterLevelMeasurement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("water_level_measurements");
            
            entity.HasIndex(e => new { e.StationId, e.Timestamp })
                .IsUnique();
        });
    }
}