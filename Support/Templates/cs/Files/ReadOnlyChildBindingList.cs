using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class ReadOnlyChildBindingList :
    ReadOnlyBindingListBase<ReadOnlyChildBindingList, ReadOnlyChild>
  {
    #region Authorization Rules

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(ReadOnlyChildBindingList), "Role");
    }

    #endregion

    #region Factory Methods

    internal static ReadOnlyChildBindingList GetReadOnlyChildBindingList(object childData)
    {
      return DataPortal.FetchChild<ReadOnlyChildBindingList>(childData);
    }

    private ReadOnlyChildBindingList()
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
