using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class DynamicRootBindingList :
    DynamicBindingListBase<DynamicRoot>
  {
    protected override object AddNewCore()
    {
      DynamicRoot item = DataPortal.Create<DynamicRoot>();
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
      foreach (var item in (List<object>)listData)
        Add(DataPortal.Fetch<DynamicRoot>(item));
      RaiseListChangedEvents = true;
    }
  }
}