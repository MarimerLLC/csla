using System;
using Csla.Xaml;

namespace InventoryDemo.ViewModels
{
  public class ProductEditViewModel : ViewModel<InvLib.ProductEdit>
  {
    private ProductListViewModel _parent;

    public ProductEditViewModel(int productId, ProductListViewModel parent)
    {
      _parent = parent;
      BeginRefresh("GetProductEdit", productId);
    }

    protected override void OnError(Exception error)
    {
      base.OnError(error);
      System.Windows.MessageBox.Show(error.ToString(), "VM error", System.Windows.MessageBoxButton.OK);
    }

    protected override void OnSaved()
    {
      base.OnSaved();
      _parent.UpdateItem(Model);
    }
  }
}
