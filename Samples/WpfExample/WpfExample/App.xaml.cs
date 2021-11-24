using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace WpfExample
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private IHost Host { get; }

    public App()
    {
      Host = new HostBuilder()
        .ConfigureServices((hostContext, services) => services
          // register window and page types here
          .AddSingleton<MainWindow>()
          .AddTransient<Pages.HomePage>()
          .AddTransient<Pages.PersonEditPage>()
          .AddTransient<Pages.PersonListPage>()

          // add other services
          .AddTransient<DataAccess.IPersonDal, DataAccess.PersonDal>()
          .AddCsla(options => options.AddXaml())
        ).Build();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
      var window = Host.Services.GetService<MainWindow>();
      window?.Show();
    }
  }
}
