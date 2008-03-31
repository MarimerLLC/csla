using Csla;
using System;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceInfo : ReadOnlyBase<ResourceInfo>
  {
    private int _id;
    private string _name;

    public int Id
    {
      get
      {
        return _id;
      }
    }

    public string Name
    {
      get
      {
        return _name;
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

    internal ResourceInfo(int id, string lastname, string firstname)
    {
      _id = id;
      _name = string.Format("{0}, {1}", lastname, firstname);
    }
  }
}