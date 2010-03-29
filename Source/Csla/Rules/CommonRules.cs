using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules.CommonRules
{
  /// <summary>
  /// Business rule that encapsulates a DataAnnotations
  /// ValidationAttribute rule.
  /// </summary>
  public class DataAnnotation : BusinessRule
  {
    /// <summary>
    /// Gets the ValidationAttribute instance.
    /// </summary>
    public System.ComponentModel.DataAnnotations.ValidationAttribute Attribute { get; private set; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="attribute">ValidationAttribute instance.</param>
    public DataAnnotation(Csla.Core.IPropertyInfo primaryProperty, System.ComponentModel.DataAnnotations.ValidationAttribute attribute)
      : base(primaryProperty)
    {
      this.Attribute = attribute;
      RuleUri.AddQueryParameter("a", attribute.GetType().FullName);
      InputProperties = new List<Core.IPropertyInfo> { primaryProperty };
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(RuleContext context)
    {
      object value = context.InputPropertyValues[PrimaryProperty];
      var ctx = new System.ComponentModel.DataAnnotations.ValidationContext(context.Target, null, null);
      var result = this.Attribute.GetValidationResult(value, ctx);
      if (result != null)
        context.AddErrorResult(result.ErrorMessage);
    }
  }

  /// <summary>
  /// Business rule for a required string.
  /// </summary>
  public class Required : BusinessRule
  {
    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    public Required(Csla.Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties = new List<Core.IPropertyInfo> { primaryProperty };
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(RuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty];
      if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        context.AddErrorResult(
          string.Format("{0} value required", PrimaryProperty.FriendlyName));
    }
  }

  /// <summary>
  /// Business rule for a maximum length string.
  /// </summary>
  public class MaxLength : BusinessRule
  {
    /// <summary>
    /// Gets the max length value.
    /// </summary>
    public int Max { get; private set; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="max">Max length value.</param>
    public MaxLength(Csla.Core.IPropertyInfo primaryProperty, int max)
      : base(primaryProperty)
    {
      Max = max;
      this.RuleUri.AddQueryParameter("max", max.ToString());
      InputProperties = new List<Core.IPropertyInfo> { primaryProperty };
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(RuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty];
      if (value != null && value.ToString().Length > Max)
        context.AddErrorResult(
          string.Format("{0} value too long", PrimaryProperty.FriendlyName));
    }
  }
}
