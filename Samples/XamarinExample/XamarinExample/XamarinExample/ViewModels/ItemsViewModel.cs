using System;
using System.Collections.ObjectModel;
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

    public ItemsViewModel()
    {
      Title = "Browse";

      LoadItemsCommand = 
        new Command(async () => await ExecuteLoadItemsCommand());

      MessagingCenter.Subscribe<NewItemPage, PersonEdit>(this, "AddItem", async (obj, item) =>
      {
        await ExecuteLoadItemsCommand();
      });
      MessagingCenter.Subscribe<EditItemPage, PersonEdit>(this, "EditItem", async (obj, item) =>
      {
        await ExecuteLoadItemsCommand();
      });

      //LoadItemsCommand.Execute(null);
    }

    async Task ExecuteLoadItemsCommand()
    {
      if (IsBusy)
        return;
      try
      {
        await RefreshAsync<PersonList>(
          () => DataPortal.FetchAsync<PersonList>());
        //OnPropertyChanged(nameof(Model));
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
      }
    }
  }
}