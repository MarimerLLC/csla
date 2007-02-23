using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceInfo :
    ReadOnlyBase<ResourceInfo>
  {
    #region Business Methods

    private int _id;
    private string _name;

    public int Id
    {
      get { return _id; }
    }

    public string Name
    {
      get { return _name; }
    }

    protected override object GetIdValue()
    {
      return _id;
    }

    public override string ToString()
    {
      return _name;
    }

    #endregion

    #region Constructors

    private ResourceInfo()
    { /* require use of factory methods */ }

    internal ResourceInfo(SafeDataReader dr)
    {
      _id = dr.GetInt32("Id");
      _name = string.Format("{0}, {1}",
                dr.GetString("LastName"),
                dr.GetString("FirstName"));
    }

    #endregion
  }
}
