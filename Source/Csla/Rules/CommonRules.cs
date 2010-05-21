using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Properties;
using System.Text.RegularExpressions;

namespace Csla.Rules.CommonRules
{
  /// <summary>
  /// Base class used to create common rules.
  /// </summary>
  public abstract class CommonBusinessRule : BusinessRule
  {
    /// <summary>
    /// Gets or sets the severity for this rule.
    /// </summary>
    public RuleSeverity Severity { get; set; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Primary property.</param>
    public CommonBusinessRule(Csla.Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      Severity = RuleSeverity.Error;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    public CommonBusinessRule()
    {
      Severity = RuleSeverity.Error;
    }
  }

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
      if (primaryProperty != null)
        InputProperties = new List<Core.IPropertyInfo> { primaryProperty };
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(RuleContext context)
    {

      var ctx = new System.ComponentModel.DataAnnotations.ValidationContext(context.Target, null, null);
      if (PrimaryProperty != null)
        ctx.MemberName = PrimaryProperty.FriendlyName;

      System.ComponentModel.DataAnnotations.ValidationResult result = null;
      try
      {
        if (PrimaryProperty != null)
        {
          object value = context.InputPropertyValues[PrimaryProperty];
          result = this.Attribute.GetValidationResult(value, ctx);
        }
        else
        {
          result = this.Attribute.GetValidationResult(null, ctx);
        }
      }
      catch (Exception ex)
      {
        context.AddErrorResult(ex.Message);
      }
      if (result != null)
        context.AddErrorResult(result.ErrorMessage);
    }
  }

