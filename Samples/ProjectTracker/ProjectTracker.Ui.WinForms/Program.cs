using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
      Csla.ApplicationContext.ContextManager = new ApplicationContextManager();

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainForm());
    }
  }
}