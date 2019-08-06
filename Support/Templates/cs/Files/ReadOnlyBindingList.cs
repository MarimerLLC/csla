using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class ReadOnlyBindingList :
    ReadOnlyBindingListBase<ReadOnlyBindingList, ReadOnlyChild>
  {
    #region Authorization Rules

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(ReadOnlyBindingList), "Role");
    }

    #endregion

    #region Factory Methods

    public static ReadOnlyBindingList GetReadOnlyBindingList(string filter)
    {
      return DataPortal.Fetch<ReadOnlyBindingList>(filter);
    }

    private ReadOnlyBindingList()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    private void DataPortal_Fetch(string criteria)
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      // TODO: load values
      object objectData = null;
      foreach (var child in (List<object>)objectData)
        Add(ReadOnlyChild.GetReadOnlyChild(child));
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
