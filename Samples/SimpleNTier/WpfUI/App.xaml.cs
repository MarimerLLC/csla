using System;
using System.Net.Http;
using System.Windows;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WpfUI
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public static ApplicationContext ApplicationContext { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
      try
      {
        var services = new ServiceCollection();
        services.AddTransient<HttpClient>();
        services.AddCsla(o => o
          .DataPortal(dp => dp.AddClientSideDataPortal(co => co
            .UseHttpProxy(hp => hp
              .DataPortalUrl = "https://localhost:5001/api/dataportal"))));
        var provider = services.BuildServiceProvider();
        ApplicationContext = provider.GetRequiredService<ApplicationContext>();

        base.OnStartup(e);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Startup Error: {ex.Message}\n\n{ex}", "Application Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
        Shutdown(1);
      }
    }
  }
}
