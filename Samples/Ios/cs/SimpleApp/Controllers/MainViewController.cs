using System.ComponentModel;
using Csla.Iosui.Binding;
using Library;
using MonoTouch.UIKit;
using SimpleApp.Views;
using System;

namespace SimpleApp
{
    public class MainViewController : UIViewController
    {
        bool Initialized;
        BindingManager Bindings;
        CustomerEdit Model;

        public MainViewController()
        {
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (Initialized) return;
            Initialized = true;

            this.View = new MainView();

            // create binding manager
            Bindings = new BindingManager(this.View);

            ((MainView)this.View).btnSave.TouchUpInside += SaveButton_Click;
            ((MainView)this.View).btnCancel.TouchUpInside += CancelButton_Click;
            ((MainView)this.View).btnMarkForDelete.TouchUpInside += DeleteButton_Click;

            ((MainView)this.View).txtName.ShouldReturn += (textField) =>
            {
              textField.ResignFirstResponder();
              return true;
            };

            var waitingOverlay = new WaitingOverlay(UIScreen.MainScreen.Bounds, "Loading...");
            View.Add(waitingOverlay);

            try
            {
                this.Model = await CustomerEdit.GetCustomerEditAsync(1);
                InitializeBindings(this.Model);
            }
            catch (Exception ex)
            {
                var alert = new UIAlertView();
                alert.Message = string.Format("the following error has occurred: {0}", ex.Message);
                alert.AddButton("Close");
                alert.DismissWithClickedButtonIndex(0, false);
                alert.Show();
            }
            finally
            {
                waitingOverlay.Hide();
            }
        }

        private void InitializeBindings(Library.CustomerEdit model)
        {
            this.Model = model;
            this.Model.BeginEdit();
            this.Bindings.RemoveAll();
            Bindings.Add(((MainView)View).txtId, "Text", this.Model, "Id", BindingDirection.OneWay);
            Bindings.Add(((MainView)View).txtName, "Text", this.Model, "Name", BindingDirection.TwoWay);
            Bindings.Add(((MainView)View).txtStatus, "Text", this.Model, "Status", BindingDirection.OneWay);
        }

        private async void SaveButton_Click(object sender, EventArgs ea)
        {
            var waitingOverlay = new WaitingOverlay(UIScreen.MainScreen.Bounds, "Saving...");
            View.Add(waitingOverlay);

            Bindings.UpdateSourceForLastView();
            this.Model.ApplyEdit();
            try
            {
                //this.Model.GetBrokenRules();
                this.Model = await this.Model.SaveAsync();
                
                var alert = new UIAlertView();
                alert.Message = "Saved...";
                alert.AddButton("Close");
                alert.DismissWithClickedButtonIndex(0, false);
                alert.Show();
            }
            catch (Exception ex)
            {
                var alert = new UIAlertView();
                alert.Message = string.Format("the following error has occurred: {0}", ex.Message);
                alert.AddButton("Close");
                alert.DismissWithClickedButtonIndex(0, false);
                alert.Show();
            }
            finally
            {
                InitializeBindings(this.Model);
                waitingOverlay.Hide();
            }
        }

        private void CancelButton_Click(object sender, EventArgs ea)
        {
            this.Model.CancelEdit();
            this.Model.BeginEdit();
        }

        private async void DeleteButton_Click(object sender, EventArgs ea)
        {
            var waitingOverlay = new WaitingOverlay(UIScreen.MainScreen.Bounds, "Deleting...");
            View.Add(waitingOverlay);

            try
            {
                this.Model.Delete();
                this.Model.ApplyEdit();
                var returnModel = await this.Model.SaveAsync();
                InitializeBindings(returnModel);
            }
            catch (Exception ex)
            {
                var alert = new UIAlertView();
                alert.Message = string.Format("the following error has occurred: {0}", ex.Message);
                alert.AddButton("Close");
                alert.DismissWithClickedButtonIndex(0, false);
                alert.Show();
            }
            finally
            {
                waitingOverlay.Hide();
            }
        }
    }
}