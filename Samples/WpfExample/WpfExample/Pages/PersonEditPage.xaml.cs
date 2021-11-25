using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BusinessLibrary;
using Csla;
using Csla.Xaml;

namespace WpfExample.Pages
{
  /// <summary>
  /// Interaction logic for PersonEditPage.xaml
  /// </summary>
  public partial class PersonEditPage : UserControl, IUseContext
  {
    public PersonEditPage(ViewModel<PersonEdit> vm, IDataPortal<PersonEdit> portal)
    {
      ViewModel = vm;
      _portal = portal;
      MainWindow.Instance.SetPageTitle("Edit person");
      InitializeComponent();
    }

    public object Context { get; set; }
    private IDataPortal<PersonEdit> _portal;
    private ViewModel<PersonEdit> ViewModel;

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      var context = Context as PersonInfo;
      ViewModel.Model = await ViewModel.RefreshAsync<PersonEdit>(async () =>
      {
        if (context == null)
          return await _portal.CreateAsync();
        else
          return await _portal.FetchAsync(context.Id);
      });
      DataContext = ViewModel;
    }

    private async void SavePerson(object sender, RoutedEventArgs e)
    {
      await ViewModel.SaveAsync();
      MainWindow.Instance.ShowPage(typeof(PersonListPage));
    }
  }
}
