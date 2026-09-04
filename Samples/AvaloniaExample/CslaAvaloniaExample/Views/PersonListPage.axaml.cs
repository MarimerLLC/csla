using Avalonia.Controls;
using Avalonia.Interactivity;
using BusinessLibrary;
using Csla;
using CslaAvaloniaExample.ViewModels;

namespace CslaAvaloniaExample.Views;

public partial class PersonListPage : UserControl
{
  private readonly IDataPortal<PersonList> _dataPortal;
  private bool _isLoaded;

  public PersonListPage(
    PersonListViewModel viewModel,
    IDataPortal<PersonList> dataPortal)
  {
    InitializeComponent();

    DataContext = viewModel;
    _dataPortal = dataPortal;

    AttachedToVisualTree += async (_, _) =>
    {
      if (_isLoaded)
        return;

      _isLoaded = true;
      await LoadDataAsync();
    };
  }

  private PersonListViewModel ViewModel =>
    (PersonListViewModel)DataContext!;

  private async Task LoadDataAsync()
  {
    await ViewModel.RefreshAsync<PersonList>(
      async () => await _dataPortal.FetchAsync());
  }

  private async void RefreshList(object? sender, RoutedEventArgs e)
  {
    await LoadDataAsync();
  }
}
