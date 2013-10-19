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
                var projectResourceListActivity = new Intent(this, typeof(ProjectResourceList));
                projectResourceListActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.Root));
                StartActivity(projectResourceListActivity);
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
                    var projectResourceEditActivity = new Intent(this, typeof(ProjectResourceEdit));
                    projectResourceEditActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.Root));
                    projectResourceEditActivity.PutExtra(Constants.EditIdParameter, newProjectResourceEdit.ResourceId);
                    StartActivity(projectResourceEditActivity);
                }
                catch (Exception ex)
                {
                    ProgressDialog.Show(this, "Error", ex.Message + Csla.DataPortal.ProxyTypeName + Csla.DataPortalClient.WcfProxy.DefaultUrl);
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