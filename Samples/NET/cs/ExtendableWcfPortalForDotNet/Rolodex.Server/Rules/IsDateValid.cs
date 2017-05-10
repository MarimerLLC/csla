using System;
using System.Collections.Generic;
using Csla;
using Csla.Core;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace Rolodex.Business.Rules
{
  public class IsDateValid : CommonBusinessRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="IsDateValid"/> class.
    /// </summary>
    /// <param name="primaryProperty">The primary property.</param>
    public IsDateValid(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties = new List<IPropertyInfo> {primaryProperty};
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IsDateValid"/> class.
    /// </summary>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="errorMessageDelegate">The error message function.</param>
    public IsDateValid(IPropertyInfo primaryProperty, Func<string> errorMessageDelegate)
      : this(primaryProperty)
    {
      MessageDelegate = errorMessageDelegate;
    }

    /// <summary>
    /// Does the check for primary propert less than compareTo property
    /// </summary>
    /// <param name="context">Rule context object.</param>
    protected override void Execute(RuleContext context)
    {
      var value = (SmartDate) context.InputPropertyValues[PrimaryProperty];

      if (!value.IsEmpty)
      {
        if (value.Date < (new DateTime(2000, 1, 1)))
        {
          var message = string.Format("{0} date must be greater that 1/1/2000!", PrimaryProperty.FriendlyName);
          context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message) {Severity = Severity});
        }
        if (value.Date > DateTime.Today)
        {
          var message = string.Format("{0} date cannot be greater than today!", PrimaryProperty.FriendlyName);
          context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message) {Severity = Severity});
        }
      }
    }
  }
}