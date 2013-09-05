using System;
using System.Security.Principal;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Csla.Axml.Binding;

namespace ProjectTracker.AndroidUI
{
    [Activity(Label = "Login")]
    public class Login : Csla.Axml.ActivityBase<ViewModels.Login, IPrincipal>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.Login);

            var fraStatusContent = FindViewById<FrameLayout>(Resource.Id.fraLoginStatusContent);

            this.viewModel = new ViewModels.Login();

            Bindings.Add(Resource.Id.txtLoginUserId, "Text", viewModel, "UserName");
            Bindings.Add(Resource.Id.txtLoginPassword, "Text", viewModel, "Password");

            this.LayoutInflater.Inflate(Resource.Layout.Menu, fraStatusContent, true);

            var btnLogin = FindViewById<Button>(Resource.Id.btnThree);
            btnLogin.Text = Resources.GetString(Resource.String.ButtonLogin);
            btnLogin.Visibility = ViewStates.Visible;
            btnLogin.Click += btnLogin_Click;

            var btnBack = FindViewById<Button>(Resource.Id.btnFour);
            btnBack.Text = Resources.GetString(Resource.String.ButtonBack);
            btnBack.Visibility = ViewStates.Visible;
            btnBack.Click += btnBack_Click;
        }

        async void btnLogin_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                var dialog = new ProgressDialog(this);
                try
                {
                    dialog.SetTitle(Resources.GetString(Resource.String.ButtonLogin));
                    dialog.SetMessage(Resources.GetString(Resource.String.MessageValidatingLogin));
                    dialog.Show();

                    Bindings.UpdateSourceForLastView();
                    await this.viewModel.LoginUserAsync();
                    NavigateNext();
                }
                catch (Exception ex)
                {
                    ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName + Csla.DataPortalClient.WcfProxy.DefaultUrl);
                    var alert = new AlertDialog.Builder(this);
                }
                finally
                {
                    dialog.Dismiss();
                }
            }
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                this.NavigateNext();
            }
        }

        private void NavigateNext()
        {
            if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
            {
                StartActivity(typeof(MainPage));
            }
            else
            {
                this.Finish();
            }
        }
    }
}