using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Csla.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProjectTracker.Configuration;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using System.Linq;

namespace ProjectTracker.AppServerCore
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    private const string BlazorClientPolicy = "AllowAllOrigins";

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddResponseCompression(options =>

      {

        options.Providers.Add<GzipCompressionProvider>();
        options.Providers.Add<BrotliCompressionProvider>();
        options.MimeTypes =
            ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/*", "text/*" });
      });
      services.Configure<GzipCompressionProviderOptions>(options =>
      {
        options.Level = CompressionLevel.Fastest;
      });
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

      services.AddDalMock();
      //services.AddDalEfCore();
      services.AddCsla();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseResponseCompression();
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }

      app.UseCors(BlazorClientPolicy); // must be before app.UseMvc()

      app.UseHttpsRedirection();
      app.UseMvc();

      app.UseCsla();
    }
  }
}
