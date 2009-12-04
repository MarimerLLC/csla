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
  public partial class WarehouseList : UserControl, Csla.Silverlight.ISupportNavigation
  {
    public WarehouseList()
    {
      InitializeComponent();
    }

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
      get { return "Warehouses"; }
    }

    #endregion

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      OnLoadCompleted();
    }
  }
}
