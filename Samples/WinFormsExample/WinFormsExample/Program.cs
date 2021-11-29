using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WinFormsExample
{
  internal static class Program
  {
    private static IHost Host { get; set; }
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      ApplicationConfiguration.Initialize();

      Host = new HostBuilder()
        .ConfigureServices((hostContext, services) => services
          // register window and page types here
          .AddSingleton<Form1>()

          // register other services here
          .AddCsla()
      ).Build();

      var form = Host.Services.GetService<Form1>();
      Application.Run(form);
    }
  }
}