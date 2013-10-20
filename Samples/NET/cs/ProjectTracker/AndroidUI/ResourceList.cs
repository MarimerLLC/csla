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

            var btnBack = FindViewById<Button>(Resource.Id.btnFour);
            btnBack.Text = Resources.GetString(Resource.String.ButtonBack);
            btnBack.Visibility = ViewStates.Visible;
            btnBack.Click += btnBack_Click;

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

                var resourceList = FindViewById<ListView>(Resource.Id.lstResources);
                resourceList.ItemClick += lstResources_OnListItemClick;

                this.LoadResourceList();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
            }
        }

        private void LoadResourceList()
        {
            var resourceList = FindViewById<ListView>(Resource.Id.lstResources);
            var listAdapter = new Adapters.ResourceListAdapter(this, this.viewModel.Model);
            resourceList.Adapter = listAdapter;
        }

        protected void lstResources_OnListItemClick(object o, AdapterView.ItemClickEventArgs e)
        {
            if (!this.viewModel.IsBusy && this.viewModel.CanEdit)
            {
                try
                {
                    var resourceEditActivity = new Intent(this, typeof(ResourceEdit));
                    resourceEditActivity.PutExtra(Constants.EditIdParameter, (int)e.Id);
                    StartActivityForResult(resourceEditActivity, Constants.RequestCodeResourceEditScreen);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
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
                    StartActivityForResult(resourceEditActivity, Constants.RequestCodeResourceEditScreen);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == Constants.RequestCodeResourceEditScreen)
            {
                switch (resultCode)
                {
                    case Result.Ok:
                        await this.viewModel.LoadAsync();
                        this.LoadResourceList();
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