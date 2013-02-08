using System;
using Csla.Xaml;
using System.Windows;
using Csla;
using System.ComponentModel;

namespace SimpleApp
{
  public class CustomerViewModel : ViewModel<Library.CustomerEdit>
  {
    protected async override System.Threading.Tasks.Task<Library.CustomerEdit> DoInitAsync()
    {
      var result = await Library.CustomerEdit.GetCustomerEditAsync(1234);
      IsBusy = false;
      return result;
    }

    public CustomerViewModel()
    {
      IsBusy = true;
      //if (!DesignerProperties.IsInDesignTool)
      //  BeginRefresh("GetCustomerEdit", 1234); //BeginRefresh("NewCustomerEdit");
    }

    public CustomerViewModel(int id)
    {
      BeginRefresh("GetCustomerEdit", id);
    }

    public async override void Save(object sender, ExecuteEventArgs e)
    {
      //base.Save(sender, e);
      IsBusy = true;
      Model.ApplyEdit();
      Model = await Model.SaveAsync();
      IsBusy = false;
    }

    protected override void OnError(Exception error)
    {
      MessageBox.Show(error.Message);
    }

    protected override void OnRefreshed()
    {
      base.OnRefreshed();
    }
  }
}
