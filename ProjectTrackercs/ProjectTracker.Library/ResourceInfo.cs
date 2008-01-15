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

    internal ResourceInfo(DalLinq.Resource resource)
    {
      _id = resource.Id;
      _name = string.Format("{0}, {1}", resource.LastName, resource.FirstName);
    }
  }
}