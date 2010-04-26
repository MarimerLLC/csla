using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla.Xaml;

namespace InventoryDemo
{
  public partial class ProductList : UserControl, Csla.Xaml.ISupportNavigation
  {
    #region ISupportNavigation Members

    public bool CreateBookmarkAfterLoadCompleted
    {
      get { return true; }
    }

    public event EventHandler LoadCompleted;

    protected virtual void OnLoadCompleted()
    {
      if (LoadCompleted != null)
        LoadCompleted(this, EventArgs.Empty);
    }

    public void SetParameters(string parameters)
    {
      // no parameters
    }

    public string Title
    {
      get { return "Products"; }
    }

    #endregion

    public ProductList()
    {
      InitializeComponent();
    }

    private InvLib.ProductInfo _selectedItem;

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // set _selectedItem
      var lb = sender as ListBox;
      if (lb != null && lb.SelectedItem != null)
        _selectedItem = lb.SelectedItem as InvLib.ProductInfo;
      EditButton.IsEnabled = _selectedItem != null;
      DeleteButton.IsEnabled = _selectedItem != null;

      // display product view
      if (_selectedItem != null)
        DisplayProductView();
    }

    private void NewButton_Click(object sender, RoutedEventArgs e)
    {
      DisplayProductEdit();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
      if (_selectedItem != null)
        DisplayProductEdit(_selectedItem.Id);
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
      MarkBusy(true);
      var dp = new Csla.DataPortal<InvLib.ProductEdit>();
      dp.DeleteCompleted += (o, e1) =>
        {
          MarkBusy(false);
          _selectedItem = null;
          this.ViewEditContent.Content = null;
          ((CslaDataProvider)Resources["ProductList"]).Refresh();
        };
      InvLib.ProductList.ClearCache();
      dp.BeginDelete(new Csla.SingleCriteria<InvLib.ProductEdit, int>(_selectedItem.Id));
    }

    private void DisplayProductView()
    {
      try
      {
        MarkBusy(true);
        InvLib.ProductDetail.GetProductDetail(_selectedItem.Id, (o, ex) =>
        {
          if (ex.Error != null)
            MessageBox.Show(ex.Error.ToString(), "Data error", MessageBoxButton.OK);
          else
          {
            this.ViewEditContent.ContentTemplate = ContentArea.Resources["ViewProduct"] as DataTemplate;
            this.ViewEditContent.Content = ex.Object;
          }
          MarkBusy(false);
        });
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Data error", MessageBoxButton.OK);
      }
    }

    private void DisplayProductEdit(int id)
    {
      try
      {
        MarkBusy(true);
        InvLib.ProductEdit.GetProductEdit(id, (o, ex) =>
        {
          if (ex.Error != null)
            MessageBox.Show(ex.Error.ToString(), "Data error", MessageBoxButton.OK);
          else
          {
            this.ViewEditContent.ContentTemplate = ContentArea.Resources["EditProduct"] as DataTemplate;
            this.ViewEditContent.Content = ex.Object;
          }
          MarkBusy(false);
        });
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Data error", MessageBoxButton.OK);
      }
    }

    private void DisplayProductEdit()
    {
      try
      {
        MarkBusy(true);
        InvLib.ProductEdit.NewProductEdit((o, ex) =>
        {
          if (ex.Error != null)
            MessageBox.Show(ex.Error.ToString(), "Data error", MessageBoxButton.OK);
          else
          {
            this.ViewEditContent.ContentTemplate = ContentArea.Resources["EditProduct"] as DataTemplate;
            this.ViewEditContent.Content = ex.Object;
          }
          MarkBusy(false);
        });
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Data error", MessageBoxButton.OK);
      }
    }

    private void Product_Saved(object sender, Csla.Core.SavedEventArgs e)
    {
      ((CslaDataProvider)Resources["ProductList"]).Refresh();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      OnLoadCompleted();
    }

    private void MarkBusy(bool isBusy)
    {
      BusyAnimation.IsRunning = isBusy;
      Overlay.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}
