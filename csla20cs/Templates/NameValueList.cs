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

    public static NameValueList GetList()
    {
      return DataPortal.Fetch<NameValueList>(
        new Criteria(typeof(NameValueList)));
    }

    private NameValueList()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    protected override void DataPortal_Fetch(object criteria)
    {
      IsReadOnly = false;
      // TODO: load values
      using (SqlDataReader dr = null)
      {
        while (dr.Read())
        {
          Add(new NameValueListBase<int, string>.NameValuePair(
            dr.GetInt32(0), dr.GetString(1)));
        }
      }
      IsReadOnly = true;
    }

    #endregion
  }
}
