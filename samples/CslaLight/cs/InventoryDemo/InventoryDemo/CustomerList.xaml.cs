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

namespace InventoryDemo
{
  public partial class CustomerList : UserControl, Csla.Silverlight.ISupportNavigation
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
      get { return "Customers"; }
    }

    #endregion

    public CustomerList()
    {
      InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      OnLoadCompleted();
    }

    private InvLib.CustomerInfo _selectedItem;

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // set _selectedItem
      var lb = sender as ListBox;
      if (lb != null && lb.SelectedItem != null)
        _selectedItem = lb.SelectedItem as InvLib.CustomerInfo;

      // display product view
      if (_selectedItem != null)
        DisplayCustomerView();
    }

    private void DisplayCustomerView()
    {
      try
      {
        BusyAnimation.IsRunning = true;
        InvLib.CustomerDetail.GetCustomerDetail(_selectedItem.Id, (o, ex) =>
        {
          if (ex.Error != null)
            MessageBox.Show(ex.Error.ToString(), "Data error", MessageBoxButton.OK);
          else
          {
            this.ViewEditContent.ContentTemplate = ContentArea.Resources["ViewCustomer"] as DataTemplate;
            this.ViewEditContent.Content = ex.Object;
          }
          BusyAnimation.IsRunning = false;
        });
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Data error", MessageBoxButton.OK);
      }
    }

  }
}
