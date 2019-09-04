using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Csla.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppServer
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    private readonly string myAllowOrigins = "_myAllowOrgins";

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors(options =>
      {
        options.AddPolicy(myAllowOrigins,
          builder =>
          {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
          });
      });

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      services.AddCsla();

      services.AddTransient(typeof(DataAccess.IPersonDal), typeof(DataAccess.PersonDal));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseCors(myAllowOrigins);

      app.UseMvc();

      app.UseCsla();
    }
  }
}
