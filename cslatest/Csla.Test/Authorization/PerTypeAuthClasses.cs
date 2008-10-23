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
    string _test = string.Empty;
    public string Test
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _test;
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      set
      {
        CanWriteProperty(true);
        if (!_test.Equals(value))
        {
          _test = value;
          PropertyHasChanged();
        }
      }
    }

    string _name = string.Empty;
    public string Name
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _name;
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      set
      {
        CanWriteProperty(true);
        if (!_name.Equals(value))
        {
          _name = value;
          PropertyHasChanged();
        }
      }
    }

    protected override object GetIdValue()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowWrite("Test", "Admin");
    }

    protected override void AddInstanceAuthorizationRules()
    {
      AuthorizationRules.InstanceAllowWrite("Name", "Admin");
    }
  }
}
