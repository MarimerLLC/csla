using System;
using System.Collections.Generic;
using Csla;

namespace Company.CslaLibrary1
{
  [Serializable]
  public class EditableChildList : 
    BusinessListBase<EditableChildList, EditableChild>
  {
    [FetchChild]
    private void Fetch(object childData)
    {
      RaiseListChangedEvents = false;
      var dataPortal = ApplicationContext.GetRequiredService<IChildDataPortal<EditableChild>>();
      foreach (var child in (List<object>)childData)
        Add(dataPortal.FetchChild(child));
      RaiseListChangedEvents = true;
    }
  }
}
