using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Csla.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CslaMvc.Mvc
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAuthorization();
      services.AddHttpContextAccessor();

      services.AddControllersWithViews((c) =>
        c.ModelBinderProviders.Insert(0, new Csla.Web.Mvc.CslaModelBinderProvider()));
      services.AddCsla(opt => opt.AddAspNetCore());
      services.AddTransient(typeof(DataAccess.IPersonDal), typeof(DataAccess.PersonDal));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
      });

      app.UseCsla();
    }
  }
}
