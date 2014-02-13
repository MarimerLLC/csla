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
    MyViewController viewController;

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
      window = new UIWindow(UIScreen.MainScreen.Bounds);

      viewController = new MyViewController();
      window.RootViewController = viewController;

      window.MakeKeyAndVisible();

      return true;
    }
  }
}

