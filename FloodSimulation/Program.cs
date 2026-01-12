using Microsoft.EntityFrameworkCore;
using FloodSimulation.Data;
using FloodSimulation.Repositories;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL Connection String
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

// Entity Framework mit PostgreSQL (OHNE NetTopologySuite erstmal)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Controllers
builder.Services.AddScoped<ITerrainRasterRepository, TerrainRasterRepository>();

builder.Services.AddControllers();


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