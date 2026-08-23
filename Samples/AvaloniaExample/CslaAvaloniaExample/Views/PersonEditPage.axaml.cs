using Avalonia.Controls;
using Avalonia.Interactivity;
using BusinessLibrary;
using Csla;
using CslaAvaloniaExample.ViewModels;

namespace CslaAvaloniaExample.Views;

public partial class PersonEditPage : UserControl
{
  private readonly IDataPortal<PersonEdit> _dataPortal;
  private bool _isLoaded;

  public PersonEditPage(
    PersonEditViewModel viewModel,
    IDataPortal<PersonEdit> dataPortal)
  {
    InitializeComponent();

    DataContext = viewModel;
    _dataPortal = dataPortal;

    AttachedToVisualTree += async (_, _) =>
    {
      if (_isLoaded)
        return;

      _isLoaded = true;
      await LoadNewAsync();
    };
  }

  private PersonEditViewModel ViewModel =>
    (PersonEditViewModel)DataContext!;

  private async Task LoadNewAsync()
  {
    await ViewModel.RefreshAsync<PersonEdit>(
      async () => await _dataPortal.CreateAsync());
  }

  private async void SavePerson(object? sender, RoutedEventArgs e)
  {
    if (ViewModel.Model?.IsSavable == true)
      await ViewModel.SaveAsync();
  }

  private async void NewPerson(object? sender, RoutedEventArgs e)
  {
    await LoadNewAsync();
  }
}
