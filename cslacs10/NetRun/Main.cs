using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;

namespace NetRun
{
	/// <summary>
	/// Summary description for Main.
	/// </summary>
	public class AppMain
	{
    [STAThread]
    static void Main(string[] args)
    {
      try
      {
        // launch the app based on the URL provided by the user
        RunAppliation(args[0]);
      }
      catch(Exception ex)
      {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendFormat("NetRun was unable to launch the application\n");
        sb.AppendFormat("{0}\n\n", args[0]);
        sb.Append(ex.ToString());
        MessageBox.Show(sb.ToString(), "Error launching application", 
          MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    #region RunApplication

    private static void RunAppliation(string appURL)
    {
      // create setup object for the new app domain 
      AppDomainSetup setupDomain = new AppDomainSetup();
      // give it a valid base path
      setupDomain.ApplicationBase = CurrentDomainPath();
      // give it a safe config file name
      setupDomain.ConfigurationFile = appURL + ".remoteconfig";

      // create new application domain 
      AppDomain newDomain = AppDomain.CreateDomain(
        GetAppName(appURL), null, setupDomain);
    
      // create launcher object in new appdomain
      Launcher launcher = 
        (Launcher)newDomain.CreateInstanceAndUnwrap(
        "NetRun", "NetRun.Launcher");

      // use launcher object from the new domain
      // to launch the remote app in that appdomain
      launcher.RunApp(appURL);
    }

    #endregion

    #region GetCurrentDomainPath

    private static string CurrentDomainPath()
    {
      // get path of current assembly
      string currentPath = Assembly.GetExecutingAssembly().CodeBase;
      // convert it to a URI for ease of use
      Uri currentURI = new Uri(currentPath);
      // get the path portion of the URI
      string currentLocalPath = currentURI.LocalPath;

      // return the full name of the path
      return new DirectoryInfo(currentLocalPath).Parent.FullName;
    }

    #endregion

    #region URL parsing functions

    public static string GetAppDirectory(string appURL)
    {

      // get the path without prog name
      Uri appURI = new System.Uri(appURL);
      string appPath = appURI.GetLeftPart(UriPartial.Path);

      for(int pos = appPath.Length - 1; pos > 0; pos--)
        if(appPath.Substring(pos, 1) == "/" || appPath.Substring(pos, 1) == @"\")
          return appPath.Substring(0, pos);
      return string.Empty;
    }

    public static string GetAppName(string appURL)
    {
      // get the prog name without path
      Uri appURI = new System.Uri(appURL);
      string appPath = appURI.GetLeftPart(UriPartial.Path);

      for(int pos = appPath.Length - 1; pos > 0; pos--)
        if(appPath.Substring(pos, 1) == "/" || appPath.Substring(pos, 1) == @"\")
          return appPath.Substring(pos + 1);
      return string.Empty;
    }

    #endregion

	}
}
