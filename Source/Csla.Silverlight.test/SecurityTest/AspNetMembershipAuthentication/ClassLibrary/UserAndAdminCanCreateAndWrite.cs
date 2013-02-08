using System;
using System.Net;
using Csla;
using Csla.Security;
using Csla.Serialization;

namespace ClassLibrary
{
  [Serializable]
  public class UserAndAdminCanCreateAndWrite : BusinessBase<UserAndAdminCanCreateAndWrite>
  {
    private static PropertyInfo<string> AProperty = RegisterProperty(new PropertyInfo<string>("A"));
    public string A
    {
      get { return GetProperty(AProperty); }
      set { SetProperty(AProperty, value); }
    }
    private static PropertyInfo<string> BProperty = RegisterProperty(new PropertyInfo<string>("B"));
    public string B
    {
      get { return GetProperty(BProperty); }
      set { SetProperty(BProperty, value); }
    }

    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowWrite(AProperty, "User Role");
      AuthorizationRules.AllowRead(BProperty, "User Role");
    }

    protected static void AddObjectAuthorizationRules()
    {
      AuthorizationRules.AllowCreate(typeof(UserAndAdminCanCreateAndWrite), "User Role");
      AuthorizationRules.AllowEdit(typeof(UserAndAdminCanCreateAndWrite), "User Role");
      AuthorizationRules.AllowDelete(typeof(UserAndAdminCanCreateAndWrite), "User Role");
    }
  }
}
