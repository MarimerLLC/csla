using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class EditableRootBindingList :
    BusinessBindingListBase<EditableRootBindingList, EditableChild>
  {
    #region Authorization Rules

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(EditableRootBindingList), "Role");
    }

    #endregion

    #region Factory Methods

    public static EditableRootBindingList NewEditableRootBindingList()
    {
      return DataPortal.Create<EditableRootBindingList>();
    }

    public static EditableRootBindingList GetEditableRootBindingList(int id)
    {
      return DataPortal.Fetch<EditableRootBindingList>(id);
    }

    private EditableRootBindingList()
    { /* Require use of factory methods */ }

    #endregion

    #region Data Access

    private void DataPortal_Fetch(int criteria)
    {
      RaiseListChangedEvents = false;
      // TODO: load values into memory
      object childData = null;
      foreach (var item in (List<object>)childData)
        this.Add(EditableChild.GetEditableChild(childData));
      RaiseListChangedEvents = true;
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      // TODO: open database, update values
      //base.Child_Update();
    }

    #endregion
  }
}
