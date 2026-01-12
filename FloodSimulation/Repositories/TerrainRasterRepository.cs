using FloodSimulation.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql; // NEU

namespace FloodSimulation.Repositories;

public class TerrainRasterRepository : ITerrainRasterRepository
{
    private readonly ApplicationDbContext _context;
    
    public TerrainRasterRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<double?> GetElevationAtAsync(double x, double y)
    {
        // Direkte SQL Execution ohne LINQ
        await using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT ST_Value(rast, ST_SetSRID(ST_Point(@x, @y), 25832)) 
            FROM terrain_raster 
            WHERE ST_Intersects(rast, ST_SetSRID(ST_Point(@x, @y), 25832))
            LIMIT 1";
        
        var paramX = command.CreateParameter();
        paramX.ParameterName = "@x";
        paramX.Value = x;
        command.Parameters.Add(paramX);
        
        var paramY = command.CreateParameter();
        paramY.ParameterName = "@y";
        paramY.Value = y;
        command.Parameters.Add(paramY);
        
        var result = await command.ExecuteScalarAsync();
        
        if (result == null || result == DBNull.Value)
            return null;
        
        return Convert.ToDouble(result);
    }
    
    public async Task<(double minX, double minY, double maxX, double maxY)?> GetRasterBoundsAsync()
    {
        await using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                ST_XMin(ST_Envelope(ST_Union(rast))) as MinX,
                ST_YMin(ST_Envelope(ST_Union(rast))) as MinY,
                ST_XMax(ST_Envelope(ST_Union(rast))) as MaxX,
                ST_YMax(ST_Envelope(ST_Union(rast))) as MaxY
            FROM terrain_raster";
        
        await using var reader = await command.ExecuteReaderAsync();
        
        if (!await reader.ReadAsync())
            return null;
        
        var minX = reader.GetDouble(0);
        var minY = reader.GetDouble(1);
        var maxX = reader.GetDouble(2);
        var maxY = reader.GetDouble(3);
        
        return (minX, minY, maxX, maxY);
    }
    
    public async Task<long> CountRasterTilesAsync()
    {
        // Diese Methode nutzt normale EF Core Query - kein Problem
        return await _context.TerrainRasters.CountAsync();
    }
}