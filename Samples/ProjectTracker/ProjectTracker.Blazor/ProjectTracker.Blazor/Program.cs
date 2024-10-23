using Csla.Configuration;
using Marimer.Blazor.RenderMode;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProjectTracker.Blazor.Components;
using ProjectTracker.Configuration;

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

builder.Services.AddDalMock();
//builder.Services.AddDalEfCore();

var app = builder.Build();

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

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ProjectTracker.Blazor.Client._Imports).Assembly);

app.Run();
