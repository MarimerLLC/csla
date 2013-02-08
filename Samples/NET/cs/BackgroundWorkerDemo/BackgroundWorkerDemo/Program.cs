using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BackgroundWorkerDemo.Security;

namespace BackgroundWorkerDemo
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

      // set up a custom principal 
      Csla.ApplicationContext.User = new MyPrincipal();
      Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("fr");
  
      // set values into ApplicationContext 
      Csla.ApplicationContext.ClientContext["TEST"] = "Client Context variable TEST";
      Csla.ApplicationContext.GlobalContext["GLOBAL1"] = "Global Context Variable GLOBAL1";

      Application.Run(new BackgroundWorkerForm());
    }
  }
}
