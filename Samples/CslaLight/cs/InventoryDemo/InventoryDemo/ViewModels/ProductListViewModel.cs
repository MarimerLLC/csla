using System;
using System.Collections.Generic;
using System.Windows;
using Csla.Xaml;
using System.Windows.Data;

namespace InventoryDemo.ViewModels
{
  public class ProductListViewModel : ViewModel<InvLib.ProductList>
  {
    public ProductListViewModel()
    {
      BeginRefresh("GetProductList");
    }

    protected override void OnError(Exception error)
    {
      base.OnError(error);
      System.Windows.MessageBox.Show(error.ToString(), "VM error", System.Windows.MessageBoxButton.OK);
    }

    public static readonly DependencyProperty SelectedItemProperty =
      DependencyProperty.Register("SelectedItem", typeof(InvLib.ProductInfo), typeof(ProductListViewModel), new PropertyMetadata((o, e) =>
      {
        var item = e.NewValue as InvLib.ProductInfo;
        if (item != null)
        {
          var vm = (ProductListViewModel)o;
          var view = new ProductEdit();
          ((CollectionViewSource)view.Resources["productEditViewModelViewSource"]).Source = new List<object> { new ViewModels.ProductEditViewModel(item.Id) };
          vm.EditView = view;
        }
      }));
    public InvLib.ProductInfo SelectedItem
    {
      get { return (InvLib.ProductInfo)GetValue(SelectedItemProperty); }
      set { SetValue(SelectedItemProperty, value); }
    }

    public static readonly DependencyProperty ProductEditProperty =
        DependencyProperty.Register("ProductEdit", typeof(ProductEditViewModel), typeof(ProductListViewModel), null);
    public ProductEditViewModel ProductEdit
    {
      get { return (ProductEditViewModel)GetValue(ProductEditProperty); }
      set { SetValue(ProductEditProperty, value); }
    }

    public static readonly DependencyProperty EditViewProperty =
      DependencyProperty.Register("EditView", typeof(ProductEdit), typeof(ProductListViewModel), null);
    public ProductEdit EditView
    {
      get { return (ProductEdit)GetValue(EditViewProperty); }
      set { SetValue(EditViewProperty, value); }
    }
  }
}
