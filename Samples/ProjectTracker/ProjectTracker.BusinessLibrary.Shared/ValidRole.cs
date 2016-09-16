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
      IsAsync = true;
      InputProperties = new System.Collections.Generic.List<Csla.Core.IPropertyInfo> { primaryProperty };
    }

    protected override async void Execute(RuleContext context)
    {
        int role = (int)context.InputPropertyValues[PrimaryProperty];
        var roles = await RoleList.CacheListAsync();
        if (!roles.ContainsKey(role))
            context.AddErrorResult("Role must be in RoleList");
        context.Complete();
    }
  }
}
