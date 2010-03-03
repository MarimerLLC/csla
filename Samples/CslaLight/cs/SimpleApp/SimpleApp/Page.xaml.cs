using System;
using System.Windows;
using System.Windows.Controls;
using Csla;
using Csla.Xaml;

namespace SimpleApp
{
  public partial class Page : UserControl
  {
    private CslaDataProvider _provider;

    public Page()
    {
      InitializeComponent();
      _provider = this.Resources["MyData"] as CslaDataProvider;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      _provider.FactoryParameters.Clear();
      _provider.FactoryParameters.Add(DataPortal.ProxyModes.LocalOnly);
      _provider.Refresh();
    }

    private void CslaDataProvider_DataChanged(object sender, EventArgs e)
    {
      if (_provider.Error != null)
        MessageBox.Show(_provider.Error.Message, "Data error", MessageBoxButton.OK);
    }
  }
}
