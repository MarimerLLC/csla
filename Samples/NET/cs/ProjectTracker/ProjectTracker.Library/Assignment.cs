using System;
using Csla;
using Csla.Data;
using Csla.Rules;

namespace ProjectTracker.Library
{
  internal static class Assignment
  {
    public static System.DateTime GetDefaultAssignedDate()
    {
      return System.DateTime.Today;
    }

#if SILVERLIGHT
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

      protected override void Execute(RuleContext context)
      {
        int role = (int)context.InputPropertyValues[PrimaryProperty];
        RoleList.GetList((o, e) =>
          {
            if (!e.Object.ContainsKey(role))
              context.AddErrorResult("Role must be in RoleList");
            context.Complete();
          });
      }
    }
#else
    /// <summary>
    /// Ensure the Role property value exists
    /// in RoleList
    /// </summary>
    public class ValidRole : BusinessRule
    {
      public ValidRole(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        InputProperties = new System.Collections.Generic.List<Csla.Core.IPropertyInfo> { primaryProperty };
      }

      protected override void Execute(RuleContext context)
      {
        int role = (int)context.InputPropertyValues[PrimaryProperty];
        if (!RoleList.GetList().ContainsKey(role))
          context.AddErrorResult("Role must be in RoleList");
      }
    }
#endif
  }
}