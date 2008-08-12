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
using DataBinding.Business;
using Csla;
using Csla.Wpf;

namespace DataBinding.Test
{
  public partial class Page : UserControl
  {
    public Page()
    {
      InitializeComponent();
    }

    private void FetchComplete(object sender, DataPortalResult<CustomerList> result)
    {
      this.DataContext = result.Object;
      BindDetails();
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      BindDetails();
    }

    private void Fetch()
    {
      busy.IsRunning = true;
      CustomerList.FetchByName(null, FetchComplete);
    }

    private void BindDetails()
    {
      Customer selected = list.SelectedItem as Customer;
      details.DataContext = null;
      details.DataContext = selected;
      busy.IsRunning = false;
    }

    private void fetch_Click(object sender, RoutedEventArgs e)
    {
      Fetch();
    }

    private void btnAdmin_Click(object sender, RoutedEventArgs e)
    {
      MockPrincipal.Login("admin");
      Fetch();
    }

    private void btnUser_Click(object sender, RoutedEventArgs e)
    {
      MockPrincipal.Login("user");
      Fetch();
    }

    private void btnGuest_Click(object sender, RoutedEventArgs e)
    {
      MockPrincipal.Login("guest");
      Fetch();
    }

    private void btnLogout_Click(object sender, RoutedEventArgs e)
    {
      MockPrincipal.Logout();
      Fetch();
    }
  }
}
