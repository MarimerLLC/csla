using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PTWin
{
  static class Program
  {
    /// <summary>
    /// Service provider for app
    /// </summary>
    public static ServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var serviceCollection = new ServiceCollection();
      serviceCollection.AddScoped((c) => Startup.LoadConfiguration());
      var startup = ActivatorUtilities.CreateInstance<Startup>(serviceCollection.BuildServiceProvider(), Array.Empty<object>());
      startup.ConfigureServices(serviceCollection);
      ServiceProvider = serviceCollection.BuildServiceProvider();
      startup.Configure();

      Application.Run(ActivatorUtilities.CreateInstance<MainForm>(ServiceProvider, Array.Empty<object>()));
    }
  }
}