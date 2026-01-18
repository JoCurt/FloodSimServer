using Microsoft.EntityFrameworkCore;
using FloodSimulation.Data;
using FloodSimulation.Repositories;
using FloodSimulation.Services;

// TODO: Fix Logging output, especially PSQL output
// Failed Fix
// using Npgsql;
//
// // Npgsql internes Logging deaktivieren
// NpgsqlLoggingConfiguration.InitializeLogging(null!, parameterLoggingEnabled: false);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

var builder = WebApplication.CreateBuilder(args);

// Logging Conf
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("System", LogLevel.Warning);
builder.Logging.AddFilter("Npgsql", LogLevel.Warning);
builder.Logging.AddFilter("FloodSimulation", LogLevel.Information); // Deine eigenen Logs


// PostgreSQL Connection String
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

// Entity Framework mit PostgreSQL (OHNE NetTopologySuite erstmal)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Controllers
builder.Services.AddScoped<ITerrainRasterRepository, TerrainRasterRepository>();
builder.Services.AddScoped<TerrainService>();
builder.Services.AddScoped<IWaterLevelRepository, WaterLevelRepository>(); 

// Services
builder.Services.AddControllers();


// HttpClient for PegelOnlineService 
builder.Services.AddHttpClient<PegelOnlineService>();

// Background Services
builder.Services.AddHostedService<WaterLevelStartupService>();
builder.Services.AddHostedService<WaterLevelPollingService>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS für Unity
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();