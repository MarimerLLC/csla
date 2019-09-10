using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3
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
      services.Configure<CookiePolicyOptions>(options =>
      {
              // This lambda determines whether user consent for non-essential cookies is needed for a given request.
              options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });


      services.AddMvc(config =>
        config.ModelBinderProviders.Insert(0, new Csla.Web.Mvc.CslaModelBinderProvider(CreateInstanceAsync, CreateChild))
        ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseCookiePolicy();

      app.UseMvc();
    }

    /// <summary>
    /// Create an instance of a root object when requested
    /// by CslaModelBinder
    /// </summary>
    /// <param name="type">Type of root object to create</param>
    private async Task<object> CreateInstanceAsync(Type type)
    {
      object result;
      if (type.Equals(typeof(Pages.MyList.MyList)))
        result = await Csla.DataPortal.FetchAsync<Pages.MyList.MyList>();
      else
        result = Csla.Reflection.MethodCaller.CreateInstance(type);
      return result;
    }

    /// <summary>
    /// Create an instance of a child object when requested
    /// by CslaModelBinder
    /// </summary>
    /// <param name="parent">Reference to parent object</param>
    /// <param name="type">Type of child object to create</param>
    /// <param name="values">Values that CslaModelBinder will load into the new child object</param>
    /// <returns></returns>
    private object CreateChild(System.Collections.IList parent, Type type, Dictionary<string, string> values)
    {
      object result = null;
      if (type.Equals(typeof(Pages.MyList.MyItem)))
      {
        var list = (Pages.MyList.MyList)parent;
        var idText = values["Id"];
        int id = string.IsNullOrWhiteSpace(idText) ? -1 : int.Parse(values["Id"]);
        result = list.Where(r => r.Id == id).FirstOrDefault();
        if (result == null)
          result = Csla.Reflection.MethodCaller.CreateInstance(type);
      }
      else
      {
        result = Csla.Reflection.MethodCaller.CreateInstance(type);
      }
      return result;
    }
  }
}
