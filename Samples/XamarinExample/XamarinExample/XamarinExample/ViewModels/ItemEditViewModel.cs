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
    public ItemEditViewModel()
    {
      Model = DataPortal.Create<PersonEdit>();
    }

    public ItemEditViewModel(int id)
    {
      var t = RefreshAsync<PersonEdit>(
        () => DataPortal.FetchAsync<PersonEdit>(id));
    }

    protected override void Initialize()
    {
      Title = "Edit item";
      var Navigation = App.Current.MainPage.Navigation;
      CancelCommand =
        new Command(async () => await Navigation.PopModalAsync());
      SaveCommand =
        new Command(async () =>
        {
          if (!CanSave)
            return;
          await Model.SaveAndMergeAsync();
          MessagingCenter.Send(this, "EditItem", Model);
          await Navigation.PopModalAsync();
        });
    }

    public ICommand SaveCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
  }
}
