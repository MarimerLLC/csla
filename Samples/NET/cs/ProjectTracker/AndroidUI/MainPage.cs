using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Csla.Axml.Binding;

namespace ProjectTracker.AndroidUI
{
    [Activity(Label = "Main Page")]
    public class MainPage : Csla.Axml.ActivityBase<ViewModels.MainPage, string[]>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.MainPage);

            var fraStatusContent = FindViewById<FrameLayout>(Resource.Id.fraStatusContent);

            Bindings = new BindingManager(this);

            this.LayoutInflater.Inflate(Resource.Layout.Menu, fraStatusContent, true);

            var btnLogin = FindViewById<Button>(Resource.Id.btnThree);
            btnLogin.Text = Resources.GetString(Resource.String.Logout);
            btnLogin.Visibility = ViewStates.Visible;
            btnLogin.Click += btnLogout_Click;

            var btnRoles = FindViewById<Button>(Resource.Id.btnFour);
            btnRoles.Text = Resources.GetString(Resource.String.Roles);
            btnRoles.Visibility = ViewStates.Visible;
            btnRoles.Click += btnRoles_Click;

            var roleList = FindViewById<ListView>(Resource.Id.lstMenuItems);

            var listAdapter = new Adapters.MainMenuAdapter(this);
            roleList.Adapter = listAdapter;
            roleList.ItemClick += lstMenuItems_OnListItemClick;
        }

        private void lstMenuItems_OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                switch (e.Id)
                {
                    case Resource.String.MenuProjects:
                        StartActivity(typeof (ProjectList));
                        break;
                    case Resource.String.MenuResources:
                        StartActivity(typeof (ResourceList));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName + Csla.DataPortalClient.WcfProxy.DefaultUrl);
            }
        }

        void btnLogout_Click(object sender, EventArgs e)
        {
            GlobalActivities.Logout(this);
        }

        void btnRoles_Click(object sender, EventArgs e)
        {
            GlobalActivities.Roles(this);
        }
    }
}