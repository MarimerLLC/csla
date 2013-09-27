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
    [Activity(Label = "Role List Edit")]
    public class RoleListEdit : ActivityBase<RoleEditList, Library.Admin.RoleEditList>
    {
        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.RoleListEdit);

            var fraStatusContent = FindViewById<FrameLayout>(Resource.Id.fraViewListEditContent);

            this.viewModel = new RoleEditList();
            
            this.LayoutInflater.Inflate(Resource.Layout.Menu, fraStatusContent, true);

            var btnBack = FindViewById<Button>(Resource.Id.btnFour);
            btnBack.Text = Resources.GetString(Resource.String.ButtonBack);
            btnBack.Visibility = ViewStates.Visible;
            btnBack.Click += btnBack_Click;

            if (this.viewModel.CanCreateObject)
            {
                var btnAddNew = FindViewById<Button>(Resource.Id.btnTwo);
                btnAddNew.Text = Resources.GetString(Resource.String.ButtonAddNew);
                btnAddNew.Visibility = ViewStates.Visible;
                btnAddNew.Click += btnAddNew_Click;
            }

            if (this.viewModel.CanEditObject)
            {
                var btnSave = FindViewById<Button>(Resource.Id.btnThree);
                btnSave.Text = Resources.GetString(Resource.String.ButtonSave);
                btnSave.Visibility = ViewStates.Visible;
                btnSave.Enabled = false;
                btnSave.Click += btnSave_Click;
            }

            try
            {
                var parameter1 = Intent.GetByteArrayExtra(Constants.EditParameter);
                if (parameter1 != null)
                {
                    var roleEditList = (Library.Admin.RoleEditList)this.DeserializeFromParameter(parameter1);
                    this.viewModel.LoadFromExisting(roleEditList);
                }
                else
                {
                    await this.viewModel.LoadAsync();
                }

                var roleList = FindViewById<ListView>(Resource.Id.lstRoles);

                var listAdapter = new Adapters.RoleEditListAdapter(this, this.viewModel.Model);
                roleList.Adapter = listAdapter;
                roleList.ItemClick += lstRoles_OnListItemClick;

                this.Bindings.Add(Resource.Id.btnThree, "Enabled", this.viewModel.Model, "IsSavable");
            }
            catch (Exception ex)
            {
                ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName);
            }
        }

        protected void lstRoles_OnListItemClick(object o, AdapterView.ItemClickEventArgs e)
        {
            if (!this.viewModel.IsBusy && this.viewModel.CanEditObject)
            {
                try
                {
                    var roleEditActivity = new Intent(this, typeof (RoleEdit));
                    roleEditActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter());
                    roleEditActivity.PutExtra(Constants.EditIdParameter, (int) e.Id);
                    StartActivity(roleEditActivity);
                }
                catch (Exception ex)
                {
                    ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName + Csla.DataPortalClient.WcfProxy.DefaultUrl);
                }
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy && this.viewModel.CanEditObject)
            {
                try
                {
                    await this.viewModel.SaveRoles();

                    var roleList = FindViewById<ListView>(Resource.Id.lstRoles);
                    var listAdapter = new Adapters.RoleEditListAdapter(this, this.viewModel.Model);
                    roleList.Adapter = listAdapter;

                    this.Bindings.RemoveAll();
                    this.Bindings.Add(Resource.Id.btnThree, "Enabled", this.viewModel.Model, "IsSavable");
                    Toast.MakeText(this, Resources.GetString(Resource.String.MessageChangesSaved), ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName + Csla.DataPortalClient.WcfProxy.DefaultUrl);
                }
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy && this.viewModel.CanCreateObject)
            {
                try
                {
                    var roleEditActivity = new Intent(this, typeof (RoleEdit));
                    roleEditActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter());
                    roleEditActivity.PutExtra(Constants.EditIdParameter, -1);
                    StartActivity(roleEditActivity);
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
                    StartActivity(typeof (MainPage));
                }
                else
                {
                    StartActivity(typeof (Welcome));
                }
            }
        }
    }
}