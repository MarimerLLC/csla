using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Csla.Axml;

namespace ProjectTracker.AndroidUI
{
    [Activity(Label = "Role Edit")]
    public class RoleEdit : ActivityBase<ViewModels.RoleEdit, Library.Admin.RoleEdit>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.RoleEdit);
            var fraStatusContent = FindViewById<FrameLayout>(Resource.Id.fraRoleEditStatusContent);
            this.LayoutInflater.Inflate(Resource.Layout.Menu, fraStatusContent, true);

            var parameter1 = Intent.GetByteArrayExtra(Constants.EditParameter);
            var roleEditList = (Library.Admin.RoleEditList)this.DeserializeFromParameter(parameter1);
            var roleId = Intent.GetIntExtra(Constants.EditIdParameter, Constants.NewRecordId);
            if (roleId == Constants.NewRecordId)
            {
                this.viewModel = new ViewModels.RoleEdit(roleEditList);
            }
            else
            {
                var roleEdit = roleEditList.GetRoleById(roleId);
                this.viewModel = new ViewModels.RoleEdit(roleEdit);
                this.ShowDeleteButton();
            }

            var btnBack = FindViewById<Button>(Resource.Id.btnFour);
            btnBack.Text = Resources.GetString(Resource.String.ButtonBack);
            btnBack.Visibility = ViewStates.Visible;
            btnBack.Click += btnBack_Click;

            var btnOk = FindViewById<Button>(Resource.Id.btnThree);
            btnOk.Text = Resources.GetString(Resource.String.ButtonOk);
            btnOk.Visibility = ViewStates.Visible;
            btnOk.Click += btnOk_Click;

            this.Bindings.Add(Resource.Id.txtRoleName, "Text", this.viewModel.Model, "Name");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy && this.viewModel.CanDeleteObject)
            {
                this.viewModel.DeleteFromParentList();

                var roleListEditActivity = new Intent(this, typeof(RoleListEdit));
                roleListEditActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.ParentList));
                StartActivity(roleListEditActivity);
                //Toast.MakeText(this, Resources.GetString(Resource.String.MessageChangesSaved), ToastLength.Short).Show();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                this.viewModel.Model.CancelEdit();
                var roleListEditActivity = new Intent(this, typeof(RoleListEdit));
                roleListEditActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.ParentList));
                StartActivity(roleListEditActivity);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!this.viewModel.IsBusy)
            {
                Bindings.UpdateSourceForLastView();
                if (this.viewModel.Model.IsNew)
                {
                    this.viewModel.ParentList.Add(this.viewModel.Model);
                }
                this.viewModel.Model.ApplyEdit();
                var roleListEditActivity = new Intent(this, typeof(RoleListEdit));
                roleListEditActivity.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.ParentList));
                StartActivity(roleListEditActivity);
            }
        }

        private void ShowDeleteButton()
        {
            if (this.viewModel.CanDeleteObject)
            {
                var btnDelete = FindViewById<Button>(Resource.Id.btnTwo);
                btnDelete.Text = Resources.GetString(Resource.String.ButtonDelete);
                btnDelete.Visibility = ViewStates.Visible;
                btnDelete.Click += btnDelete_Click;
            }
        }
    }
}