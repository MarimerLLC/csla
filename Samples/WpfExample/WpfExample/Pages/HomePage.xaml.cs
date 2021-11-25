using System.Security.Claims;
using System.Windows;
using System.Windows.Controls;
using Csla;

namespace WpfExample.Pages
{
  /// <summary>
  /// Interaction logic for HomePage.xaml
  /// </summary>
  public partial class HomePage : UserControl
  {
    public HomePage()
    {
      InitializeComponent();
    }

    private void PersonList(object sender, RoutedEventArgs e)
    {
      MainWindow.Instance.ShowPage(typeof(PersonListPage));
    }

    private void PersonAdd(object sender, RoutedEventArgs e)
    {
      MainWindow.Instance.ShowPage(typeof(PersonEditPage));
    }
  }
}
