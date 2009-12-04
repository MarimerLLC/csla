//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Sample.Business;

namespace Sample
{
  public partial class App : Application
  {

    public App()
    {
      this.Startup += new StartupEventHandler(Application_Startup);
      this.Exit += new System.EventHandler(Application_Exit);
      this.UnhandledException += new EventHandler<ApplicationUnhandledExceptionEventArgs>(Application_UnhandledException);
      InitializeComponent();
    }

    private void Application_Startup(object o, StartupEventArgs e)
    {
      this.RootVisual = new Page();
    }



    private void Application_Exit(object o, EventArgs e)
    {

    }

    private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
    {

      // If the app is running outside of the debugger then report the exception using
      // the browser's exception mechanism. On IE this will display it a yellow alert 
      // icon in the status bar and Firefox will display a script error.
      if (!System.Diagnostics.Debugger.IsAttached)
      {

        // NOTE: This will allow the application to continue running after an exception has been thrown
        // but not handled. 
        // For production applications this error handling should be replaced with something that will 
        // report the error to the website and stop the application.
        e.Handled = true;

        try
        {
          string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
          errorMsg = errorMsg.Replace('\"', '\\').Replace("\\r\\n", "\\n");

          System.Windows.Browser.HtmlPage.Window.Eval("throw New Error(\"Unhandled Error in Silverlight 2 Application " + errorMsg + "\");");
        }
        catch
        {

        }
      }
    }

  }

} //end of root namespace