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

namespace Csla.Blazor.Test
{

  [TestClass]
  public class Startup
  {

    [AssemblyInitialize]
    public static void Initialize(TestContext testContext)
    {
      IPrincipal genericPrincipal;
      ServiceProvider serviceProvider;
      Core.ApplicationContextManagerStatic contextManager;

      // Initialise DI
      var services = new ServiceCollection();

      // Add Csla
      services.AddCsla();
      services.AddSingleton<Core.IContextManager, Core.ApplicationContextManagerStatic>();
      serviceProvider = services.BuildServiceProvider();

      // Initialise CSLA security
      contextManager = (Core.ApplicationContextManagerStatic)serviceProvider.GetRequiredService<Core.IContextManager>();
      contextManager.SetDefaultServiceProvider(serviceProvider);
      genericPrincipal = new GenericPrincipal(new GenericIdentity("Fred"), new string[] { "Users" });
      contextManager.SetUser(genericPrincipal);

      // Set up the data portal factory for tests
      DataPortalFactory.SetServiceProvider(serviceProvider);

    }

  }
}
