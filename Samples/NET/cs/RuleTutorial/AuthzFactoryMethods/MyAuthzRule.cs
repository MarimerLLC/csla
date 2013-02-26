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
  public class MyObjectAuthzRule : IsInRole
  {
    private readonly IPropertyInfo _prop1;
    private readonly IPropertyInfo _prop2;

    public MyObjectAuthzRule(AuthorizationActions action, PropertyInfo<string> prop1, PropertyInfo<string> prop2, params string[] roles)
      : base(action, roles)
    {
      _prop1 = prop1;
      _prop2 = prop2;
    }

    public MyObjectAuthzRule(AuthorizationActions action, PropertyInfo<string> prop1, PropertyInfo<string> prop2, List<string> roles)
      : base(action, roles)
    {
      _prop1 = prop1;
      _prop2 = prop2;
    }

    protected override void Execute(AuthorizationContext context)
    {
      // chect the base rule 
      base.Execute(context);
      if (!context.HasPermission) return;

      if (context.Target != null)
      {
        var val1 = (string) ReadProperty(context.Target, _prop1);
        var val2 = (string) ReadProperty(context.Target, _prop2);

        // add my own if tests here 
      }
    }
  }
}
