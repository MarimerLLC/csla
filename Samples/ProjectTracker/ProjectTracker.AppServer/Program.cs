using Csla.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProjectTracker.Configuration;
using ProjectTracker.DalEfCore;

const string BlazorClientPolicy = "AllowAllOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// CSLA requires AddHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// support CORS so clients can call services
builder.Services.AddCors(options =>
{
  options.AddPolicy(BlazorClientPolicy,
    builder =>
    {
      builder
      .AllowAnyOrigin()
      .AllowAnyHeader()
      .AllowAnyMethod();
    });
});

builder.Services.AddCsla(o => o
    .Security(so => so.FlowSecurityPrincipalFromClient = true));

// Use in-memory mock database DAL
//builder.Services.AddDalMock();

// Use SQLite-backed EF Core DAL
builder.Services.AddDalEfCore("Data Source=PTracker.db");

var app = builder.Build();

// Initialize SQLite database and seed data on startup
PTrackerDatabaseInitializer.Initialize(app.Services);

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
