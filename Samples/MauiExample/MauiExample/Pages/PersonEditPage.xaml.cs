using BusinessLibrary;
using Csla;
using MauiExample.ViewModels;

namespace MauiExample.Pages;

public partial class PersonEditPage : ContentPage
{
	public PersonEditPage()
	{
    BindingContext = new PersonEditViewModel();
    InitializeComponent();
  }

  public PersonEditPage(int personId)
    :this()
  {
    _personId = personId;
  }

  private bool _isLoaded;
  private int _personId;

  protected override async void OnAppearing()
  {
    if (!_isLoaded)
    {
      _isLoaded = true;
      await RefreshData(_personId);
    }
  }

  private async Task RefreshData(int id)
  {
    var vm = (PersonEditViewModel)BindingContext;
    var dp = App.ApplicationContext.GetRequiredService<IDataPortal<PersonEdit>>();
    if (id < 1)
      await vm.RefreshAsync<PersonEdit>(async () => await dp.CreateAsync());
    else
      await vm.RefreshAsync<PersonEdit>(async () => await dp.FetchAsync(id));
  }

  private async void SavePerson(object sender, EventArgs e)
  {
    var vm = (PersonEditViewModel)BindingContext;
    await vm.SaveAsync();
    await Navigation.PopModalAsync();
  }

  private async void ClosePage(object sender, EventArgs e)
  {
    await Navigation.PopModalAsync();
  }
}