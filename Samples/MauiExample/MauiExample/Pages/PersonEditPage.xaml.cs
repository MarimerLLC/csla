using System.Diagnostics.CodeAnalysis;
using BusinessLibrary;
using Csla;
using MauiExample.ViewModels;

// csla:PropertyInfo walks its Path over the binding source by reflection, so it's marked
// RequiresUnreferencedCode. InitializeComponent is generated from the XAML, so the suppression
// targets it from the assembly level.
[assembly: UnconditionalSuppressMessage("Trimming", "IL2026",
  Scope = "member", Target = "M:MauiExample.Pages.PersonEditPage.InitializeComponent",
  Justification = "PropertyInfo's binding source is the PersonEdit business object. CSLA's base classes and IDataPortal<PersonEdit> preserve its members.")]

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
      await vm.RefreshAsync(async () => await _personDataPortal.CreateAsync());
    else
      await vm.RefreshAsync(async () => await _personDataPortal.FetchAsync(id));
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