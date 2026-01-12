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

### Terrain Data

#### `GET /api/terrain/info`
Get overview of available terrain data including bounds and tile count.

**Response:**
```json
{
  "rasterTileCount": 1,
  "bounds": {
    "minX": 323999.5,
    "minY": 5507999.5,
    "maxX": 325000.5,
    "maxY": 5509000.5,
    "coordinateSystem": "EPSG:25832",
    "rasterTileCount": 1,
    "width": 1001.0,
    "height": 1001.0,
    "centerX": 324500.0,
    "centerY": 5508500.0
  },
  "coordinateSystem": "EPSG:25832",
  "status": "ready",
  "isReady": true
}
```

#### `GET /api/terrain/bounds`
Get the geographic bounding box of all terrain raster data.

**Response:**
```json
{
  "minX": 323999.5,
  "minY": 5507999.5,
  "maxX": 325000.5,
  "maxY": 5509000.5,
  "coordinateSystem": "EPSG:25832",
  "rasterTileCount": 1,
  "width": 1001.0,
  "height": 1001.0,
  "centerX": 324500.0,
  "centerY": 5508500.0
}
```

#### `GET /api/terrain/elevation?x={x}&y={y}`
Get elevation at a specific UTM coordinate (EPSG:25832).

**Parameters:**
- `x` (required): UTM X coordinate in meters
- `y` (required): UTM Y coordinate in meters

**Example:**
```
GET /api/terrain/elevation?x=324500&y=5508500
```

**Response (Success):**
```json
{
  "x": 324500.0,
  "y": 5508500.0,
  "elevation": 156.3,
  "unit": "meters",
  "coordinateSystem": "EPSG:25832",
  "hasElevation": true
}
```

**Response (Point outside raster):**
```json
{
  "error": "No elevation data at this coordinate",
  "message": "Point may be outside the raster bounds",
  "coordinates": { "x": 999999, "y": 999999 }
}
```

### Health & Diagnostics

#### `GET /api/test/health`
Server health check endpoint.

#### `GET /api/test/db`
Database connectivity test with raster tile count.


## Project Structure

```
FloodSimulation/
├── Controllers/          # API controllers
│   └── TestController.cs
│   └── TestController.cs   # Health checks & diagnostics
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
├── Services/ 
│   └── TerrainService.cs       # Terrain data orchestration
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


### Database Connection Issues

- Verify PostgreSQL is running: `systemctl status postgresql`
- Check PostGIS extensions: `SELECT PostGIS_Version();`
- Verify raster extension: `SELECT PostGIS_Raster_Lib_Version();`

### No Elevation Data Returned

- Ensure raster data is loaded into `terrain_raster` table
- Verify coordinates are within raster bounds
- Check coordinate reference system matches (EPSG:25832)


## Contributing

This is a research/educational project. For questions or suggestions, contact the project maintainer.