  /// <summary>
  /// Business rule for a required string.
  /// </summary>
  public class Required : CommonBusinessRule
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
      {
        var message = string.Format(Resources.StringRequiredRule, PrimaryProperty.FriendlyName);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message) { Severity = Severity });
      }
    }
  }

  /// <summary>
  /// Business rule for a maximum length string.
  /// </summary>
  public class MaxLength : CommonBusinessRule
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
      {
        var message = string.Format(Resources.StringMaxLengthRule, PrimaryProperty.FriendlyName, Max);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message) { Severity = Severity });
      }
    }
  }

  /// <summary>
  /// Business rule for a minimum length string.
  /// </summary>
  public class MinLength : CommonBusinessRule
  {
    /// <summary>
    /// Gets the min length value.
    /// </summary>
    public int Min { get; private set; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="min">Min length value.</param>
    public MinLength(Csla.Core.IPropertyInfo primaryProperty, int min)
      : base(primaryProperty)
    {
      Min = min;
      this.RuleUri.AddQueryParameter("min", min.ToString());
      InputProperties = new List<Core.IPropertyInfo> { primaryProperty };
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(RuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty];
      if (value != null && value.ToString().Length < Min)
      {
        var message = string.Format(Resources.StringMinLengthRule, PrimaryProperty.FriendlyName, Min);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message) { Severity = Severity });
      }
    }
  }

  /// <summary>
  /// Business rule for a minimum value.
  /// </summary>
  public class MinValue<T> : CommonBusinessRule
    where T : IComparable
  {
    /// <summary>
    /// Gets the min value.
    /// </summary>
    public T Min { get; private set; }
    /// <summary>
    /// Gets or sets the format string used
    /// to format the Min value.
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="min">Min length value.</param>
    public MinValue(Csla.Core.IPropertyInfo primaryProperty, T min) 
      : base(primaryProperty) 
    {
      Min = min;
      this.RuleUri.AddQueryParameter("min", min.ToString());
      InputProperties = new List<Core.IPropertyInfo> { primaryProperty };
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(RuleContext context)
    {
      T value = (T)context.InputPropertyValues[PrimaryProperty];
      int result = value.CompareTo(Min);
      if (result <= -1)
      {
        string outValue;
        if (string.IsNullOrEmpty(Format))
          outValue = Min.ToString();
        else
          outValue = string.Format(string.Format("{{0:{0}}}", Format), Min);
        var message = string.Format(Resources.MinValueRule, PrimaryProperty.FriendlyName, outValue);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message) { Severity = Severity });
      }
    }
  }

  /// <summary>
  /// Business rule for a maximum value.
  /// </summary>
  public class MaxValue<T> : CommonBusinessRule
    where T : IComparable
  {
    /// <summary>
    /// Gets the max value.
    /// </summary>
    public T Max { get; private set; }
    /// <summary>
    /// Gets or sets the format string used
    /// to format the Max value.
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="max">Max length value.</param>
    public MaxValue(Csla.Core.IPropertyInfo primaryProperty, T max)
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
      T value = (T)context.InputPropertyValues[PrimaryProperty];
      int result = value.CompareTo(Max);
      if (result >= 1)
      {
        string outValue;
        if (string.IsNullOrEmpty(Format))
          outValue = Max.ToString();
        else
          outValue = string.Format(string.Format("{{0:{0}}}", Format), Max);
        var message = string.Format(Resources.MaxValueRule, PrimaryProperty.FriendlyName, outValue);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message) { Severity = Severity });
      }
    }
  }

  /// <summary>
  /// Business rule that evaluates a regular expression.
  /// </summary>
  public class RegExMatch : CommonBusinessRule
  {
    #region NullResultOptions

    /// <summary>
    /// List of options for the NullResult
    /// property.
    /// </summary>
    public enum NullResultOptions
    {
      /// <summary>
      /// Indicates that a null value
      /// should always result in the 
      /// rule returning false.
      /// </summary>
      ReturnFalse,
      /// <summary>
      /// Indicates that a null value
      /// should always result in the 
      /// rule returning true.
      /// </summary>
      ReturnTrue,
      /// <summary>
      /// Indicates that a null value
      /// should be converted to an
      /// empty string before the
      /// regular expression is
      /// evaluated.
      /// </summary>
      ConvertToEmptyString
    }

    #endregion

    /// <summary>
    /// Gets the regular expression
    /// to be evaluated.
    /// </summary>
    public string Expression { get; private set; }

    /// <summary>
    /// Gets or sets a value that controls how
    /// null input values are handled.
    /// </summary>
    public NullResultOptions NullOption { get; set; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Primary property.</param>
    /// <param name="expression">Regular expression.</param>
    public RegExMatch(Csla.Core.IPropertyInfo primaryProperty, string expression)
      : base(primaryProperty)
    {
      Expression = expression;
      RuleUri.AddQueryParameter("e", expression);
      InputProperties = new List<Core.IPropertyInfo> { primaryProperty };
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(RuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty];
      bool ruleSatisfied = false;
      Regex expression = new Regex(Expression);

      if (value == null && NullOption == NullResultOptions.ConvertToEmptyString)
        value = string.Empty;

      if (value == null)
      {
        // if the value is null at this point
        // then return the pre-defined result value
        ruleSatisfied = (NullOption == NullResultOptions.ReturnTrue);
      }
      else
      {
        // the value is not null, so run the 
        // regular expression
        ruleSatisfied = expression.IsMatch(value.ToString());
      }

      if (!ruleSatisfied)
      {
        var message = string.Format(Resources.RegExMatchRule, PrimaryProperty.FriendlyName);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message) { Severity = Severity });
      }
    }
  }

  /// <summary>
  /// Adds an information message to a property.
  /// </summary>
  /// <remarks>
  /// Message text is the DefaultDescription property.
  /// The default priority for this rule is 1, so if any
  /// rules are broken the infomational text won't appear.
  /// </remarks>
  public class InfoMessage : BusinessRule
  {
    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property for message.</param>
    public InfoMessage(Csla.Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      Priority = 1;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property for message.</param>
    /// <param name="messageText">Message text.</param>
    public InfoMessage(Csla.Core.IPropertyInfo primaryProperty, string messageText)
      : this(primaryProperty)
    {
      DefaultDescription = messageText;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(RuleContext context)
    {
      context.AddInformationResult(DefaultDescription);
    }
  }

  /// <summary>
  /// A business rule defined by a lambda expression.
  /// </summary>
  public class Lambda : BusinessRule
  {
    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="rule">Rule implementation.</param>
    public Lambda(Action<RuleContext> rule)
    {
      Rule = rule;
      base.RuleUri.AddQueryParameter("r", Rule.GetHashCode().ToString());
    }
    
    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Primary property for the rule.</param>
    /// <param name="rule">Rule implementation.</param>
    public Lambda(Csla.Core.IPropertyInfo primaryProperty, Action<RuleContext> rule)
      : base(primaryProperty)
    {
      Rule = rule;
      base.RuleUri.AddQueryParameter("r", Rule.GetHashCode().ToString());
    }

    private Action<RuleContext> Rule { get; set; }

    /// <summary>
    /// Executes the rule.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(RuleContext context)
    {
      Rule(context);
    }
  }

}
