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
                await this.viewModel.LoadAsync();

                var roleList = FindViewById<ListView>(Resource.Id.lstRoles);
                roleList.ItemClick += lstRoles_OnListItemClick;

                this.LoadRoleList();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
            }
        }

        private void LoadRoleList()
        {
            var roleList = FindViewById<ListView>(Resource.Id.lstRoles);

            var listAdapter = new Adapters.RoleEditListAdapter(this, this.viewModel.Model);
            roleList.Adapter = listAdapter;

            this.Bindings.Add(Resource.Id.btnThree, "Enabled", this.viewModel.Model, "IsSavable");
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
                    StartActivityForResult(roleEditActivity, Constants.RequestCodeRoleEditScreen);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
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
                    Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
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
                    StartActivityForResult(roleEditActivity, Constants.RequestCodeRoleEditScreen);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                this.Finish();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == Constants.RequestCodeRoleEditScreen)
            {
                switch (resultCode)
                {
                    case Result.Ok:
                        var parameter1 = data.GetByteArrayExtra(Constants.EditParameter);
                        if (parameter1 != null)
                        {
                            var roleEditList = (Library.Admin.RoleEditList)this.DeserializeFromParameter(parameter1);
                            this.viewModel.LoadFromExisting(roleEditList);
                            this.LoadRoleList();
                        }
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