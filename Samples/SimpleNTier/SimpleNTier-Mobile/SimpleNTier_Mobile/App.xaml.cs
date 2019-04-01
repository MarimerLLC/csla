using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace SimpleNTier_Mobile
{
  public partial class App : Application
  {
    public App()
    {
      InitializeComponent();

      MainPage = new SimpleNTier_Mobile.MainPage();
    }

    protected override void OnStart()
    {
      // Handle when your app starts
      Csla.ApplicationContext.DataPortalProxy = typeof(Csla.DataPortalClient.HttpProxy).AssemblyQualifiedName;
      Csla.ApplicationContext.DataPortalUrlString = "";
    }

    protected override void OnSleep()
    {
      // Handle when your app sleeps
    }

    protected override void OnResume()
    {
      // Handle when your app resumes
    }
  }
}
