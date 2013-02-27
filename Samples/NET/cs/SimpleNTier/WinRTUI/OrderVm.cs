using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Xaml;

namespace WinRTUI
{
  public class OrderVm : ViewModelBase<BusinessLibrary.Order>
  {
    public OrderVm()
    {
      ManageObjectLifetime = true;
    }

    protected async override System.Threading.Tasks.Task<BusinessLibrary.Order> DoInitAsync()
    {
      IsBusy = true;
      var result = await BusinessLibrary.Order.GetOrderAsync(441);
      return result;
    }

    protected override void OnRefreshed()
    {
      IsBusy = false;
      base.OnRefreshed();
    }

    protected override void OnModelChanged(BusinessLibrary.Order oldValue, BusinessLibrary.Order newValue)
    {
      IsBusy = false;
      base.OnModelChanged(oldValue, newValue);
    }

    protected async override void OnError(Exception error)
    {
      IsBusy = false;
      await new Windows.UI.Popups.MessageDialog(error.Message).ShowAsync();
    }

    public async new Task SaveAsync()
    {
      IsBusy = true;
      await base.SaveAsync();
    }

    public void Cancel()
    {
      DoCancel();
    }
  }
}
