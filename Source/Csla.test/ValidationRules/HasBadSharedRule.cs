//-----------------------------------------------------------------------
// <copyright file="HasBadSharedRule.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.ValidationRules
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [Serializable]
  public class HasBadSharedRule : BusinessBase<HasBadSharedRule>
  {
    public static PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new BadRule { PrimaryProperty = DataProperty });
    }

    public class BadRule : Rules.BusinessRule
    {
      protected override void Execute(Rules.IRuleContext context)
      {
        context.AddErrorResult("Bad rule");
      }
    }
  }
}