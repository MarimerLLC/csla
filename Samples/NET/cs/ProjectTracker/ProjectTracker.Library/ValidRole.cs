using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Rules;

namespace ProjectTracker.Library
{
  /// <summary>
  /// Ensure the Role property value exists
  /// in RoleList
  /// </summary>
  public class ValidRole : BusinessRule
  {
    public ValidRole(Csla.Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
#if SILVERLIGHT
        IsAsync = true;
#endif
      InputProperties = new System.Collections.Generic.List<Csla.Core.IPropertyInfo> { primaryProperty };
    }

    protected override void Execute(RuleContext context)
    {
#if SILVERLIGHT
        int role = (int)context.InputPropertyValues[PrimaryProperty];
        RoleList.GetList((o, e) =>
          {
            if (!e.Object.ContainsKey(role))
              context.AddErrorResult("Role must be in RoleList");
            context.Complete();
          });
#else
      int role = (int)context.InputPropertyValues[PrimaryProperty];
      if (!RoleList.GetList().ContainsKey(role))
        context.AddErrorResult("Role must be in RoleList");
#endif
    }
  }
}
