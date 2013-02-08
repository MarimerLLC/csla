
using System;
using System.Collections.Generic;
using System.Linq;
using Csla;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SimpleApp
{
  public class Application
  {
    static void Main (string[] args)
    {
      UIApplication.Main (args);
      Csla.DataPortal.ProxyTypeName = "Local";
    }
  }

  // The name AppDelegate is referenced in the MainWindow.xib file.
  public partial class AppDelegate : UIApplicationDelegate
  {
    private DialogViewController customerViewController;
    // This method is invoked when the application has loaded its UI and its ready to run
    public override bool FinishedLaunching (UIApplication app, NSDictionary options)
    {

      window.AddSubview (navigationController.View);
      
      Library.CustomerEdit.BeginNewCustomer (Csla.DataPortal.ProxyModes.LocalOnly, (o, e) =>
      {
        if (e.Error == null) {
          var customer = e.Object;
          // Now make sure we invoke on the main thread the updates
          navigationController.InvokeOnMainThread (delegate {
            var context = new BindingContext (this, customer, "Simple App");
            customerViewController = new DialogViewController (context.Root, true);
            // When the view goes out of screen, we fetch the data.
            customerViewController.ViewDissapearing += delegate {
              // This reflects the data back to the object instance
              context.Fetch ();
              Console.WriteLine ("Customer Name: {0}", customer.Name);
              Console.WriteLine ("Date of Birth: {0}", customer.BirthDate);
              Console.WriteLine ("Status: {0}", customer.Status);
            };
            navigationController.PushViewController (customerViewController, true);
          });
          
        }

        
        else {
          throw e.Error;
        }
        
      });
      
      window.MakeKeyAndVisible ();
      
      return true;
    }

    // This method is required in iPhoneOS 3.0
    public override void OnActivated (UIApplication application)
    {
    }
    
  }
  /*
    public override void WillTerminate (UIApplication application)
    {
      //Save data here
    }
    */  
}

