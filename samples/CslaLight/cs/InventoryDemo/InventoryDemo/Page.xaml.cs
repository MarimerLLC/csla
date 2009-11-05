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
using Csla.Silverlight;

namespace InventoryDemo
{
  public partial class Page : UserControl
  {
    public Page()
    {
      InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      Navigator.Current.ContentPlaceholder = this.PlaceHolder;
      Navigator.Current.ProcessInitialNavigation();
    }
  }
}
