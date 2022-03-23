using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using BusinessLibrary;
using Csla;
using Xamarin.Forms;

namespace XamarinExample.ViewModels
{
  public class ItemEditViewModel : ViewModel<PersonEdit>
  {
    public ItemEditViewModel(IDataPortal<PersonEdit> portal)
      : this(-1)
    { 
      PersonPortal = portal;
    }

    private IDataPortal<PersonEdit> PersonPortal { get; set; }

    public ItemEditViewModel(int id)
    {
      Initialize();
      if (id == -1)
        RefreshAsync<PersonEdit>(() => PersonPortal.CreateAsync());
      else
        RefreshAsync<PersonEdit>(() => PersonPortal.FetchAsync(id));
    }

    protected override void Initialize()
    {
      Title = "Edit item";
      CancelCommand =
        new Command(async () => await Navigation.PopModalAsync());
      SaveCommand =
        new Command(async () =>
        {
          if (CanSave)
          {
            await Model.SaveAndMergeAsync();
            MessagingCenter.Send(this, "EditItem", Model);
          }
          else if (IsDirty)
          {
            return;
          }
          await Navigation.PopModalAsync();
        });
    }

    public ICommand SaveCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
  }
}
