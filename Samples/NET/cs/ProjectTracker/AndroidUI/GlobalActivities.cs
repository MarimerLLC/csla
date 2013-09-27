using Android.App;

namespace ProjectTracker.AndroidUI
{
    public static class GlobalActivities
    {

        public static void Roles(Activity activity)
        {
            activity.StartActivity(typeof(RoleListEdit));
        }

        public static void Logout(Activity activity)
        {
            Csla.ApplicationContext.User = new Csla.Security.UnauthenticatedPrincipal();
            activity.StartActivity(typeof(Welcome));
        }

        public static void Login(Activity activity)
        {
            activity.StartActivity(typeof(Login));
        }
    }
}