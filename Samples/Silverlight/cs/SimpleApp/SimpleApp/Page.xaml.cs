using System;
using System.Windows;
using System.Windows.Controls;
using Csla;
using Csla.Xaml;

namespace SimpleApp
{
  public partial class Page : UserControl
  {
    public Page()
    {
      InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
      var dc = new CustomerViewModel();
      this.DataContext = dc;
      await dc.InitAsync();
    }
  }
}
