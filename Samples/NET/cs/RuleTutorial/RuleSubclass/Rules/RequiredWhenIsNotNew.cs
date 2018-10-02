using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using Csla.Rules.CommonRules;

namespace RuleSubclass.Rules
{
  /// <summary>
  /// Rule: Field is required when Not Target.IsNew
  /// </summary>
  public class RequiredWhenIsNotNew : Required
  {
    public RequiredWhenIsNotNew(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    { }

    public RequiredWhenIsNotNew(IPropertyInfo primaryProperty, string message)
      : base(primaryProperty, message)
    { }

    public RequiredWhenIsNotNew(IPropertyInfo primaryProperty, Func<string> messageDelegate)
      : base(primaryProperty, messageDelegate)
    { }

    protected override void Execute(Csla.Rules.IRuleContext context)
    {
      var bb = (BusinessBase)context.Target;
      if (!bb.IsNew)
        base.Execute(context);
    }
  }
}
