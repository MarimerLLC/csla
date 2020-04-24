using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Csla.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace BlazorExample.Server
{
  public class Startup
  {
    private const string BlazorClientPolicy = "AllowAllOrigins";

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors(options =>
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
      services.AddMvc(
        (o) => o.EnableEndpointRouting = false)
        .AddNewtonsoftJson();
      services.AddResponseCompression(opts =>
      {
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                  new[] { "application/octet-stream" });
      });
      services.AddMvc((o) => o.EnableEndpointRouting = false);

      // If using Kestrel:
      services.Configure<KestrelServerOptions>(options =>
      {
        options.AllowSynchronousIO = true;
      });

      // If using IIS:
      services.Configure<IISServerOptions>(options =>
      {
        options.AllowSynchronousIO = true;
      });
      services.AddHttpContextAccessor();

      //for EF Db
      //services.AddTransient(typeof(DataAccess.IPersonDal), typeof(DataAccess.EF.PersonEFDal));
      //services.AddDbContext<DataAccess.EF.PersonDbContext>(
      //options => options.UseSqlServer("Server=servername;Database=personDB;User ID=sa; Password=pass;Trusted_Connection=True;MultipleActiveResultSets=true"));

      // for Mock Db
      services.AddTransient(typeof(DataAccess.IPersonDal), typeof(DataAccess.Mock.PersonDal));
      
      services.AddCsla();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseResponseCompression();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseWebAssemblyDebugging();
      }

      app.UseCors(BlazorClientPolicy);
      app.UseBlazorFrameworkFiles();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapFallbackToFile("index.html");
      });

      app.UseCsla();
    }
  }
}
