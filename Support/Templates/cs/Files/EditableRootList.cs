using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class EditableRootList : 
    BusinessListBase<EditableRootList, EditableChild>
  {
    #region Authorization Rules

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(EditableRootList), "Role");
    }

    #endregion

    #region Factory Methods

    public static EditableRootList NewEditableRootList()
    {
      return DataPortal.Create<EditableRootList>();
    }

    public static EditableRootList GetEditableRootList(int id)
    {
      return DataPortal.Fetch<EditableRootList>(id);
    }

    private EditableRootList()
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
