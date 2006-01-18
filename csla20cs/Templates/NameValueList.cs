using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

namespace Templates
{
  [Serializable()]
  class NameValueList : NameValueListBase<int, string>
  {
    #region Factory Methods

    private static NameValueList _list;

    public static NameValueList GetNameValueList()
    {
      if (_list == null)
        _list = DataPortal.Fetch<NameValueList>(
          new Criteria(typeof(NameValueList)));
      return _list;
    }

    public static void InvalidateCache()
    {
      _list = null;
    }

    private NameValueList()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    protected override void DataPortal_Fetch(object criteria)
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      // TODO: load values
      using (SqlDataReader dr = null)
      {
        while (dr.Read())
        {
          Add(new NameValueListBase<int, string>.
            NameValuePair(dr.GetInt32(0), dr.GetString(1)));
        }
      }
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
