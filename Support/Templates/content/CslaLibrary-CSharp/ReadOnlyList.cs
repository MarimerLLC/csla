using System;
using System.Collections.Generic;
using Csla;

namespace Company.CslaLibrary1
{
  [Serializable]
  public class ReadOnlyList : ReadOnlyListBase<ReadOnlyList, ReadOnlyChild>
  {
    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(ReadOnlyList), "Role");
    }

    [Fetch]
    private void Fetch(string criteria)
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      // TODO: load values
      object objectData = null;
      var dataPortal = ApplicationContext.GetRequiredService<IChildDataPortal<ReadOnlyChild>>();
      foreach (var item in (List<object>)objectData)
        Add(dataPortal.FetchChild(item));
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }
  }
}
