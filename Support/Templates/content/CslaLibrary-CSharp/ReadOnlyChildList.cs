using System;
using System.Collections.Generic;
using Csla;

namespace Company.CslaLibrary1
{
  [Serializable]
  public class ReadOnlyChildList :
    ReadOnlyListBase<ReadOnlyChildList, ReadOnlyChild>
  {
    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(ReadOnlyChildList), "Role");
    }

    [FetchChild]
    private void Fetch(object childData)
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      // TODO: load values
      var dataPortal = ApplicationContext.GetRequiredService<IChildDataPortal<ReadOnlyChild>>();
      foreach (var child in (List<object>)childData)
        Add(dataPortal.FetchChild(child));
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }
  }
}
