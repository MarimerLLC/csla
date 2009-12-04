using Csla;
using System;

namespace WpfDesignTime.Business
{
  [Serializable()]
  public class ResourceInfo : ReadOnlyBase<ResourceInfo>
  {
    private int _id;
    public int Id
    {
      get { return _id; }
    }

    private string _name;
    public string Name
    {
      get { return _name; }
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