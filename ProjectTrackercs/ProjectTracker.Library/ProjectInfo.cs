using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectInfo :
    ReadOnlyBase<ProjectInfo>
  {
    #region Business Methods

    private Guid _id;
    private string _name;

    public Guid Id
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

    private ProjectInfo()
    { /* require use of factory methods */ }

    internal ProjectInfo(Guid id, string name)
    {
      _id = id;
      _name = name;
    }
    #endregion
  }
}
