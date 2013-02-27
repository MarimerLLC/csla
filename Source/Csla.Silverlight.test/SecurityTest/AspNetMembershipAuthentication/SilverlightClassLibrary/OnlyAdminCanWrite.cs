using System;
using System.Net;
using Csla.Serialization;
using Csla;
using Csla.Security;

namespace ClassLibrary
{
  [Serializable]
  public class OnlyAdminCanWrite : BusinessBase<OnlyAdminCanWrite>
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
      AuthorizationRules.AllowWrite(AProperty, "Admin Role");
      AuthorizationRules.AllowRead(BProperty, "Admin Role");
    }

    protected static void AddObjectAuthorizationRules()
    {
    }
  }
}
