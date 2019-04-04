using System;
using System.Collections.Generic;
using Csla;

namespace Templates
{
  [Serializable]
  public class ReadOnlyList : 
    ReadOnlyListBase<ReadOnlyList, ReadOnlyChild>
  {
    #region Authorization Rules

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowGet(typeof(ReadOnlyList), "Role");
    }

    #endregion

    #region Factory Methods

    public static ReadOnlyList GetReadOnlyList(string filter)
    {
      return DataPortal.Fetch<ReadOnlyList>(filter);
    }

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
