using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

namespace Templates
{
  [Serializable()]
  class ReadOnlyList : ReadOnlyListBase<ReadOnlyList, ReadOnlyList.ROLChild>
  {
    #region ROLChild

    [Serializable()]
    public class ROLChild : ReadOnlyBase<ROLChild>
    {
      #region Business Methods

      private int _id;
      private string _data = string.Empty;

      public int Id
      {
        get { return _id; }
      }

      public string Data
      {
        get { return _data; }
      }

      protected override object GetIdValue()
      {
        return _id;
      }

      public override string ToString()
      {
        return _data;
      }

      #endregion

      #region Factory Methods

      internal static ROLChild GetChild(SqlDataReader dr)
      {
        return new ROLChild(dr);
      }

      private ROLChild()
      { /* require use of factory methods */ }

      private ROLChild(SqlDataReader dr)
      {
        Fetch(dr);
      }

      #endregion

      #region Data Access

      private void Fetch(SqlDataReader dr)
      {
        // load values
        _id = dr.GetInt32(0);
        _data = dr.GetString(1);
      }

      #endregion
    }

    #endregion

    #region Factory Methods

    public static ReadOnlyList GetList(string filter)
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
      // load values
      using (SqlDataReader dr = null)
      {
        while (dr.Read())
          Add(ROLChild.GetChild(dr));
      }
    }

    #endregion
  }
}
