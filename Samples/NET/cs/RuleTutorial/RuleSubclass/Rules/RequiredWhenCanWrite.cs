using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using Csla.Rules.CommonRules;

namespace RuleSubclass.Rules
{
  /// <summary>
  /// Rule: Field is required when user can write
  /// </summary>
  public class RequiredWhenCanWrite : Required
  {
    public RequiredWhenCanWrite(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    { }

    public RequiredWhenCanWrite(IPropertyInfo primaryProperty, string message)
      : base(primaryProperty, message)
    { }

    public RequiredWhenCanWrite(IPropertyInfo primaryProperty, Func<string> messageDelegate)
      : base(primaryProperty, messageDelegate)
    { }

    protected override void Execute(Csla.Rules.IRuleContext context)
    {
      var bb = (BusinessBase) context.Target;
      if (bb.CanWriteProperty(PrimaryProperty))
        base.Execute(context);
    }
  }
}
