using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Csla.Axml;

namespace ProjectTracker.AndroidUI
{
    [Activity(Label = "Project Resource Add")]
    public class ProjectResourceAdd : ActivityBase<ViewModels.ProjectResourceAdd, Library.ProjectResourceEditCreator>
    {
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.ProjectResourceAdd);
            var fraStatusContent = FindViewById<FrameLayout>(Resource.Id.fraProjectResourceAddStatusContent);
            this.LayoutInflater.Inflate(Resource.Layout.Menu, fraStatusContent, true);

            this.viewModel = new ViewModels.ProjectResourceAdd();

            var parameter1 = Intent.GetByteArrayExtra(Constants.EditParameter);
            var projectEdit = (Library.ProjectEdit)this.DeserializeFromParameter(parameter1);
            this.viewModel.LoadProject(projectEdit);

            var btnCancel = FindViewById<Button>(Resource.Id.btnFour);
            btnCancel.Text = Resources.GetString(Resource.String.ButtonCancel);
            btnCancel.Visibility = ViewStates.Visible;
            btnCancel.Click += btnCancel_Click;

            var btnOk = FindViewById<Button>(Resource.Id.btnThree);
            btnOk.Text = Resources.GetString(Resource.String.ButtonOk);
            btnOk.Visibility = ViewStates.Visible;
            btnOk.Click += btnOk_Click;

            await this.LoadControlsAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                this.viewModel.CancelEdit();
                this.SetResult(Result.Canceled);
                this.Finish();
            }
        }

        private async void btnOk_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                try
                {
                    Bindings.UpdateSourceForLastView();

                    var newProjectResourceEdit = await this.viewModel.CreateProjectResourceAsync();
                    this.viewModel.ApplyEdit();
                    var projectResourceListActivity = new Intent(this, typeof(ProjectResourceList));
                    projectResourceListActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.Root));
                    projectResourceListActivity.PutExtra(Constants.EditIdParameter, newProjectResourceEdit.ResourceId);
                    this.SetResult(Result.Ok, projectResourceListActivity);
                    this.Finish();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
                }
            }
        }

        private void cboProjectAddResource_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            this.viewModel.ResourceId = (int)e.Id;
        }

        #region Methods

        private async Task LoadControlsAsync()
        {
            var cboProjectAddResource = FindViewById<Spinner>(Resource.Id.cboProjectAddResource);

            cboProjectAddResource.ItemSelected += cboProjectAddResource_ItemSelected;
            var resourceAdapter = new Adapters.ResourceListAdapter(this, await this.viewModel.GetResourcesAsync());

            cboProjectAddResource.Adapter = resourceAdapter;
        }

        #endregion Methods
    }
}