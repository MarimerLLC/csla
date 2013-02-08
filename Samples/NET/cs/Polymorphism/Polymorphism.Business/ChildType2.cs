using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Csla;
using Csla.Rules;

namespace Polymorphism.Business
{

  [Serializable]
  public class ChildType2 : ChildType<ChildType2>
  {
    public static readonly PropertyInfo<string> AddressProperty = RegisterProperty<string>(c => c.Address);
    public string Address
    {
      get { return GetProperty(AddressProperty); }
      set { SetProperty(AddressProperty, value); }
    }

    public ChildType2()
    {
      if (!Csla.Rules.BusinessRules.HasPermission(AuthorizationActions.CreateObject, typeof(ChildType2)))
        throw new SecurityException("No access");
    }

    public ChildType2(int id, string name, string address)
      : this()
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = name;
        Address = address;
      }
      MarkAsChild();
    }
  }
}
