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
#if NETFX_CORE
        IsAsync = true;
#endif
      InputProperties = new System.Collections.Generic.List<Csla.Core.IPropertyInfo> { primaryProperty };
    }

#if __ANDROID__
    protected override async void Execute(RuleContext context)
#else
    protected override void Execute(RuleContext context)
#endif
    {
        int role = (int)context.InputPropertyValues[PrimaryProperty];
#if __ANDROID__
        var roles = await RoleList.GetListAsync();
        if (!(await RoleList.GetListAsync()).ContainsKey(role))
            context.AddErrorResult("Role must be in RoleList");
        context.Complete();
#else
      if (!RoleList.GetList().ContainsKey(role))
        context.AddErrorResult("Role must be in RoleList");
#endif
    }
  }
}
