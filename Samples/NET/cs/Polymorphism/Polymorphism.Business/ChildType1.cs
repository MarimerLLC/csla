using System;
using System.Diagnostics;
using System.Security;
using Csla;
using Csla.Rules;
using Csla.Serialization.Mobile;

namespace Polymorphism.Business
{

  [Serializable]
  public class ChildType1 : ChildType<ChildType1>
  {

    public static readonly PropertyInfo<string> GroupProperty = RegisterProperty<string>(c => c.Group);
    public string Group
    {
      get { return GetProperty(GroupProperty); }
      set { SetProperty(GroupProperty, value); }
    }
    public ChildType1()
    {
        if (!Csla.Rules.BusinessRules.HasPermission(AuthorizationActions.CreateObject, typeof(ChildType1)))
          throw new SecurityException("No access");
    }

    protected static new void AddObjectAuthorizationRules()
    {
      Debug.Print("ChildType1 - AddObjectAuthorizationRules");
    }

    public ChildType1(int id, string name, string group) : this()
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = name;
        Group = group;
      }
      MarkAsChild();
    }
  }
}
