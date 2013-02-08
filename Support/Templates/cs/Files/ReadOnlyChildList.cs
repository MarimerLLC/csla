using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class ReadOnlyChildList :
    ReadOnlyListBase<ReadOnlyChildList, ReadOnlyChild>
  {
    #region Authorization Rules

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(ReadOnlyChildList), "Role");
    }

    #endregion

    #region Factory Methods

    internal static ReadOnlyChildList GetReadOnlyChildList(object childData)
    {
      return DataPortal.FetchChild<ReadOnlyChildList>(childData);
    }

    private ReadOnlyChildList()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    private void Child_Fetch(object childData)
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      // TODO: load values
      foreach (var child in (List<object>)childData)
        Add(ReadOnlyChild.GetReadOnlyChild(child));
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
