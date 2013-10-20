
using Android.App;
using Android.OS;
using ProjectTracker.Library.Security;

namespace ProjectTracker.AndroidUI
{
    [Activity(Label = "Project Tracker", MainLauncher = true, Theme = "@style/Theme.Splash", Icon = "@drawable/icon")]
    public class Splash : Activity
    {
        //int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Csla.DataPortal.ProxyTypeName = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
            Csla.DataPortalClient.WcfProxy.DefaultUrl = Resources.GetString(Resource.String.DataPortalURL);
            
            // Set the CSLA.ApplicationContext.User
            PTPrincipal.Logout();

            StartActivity(typeof(Welcome));
            this.Finish();
        }
    }
}

