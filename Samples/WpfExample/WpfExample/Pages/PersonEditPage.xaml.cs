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

    public object? Context { get; set; }
    private readonly IDataPortal<PersonEdit> _portal;
    private readonly ViewModel<PersonEdit> ViewModel;

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      var context = Context as PersonInfo;
      await ViewModel.RefreshAsync(async () =>
      {
        if (context == null)
          return await _portal.CreateAsync();
        else
          return await _portal.FetchAsync(context.Id);
      });
      DataContext = ViewModel;
      if (ViewModel.Exception is not null)
        ShowError();
    }

    private async void SavePerson(object sender, RoutedEventArgs e)
    {
      // SaveAsync doesn't throw. A failed save sets Exception, and an
      // invalid model sets only ViewModelErrorText, so check both.
      await ViewModel.SaveAsync();
      if (ViewModel.Exception is null && string.IsNullOrEmpty(ViewModel.ViewModelErrorText))
        MainWindow.Instance.ShowPage(typeof(PersonListPage));
      else
        ShowError();
    }

    private void ShowError()
    {
      MessageBox.Show(ViewModel.ViewModelErrorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }
}
