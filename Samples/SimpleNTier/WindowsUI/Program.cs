using System;
using System.Net.Http;
using System.Windows.Forms;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WindowsUI
{
  static class Program
  {
    public static Csla.ApplicationContext ApplicationContext { get; private set; }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      var services = new ServiceCollection();
      services.AddTransient<HttpClient>();
      services.AddCsla(o => o
        .DataPortal(dp => dp
          .UseHttpProxy(hp => hp
            .DataPortalUrl = "https://localhost:44332/api/dataportal")));
      var provider = services.BuildServiceProvider();
      ApplicationContext = provider.GetService<Csla.ApplicationContext>();

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1());
    }
  }
}
