using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

namespace Templates
{
  [Serializable()]
  class ReadOnlyList : 
    ReadOnlyListBase<ReadOnlyList, ReadOnlyChild>
  {
    #region Authorization Rules

    public static bool CanGetObject()
    {
      // TODO: customize to check user role
      //return ApplicationContext.User.IsInRole("");
      return true;
    }

    #endregion

    #region Factory Methods

    public static ReadOnlyList GetReadOnlyList(string filter)
    {
      return DataPortal.Fetch<ReadOnlyList>(new Criteria(filter));
    }

    private ReadOnlyList()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    [Serializable()]
    private class Criteria
    {
      private string _filter;
      public string Filter
      {
        get { return _filter; }
      }
      public Criteria(string filter)
      {
        _filter = filter;
      }
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      // TODO: load values
      using (SqlDataReader dr = null)
      {
        while (dr.Read())
          Add(ReadOnlyChild.GetReadOnlyChild(dr));
      }
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
