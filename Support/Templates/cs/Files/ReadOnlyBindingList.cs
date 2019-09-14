using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class ReadOnlyBindingList :
    ReadOnlyBindingListBase<ReadOnlyBindingList, ReadOnlyChild>
  {
    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(ReadOnlyBindingList), "Role");
    }

    [Fetch]
    private void Fetch(string criteria)
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      // TODO: load values
      object objectData = null;
      foreach (var child in (List<object>)objectData)
        Add(DataPortal.Fetch<ReadOnlyChild>(child));
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }
  }
}
