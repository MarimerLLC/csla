using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class ReadOnlyChildBindingList :
    ReadOnlyBindingListBase<ReadOnlyChildBindingList, ReadOnlyChild>
  {
    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(ReadOnlyChildBindingList), "Role");
    }

    [FetchChild]
    private void Fetch(object childData)
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      // TODO: load values
      foreach (var child in (List<object>)childData)
        Add(DataPortal.FetchChild<ReadOnlyChild>(child));
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }
  }
}
