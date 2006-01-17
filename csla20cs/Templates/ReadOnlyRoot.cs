using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace Templates
{
  [Serializable()]
  class ReadOnlyRoot : ReadOnlyBase<ReadOnlyRoot>
  {
    #region Business Methods

    private int _id;

    public int Id
    {
      get { return _id; }
    }

    protected override object GetIdValue()
    {
      return _id;
    }

    #endregion

    #region Factory Methods

    public static ReadOnlyRoot GetReadOnlyRoot(int id)
    {
      return DataPortal.Fetch<ReadOnlyRoot>(new Criteria(id));
    }

    private ReadOnlyRoot()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    [Serializable()]
    private class Criteria
    {
      private int _id;

      public int Id
      {
        get { return _id; }
      }
      public Criteria(int id)
      {
        _id = id;
      }
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      // load values
      _id = criteria.Id;
    }

    #endregion
  }
}
