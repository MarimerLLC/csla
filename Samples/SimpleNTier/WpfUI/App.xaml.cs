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
      var services = new ServiceCollection();
      services.AddTransient<HttpClient>();
      services.AddCsla(o => o
        .DataPortal(dp => dp
          .UseHttpProxy(hp => hp
            .DataPortalUrl = "https://localhost:44332/api/dataportal")));
      var provider = services.BuildServiceProvider();
      ApplicationContext = provider.GetService<ApplicationContext>();

      base.OnStartup(e);
    }
  }
}
