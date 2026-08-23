using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Csla;
using BusinessLibrary;
using CslaAvaloniaExample.ViewModels;

namespace CslaAvaloniaExample.Views;

public partial class PersonListPage : ContentPage
{

  private IDataPortal<PersonList> _personListDataPortal;

  public PersonListPage(PersonListViewModel viewModel, IDataPortal<PersonList> dataPortal)
  {
    InitializeComponent();

    _personListDataPortal = dataPortal;
    this.DataContext = viewModel;
  }

  protected override void OnNavigatedTo(NavigatedToEventArgs args)
  {
    base.OnNavigatedTo(args);
    
    _ = LoadDataAsync();
  }

  private async Task LoadDataAsync()
  {
    var vm = (PersonListViewModel)this.DataContext;
    
    if(vm.Model == null)
      await vm.RefreshAsync<PersonList>(async () => await _personListDataPortal.FetchAsync());
  }

  private async void EditPerson(object sender, RoutedEventArgs e)
  {
    var btn = (Button)sender;
    var data = (PersonInfo)this.DataContext;

    //await Navigation.PushAsync(new PersonEditPage(data.Id));
    await Navigation.PushAsync(new PersonEditPage(data, _personListDataPortal)}
}