using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BusinessLibrary;
using Csla;
using Xamarin.Forms;

using XamarinExample.Views;

namespace XamarinExample.ViewModels
{
  public class ItemsViewModel : ViewModel<PersonList>
  {
    public Command LoadItemsCommand { get; set; }

    public ItemsViewModel(IDataPortal<PersonList> personListPortal, IDataPortal<PersonEdit> personEditPortal)
    {
      PersonListPortal = personListPortal;
      PersonEditPortal = personEditPortal;

      Title = "Browse";

      LoadItemsCommand = 
        new Command(async () => await ExecuteLoadItemsCommand());

      MessagingCenter.Subscribe<ItemEditViewModel, PersonEdit>(this, "EditItem", async (obj, item) =>
      {
        await ExecuteLoadItemsCommand();
      });
    }

    private IDataPortal<PersonEdit> PersonEditPortal { get; set; }
    private IDataPortal<PersonList> PersonListPortal { get; set; }

    public async Task EditItemAsync(PersonInfo item)
    {
      if (item == null)
        return;
      await Navigation.PushModalAsync(
        new NavigationPage(new EditItemPage(new ItemEditViewModel(PersonEditPortal, item.Id))));
    }

    public async Task AddItemAsync()
    {
      await Navigation.PushModalAsync(
        new NavigationPage(new EditItemPage(new ItemEditViewModel(PersonEditPortal))));
    }

    async Task ExecuteLoadItemsCommand()
    {
      if (IsBusy)
        return;
      try
      {
        await RefreshAsync<PersonList>(
          () => PersonListPortal.FetchAsync());
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
      }
    }
  }
}