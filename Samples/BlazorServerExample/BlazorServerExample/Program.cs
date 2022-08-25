using BlazorServerExample.Areas.Identity;
using BlazorServerExample.Data;
using Csla.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
// necessary for auth
builder.Services.AddAuthentication(options =>
{
  options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(); 
builder.Services.AddScoped<IdentityUser>();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddCsla(o => o
  .AddAspNetCore()
  .AddServerSideBlazor());

builder.Services.AddTransient(typeof(DataAccess.IPersonDal), typeof(DataAccess.Mock.PersonDal));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
