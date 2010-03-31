using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Csla.Test.Authorization
{
#if TESTING
  [DebuggerNonUserCode]
#endif
  public class PerTypeAuthorization : BusinessBase<PerTypeAuthorization>
  {
    public static PropertyInfo<string> TestProperty = RegisterProperty<string>(c => c.Test);
    public string Test
    {
      get { return GetProperty(TestProperty); }
      set { SetProperty(TestProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowWrite("Test", "Admin");
    }
  }
}
