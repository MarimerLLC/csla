using BusinessLibrary;
using Csla;

namespace XamarinExample.ViewModels
{
  public class ItemDetailViewModel : ViewModel<PersonInfo>
  {
    public ItemDetailViewModel(PersonInfo item = null)
    {
      if (item == null)
        item = DataPortal.Create<PersonInfo>();
      Model = item;
    }
  }
}
