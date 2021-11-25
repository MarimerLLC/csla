using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessLibrary;
using Csla;
using Csla.Xaml;

namespace WpfExample.Pages
{
  /// <summary>
  /// Interaction logic for PersonListPage.xaml
  /// </summary>
  public partial class PersonListPage : UserControl
  {
    public PersonListPage(ViewModel<PersonList> vm, IDataPortal<PersonList> portal)
    {
      ViewModel = vm;
      _portal = portal;
      MainWindow.Instance.SetPageTitle("Person list");
      InitializeComponent();
    }

    private IDataPortal<PersonList> _portal;
    private ViewModel<PersonList> ViewModel;

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      ViewModel.Model = await _portal.FetchAsync();
      DataContext = ViewModel;
    }

    private void EditPerson(object sender, RoutedEventArgs e)
    {
      var source = (Button)sender;
      if (source.DataContext is PersonInfo info)
      {
        MainWindow.Instance.ShowPage(typeof(PersonEditPage), info);
      }
    }
  }
}
