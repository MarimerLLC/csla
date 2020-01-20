using Csla.Configuration;
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

    public App ()
		{
			InitializeComponent();

      CslaConfiguration.Configure()
        .DataPortal()
          .DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), "https://ptrackerserver.azurewebsites.net/api/dataportal");

      Library.Security.PTPrincipal.Logout();

      MainPage = new NavigationPage(new XamarinFormsUi.Views.Dashboard());
      RootPage = MainPage;
    }

    protected override async void OnStart ()
		{
      await Library.Security.PTPrincipal.LoginAsync("manager", "manager");

      await RoleList.CacheListAsync();
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
      await RootPage.Navigation.PushAsync(page);
    }
  }
}
