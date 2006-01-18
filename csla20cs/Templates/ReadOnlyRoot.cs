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

    // TODO: add your own fields, properties and methods
    private int _id;

    public int Id
    {
      get 
      {
        CanReadProperty(true);
        return _id; 
      }
    }

    protected override object GetIdValue()
    {
      return _id;
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowRead("", "");
    }

    public static bool CanGetObject()
    {
      // TODO: customize to check user role
      //return ApplicationContext.User.IsInRole("");
      return true;
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
      // TODO: load values
      _id = criteria.Id;
    }

    #endregion
  }
}
