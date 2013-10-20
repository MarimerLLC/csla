using System;
using System.Security.Principal;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

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
            try
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

                        this.NavigateNext();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
                    }
                    finally
                    {
                        dialog.Dismiss();
                    }
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
            }
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.viewModel.IsBusy)
                {
                    this.NavigateNext();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
            }
        }

        private void NavigateNext()
        {
            var myIntent = new Intent(this, typeof(Welcome));

            if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
            {
                this.SetResult(Result.Ok, myIntent);
            }
            else
            {
                this.SetResult(Result.Canceled, myIntent);
            }
            this.Finish();
        }
    }
}