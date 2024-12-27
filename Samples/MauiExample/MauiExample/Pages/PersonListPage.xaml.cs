using BusinessLibrary;
using Csla;
using MauiExample.ViewModels;

namespace MauiExample.Pages;

public partial class PersonListPage : ContentPage
{

  private IDataPortal<PersonList> _PersonListDataPortal;

  public PersonListPage(PersonListViewModel viewModel, IDataPortal<PersonList> dataPortal)
  {
    InitializeComponent();

    _PersonListDataPortal = dataPortal;
    this.BindingContext = viewModel;
  }

  protected override async void OnAppearing()
  {
    var vm = (PersonListViewModel)BindingContext;
    if (vm.Model == null)
      await RefreshData();
  }

  protected override async void OnNavigatedTo(NavigatedToEventArgs args) => await RefreshData();

  private async Task RefreshData()
  {
    var vm = (PersonListViewModel)BindingContext;
    await vm.RefreshAsync<PersonList>(async () => await _PersonListDataPortal.FetchAsync());
  }

  private async void EditPerson(object sender, EventArgs e)
  {
    var btn = (Button)sender;
    var data = (PersonInfo)btn.BindingContext;

    var pageParams = new ShellNavigationQueryParameters();

    pageParams.Add(Constants.PersonIdParameter, data.Id);
    await Shell.Current.GoToAsync(Constants.PersonEditRoute, true, pageParams);
  }
}
