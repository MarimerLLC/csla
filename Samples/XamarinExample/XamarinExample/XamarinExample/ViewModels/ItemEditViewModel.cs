using System;
using System.Collections.Generic;
using System.Text;
using BusinessLibrary;
using Csla;

namespace XamarinExample.ViewModels
{
  public class ItemEditViewModel : ViewModel<PersonEdit>
  {
    public ItemEditViewModel()
    {
      Title = "New item";
      Model = DataPortal.Create<PersonEdit>();
    }

    public ItemEditViewModel(int id)
    {
      Title = "Edit item";
      var t = RefreshAsync<PersonEdit>(
        () => DataPortal.FetchAsync<PersonEdit>(id));
    }
  }
}
