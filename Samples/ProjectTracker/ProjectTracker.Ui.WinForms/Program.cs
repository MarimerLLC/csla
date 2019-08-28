using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace PTWin
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      // basically a Windows Forms "app builder" implementation
      var serviceCollection = new ServiceCollection();
      serviceCollection.AddScoped((c) => 
        Startup.LoadAppConfiguration(Array.Empty<string>()));
      var startup = ActivatorUtilities.CreateInstance<Startup>(
        serviceCollection.BuildServiceProvider(), Array.Empty<object>());
      startup.ConfigureServices(serviceCollection);
      var provider = serviceCollection.BuildServiceProvider();
      startup.Configure();

      // run the start form, creating it with DI support
      Application.Run(
        ActivatorUtilities.CreateInstance<MainForm>(provider, Array.Empty<object>()));
    }
  }
}