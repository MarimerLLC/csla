﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.Json;

namespace WpfUI
{
  public class Startup
  {
    /// <summary>
    /// Load app configuration via ConfigurationBuilder
    /// </summary>
    /// <returns>App configuration</returns>
    public static IConfiguration LoadAppConfiguration(string[] args)
    {
      return new ConfigurationBuilder().AddInMemoryCollection().Build();
    }

    /// <summary>
    /// Creates instance of type, 
    /// getting services via DI
    /// </summary>
    /// <param name="config">App configuration</param>
    public Startup(IConfiguration config)
    {
      Configuration = config;
    }

    public IConfiguration Configuration { get; }

    /// <summary>
    /// This method gets called by the runtime. 
    /// Use this method to add services to the container.
    /// </summary>
    /// <param name="services">Service collection</param>
    public void ConfigureServices(IServiceCollection services)
    {
    }

    /// <summary>
    /// This method gets called by the runtime. 
    /// Use this method to configure the app.
    /// </summary>
    public void Configure()
    {
      // CslaConfiguration.Configure()        .DataPortal()          .DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), "https://ptrackerserver.azurewebsites.net/api/dataportal");
      var config = new ConfigurationBuilder()
.AddJsonFile("appsettings.json")
.Build()
.ConfigureCsla();
      //CslaConfiguration.Configure().
      //  DataPortal().DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), "http://localhost:8040/api/dataportal/");
    }
  }
}
