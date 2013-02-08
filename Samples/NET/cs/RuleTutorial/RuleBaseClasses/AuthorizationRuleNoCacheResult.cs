using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Rules;

namespace RuleBaseClasses
{
  public abstract class AuthorizationRuleNoCacheResult : AuthorizationRule
  {
    public AuthorizationRuleNoCacheResult(AuthorizationActions action, Csla.Core.IMemberInfo element) : base(action, element)
    {
      CacheResult = false;
    }
  }
}
