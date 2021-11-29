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
          .AddSingleton<MainForm>()
          .AddTransient<Pages.HomePage>()
          .AddTransient<Pages.PersonEditPage>()
          .AddTransient<Pages.PersonListPage>()

          // register other services here
          .AddTransient<DataAccess.IPersonDal, DataAccess.PersonDal>()
          .AddCsla()
      ).Build();

      var form = Host.Services.GetService<MainForm>();
      Application.Run(form);
    }
  }
}