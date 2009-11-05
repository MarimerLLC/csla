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
using Csla;
using Csla.Silverlight;

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
