﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla;
using Csla.Rules;

namespace ProjectTracker.Library
{
  /// <summary>
  /// Ensure the Role property value exists
  /// in RoleList
  /// </summary>
  public class ValidRole : BusinessRuleAsync
  {
    public ValidRole(Csla.Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties.Add(primaryProperty);
    }

    protected override async Task ExecuteAsync(IRuleContext context)
    {
      int role = (int)context.InputPropertyValues[PrimaryProperty];
      var portal = context.ApplicationContext.GetRequiredService<IDataPortal<RoleList>>();
      var roles = await portal.FetchAsync();
      if (!roles.ContainsKey(role))
        context.AddErrorResult("Role must be in RoleList");
    }
  }
}
