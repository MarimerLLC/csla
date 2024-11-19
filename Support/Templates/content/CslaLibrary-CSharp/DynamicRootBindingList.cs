using System;
using System.Collections.Generic;
using Csla;

namespace Company.CslaLibrary1
{
  [Serializable]
  public class DynamicRootBindingList :
    DynamicBindingListBase<DynamicRoot>
  {
    protected override object AddNewCore()
    {
      var dataPortal = ApplicationContext.GetRequiredService<IDataPortal<DynamicRoot>>();
      DynamicRoot item = dataPortal.Create();
      Add(item);
      return item;
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(DynamicRootBindingList), "Role");
      //AuthorizationRules.AllowEdit(typeof(DynamicRootBindingList), "Role");
    }

    [Fetch]
    private void Fetch()
    {
      // TODO: load values
      RaiseListChangedEvents = false;
      object listData = null;
      var dataPortal = ApplicationContext.GetRequiredService<IDataPortal<DynamicRoot>>();
      foreach (var item in (List<object>)listData)
        Add(dataPortal.Fetch(item));
      RaiseListChangedEvents = true;
    }
  }
}