using Csla.Configuration;
using Csla.Server;
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

    private static ApplicationContext _applicationContext;

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
      services.AddTransient<IDataPortalServer, SimpleDataPortal>();

      serviceProvider = services.BuildServiceProvider();

      // Make the service provider available to CSLA
      // CslaConfiguration.Configure().ServiceProviderScope(serviceProvider);
      // CslaConfiguration.Configure().DataPortal().DefaultProxy(typeof(DataPortalClient.LocalProxy), "");

      // Initialise CSLA security
      contextManager = new Core.ApplicationContextManagerStatic();
      contextManager.SetDefaultServiceProvider(serviceProvider);
      genericPrincipal = new GenericPrincipal(new GenericIdentity("Fred"), new string[] { "Users" });
      contextManager.SetUser(genericPrincipal);
      _applicationContext = new ApplicationContext(contextManager, serviceProvider);

    }

    public static ApplicationContext ApplicationContext => _applicationContext;

  }
}
