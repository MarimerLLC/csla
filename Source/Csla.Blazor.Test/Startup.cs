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
      ApplicationContext context;

      // Initialise DI
      var services = new ServiceCollection();

      // Add Csla
      services.AddCsla();
      serviceProvider = services.BuildServiceProvider();

      // Initialise CSLA security
      context = serviceProvider.GetRequiredService<ApplicationContext>();
      genericPrincipal = new GenericPrincipal(new GenericIdentity("Fred"), new string[] { "Users" });
      context.User = genericPrincipal;

      // Set up the workaround for accessing DI from tests
      DataPortalFactory.SetServiceProvider(serviceProvider);

    }

  }
}
