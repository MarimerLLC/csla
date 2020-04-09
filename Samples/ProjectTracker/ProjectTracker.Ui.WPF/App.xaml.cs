using System.Windows;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ProjectTracker.Ui.WPF
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      var builder = new HostBuilder()
         .UseCsla((config) =>
         {
           config
            .PropertyChangedMode(Csla.ApplicationContext.PropertyChangedModes.Windows)
            .DataPortal()
              .DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy),
                            "http://localhost:8040/api/dataportal/");
           //                 "https://ptrackerserver.azurewebsites.net/api/dataportal");
         })
         .ConfigureServices((hostContext, services) =>
         {
           services.AddLogging(configure => configure.AddConsole());
         }).Build();
    }
  }
}
