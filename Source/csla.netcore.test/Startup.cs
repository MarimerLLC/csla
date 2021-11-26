//-----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Initialisation of environment for test runs</summary>
//-----------------------------------------------------------------------
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Csla.TestHelpers;

namespace Csla.Test
{

  [TestClass]
  public class Startup
  {

    [AssemblyInitialize]
    public static void Initialize(TestContext testContext)
    {
      IPrincipal genericPrincipal;
      ServiceProvider serviceProvider;
      ApplicationContext context;

      // Initialise DI
      var services = new ServiceCollection();

      // Add Csla
      services.AddSingleton<Server.Dashboard.IDashboard, Server.Dashboard.Dashboard>();
      services.AddCsla();
      serviceProvider = services.BuildServiceProvider();

      // Initialise CSLA security
      context = serviceProvider.GetRequiredService<ApplicationContext>();
      genericPrincipal = new GenericPrincipal(new GenericIdentity("Fred"), new string[] { "Users" });
      context.User = genericPrincipal;

      // Set up the workaround for accessing DI from tests
      RootServiceProvider.SetServiceProvider(serviceProvider);

    }

  }
}
