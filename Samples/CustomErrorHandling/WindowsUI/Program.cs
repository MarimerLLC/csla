using System;
using System.Windows.Forms;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WindowsUI
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      var services = new ServiceCollection();
      services.AddTransient<System.Net.Http.HttpClient>();
      services.AddCsla(o => o
        .AddWindowsForms()
        .DataPortal(dpo => dpo
          .UseHttpProxy(hpo => hpo
            .DataPortalUrl = "https://localhost:44364/api/dataportal")));
      var provider = services.BuildServiceProvider();
      App.ApplicationContext = provider.GetRequiredService<Csla.ApplicationContext>();

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1());
    }
  }
}
