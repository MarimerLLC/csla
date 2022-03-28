using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessRuleDemo
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
      App.ApplicationContext = provider.GetRequiredService<Csla.ApplicationContext>();

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1());
    }
  }
}
