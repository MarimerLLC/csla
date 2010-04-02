using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class EditableChildList : 
    BusinessListBase<EditableChildList, EditableChild>
  {
    #region Factory Methods

    internal static EditableChildList NewEditableChildList()
    {
      return DataPortal.CreateChild<EditableChildList>();
    }

    internal static EditableChildList GetEditableChildList(
      object childData)
    {
      return DataPortal.FetchChild<EditableChildList>(childData);
    }

    private EditableChildList()
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
