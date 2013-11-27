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
            try
            {
                if (!this.viewModel.IsBusy && this.viewModel.CanDeleteObject)
                {
                    this.viewModel.DeleteFromParentList();

                    this.FinishSavedActivity();
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.viewModel.IsBusy)
                {
                    this.viewModel.Model.CancelEdit();
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
                    Bindings.UpdateSourceForLastView();
                    if (this.viewModel.Model.IsNew)
                    {
                        this.viewModel.ParentList.Add(this.viewModel.Model);
                    }
                    this.viewModel.Model.ApplyEdit();
                    this.FinishSavedActivity();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, string.Format(this.GetString(Resource.String.Error), ex.Message), ToastLength.Long).Show();
            }
        }

        private void FinishSavedActivity()
        {
            var roleListEditIntent = new Intent(this, typeof (RoleListEdit));
            roleListEditIntent.PutExtra(Constants.EditParameter, this.SerilizeModelForParameter(this.viewModel.ParentList));
            this.SetResult(Result.Ok, roleListEditIntent);
            this.Finish();
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