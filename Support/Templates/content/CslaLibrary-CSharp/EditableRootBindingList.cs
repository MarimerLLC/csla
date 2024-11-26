using System;
using System.Collections.Generic;
using Csla;

namespace Company.CslaLibrary1
{
  [Serializable]
  public class EditableRootBindingList :
    BusinessBindingListBase<EditableRootBindingList, EditableChild>
  {
    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(EditableRootBindingList), "Role");
    }

    [Fetch]
    private void Fetch(int criteria)
    {
      RaiseListChangedEvents = false;
      // TODO: load values into memory
      object childData = null;
      var dataPortal = ApplicationContext.GetRequiredService<IChildDataPortal<EditableChild>>();
      foreach (var item in (List<object>)childData)
        Add(dataPortal.FetchChild(item));
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
