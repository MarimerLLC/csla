using Csla;
using System;
using Csla.Validation.Test;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceInfo : Csla.Validation.ReadOnlyBase<ResourceInfo>
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

    protected static void AddObjectAuthorizationRules()
    {
      // add object-level authorization rules here
      Csla.Security.AuthorizationRules.AllowCreate(typeof(ResourceInfo), "ProjectManager");
      Csla.Security.AuthorizationRules.AllowGet(typeof(ResourceInfo), "Administrator");
    }
    public ResourceInfo()
    {
    }
  }
}