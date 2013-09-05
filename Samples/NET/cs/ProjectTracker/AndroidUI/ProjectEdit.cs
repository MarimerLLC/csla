using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Csla.Axml;

namespace ProjectTracker.AndroidUI
{
    [Activity(Label = "Project Edit")]
    public class ProjectEdit :  ActivityBase<ViewModels.ProjectEdit, Library.ProjectEdit>
    {
        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.ProjectEdit);
            var fraStatusContent = FindViewById<FrameLayout>(Resource.Id.fraProjectEditStatusContent);
            this.LayoutInflater.Inflate(Resource.Layout.Menu, fraStatusContent, true);

            try
            {
                this.viewModel = new ViewModels.ProjectEdit();

                var projectId = Intent.GetIntExtra(Constants.EditIdParameter, Constants.NewRecordId);
                var parameter1 = Intent.GetByteArrayExtra(Constants.EditParameter);
                if (parameter1 != null)
                {
                    var projectEdit = (Library.ProjectEdit)this.DeserializeFromParameter(parameter1);
                    this.viewModel.LoadFromExisting(projectEdit);
                }
                else
                {
                    await this.viewModel.LoadProjectAsync(projectId);
                }
                if (projectId > Constants.NewRecordId)
                {
                    this.ShowDeleteButton();
                }

                var btnCancel = FindViewById<Button>(Resource.Id.btnFour);
                btnCancel.Text = Resources.GetString(Resource.String.ButtonCancel);
                btnCancel.Visibility = ViewStates.Visible;
                btnCancel.Click += btnCancel_Click;

                var btnSave = FindViewById<Button>(Resource.Id.btnThree);
                btnSave.Text = Resources.GetString(Resource.String.ButtonSave);
                btnSave.Visibility = ViewStates.Visible;
                btnSave.Click += btnSave_Click;

                var startDate = FindViewById<Button>(Resource.Id.btnProjectStart);
                startDate.Click += delegate { ShowDialog(Constants.StartDateDialogId); };

                var EndDate = FindViewById<Button>(Resource.Id.btnProjectEnd);
                EndDate.Click += delegate { ShowDialog(Constants.EndDateDialogId); };

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
                    await this.viewModel.DeleteProject();
                    Toast.MakeText(this, Resources.GetString(Resource.String.MessageChangesSaved), ToastLength.Short).Show();
                    StartActivity(typeof(ProjectList));

                }
                catch (Exception ex)
                {
                    ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName + Csla.DataPortalClient.WcfProxy.DefaultUrl);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                this.viewModel.Model.CancelEdit();
                StartActivity(typeof(ProjectList));
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                try
                {
                    Bindings.UpdateSourceForLastView();

                    await this.viewModel.SaveProject();
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

        private void btnResources_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy && this.viewModel.CanEditObject)
            {
                try
                {
                    var projectResourceListActivity = new Intent(this, typeof(ProjectResourceList));
                    projectResourceListActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter());
                    StartActivity(projectResourceListActivity);
                }
                catch (Exception ex)
                {
                    ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName + Csla.DataPortalClient.WcfProxy.DefaultUrl);
                }
            }
        }

        void OnStartDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            this.viewModel.Started = e.Date.ToString();
        }

        void OnEndDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            this.viewModel.Ended = e.Date.ToString();
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

                var btnResources = FindViewById<Button>(Resource.Id.btnOne);
                btnResources.Text = Resources.GetString(Resource.String.MenuResources);
                btnResources.Visibility = ViewStates.Visible;
                btnResources.Click += btnResources_Click;
            }
        }

        private void RefreshBindings()
        {
            this.Bindings.RemoveAll();
            Bindings.Add(Resource.Id.txtProjectName, "Text", viewModel.Model, "Name");
            Bindings.Add(Resource.Id.txtProjectDescription, "Text", viewModel.Model, "Description");
            Bindings.Add(Resource.Id.txtProjectStarted, "Text", viewModel, "Started");
            Bindings.Add(Resource.Id.txtProjectEnded, "Text", viewModel, "Ended");            
        }

        protected override Dialog OnCreateDialog(int id)
        {
            DateTime displayDate;
            Bindings.UpdateSourceForLastView();
            switch (id)
            {
                case Constants.StartDateDialogId:
                    displayDate = this.viewModel.Model.Started ?? DateTime.Now;
                    return new DatePickerDialog(this, OnStartDateSet, displayDate.Year, displayDate.Month - 1, displayDate.Day);
                case Constants.EndDateDialogId:
                    displayDate = this.viewModel.Model.Ended ?? DateTime.Now;
                    return new DatePickerDialog(this, OnEndDateSet, displayDate.Year, displayDate.Month - 1, displayDate.Day);
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion Methods

    }
}