using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Csla.Configuration;

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
      CslaConfiguration.Configure()
        .PropertyChangedMode(Csla.ApplicationContext.PropertyChangedModes.Windows);

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1());
    }
  }
}
