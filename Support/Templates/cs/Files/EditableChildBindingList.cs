using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class EditableChildBindingList :
    BusinessBindingListBase<EditableChildBindingList, EditableChild>
  {
    [FetchChild]
    private void Fetch(object childData)
    {
      RaiseListChangedEvents = false;
      foreach (var child in (IList<object>)childData)
        this.Add(DataPortal.FetchChild<EditableChild>(child));
      RaiseListChangedEvents = true;
    }
  }
}
