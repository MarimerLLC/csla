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
  /// Base class for async lookup business rule 
  /// </summary>
  public abstract class AsyncLookupRule : CommonBusinessRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLookupRule"/> class.
    /// </summary>
    /// <param name="primaryProperty">Primary property for this rule.</param>
    protected AsyncLookupRule(IPropertyInfo primaryProperty) : base(primaryProperty)
    {
      IsAsync = true;
      CanRunAsAffectedProperty = false;
      CanRunInCheckRules = false;
      CanRunOnServer = false;
    }
  }
}
