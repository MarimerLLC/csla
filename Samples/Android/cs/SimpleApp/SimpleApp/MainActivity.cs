using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Csla.Axml.Binding;

namespace SimpleApp
{
  [Activity(Label = "SimpleApp", MainLauncher = true, Icon = "@drawable/icon")]
  public class MainActivity : Activity
  {
    protected override async void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);

      Csla.DataPortal.ProxyTypeName = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
      Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://10.10.84.152:1993/WcfPortal.svc";
      SetContentView(Resource.Layout.Main);
      await Initialize();
    }

    bool Initialized;
    BindingManager Bindings;
    //TextView ErrorText;
    Library.CustomerEdit Model;

    private async Task Initialize()
    {
      if (Initialized) return;
      Initialized = true;

      // create binding manager
      Bindings = new BindingManager(this);

      //ErrorText = FindViewById<TextView>(Resource.Id.ErrorText);

      var saveButton = FindViewById<Button>(Resource.Id.SaveButton);
      saveButton.Click += SaveButton_Click;
      var cancelButton = FindViewById<Button>(Resource.Id.CancelButton);
      cancelButton.Click += CancelButton_Click;
      var deleteButton = FindViewById<Button>(Resource.Id.DeleteButton);
      deleteButton.Click += DeleteButton_Click;

      var dialog = new ProgressDialog(this);
      dialog.SetMessage(Resources.GetString(Resource.String.Loading));
      dialog.SetCancelable(false);
      dialog.Show();

      try
      {
        var customerEdit = await Library.CustomerEdit.GetCustomerEditAsync(1);
        InitializeBindings(customerEdit);
      }
      catch (Exception ex)
      {
        var alert = new AlertDialog.Builder(this);
        alert.SetMessage(string.Format(Resources.GetString(Resource.String.Error), ex.Message));
        alert.Show();
      }
      finally
      {
        dialog.Hide();
      }
    }

    private void InitializeBindings(Library.CustomerEdit model)
    {
      this.Model = model;
      this.Model.BeginEdit();
      this.Bindings.RemoveAll();
      Bindings.Add(Resource.Id.IdTextBox, "Text", this.Model, "Id");
      Bindings.Add(Resource.Id.NameTextBox, "Text", this.Model, "Name");
      Bindings.Add(Resource.Id.StatusTextBox, "Text", this.Model, "Status");
    }

    private async void SaveButton_Click(object sender, EventArgs ea)
    {
      var dialog = new ProgressDialog(this);
      dialog.SetMessage(Resources.GetString(Resource.String.Saving));
      dialog.SetCancelable(false);
      dialog.Show();

      try
      {
        Bindings.UpdateSourceForLastView();
        this.Model.ApplyEdit();
        var returnModel = await this.Model.SaveAsync();
        InitializeBindings(returnModel);
      }
      catch (Exception ex)
      {
        var alert = new AlertDialog.Builder(this);
        alert.SetMessage(string.Format(Resources.GetString(Resource.String.Error), ex.Message));
        alert.Show();
      }
      finally
      {
        dialog.Hide();
      }
    }

    private void CancelButton_Click(object sender, EventArgs ea)
    {
      this.Model.CancelEdit();
      this.Model.BeginEdit();
    }

    private async void DeleteButton_Click(object sender, EventArgs ea)
    {
      var dialog = new ProgressDialog(this);
      dialog.SetMessage(Resources.GetString(Resource.String.Deleting));
      dialog.SetCancelable(false);
      dialog.Show();

      try
      {
        this.Model.Delete();
        this.Model.ApplyEdit();
        var returnModel = await this.Model.SaveAsync();
        InitializeBindings(returnModel);
      }
      catch (Exception ex)
      {
        var alert = new AlertDialog.Builder(this);
        alert.SetMessage(string.Format(Resources.GetString(Resource.String.Error), ex.Message));
        alert.Show();
      }
      finally
      {
        dialog.Hide();
      }
    }
  }
}

