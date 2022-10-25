using BusinessLibrary;
using Csla;
using MauiExample.ViewModels;

namespace MauiExample.Pages;

public partial class PersonListPage : ContentPage
{
  public PersonListPage()
  {
    InitializeComponent();

    this.BindingContext = new PersonListViewModel();
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
    var dp = App.ApplicationContext.GetRequiredService<IDataPortal<PersonList>>();
    await vm.RefreshAsync<PersonList>(async () => await dp.FetchAsync());
  }

  private void EditPerson(object sender, EventArgs e)
  {
    var btn = sender as Button;
    var data = btn.BindingContext as PersonInfo;
    Navigation.PushModalAsync(new PersonEditPage(data.Id));
  }
}
