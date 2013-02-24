using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace RuleBaseClasses
{
  /// <summary>
  /// Base class for sync lookup business rule
  /// </summary>
  public abstract class SyncLookupRule : CommonBusinessRule
  {
    protected SyncLookupRule(IPropertyInfo primaryProperty) : base(primaryProperty)
    {
      IsAsync = false;
      CanRunAsAffectedProperty = false;
      CanRunInCheckRules = true;
      CanRunOnServer = false;
    }
  }
}