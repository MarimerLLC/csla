using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla;
using Csla.Core;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace AuthzFactoryMethods
{
  public class MyVeryOwnObjectAuthzRule : AuthorizationRule
  {
    private readonly IPropertyInfo _prop1;
    private readonly IPropertyInfo _prop2;

    public MyVeryOwnObjectAuthzRule(AuthorizationActions action, PropertyInfo<string> prop1, PropertyInfo<string> prop2)
      : base(action)
    {
      _prop1 = prop1;
      _prop2 = prop2;
    }

    protected override void Execute(AuthorizationContext context)
    {
      if (context.Target != null)
      {
        var val1 = (string)ReadProperty(context.Target, _prop1);
        var val2 = (string)ReadProperty(context.Target, _prop2);

        // add my own if tests here 
      }
    }
  }
}
