using Android.App;

namespace ProjectTracker.AndroidUI
{
    public static class GlobalActivities
    {

        public static void Roles(Activity activity)
        {
            activity.StartActivity(typeof(RoleListEdit));
        }
    }
}