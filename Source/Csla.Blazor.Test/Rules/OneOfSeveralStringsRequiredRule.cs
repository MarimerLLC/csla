using System;
using System.Collections.Generic;
using System.Text;
using Csla.Core;
using Csla.Rules;

namespace Csla.Blazor.Test.Rules
{
  /// <summary>
  /// Business rule for when one of two strings must be provided
  /// </summary>
  public class OneOfSeveralStringsRequiredRule : PropertyRule
  {

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="affectedProperties">Array of properties to which the rule applies.</param>
    public OneOfSeveralStringsRequiredRule(params Csla.Core.IPropertyInfo[] affectedProperties)
      : base(affectedProperties[0])
    {
      foreach (Csla.Core.IPropertyInfo affectedProperty in affectedProperties)
      {
        InputProperties.Add(affectedProperty);
      }
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="affectedProperties">Array of properties to which the rule applies.</param>
    /// <param name="message">The message.</param>
    public OneOfSeveralStringsRequiredRule(string message, params Csla.Core.IPropertyInfo[] affectedProperties)
      : this(affectedProperties)
    {
      MessageText = message;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="affectedProperties">Array of properties to which the rule applies.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    public OneOfSeveralStringsRequiredRule(Func<string> messageDelegate, params Csla.Core.IPropertyInfo[] affectedProperties)
      : this(affectedProperties)
    {
      MessageDelegate = messageDelegate;
    }

    public RuleSeverity Severity { get; set; } = RuleSeverity.Error;

    /// <summary>
    /// Gets the error message.
    /// </summary>
    /// <value></value>
    protected override string GetMessage()
    {
      return HasMessageDelegate ? base.MessageText : "One of the properties must be provided";
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      bool valueProvided = false;

      // Iterate all of the properties and see if any of them have a value
      foreach (var value in context.InputPropertyValues.Values)
      {
        if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
        {
          // Property has a value, so the rule is satisfied
          valueProvided = true;
          break;
        }
      }

      if (!valueProvided)
      {
        // Rule not satisfied, so report it as such
        foreach (Csla.Core.IPropertyInfo propertyInfo in InputProperties)
        { 
          var message = string.Format(GetMessage(), propertyInfo.FriendlyName);
          context.Results.Add(new RuleResult(RuleName, propertyInfo, message) { Severity = Severity });
        }
      }
    }
  }

}
