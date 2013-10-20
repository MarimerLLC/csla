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
                projectResourceList.ItemClick += lstProjectResources_OnListItemClick;

                this.LoadProjectResourceList();

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
            }
        }

        private void LoadProjectResourceList()
        {
            var projectResourceList = FindViewById<ListView>(Resource.Id.lstProjectResources);
            var listAdapter = new Adapters.ProjectResourceListAdapter(this, this.viewModel.Model);
            projectResourceList.Adapter = listAdapter;
        }

        protected void lstProjectResources_OnListItemClick(object o, AdapterView.ItemClickEventArgs e)
        {
            if (!this.viewModel.IsBusy && this.viewModel.CanEditObject)
            {
                try
                {
                    this.NavigateToEditScreen((int)e.Id);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
                }
            }
        }

        private void NavigateToEditScreen(int id)
        {
            var projectResourceEditActivity = new Intent(this, typeof (ProjectResourceEdit));
            projectResourceEditActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.Root));
            projectResourceEditActivity.PutExtra(Constants.EditIdParameter, id);
            StartActivityForResult(projectResourceEditActivity, Constants.RequestCodeProjectResourceEditScreen);
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy && this.viewModel.CanCreateObject)
            {
                try
                {
                    var projectResourceAddActivity = new Intent(this, typeof(ProjectResourceAdd));
                    projectResourceAddActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.Root));
                    StartActivityForResult(projectResourceAddActivity, Constants.RequestCodeProjectResourceAddScreen);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                this.viewModel.ApplyEdit();
                this.FinishSavedActivity();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                this.viewModel.CancelEdit();
                this.FinishSavedActivity();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == Constants.RequestCodeProjectResourceEditScreen)
            {
                this.HandleEditResult(resultCode, data);
            }
            else if (requestCode == Constants.RequestCodeProjectResourceAddScreen)
            {
                this.HandleAddResult(resultCode, data);  
            }
            else
            {
                Toast.MakeText(this, string.Format("Unexpected request code, received: {0}", requestCode), ToastLength.Long).Show();
            }
        }

        private void HandleAddResult(Result resultCode, Intent data)
        {
            switch (resultCode)
            {
                case Result.Ok:
                    var projectResourceId = data.GetIntExtra(Constants.EditIdParameter, Constants.NewRecordId);
                    var parameter1 = data.GetByteArrayExtra(Constants.EditParameter);
                    if (parameter1 != null)
                    {
                        var projectEdit = (Library.ProjectEdit) this.DeserializeFromParameter(parameter1);
                        this.viewModel.LoadFromExisting(projectEdit.Resources);
                        this.LoadProjectResourceList();
                        this.NavigateToEditScreen(projectResourceId);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    break;
                case Result.Canceled:
                    break;
                default:
                    Toast.MakeText(this, string.Format("Unexpected result code, received: {0}", resultCode), ToastLength.Long).Show();
                    break;
            }
        }

        private void HandleEditResult(Result resultCode, Intent data)
        {
            switch (resultCode)
            {
                case Result.Ok:
                    var parameter1 = data.GetByteArrayExtra(Constants.EditParameter);
                    if (parameter1 != null)
                    {
                        var projectEdit = (Library.ProjectEdit) this.DeserializeFromParameter(parameter1);
                        this.viewModel.LoadFromExisting(projectEdit.Resources);
                        this.LoadProjectResourceList();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    break;
                case Result.Canceled:
                    break;
                default:
                    Toast.MakeText(this, string.Format("Unexpected result code, received: {0}", resultCode), ToastLength.Long).Show();
                    break;
            }
        }

        private void FinishSavedActivity()
        {
            var projectEditActivity = new Intent(this, typeof(ProjectEdit));
            projectEditActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.Root));
            this.SetResult(Result.Ok, projectEditActivity);
            this.Finish();
        }
    }
}