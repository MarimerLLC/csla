using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {
    public Window1()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      CustomIdentity identity = null;
      try
      {
        var username = "Guest"; // Admin, Guest, Mary, Bob
        identity =
          Csla.DataPortal.Fetch<CustomIdentity>(
          new Csla.Security.UsernameCriteria(username, ""));
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Login error", MessageBoxButton.OK, MessageBoxImage.Exclamation);        
      }
      var principal = new CustomPrincipal(identity);
      Csla.ApplicationContext.User = principal;
      UserTextBlock.Text = Csla.ApplicationContext.User.Identity.Name;

      try
      {
        var cust = CustomerEdit.GetCustomer(123);
        var dp = Resources["Customer"] as Csla.Xaml.CslaDataProvider;
        dp.ObjectInstance = cust;
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);        
      }

      //if (User.HasPermission("Customer.City.Read"))
      //  CityPanel.Visibility = Visibility.Visible;
      //else
      //  CityPanel.Visibility = Visibility.Collapsed;
    }
  }
}
