using System;
using System.Collections.Generic;
using Csla;

namespace Company.CslaLibrary1
{
  [Serializable]
  public class EditableRootList : 
    BusinessListBase<EditableRootList, EditableChild>
  {
    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(EditableRootList), "Role");
    }

    [Fetch]
    private void Fetch(int criteria)
    {
      RaiseListChangedEvents = false;
      // TODO: load values into memory
      object childData = null;
      var dataPortal = ApplicationContext.GetRequiredService<IChildDataPortal<EditableChild>>();
      foreach (var child in (List<object>)childData)
        Add(dataPortal.FetchChild(child));
      RaiseListChangedEvents = true;
    }

    [Update]
    [Transactional(TransactionalTypes.TransactionScope)]
    private void Update()
    {
      // TODO: open database, update values
      //base.Child_Update();
    }
  }
}
