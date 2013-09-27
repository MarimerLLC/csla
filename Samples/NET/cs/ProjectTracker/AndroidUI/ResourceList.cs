using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Csla.Axml;
using ProjectTracker.AndroidUI.ViewModels;

namespace ProjectTracker.AndroidUI
{
    [Activity(Label = "Resource List Edit")]
    public class ResourceList : ActivityBase<ResourceEditList, Library.ResourceList>
    {
        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.ResourceListEdit);

            var fraStatusContent = FindViewById<FrameLayout>(Resource.Id.fraResourceListContent);

            this.viewModel = new ResourceEditList();

            this.LayoutInflater.Inflate(Resource.Layout.Menu, fraStatusContent, true);

            var btnCancel = FindViewById<Button>(Resource.Id.btnFour);
            btnCancel.Text = Resources.GetString(Resource.String.ButtonCancel);
            btnCancel.Visibility = ViewStates.Visible;
            btnCancel.Click += btnBack_Click;

            if (this.viewModel.CanCreateObject)
            {
                var btnAddNew = FindViewById<Button>(Resource.Id.btnThree);
                btnAddNew.Text = Resources.GetString(Resource.String.ButtonAddNew);
                btnAddNew.Visibility = ViewStates.Visible;
                btnAddNew.Click += btnAddNew_Click;
            }

            try
            {
                await this.viewModel.LoadAsync();

                var projectList = FindViewById<ListView>(Resource.Id.lstResources);

                var listAdapter = new Adapters.ResourceListAdapter(this, this.viewModel.Model);
                projectList.Adapter = listAdapter;
                projectList.ItemClick += lstResources_OnListItemClick;
            }
            catch (Exception ex)
            {
                ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName);
            }
        }

        protected void lstResources_OnListItemClick(object o, AdapterView.ItemClickEventArgs e)
        {
            if (!this.viewModel.IsBusy && this.viewModel.CanEdit)
            {
                try
                {
                    var resourceEditActivity = new Intent(this, typeof(ResourceEdit));
                    resourceEditActivity.PutExtra(Constants.EditIdParameter, (int)e.Id);
                    StartActivity(resourceEditActivity);
                }
                catch (Exception ex)
                {
                    ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName + Csla.DataPortalClient.WcfProxy.DefaultUrl);
                }
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy && this.viewModel.CanAdd)
            {
                try
                {
                    var resourceEditActivity = new Intent(this, typeof(ResourceEdit));
                    resourceEditActivity.PutExtra(Constants.EditIdParameter, Constants.NewRecordId);
                    StartActivity(resourceEditActivity);
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
                if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
                {
                    StartActivity(typeof(MainPage));
                }
                else
                {
                    StartActivity(typeof(Welcome));
                }
            }
        }
    }
}