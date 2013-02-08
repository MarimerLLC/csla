using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class EditableChildBindingList :
    BusinessBindingListBase<EditableChildBindingList, EditableChild>
  {
    #region Factory Methods

    internal static EditableChildBindingList NewEditableChildBindingList()
    {
      return DataPortal.CreateChild<EditableChildBindingList>();
    }

    internal static EditableChildBindingList GetEditableChildBindingList(
      object childData)
    {
      return DataPortal.FetchChild<EditableChildBindingList>(childData);
    }

    private EditableChildBindingList()
    { }

    #endregion

    #region Data Access

    private void Child_Fetch(object childData)
    {
      RaiseListChangedEvents = false;
      foreach (var child in (IList<object>)childData)
        this.Add(EditableChild.GetEditableChild(child));
      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
