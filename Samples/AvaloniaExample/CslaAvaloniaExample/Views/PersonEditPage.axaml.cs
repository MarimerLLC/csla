using System.Net.Security;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BusinessLibrary;
using Csla;
using CslaAvaloniaExample.ViewModels;

namespace CslaAvaloniaExample.Views;

public partial class PersonEditPage : ContentPage
{
  private IDataPortal<PersonEdit> _dataPortal;
  private bool _isLoaded;
  private int _personId;
  public PersonEditPage(PersonEditViewModel viewModel, IDataPortal<PersonEdit> dataPortal)
  {
    InitializeComponent();
    
    DataContext = viewModel;
    _dataPortal = dataPortal;
  }
  
  private async Task LoadDataAsync(int id)
  {
    var vm = (PersonEditViewModel)DataContext;
    var person = await _dataPortal.FetchAsync(_personId);
    if (id < 1)
      await vm.RefreshAsync<PersonEdit>(async () => await _dataPortal.CreateAsync());
    else
      await vm.RefreshAsync<PersonEdit>(async () => await _dataPortal.FetchAsync(id));
  }

  private async void SavePerson(object sender, EventArgs e)
  {
    var vm = (PersonEditViewModel)DataContext;
    await vm.SaveAsync();
    //CurrentPage = 
  }
  
  private async void ClosePage(object sender, EventArgs e)
  {
    var vm = (PersonEditViewModel)DataContext;
  }
}