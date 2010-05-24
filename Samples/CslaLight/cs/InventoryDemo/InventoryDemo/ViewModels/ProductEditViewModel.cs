using System;
using Csla.Xaml;

namespace InventoryDemo.ViewModels
{
  public class ProductEditViewModel : ViewModel<InvLib.ProductEdit>
  {
    public ProductEditViewModel(int productId)
    {
      BeginRefresh("GetProductEdit", productId);
    }

    protected override void OnError(Exception error)
    {
      base.OnError(error);
      System.Windows.MessageBox.Show(error.ToString(), "VM error", System.Windows.MessageBoxButton.OK);
    }
  }
}
