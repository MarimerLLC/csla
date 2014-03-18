using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SimpleApp
{
  [Register("AppDelegate")]
  public partial class AppDelegate : UIApplicationDelegate
  {
    UIWindow window;
    MainViewController viewController;

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
      Csla.DataPortal.ProxyTypeName = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
      Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://testcslaportal.azurewebsites.net/wcfportal.svc";
        
      window = new UIWindow(UIScreen.MainScreen.Bounds);

      viewController = new MainViewController();
      window.RootViewController = viewController;

      window.MakeKeyAndVisible();

      return true;
    }
  }
}

