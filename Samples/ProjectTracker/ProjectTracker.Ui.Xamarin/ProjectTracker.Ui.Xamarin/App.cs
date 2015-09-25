using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ProjectTracker.Ui.Xamarin
{
  public class App : Application
  {
    public App ()
    {
      // The root page of your application
      MainPage = new ContentPage {
        Content = new StackLayout {
          VerticalOptions = LayoutOptions.Center,
          Children = {
            new Label {
              XAlign = TextAlignment.Center,
              Text = "Welcome to Xamarin Forms!"
            }
          }
        }
      };
    }

    protected async override void OnStart ()
    {
      // Handle when your app starts

      Csla.ApplicationContext.DataPortalProxy = typeof(Csla.DataPortalClient.HttpProxy).AssemblyQualifiedName;
      Csla.ApplicationContext.DataPortalUrlString = "http://cslaprojecttracker.azurewebsites.net/api/DataPortal/PostAsync"; // MVC 5 and Web API

      Csla.ApplicationContext.User = new Csla.Security.UnauthenticatedPrincipal();

      await Library.Security.PTPrincipal.LoginAsync("manager", "manager");

      try
      {
        var obj = await Csla.DataPortal.FetchAsync<Library.Dashboard>();
      }
      catch (Exception ex)
      {
        var tmp = ex;
      }
    }

    protected override void OnSleep ()
    {
      // Handle when your app sleeps
    }

    protected override void OnResume ()
    {
      // Handle when your app resumes
    }
  }
}
