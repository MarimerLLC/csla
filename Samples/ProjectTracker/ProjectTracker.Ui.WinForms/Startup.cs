using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PTWin
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public static IConfiguration LoadConfiguration()
    {
      return new ConfigurationBuilder().AddInMemoryCollection().Build();
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
    }

    // This method gets called by the runtime. Use this method to configure the app.
    public void Configure()
    {
      CslaConfiguration.Configure().
        PropertyChangedMode(Csla.ApplicationContext.PropertyChangedModes.Windows).
        DataPortal().DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), "http://localhost:8040/api/dataportal/");

      //http://ptrackerserver.azurewebsites.net/api/dataportal"
    }
  }
}
