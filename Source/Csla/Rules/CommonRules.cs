//-----------------------------------------------------------------------
// <copyright file="CommonRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create common rules.</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Csla.Properties;

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
    protected CommonBusinessRule(Core.IPropertyInfo? primaryProperty)  : base(primaryProperty)
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
    public System.ComponentModel.DataAnnotations.ValidationAttribute Attribute { get; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="attribute">ValidationAttribute instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="attribute"/> is <see langword="null"/>.</exception>
    public DataAnnotation(Core.IPropertyInfo? primaryProperty, System.ComponentModel.DataAnnotations.ValidationAttribute attribute)
      : base(primaryProperty)
    {
      Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
      RuleUri.AddQueryParameter("a", attribute.GetType().FullName!);
      if (primaryProperty != null)
        InputProperties.Add(primaryProperty);
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var ctx = new System.ComponentModel.DataAnnotations.ValidationContext(context.Target!, context.ApplicationContext.CurrentServiceProvider, null);
      if (PrimaryProperty != null)
        ctx.MemberName = PrimaryProperty.FriendlyName;

      System.ComponentModel.DataAnnotations.ValidationResult? result = null;
      try
      {
        if (PrimaryProperty != null)
        {
          object? value = context.InputPropertyValues[PrimaryProperty];
          result = Attribute.GetValidationResult(value, ctx);
        }
        else
        {
          result = Attribute.GetValidationResult(null, ctx);
        }
      }
      catch (Exception ex)
      {
        context.AddErrorResult(ex.Message);
      }
      if (result != null)
        context.AddErrorResult(result.ErrorMessage ?? "");
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
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> is <see langword="null"/>.</exception>
    public Required(Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      if (primaryProperty is null)
        throw new ArgumentNullException(nameof(primaryProperty));
      InputProperties.Add(primaryProperty);
    }


    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public Required(Core.IPropertyInfo primaryProperty, string message)
      : this(primaryProperty)
    {
      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(message)), nameof(message));
      MessageText = message;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> or <paramref name="messageDelegate"/> is <see langword="null"/>.</exception>
    public Required(Core.IPropertyInfo primaryProperty, Func<string> messageDelegate )
      : this(primaryProperty)
    {
      if (messageDelegate is null)
        throw new ArgumentNullException(nameof(messageDelegate));
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    protected override string GetMessage()
    {
      return HasMessageDelegate ? MessageText : Properties.Resources.StringRequiredRule;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty!];
      if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
      {
        var message = string.Format(GetMessage(), PrimaryProperty!.FriendlyName);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message, DisplayIndex) { Severity = Severity });
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
    public int Max { get; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="max">Max length value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> is <see langword="null"/>.</exception>
    public MaxLength(Core.IPropertyInfo primaryProperty, int max)
      : base(primaryProperty)
    {
      if (primaryProperty is null)
        throw new ArgumentNullException(nameof(primaryProperty));
      Max = max;
      RuleUri.AddQueryParameter("max", max.ToString());
      InputProperties.Add(primaryProperty);
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="message">The message.</param>
    /// <param name="max">Max length value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public MaxLength(Core.IPropertyInfo primaryProperty, int max, string message)
      : this(primaryProperty, max)
    {
      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(message)), nameof(message));
      MessageText = message;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="max">Max length value.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> or <paramref name="messageDelegate"/> is <see langword="null"/>.</exception>
    public MaxLength(Core.IPropertyInfo primaryProperty, int max, Func<string> messageDelegate)
      : this(primaryProperty, max)
    {
      if (messageDelegate is null)
        throw new ArgumentNullException(nameof(messageDelegate));
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    protected override string GetMessage()
    {
        return HasMessageDelegate ? MessageText : Properties.Resources.StringMaxLengthRule;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty!]?.ToString();
      if (value != null && value.Length > Max)
      {
        var message = string.Format(GetMessage(), PrimaryProperty!.FriendlyName, Max);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message, DisplayIndex) { Severity = Severity });
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
    public int Min { get; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="min">Min length value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> is <see langword="null"/>.</exception>
    public MinLength(Core.IPropertyInfo primaryProperty, int min)
      : base(primaryProperty)
    {
      if (primaryProperty is null)
        throw new ArgumentNullException(nameof(primaryProperty));
      Min = min;
      RuleUri.AddQueryParameter("min", min.ToString());
      InputProperties.Add(primaryProperty);
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="message">The message.</param>
    /// <param name="min">The minimum length.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public MinLength(Core.IPropertyInfo primaryProperty, int min, string message)
      : this(primaryProperty, min)
    {
      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(message)), nameof(message));
      MessageText = message;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="min">Min length value.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> or <paramref name="messageDelegate"/> is <see langword="null"/>.</exception>
    public MinLength(Core.IPropertyInfo primaryProperty, int min, Func<string> messageDelegate)
      : this(primaryProperty, min)
    {
      if (messageDelegate is null)
        throw new ArgumentNullException(nameof(messageDelegate));
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    protected override string GetMessage()
    {
      return HasMessageDelegate ? MessageText : Properties.Resources.StringMinLengthRule;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty!]?.ToString();
      if (value != null && value.Length < Min)
      {
        var message = string.Format(GetMessage(), PrimaryProperty!.FriendlyName, Min);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message, DisplayIndex) { Severity = Severity });
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
    public T Min { get; }
    /// <summary>
    /// Gets or sets the format string used
    /// to format the Min value.
    /// </summary>
    public string Format { get; set; } = "";

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="min">Min value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> or <paramref name="min"/> is <see langword="null"/>.</exception>
    public MinValue(Core.IPropertyInfo primaryProperty, T min) 
      : base(primaryProperty) 
    {
      if (primaryProperty is null)
        throw new ArgumentNullException(nameof(primaryProperty));
      if (min is null)
        throw new ArgumentNullException(nameof(min));

      Min = min;
      RuleUri.AddQueryParameter("min", min.ToString()!);
      InputProperties.Add(primaryProperty);
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="min">Min value.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> or <paramref name="min"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public MinValue(Core.IPropertyInfo primaryProperty, T min,  string message)
      : this(primaryProperty, min)
    {
      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(message)), nameof(message));
      MessageText = message;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="min">Min value.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/>, <paramref name="min"/> or <paramref name="messageDelegate"/> is <see langword="null"/>.</exception>
    public MinValue(Core.IPropertyInfo primaryProperty, T min, Func<string> messageDelegate)
      : this(primaryProperty, min)
    {
      if (messageDelegate is null)
        throw new ArgumentNullException(nameof(messageDelegate));
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    protected override string GetMessage()
    {
      return HasMessageDelegate ? MessageText : Properties.Resources.MinValueRule;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty!] != null
                      ? (T)context.InputPropertyValues[PrimaryProperty!]!
                      : PrimaryProperty!.DefaultValue != null
                        ? (T)PrimaryProperty.DefaultValue
                        : default(T);

      
      if (value is null)
      {
        if (typeof(T) != typeof(string))
        {
          context.AddErrorResult(string.Format(Resources.NullValueInCompareToRule, PrimaryProperty!.Name, nameof(MinValue<T>)));
          return;
        }
        
        value = (T)(object)string.Empty;
      }
      var result = value.CompareTo(Min);
      if (result <= -1)
      {
        string outValue;
        if (string.IsNullOrEmpty(Format))
          outValue = Min.ToString()!;
        else
          outValue = string.Format($"{{0:{Format}}}", Min);
        var message = string.Format(GetMessage(), PrimaryProperty!.FriendlyName, outValue);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message, DisplayIndex) { Severity = Severity });
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
    public T Max { get; }
    /// <summary>
    /// Gets or sets the format string used
    /// to format the Max value.
    /// </summary>
    public string Format { get; set; } = "";

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="max">Max length value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> or <paramref name="max"/> is <see langword="null"/>.</exception>
    public MaxValue(Core.IPropertyInfo primaryProperty, T max)
      : base(primaryProperty)
    {
      if (primaryProperty is null)
        throw new ArgumentNullException(nameof(primaryProperty));
      if (max is null)
        throw new ArgumentNullException(nameof(max));
      
      Max = max;
      RuleUri.AddQueryParameter("max", max.ToString()!);
      InputProperties.Add(primaryProperty);
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="max">Max value.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> or <paramref name="max"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public MaxValue(Core.IPropertyInfo primaryProperty, T max, string message)
      : this(primaryProperty, max)
    {
      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(message)), nameof(message));
      MessageText = message;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="max">Max value.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="messageDelegate"/> is <see langword="null"/>.</exception>
    public MaxValue(Core.IPropertyInfo primaryProperty, T max, Func<string> messageDelegate)
      : this(primaryProperty, max)
    {
      if (messageDelegate is null)
        throw new ArgumentNullException(nameof(messageDelegate));
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    protected override string GetMessage()
    {
      return HasMessageDelegate ? MessageText : Properties.Resources.MaxValueRule;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty!] != null
                      ? (T)context.InputPropertyValues[PrimaryProperty!]!
                      : PrimaryProperty!.DefaultValue != null 
                        ? (T)PrimaryProperty.DefaultValue
                        : default(T);
      
      if (value is null)
      {
        if (typeof(T) != typeof(string))
        {
          context.AddErrorResult(string.Format(Resources.NullValueInCompareToRule, PrimaryProperty!.Name, nameof(MaxValue<T>)));
          return;
        }

        value = (T)(object)string.Empty;
      }
      var result = value.CompareTo(Max);
      if (result >= 1)
      {
        string outValue;
        if (string.IsNullOrEmpty(Format))
          outValue = Max.ToString()!;
        else
          outValue = string.Format($"{{0:{Format}}}", Max);
        var message = string.Format(GetMessage(), PrimaryProperty!.FriendlyName, outValue);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message, DisplayIndex) { Severity = Severity });
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
    public string Expression { get; }

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
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="expression"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public RegExMatch(Core.IPropertyInfo primaryProperty, string expression)
      : base(primaryProperty)
    {
      if (primaryProperty is null)
        throw new ArgumentNullException(nameof(primaryProperty));
      if (string.IsNullOrWhiteSpace(expression))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(expression)), nameof(expression));
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
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="expression"/> or <paramref name="message"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public RegExMatch(Core.IPropertyInfo primaryProperty, string expression, string message)
      : this(primaryProperty, expression)
    {
      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(message)), nameof(message));
      MessageText = message;
    }


    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property to which the rule applies.</param>
    /// <param name="expression">Regular expression.</param>
    /// <param name="messageDelegate">The localizable message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> or <paramref name="messageDelegate"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="expression"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public RegExMatch(Core.IPropertyInfo primaryProperty, string expression, Func<string> messageDelegate)
      : this(primaryProperty, expression)
    {
      if (messageDelegate is null)
        throw new ArgumentNullException(nameof(messageDelegate));
      MessageDelegate = messageDelegate;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    protected override string GetMessage()
    {
      return HasMessageDelegate ? MessageText : Properties.Resources.RegExMatchRule;
    }

    /// <summary>
    /// Rule implementation.
    /// </summary>
    /// <param name="context">Rule context.</param>
    protected override void Execute(IRuleContext context)
    {
      var value = context.InputPropertyValues[PrimaryProperty!];
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
        ruleSatisfied = expression.IsMatch(value.ToString()!);
      }

      if (!ruleSatisfied)
      {
        var message = string.Format(GetMessage(), PrimaryProperty!.FriendlyName);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message, DisplayIndex) { Severity = Severity });
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
    /// <exception cref="InvalidOperationException"><see cref="MessageDelegate"/> is <see langword="null"/>.</exception>
    public string MessageText
    {
      get {
        if (MessageDelegate == null)
          throw new InvalidOperationException($"{nameof(MessageDelegate)} == null");
        return MessageDelegate.Invoke(); 
      }
      protected set { MessageDelegate = () => value; }
    }

    /// <summary>
    /// Gets or sets the localizable message function.
    /// This will override the MessageText if set.
    /// </summary>
    /// <value>
    /// The localizable message.
    /// </value>
    public Func<string>? MessageDelegate { get; set; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property for message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> is <see langword="null"/>.</exception>
    public InfoMessage(Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      if (primaryProperty is null)
        throw new ArgumentNullException(nameof(primaryProperty));
      Priority = 1;
      DisplayIndex = 1;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property for message.</param>
    /// <param name="messageText">Message text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="messageText"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public InfoMessage(Core.IPropertyInfo primaryProperty, string messageText)
      : this(primaryProperty)
    {
      if (string.IsNullOrWhiteSpace(messageText))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(messageText)), nameof(messageText));
      MessageText = messageText;
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Property for message.</param>
    /// <param name="messageDelegate">Message text function.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> or <paramref name="messageDelegate"/> is <see langword="null"/>.</exception>
    public InfoMessage(Core.IPropertyInfo primaryProperty, Func<string> messageDelegate)
      : this(primaryProperty)
    {
      MessageDelegate = messageDelegate ?? throw new ArgumentNullException(nameof(messageDelegate));
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
    private Action<IRuleContext> Rule { get; set; }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="rule">Rule implementation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public Lambda(Action<IRuleContext> rule)
    {
      Rule = rule ?? throw new ArgumentNullException(nameof(rule));
      Initialize(rule);
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Primary property for the rule.</param>
    /// <param name="rule">Rule implementation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> or <paramref name="rule"/> is <see langword="null"/>.</exception>
    public Lambda(Core.IPropertyInfo primaryProperty, Action<IRuleContext> rule)
      : base(primaryProperty)
    {
      if (primaryProperty is null)
        throw new ArgumentNullException(nameof(primaryProperty));
      Rule = rule ?? throw new ArgumentNullException(nameof(rule));
      Initialize(rule);
    }

    private void Initialize(Action<IRuleContext> rule)
    {
      var methodName = rule.Method.ToString()!;
      RuleUri.AddQueryParameter("r", Convert.ToBase64String(Encoding.Unicode.GetBytes(methodName)));
    }

    /// <summary>
    /// Add a query parameter to make the RuleUri uniques for this rule and primary property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is <see langword="null"/>.</exception>
    public void AddQueryParameter(string key, string value)
    {
      if (key is null)
        throw new ArgumentNullException(nameof(key));
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      RuleUri.AddQueryParameter(key, value);
    }

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
    /// <exception cref="ArgumentNullException"><paramref name="primaryProperty"/> or <paramref name="dependencyProperty"/> is <see langword="null"/>.</exception>
    public Dependency(Core.IPropertyInfo primaryProperty, params Core.IPropertyInfo[] dependencyProperty)
      : base(primaryProperty)
    {
      if (primaryProperty is null)
        throw new ArgumentNullException(nameof(primaryProperty));
      if (dependencyProperty is null)
        throw new ArgumentNullException(nameof(dependencyProperty));
      AffectedProperties.AddRange(dependencyProperty);
    }
  }
}