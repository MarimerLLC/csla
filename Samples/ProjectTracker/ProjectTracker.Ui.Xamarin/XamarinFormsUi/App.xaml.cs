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
        private Dashboard startPage = new Dashboard();

        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage(startPage);
        }

        protected async override void OnStart()
        {
            // Handle when your app starts

            Csla.ApplicationContext.DataPortalProxy = typeof(Csla.DataPortalClient.HttpProxy).AssemblyQualifiedName;
            Csla.ApplicationContext.DataPortalUrlString = "http://ptrackerserver.azurewebsites.net/api/DataPortal/PostAsync";

            Csla.ApplicationContext.User = new Csla.Security.UnauthenticatedPrincipal();

            await ProjectTracker.Library.Security.PTPrincipal.LoginAsync("manager", "manager");

            await RoleList.CacheListAsync();

            await startPage.LoadData();
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
