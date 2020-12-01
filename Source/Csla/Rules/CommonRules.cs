//-----------------------------------------------------------------------
// <copyright file="CommonRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create common rules.</summary>
//-----------------------------------------------------------------------
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
  public abstract class CommonBusinessRule : PropertyRule
  {
    /// <summary>
    /// Gets or sets the severity for this rule.
    /// </summary>
    public RuleSeverity Severity { get; set; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Primary property.</param>
    protected CommonBusinessRule(Csla.Core.IPropertyInfo primaryProperty)  : base(primaryProperty)
    {
      Severity = RuleSeverity.Error;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    protected CommonBusinessRule()
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
        InputProperties.Add(primaryProperty);
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
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
      InputProperties.Add(primaryProperty);
    }


    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="message">The message.</param>
    public Required(Csla.Core.IPropertyInfo primaryProperty, string message)
      : this(primaryProperty)
    {
      MessageText = message;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    public Required(Csla.Core.IPropertyInfo primaryProperty, Func<string> messageDelegate )
      : this(primaryProperty)
    {
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    /// <value></value>
    protected override string GetMessage()
    {
      return HasMessageDelegate ? base.MessageText : Csla.Properties.Resources.StringRequiredRule;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty];
      if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
      {
        var message = string.Format(GetMessage(), PrimaryProperty.FriendlyName);
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
      InputProperties.Add(primaryProperty);
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="message">The message.</param>
    /// <param name="max">Max length value.</param>
    public MaxLength(Csla.Core.IPropertyInfo primaryProperty, int max, string message)
      : this(primaryProperty, max)
    {
      MessageText = message;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="max">Max length value.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    public MaxLength(Csla.Core.IPropertyInfo primaryProperty, int max, Func<string> messageDelegate)
      : this(primaryProperty, max)
    {
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    /// <value></value>
    protected override string GetMessage()
    {
        return HasMessageDelegate ? base.MessageText : Csla.Properties.Resources.StringMaxLengthRule;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty];
      if (value != null && value.ToString().Length > Max)
      {
        var message = string.Format(GetMessage(), PrimaryProperty.FriendlyName, Max);
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
      InputProperties.Add(primaryProperty);
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="message">The message.</param>
    /// <param name="min">The minimum length.</param>
    public MinLength(Csla.Core.IPropertyInfo primaryProperty, int min, string message)
      : this(primaryProperty, min)
    {
      MessageText = message;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="min">Min length value.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    public MinLength(Csla.Core.IPropertyInfo primaryProperty, int min, Func<string> messageDelegate)
      : this(primaryProperty, min)
    {
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    /// <value></value>
    protected override string GetMessage()
    {
      return HasMessageDelegate ? base.MessageText : Csla.Properties.Resources.StringMinLengthRule;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty];
      if (value != null && value.ToString().Length < Min)
      {
        var message = string.Format(GetMessage(), PrimaryProperty.FriendlyName, Min);
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
    /// <param name="min">Min value.</param>
    public MinValue(Csla.Core.IPropertyInfo primaryProperty, T min) 
      : base(primaryProperty) 
    {
      Min = min;
      this.RuleUri.AddQueryParameter("min", min.ToString());
      InputProperties.Add(primaryProperty);
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="min">Min value.</param>
    /// <param name="message">The message.</param>
    public MinValue(Csla.Core.IPropertyInfo primaryProperty, T min,  string message)
      : this(primaryProperty, min)
    {
      MessageText = message;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="min">Min value.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    public MinValue(Csla.Core.IPropertyInfo primaryProperty, T min, Func<string> messageDelegate)
      : this(primaryProperty, min)
    {
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    /// <value></value>
    protected override string GetMessage()
    {
      return HasMessageDelegate ? base.MessageText : Csla.Properties.Resources.MinValueRule;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty] != null
                      ? (T)context.InputPropertyValues[PrimaryProperty]
                      : PrimaryProperty.DefaultValue != null
                        ? (T)PrimaryProperty.DefaultValue
                        : default(T);

      var result = value.CompareTo(Min);
      if (result <= -1)
      {
        string outValue;
        if (string.IsNullOrEmpty(Format))
          outValue = Min.ToString();
        else
          outValue = string.Format(string.Format("{{0:{0}}}", Format), Min);
        var message = string.Format(GetMessage(), PrimaryProperty.FriendlyName, outValue);
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
      InputProperties.Add(primaryProperty);
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="max">Max value.</param>
    /// <param name="message">The message.</param>
    public MaxValue(Csla.Core.IPropertyInfo primaryProperty, T max, string message)
      : this(primaryProperty, max)
    {
      MessageText = message;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="max">Max value.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    public MaxValue(Csla.Core.IPropertyInfo primaryProperty, T max, Func<string> messageDelegate)
      : this(primaryProperty, max)
    {
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    /// <value></value>
    protected override string GetMessage()
    {
      return HasMessageDelegate ? base.MessageText : Csla.Properties.Resources.MaxValueRule;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty] != null
                      ? (T)context.InputPropertyValues[PrimaryProperty]
                      : PrimaryProperty.DefaultValue != null 
                        ? (T)PrimaryProperty.DefaultValue
                        : default(T);

      var result = value.CompareTo(Max);
      if (result >= 1)
      {
        string outValue;
        if (string.IsNullOrEmpty(Format))
          outValue = Max.ToString();
        else
          outValue = string.Format(string.Format("{{0:{0}}}", Format), Max);
        var message = string.Format(GetMessage(), PrimaryProperty.FriendlyName, outValue);
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
      InputProperties.Add(primaryProperty);
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="expression">Regular expression.</param>
    /// <param name="message">The message.</param>
    public RegExMatch(Csla.Core.IPropertyInfo primaryProperty, string expression, string message)
      : this(primaryProperty, expression)
    {
      MessageText = message;
    }


    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="expression">Regular expression.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    public RegExMatch(Csla.Core.IPropertyInfo primaryProperty, string expression, Func<string> messageDelegate)
      : this(primaryProperty, expression)
    {
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    /// <value></value>
    protected override string GetMessage()
    {
      return HasMessageDelegate ? base.MessageText : Csla.Properties.Resources.RegExMatchRule;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty];
      bool ruleSatisfied;
      var expression = new Regex(Expression);

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
        var message = string.Format(GetMessage(), PrimaryProperty.FriendlyName);
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
    /// Gets the default description used by this rule.
    /// </summary>
    public string MessageText
    {
      get { return _messageDelegate.Invoke(); }
      protected set { MessageDelegate = () => value; }
    }

    private Func<string> _messageDelegate;

    /// <summary>
    /// Gets or sets the localizable message function.
    /// This will override the MessageText if set.
    /// </summary>
    /// <value>
    /// The localizable message.
    /// </value>
    public Func<string> MessageDelegate
    {
      get { return _messageDelegate; }
      set { _messageDelegate = value; }
    }

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
      MessageText = messageText;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property for message.</param>
    /// <param name="messageDelegate">Message text function.</param>
    public InfoMessage(Csla.Core.IPropertyInfo primaryProperty, Func<string> messageDelegate)
      : this(primaryProperty)
    {
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      context.AddInformationResult(MessageText);
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
    public Lambda(Action<IRuleContext> rule)
    {
      Initialize(rule);
    }
    
    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Primary property for the rule.</param>
    /// <param name="rule">Rule implementation.</param>
    public Lambda(Csla.Core.IPropertyInfo primaryProperty, Action<IRuleContext> rule)
      : base(primaryProperty)
    {
      Initialize(rule);
    }

    private void Initialize(Action<IRuleContext> rule)
    {
      Rule = rule;
      var methodName = Rule.Method.ToString();
      base.RuleUri.AddQueryParameter("r", Convert.ToBase64String(Encoding.Unicode.GetBytes(methodName)));
    }

    /// <summary>
    /// Add a query parameter to make the RuleUri uniques for this rule and primary property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public void AddQueryParameter(string key, string value)
    {
      base.RuleUri.AddQueryParameter(key, value);
    }

    private Action<IRuleContext> Rule { get; set; }

    /// <summary>
    /// Executes the rule.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      Rule(context);
    }
  }

  /// <summary>
  /// A rule that establishes a dependency between two properties.
  /// </summary>
  public class Dependency : BusinessRule
  {
    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Primary property for the rule.</param>
    /// <param name="dependencyProperty">Dependent property.</param>
    /// <remarks>
    /// When rules are run for the primary property, they will also be run for the dependent
    /// property. Add a Dependency rule to a property when changing that property should run rules
    /// on some other property, and you have no other rules that would establish this dependent
    /// or affected property relationship.
    /// </remarks>
    public Dependency(Csla.Core.IPropertyInfo primaryProperty, params Csla.Core.IPropertyInfo[] dependencyProperty)
      : base(primaryProperty)
    {
      AffectedProperties.AddRange(dependencyProperty);
    }
  }
}