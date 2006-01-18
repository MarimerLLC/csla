using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

namespace Templates
{
  [Serializable()]
  class ReadOnlyChild : ReadOnlyBase<ReadOnlyChild>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods
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

    #endregion

    #region Factory Methods

    internal static ReadOnlyChild GetReadOnlyChild(SqlDataReader dr)
    {
      return new ReadOnlyChild(dr);
    }

    private ReadOnlyChild()
    { /* require use of factory methods */ }

    private ReadOnlyChild(SqlDataReader dr)
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
}
