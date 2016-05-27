using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectTracker.Library;

using Xamarin.Forms;

namespace XamarinFormsUi
{
  public partial class App : Application
  {
    public static Page RootPage { get; private set; }
    private Dashboard startPage = new Dashboard();

    public App()
    {
      // The root page of your application
      MainPage = new NavigationPage(startPage);
      RootPage = MainPage;
    }

    protected async override void OnStart()
    {
      // Handle when your app starts

      Csla.ApplicationContext.DataPortalProxy = typeof(Csla.DataPortalClient.HttpProxy).AssemblyQualifiedName;
      Csla.ApplicationContext.DataPortalUrlString = "http://ptrackerserver.azurewebsites.net/api/DataPortal/PostAsync";

      Csla.ApplicationContext.User = new Csla.Security.UnauthenticatedPrincipal();

      await ProjectTracker.Library.Security.PTPrincipal.LoginAsync("manager", "manager");

      await RoleList.CacheListAsync();

      await startPage.InitAsync();
    }

    protected override void OnSleep()
    {
      // Handle when your app sleeps
    }

    protected override void OnResume()
    {
      // Handle when your app resumes
    }

    public static async Task NavigateTo(Type page)
    {
      var target = (Page)Activator.CreateInstance(page);
      await NavigateTo(target);
    }

    public static async Task NavigateTo(Type page, object parameter)
    {
      var target = (Page)Activator.CreateInstance(page, parameter);
      await NavigateTo(target);
    }

    private static async Task NavigateTo(Page page)
    {
      await Csla.Reflection.MethodCaller.CallMethodTryAsync(page, "InitAsync");
      await RootPage.Navigation.PushAsync(page);
    }
  }
}
