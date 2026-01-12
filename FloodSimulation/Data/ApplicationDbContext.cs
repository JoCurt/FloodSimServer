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
    }
}