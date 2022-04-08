using Csla.Configuration;
using Csla.Web.Mvc;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RazorPagesExample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages().AddMvcOptions(options =>
{
  options.ModelBinderProviders.Insert(0, new CslaModelBinderProvider());
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddCsla(o => o
  .AddAspNetCore());

builder.Services.AddTransient(typeof(DataAccess.IPersonDal), typeof(DataAccess.PersonDal));

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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
