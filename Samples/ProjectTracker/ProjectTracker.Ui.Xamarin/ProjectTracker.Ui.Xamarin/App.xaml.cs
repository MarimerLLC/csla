using ProjectTracker.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ProjectTracker.Ui.Xamarin
{
	public partial class App : Application
	{
    public static Page RootPage { get; private set; }
    private XamarinFormsUi.Dashboard startPage = new XamarinFormsUi.Dashboard();

    public App ()
		{
			InitializeComponent();

      //MainPage = new ProjectTracker.Ui.Xamarin.MainPage();
      MainPage = new NavigationPage(startPage);
      RootPage = MainPage;
    }

    protected override async void OnStart ()
		{
      // Handle when your app starts
      Csla.ApplicationContext.DataPortalProxy = typeof(Csla.DataPortalClient.HttpProxy).AssemblyQualifiedName;
      Csla.ApplicationContext.DataPortalUrlString = "http://ptrackerserver.azurewebsites.net/api/DataPortal/PostAsync";

      Library.Security.PTPrincipal.Logout();
      await ProjectTracker.Library.Security.PTPrincipal.LoginAsync("manager", "manager");

      await RoleList.CacheListAsync();

      await startPage.InitAsync();
    }

    protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
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
