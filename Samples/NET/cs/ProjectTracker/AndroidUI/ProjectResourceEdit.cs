using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Csla.Axml;

namespace ProjectTracker.AndroidUI
{
    [Activity(Label = "Project Resource Edit")]
    public class ProjectResourceEdit : ActivityBase<ViewModels.ProjectResourceEdit, Library.ProjectResourceEdit>
    {
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.ProjectResourceEdit);
            var fraStatusContent = FindViewById<FrameLayout>(Resource.Id.fraProjectResourceEditStatusContent);
            this.LayoutInflater.Inflate(Resource.Layout.Menu, fraStatusContent, true);

            this.viewModel = new ViewModels.ProjectResourceEdit();
            var projectResourceId = Intent.GetIntExtra(Constants.EditIdParameter, Constants.NewRecordId);
            var parameter1 = Intent.GetByteArrayExtra(Constants.EditParameter);
            var projectEdit = (Library.ProjectEdit)this.DeserializeFromParameter(parameter1);
            this.viewModel.LoadProject(projectEdit, projectResourceId);
            this.ShowDeleteButton();

            var btnCancel = FindViewById<Button>(Resource.Id.btnFour);
            btnCancel.Text = Resources.GetString(Resource.String.ButtonCancel);
            btnCancel.Visibility = ViewStates.Visible;
            btnCancel.Click += btnCancel_Click;

            var btnOk = FindViewById<Button>(Resource.Id.btnThree);
            btnOk.Text = Resources.GetString(Resource.String.ButtonOk);
            btnOk.Visibility = ViewStates.Visible;
            btnOk.Click += btnOk_Click;

            await this.LoadControlsAsync();
            await this.RefreshBindingsAsync();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                this.viewModel.Root.Resources.Remove(this.viewModel.Model);
                this.viewModel.ApplyEdit();
                this.FinishSavedActivity();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.viewModel.IsBusy)
                {
                    this.viewModel.CancelEdit();
                    this.SetResult(Result.Canceled);
                    this.Finish();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.viewModel.IsBusy)
                {
                    this.viewModel.ApplyEdit();
                    this.FinishSavedActivity();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
            }
        }

        private void cboRole_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            this.viewModel.Model.Role = (int)e.Id;
        }

        #region Methods

        private void ShowDeleteButton()
        {
            if (this.viewModel.EditMode && this.viewModel.CanDeleteObject)
            {
                var btnDelete = FindViewById<Button>(Resource.Id.btnTwo);
                btnDelete.Text = Resources.GetString(Resource.String.ButtonDelete);
                btnDelete.Visibility = ViewStates.Visible;
                btnDelete.Click += btnDelete_Click;
            }
        }

        private async Task LoadControlsAsync()
        {
            var cboRoles = FindViewById<Spinner>(Resource.Id.cboRole);

            cboRoles.ItemSelected += cboRole_ItemSelected;
            var roleAdapter = new Adapters.NameValueListAdapter<int, string>(this, await this.viewModel.GetRolesAsync());

            cboRoles.Adapter = roleAdapter;
        }

        private async Task RefreshBindingsAsync()
        {
            this.Bindings.RemoveAll();
            Bindings.Add(Resource.Id.txtResourceAssigned, "Text", this.viewModel.Model, "Assigned");
            Bindings.Add(Resource.Id.txtResourceName, "Text", this.viewModel.Model, "FullName");

            var cboRole = FindViewById<Spinner>(Resource.Id.cboRole);
            var roles = await this.viewModel.GetRolesAsync();
            cboRole.SetSelection(roles.IndexOf(roles.Single(r => r.Key == this.viewModel.Model.Role)));
        }

        private void FinishSavedActivity()
        {
            var projectResourceListActivity = new Intent(this, typeof(ProjectResourceList));
            projectResourceListActivity.PutExtra(Constants.EditIdParameter, this.viewModel.Root.Id);
            projectResourceListActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.Root));
            this.SetResult(Result.Ok, projectResourceListActivity);
            this.Finish();
        }

        #endregion Methods
    }
}