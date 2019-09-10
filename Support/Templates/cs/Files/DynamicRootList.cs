using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class DynamicRootList :
    DynamicListBase<DynamicRoot>
  {
    protected override DynamicRoot AddNewCore()
    {
      DynamicRoot item = DataPortal.Create<DynamicRoot>();
      Add(item);
      return item;
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(DynamicRootList), "Role");
      //AuthorizationRules.AllowEdit(typeof(DynamicRootList), "Role");
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