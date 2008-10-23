using System;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public partial class ProjectInfo : ReadOnlyBase<ProjectInfo>
  {
    private Guid _id;
    private string _name;

    public Guid Id
    {
      get
      {
        return _id;
      }
      internal set
      {
        _id = value;
      }
    }

    public string Name
    {
      get
      {
        return _name;
      }
      internal set
      {
        _name = value;
      }
    }

    protected override object GetIdValue()
    {
      return _id;
    }

    public override string ToString()
    {
      return _name;
    }

    private ProjectInfo()
    {
      // require use of factory methods
    }

    internal ProjectInfo(Guid id, string name)
    {
      _id = id;
      _name = name;
    }
  }
}