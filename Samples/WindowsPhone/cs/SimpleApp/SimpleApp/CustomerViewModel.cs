using System;
using System.Windows;
using Csla;
using Csla.Xaml;

namespace SimpleApp
{
  public class CustomerViewModel : Csla.Xaml.ViewModelBase<Library.CustomerEdit>
  {
    public CustomerViewModel()
    {
      BeginRefresh(c => Library.CustomerEdit.NewCustomerEdit(c));
    }

    public CustomerViewModel(int id)
    {
      BeginRefresh(c => Library.CustomerEdit.GetCustomerEdit(id, c));
    }

    public async void Save()
    {
      await base.SaveAsync();
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
