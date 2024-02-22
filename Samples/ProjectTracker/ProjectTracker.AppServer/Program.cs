using Csla.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProjectTracker.Configuration;

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

// Required by CSLA data portal controller. If using Kestrel:
builder.Services.Configure<KestrelServerOptions>(options =>
{
  options.AllowSynchronousIO = true;
});

// Required by CSLA data portal controller. If using IIS:
builder.Services.Configure<IISServerOptions>(options =>
{
  options.AllowSynchronousIO = true;
});

builder.Services.AddCsla(o => o
    .Security(so => so.FlowSecurityPrincipalFromClient = true));

builder.Services.AddDalMock();
//builder.Services.AddDalEfCore();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
