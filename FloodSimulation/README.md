# FloodSimulation Server

An ASP.NET Core Web API server for flood simulation applications that provides terrain elevation data stored in PostgreSQL with PostGIS raster extensions.

## Overview

FloodSimulation is a backend service designed to serve terrain elevation data for flood simulation applications. It uses PostGIS raster functionality to store and query Digital Elevation Model (DEM) data efficiently, providing REST API endpoints for retrieving elevation information at specific coordinates.

## Features

- **Terrain Elevation Queries**: Get elevation values at specific UTM coordinates
- **Raster Data Management**: Store and query terrain raster tiles using PostGIS
- **Spatial Operations**: Calculate terrain bounds and intersections
- **CORS Support**: Pre-configured for Unity client integration
- **API Documentation**: Interactive Swagger/OpenAPI documentation
- **Health Checks**: Database connectivity and service health monitoring

## Technology Stack

- **.NET 10.0**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **Entity Framework Core 10.0**: ORM for database access
- **PostgreSQL**: Primary database with spatial extensions
- **PostGIS**: Geospatial extension for raster data storage
- **NetTopologySuite**: .NET spatial library for geometric operations
- **Swagger/Swashbuckle**: API documentation and testing

## Prerequisites

- .NET 10.0 SDK or later
- PostgreSQL 12+ with PostGIS extension
- PostGIS Raster extension enabled

## Getting Started

### Database Setup

1. Install PostgreSQL with PostGIS extensions:
```sql
CREATE DATABASE flood_sim_test_db;
\c flood_sim_test_db
CREATE EXTENSION postgis;
CREATE EXTENSION postgis_raster;
```

2. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=flood_sim_test_db;Username=your_user;Password=your_password"
  }
}
```

### Running the Application

1. Restore dependencies:
```bash
dotnet restore
```

2. Run the application:
```bash
dotnet run
```

3. Access Swagger UI at: `http://localhost:5000/swagger`

## API Endpoints

### Health & Diagnostics

#### `GET /api/test/health`
Check if the server is running.

**Response:**
```json
{
  "status": "Server is running!",
  "timestamp": "2026-01-12T10:30:00Z"
}
```

#### `GET /api/test/db`
Test database connectivity and get raster tile count.

**Response:**
```json
{
  "status": "Database connection OK!",
  "rasterTiles": 42,
  "connectionString": "Connected to flood_sim_test_db"
}
```

### Terrain Data

#### `GET /api/test/repository/bounds`
Get the bounding box of all terrain raster data.

**Response:**
```json
{
  "status": "Repository works!",
  "bounds": {
    "minX": 324000,
    "minY": 5508000,
    "maxX": 325000,
    "maxY": 5509000
  },
  "width": 1000,
  "height": 1000
}
```

#### `GET /api/test/repository/elevation?x={x}&y={y}`
Get elevation at a specific UTM coordinate (EPSG:25832).

**Parameters:**
- `x` (optional): UTM X coordinate (default: 324500)
- `y` (optional): UTM Y coordinate (default: 5508500)

**Response:**
```json
{
  "status": "Repository works!",
  "coordinates": {
    "x": 324500,
    "y": 5508500
  },
  "elevation": 123.45,
  "unit": "meters",
  "hasData": true
}
```

#### `GET /api/test/repository/tiles`
Get the count of raster tiles in the database.

**Response:**
```json
{
  "status": "Repository works!",
  "tileCount": 42
}
```

## Project Structure

```
FloodSimulation/
├── Controllers/          # API controllers
│   └── TestController.cs
├── Data/                # Database context
│   └── ApplicationDbContext.cs
├── Models/
│   ├── DTOs/           # Data transfer objects
│   │   ├── ElevationResponse.cs
│   │   ├── TerrainBoundsResponse.cs
│   │   └── TerrainInfoResponse.cs
│   └── Entities/       # Database entities
│       └── TerrainRaster.cs
├── Repositories/        # Data access layer
│   ├── ITerrainRasterRepository.cs
│   └── TerrainRasterRepository.cs
├── Properties/          # Launch settings
├── appsettings.json    # Configuration
└── Program.cs          # Application entry point
```

## Configuration

### CORS Policy

The application includes a permissive CORS policy (`AllowAll`) configured for Unity client integration. For production deployments, configure specific origins:

```csharp
options.AddPolicy("Production", policy =>
{
    policy.WithOrigins("https://your-client-domain.com")
          .AllowAnyMethod()
          .AllowAnyHeader();
});
```

### Database Connection

Configure your PostgreSQL connection in `appsettings.json` or use environment variables for secure deployments.

## Data Model

### TerrainRaster Entity

Represents a single raster tile in the database:

- `Rid`: Unique identifier (Primary Key)
- `Rast`: PostGIS raster data (byte array)
- `Filename`: Source filename for reference

## Coordinate System

The application uses **UTM Zone 32N (EPSG:25832)** coordinate reference system. Ensure your terrain data is projected to this CRS before importing.

## Loading Terrain Data

To import raster data into PostgreSQL, use the `raster2pgsql` utility:

```bash
raster2pgsql -s 25832 -I -C -M -t 256x256 your_dem.tif terrain_raster | psql -d flood_sim_test_db
```

Options:
- `-s 25832`: Set SRID to EPSG:25832 (UTM Zone 32N)
- `-I`: Create spatial index
- `-C`: Apply raster constraints
- `-M`: Vacuum analyze after import
- `-t 256x256`: Tile size

## Development

### Adding New Endpoints

1. Create a new controller in `Controllers/`
2. Register services in `Program.cs`
3. Add repository methods in `Repositories/`
4. Update database context if needed in `Data/`

### Running Migrations

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Troubleshooting

### Database Connection Issues

- Verify PostgreSQL is running: `systemctl status postgresql`
- Check PostGIS extensions: `SELECT PostGIS_Version();`
- Verify raster extension: `SELECT PostGIS_Raster_Lib_Version();`

### No Elevation Data Returned

- Ensure raster data is loaded into `terrain_raster` table
- Verify coordinates are within raster bounds
- Check coordinate reference system matches (EPSG:25832)

## License

This project is part of a flood simulation research application.

## Contributing

This is a research/educational project. For questions or suggestions, contact the project maintainer.
