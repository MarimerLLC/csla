using System;
using System.Windows.Forms;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WinSortFilter
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
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      ApplicationContext = provider.GetRequiredService<Csla.ApplicationContext>();
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1());
    }

    public static Csla.ApplicationContext ApplicationContext { get; private set; }
  }
}
