using Csla.Configuration;
using Marimer.Blazor.RenderMode;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProjectTracker.Blazor.Components;
using ProjectTracker.Configuration;
using ProjectTracker.DalEfCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddCookie();
builder.Services.AddCascadingAuthenticationState();

// Add render mode detection services
builder.Services.AddRenderModeDetection();

// CSLA requires AddHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add CSLA
builder.Services.AddCsla(o => o
  .AddAspNetCore()
  .AddServerSideBlazor(o => o.UseInMemoryApplicationContextManager = false));

// Use in-memory mock database DAL
//builder.Services.AddDalMock();

// Use SQLite-backed EF Core DAL
builder.Services.AddDalEfCore("Data Source=PTracker.db");

var app = builder.Build();

// Initialize SQLite database and seed data on startup
PTrackerDatabaseInitializer.Initialize(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseWebAssemblyDebugging();
}
else
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ProjectTracker.Blazor.Client._Imports).Assembly);

app.Run();
