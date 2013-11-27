using System;
using System.Security.Principal;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Csla.Axml.Binding;

namespace ProjectTracker.AndroidUI
{
    [Activity(Label = "Project Tracker")]
    public class Welcome : Csla.Axml.ActivityBase<ViewModels.Welcome, IPrincipal>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.Welcome);

            var fraStatusContent = FindViewById<FrameLayout>(Resource.Id.fraStatusContent);


            Bindings = new BindingManager(this);
            this.viewModel = new ViewModels.Welcome();

            Bindings.Add(Resource.Id.lblUser, "Text", this.viewModel, "UserName");

            this.LayoutInflater.Inflate(Resource.Layout.Menu, fraStatusContent, true);

            var btnLogin = FindViewById<Button>(Resource.Id.btnThree);
            btnLogin.Text = Resources.GetString(Resource.String.ButtonLogin);
            btnLogin.Visibility = ViewStates.Visible;
            btnLogin.Click += btnLogin_Click;

            var btnRoles = FindViewById<Button>(Resource.Id.btnFour);
            btnRoles.Text = Resources.GetString(Resource.String.ButtonRoles);
            btnRoles.Visibility = ViewStates.Visible;
            btnRoles.Click += btnRoles_Click;
        }

        void btnLogin_Click(object sender, EventArgs e)
        {
            var loginActivity = new Intent(this, typeof(Login));
            StartActivityForResult(loginActivity, Constants.RequestCodeLoginScreen);
        }

        void btnRoles_Click(object sender, EventArgs e)
        {
            GlobalActivities.Roles(this);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == Constants.RequestCodeLoginScreen)
            {
                switch (resultCode)
                {
                    case Result.Ok:
                        StartActivity(typeof(MainPage));
                        this.Finish();
                        break;
                    case Result.Canceled:
                        break;
                    default:
                        Toast.MakeText(this, string.Format("Unexpected result code, received: {0}", resultCode), ToastLength.Long).Show();
                        break;
                }
            }
            else
            {
                Toast.MakeText(this, string.Format("Unexpected request code, received: {0}", requestCode), ToastLength.Long).Show();
            }
        }
    }
}