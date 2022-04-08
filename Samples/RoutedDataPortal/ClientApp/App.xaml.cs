using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Windows;

namespace ClientApp
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      var services = new ServiceCollection();
      services.AddTransient<HttpClient>();

      // configure to use the custom proxy
      services.AddScoped<Csla.Channels.Http.HttpProxyOptions>();
      CustomProxy.ServerUrl = "";
      services.AddScoped(typeof(Csla.DataPortalClient.IDataPortalProxy), typeof(CustomProxy));

      services.AddCsla();
      var provider = services.BuildServiceProvider();
      ApplicationContext = provider.GetRequiredService<ApplicationContext>();
    }

    public static ApplicationContext ApplicationContext { get; private set; }
  }
}
