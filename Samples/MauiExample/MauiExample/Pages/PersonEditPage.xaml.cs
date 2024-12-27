using BusinessLibrary;
using Csla;
using MauiExample.ViewModels;

namespace MauiExample.Pages;

public partial class PersonEditPage : ContentPage, IQueryAttributable
{
  private IDataPortal<PersonEdit>  _personDataPortal;

	public PersonEditPage(PersonEditViewModel viewModel, IDataPortal<PersonEdit> dataPortal)
	{
    InitializeComponent();

    BindingContext = viewModel;
    _personDataPortal = dataPortal;
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
    if (id < 1)
      await vm.RefreshAsync<PersonEdit>(async () => await _personDataPortal.CreateAsync());
    else
      await vm.RefreshAsync<PersonEdit>(async () => await _personDataPortal.FetchAsync(id));
  }

  private async void SavePerson(object sender, EventArgs e)
  {
    var vm = (PersonEditViewModel)BindingContext;
    await vm.SaveAsync();
    await Shell.Current.GoToAsync("..", true);
  }

  private async void ClosePage(object sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("..", true);
  }

  public void ApplyQueryAttributes(IDictionary<string, object> query)
  {
    if (query != null && query.ContainsKey(Constants.PersonIdParameter))
    {
      int? personId = query[Constants.PersonIdParameter] as int?;
      if (personId.HasValue)
        _personId = personId.Value;
    }
  }
}