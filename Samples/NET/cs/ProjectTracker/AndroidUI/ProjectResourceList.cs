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
    [Activity(Label = "Project Resource List")]
    public class ProjectResourceList : ActivityBase<ProjectResourceEditList, Library.ProjectResources>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.ProjectResourceListEdit);

            var fraStatusContent = FindViewById<FrameLayout>(Resource.Id.fraProjectResourceListContent);

            this.viewModel = new ProjectResourceEditList();

            this.LayoutInflater.Inflate(Resource.Layout.Menu, fraStatusContent, true);

            var btnCancel = FindViewById<Button>(Resource.Id.btnFour);
            btnCancel.Text = Resources.GetString(Resource.String.ButtonCancel);
            btnCancel.Visibility = ViewStates.Visible;
            btnCancel.Click += btnCancel_Click;

            var btnOk = FindViewById<Button>(Resource.Id.btnThree);
            btnOk.Text = Resources.GetString(Resource.String.ButtonOk);
            btnOk.Visibility = ViewStates.Visible;
            btnOk.Click += btnOk_Click;

            if (this.viewModel.CanCreateObject)
            {
                var btnAddNew = FindViewById<Button>(Resource.Id.btnTwo);
                btnAddNew.Text = Resources.GetString(Resource.String.ButtonAddNew);
                btnAddNew.Visibility = ViewStates.Visible;
                btnAddNew.Click += btnAddNew_Click;
            }

            try
            {
                var parameter1 = Intent.GetByteArrayExtra(Constants.EditParameter);
                if (parameter1 != null)
                {
                    var projectEdit = (Library.ProjectEdit)this.DeserializeFromParameter(parameter1);
                    this.viewModel.LoadFromExisting(projectEdit.Resources);
                }
                else
                {
                    throw new NotImplementedException();
                }

                var projectResourceList = FindViewById<ListView>(Resource.Id.lstProjectResources);

                var listAdapter = new Adapters.ProjectResourceListAdapter(this, this.viewModel.Model);
                projectResourceList.Adapter = listAdapter;
                projectResourceList.ItemClick += lstProjectResources_OnListItemClick;

            }
            catch (Exception ex)
            {
                ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName);
            }
        }

        protected void lstProjectResources_OnListItemClick(object o, AdapterView.ItemClickEventArgs e)
        {
            if (!this.viewModel.IsBusy && this.viewModel.CanEditObject)
            {
                try
                {
                    var projectResourceEditActivity = new Intent(this, typeof(ProjectResourceEdit));
                    projectResourceEditActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.Root));
                    projectResourceEditActivity.PutExtra(Constants.EditIdParameter, (int)e.Id);
                    StartActivity(projectResourceEditActivity);
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
                    var projectResourceAddActivity = new Intent(this, typeof(ProjectResourceAdd));
                    projectResourceAddActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.Root));
                    StartActivity(projectResourceAddActivity);
                }
                catch (Exception ex)
                {
                    ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName + Csla.DataPortalClient.WcfProxy.DefaultUrl);
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                var projectEditActivity = new Intent(this, typeof(ProjectEdit));
                this.viewModel.ApplyEdit();
                projectEditActivity.PutExtra(Constants.EditIdParameter, this.viewModel.Root.Id);
                projectEditActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.Root));
                StartActivity(projectEditActivity);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                var projectEditActivity = new Intent(this, typeof(ProjectEdit));
                this.viewModel.CancelEdit();
                projectEditActivity.PutExtra(Constants.EditIdParameter, this.viewModel.Root.Id);
                projectEditActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.Root));
                StartActivity(projectEditActivity);
            }
        }
    }
}