using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Csla.Axml;

namespace ProjectTracker.AndroidUI
{
    [Activity(Label = "Resource Edit")]
    public class ResourceEdit : ActivityBase<ViewModels.ResourceEdit, Library.ResourceEdit>
    {
        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.ResourceEdit);
            var fraStatusContent = FindViewById<FrameLayout>(Resource.Id.fraResourceEditStatusContent);
            this.LayoutInflater.Inflate(Resource.Layout.Menu, fraStatusContent, true);

            try
            {
                var resourceId = Intent.GetIntExtra(Constants.EditIdParameter, Constants.NewRecordId);
                this.viewModel = new ViewModels.ResourceEdit();
                await this.viewModel.LoadProjectAsync(resourceId);
                if (resourceId > Constants.NewRecordId)
                {
                    this.ShowDeleteButton();
                }

                var btnBack = FindViewById<Button>(Resource.Id.btnFour);
                btnBack.Text = Resources.GetString(Resource.String.ButtonBack);
                btnBack.Visibility = ViewStates.Visible;
                btnBack.Click += btnBack_Click;

                var btnOk = FindViewById<Button>(Resource.Id.btnThree);
                btnOk.Text = Resources.GetString(Resource.String.ButtonOk);
                btnOk.Visibility = ViewStates.Visible;
                btnOk.Click += btnOk_Click;

                this.RefreshBindings();
            }
            catch (Exception ex)
            {
                ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName + Csla.DataPortalClient.WcfProxy.DefaultUrl);
            }
        }

        #region Event Handlers

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy && this.viewModel.CanDeleteObject)
            {
                try
                {
                    await this.viewModel.DeleteResource();
                    Toast.MakeText(this, Resources.GetString(Resource.String.MessageChangesSaved), ToastLength.Short).Show();
                    StartActivity(typeof(ResourceList));
                }
                catch (Exception ex)
                {
                    ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName + Csla.DataPortalClient.WcfProxy.DefaultUrl);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                this.viewModel.Model.CancelEdit();
                StartActivity(typeof(ResourceList));
            }
        }

        private async void btnOk_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                try
                {
                    Bindings.UpdateSourceForLastView();

                    await this.viewModel.SaveResource();
                    this.ShowDeleteButton();
                    this.RefreshBindings();
                    Toast.MakeText(this, Resources.GetString(Resource.String.MessageChangesSaved), ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName + Csla.DataPortalClient.WcfProxy.DefaultUrl);
                }
            }
        }

        #endregion Event Handlers

        #region Methods

        private void ShowDeleteButton()
        {
            if (this.viewModel.CanDeleteObject)
            {
                var btnDelete = FindViewById<Button>(Resource.Id.btnTwo);
                btnDelete.Text = Resources.GetString(Resource.String.ButtonDelete);
                btnDelete.Visibility = ViewStates.Visible;
                btnDelete.Click += btnDelete_Click;
            }
        }

        private void RefreshBindings()
        {
            this.Bindings.RemoveAll();
            Bindings.Add(Resource.Id.txtResourceFirstName, "Text", this.viewModel.Model, "FirstName");
            Bindings.Add(Resource.Id.txtResourceLastName, "Text", this.viewModel.Model, "LastName");
        }

        #endregion Methods
    }
}