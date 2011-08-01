using System;

namespace Csla.Rules
{

  /// <summary>
  /// Redifines the "old style" rulargs from Csla 3.8.x and previous for use with rule extension
  /// </summary>
  public class RuleArgs
  {
    private string _description;
    private Func<string> _localizableDescription;

    /// <summary>
    /// Gets or sets the description.
    /// If a DescriptionDelegate is set this overries the setter to return the result of the delegate. 
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    public string Description
    {
      get {
        if (_localizableDescription != null)
          return _localizableDescription.Invoke();
        return _description; }
      set
      {
        _description = value;
      }
    }


    /// <summary>
    /// Gets or sets the localizable description delegate function.
    /// </summary>
    /// <value>
    /// The description delegate.
    /// </value>
    public Func<string> LocalizableDescription
    {
      get { return _localizableDescription; }
      set { _localizableDescription = value; }
    }

    /// <summary>
    /// Gets or sets the severity.
    /// </summary>
    /// <value>
    /// The severity.
    /// </value>
    public RuleSeverity Severity { get; set; }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    /// <value>
    /// The priority.
    /// </value>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether rule should shortcircuit processing when broken.
    /// </summary>
    /// <value>
    ///   <c>true</c> stop processing, otherwise, <c>false</c>.
    /// </value>
    public bool StopProcessing { get; set; }

        /// Adds a lambda object level rule to business rules.
    /// <summary>
    /// Initializes a new instance of the <see cref="RuleArgs"/> class
    /// and sets Severity default value to Error. 
    /// </summary>
    public RuleArgs()
    {
      Severity = RuleSeverity.Error;
      StopProcessing = false;
    }
  }

  /// <summary>
  /// Rule extensions for easily migrating to use "old style" rules as lambda rules in 4.x
  /// </summary>
  public static class RuleExtensions
  {
    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">The business rules.</param>
    /// <param name="ruleHandler">The rule handler.</param>
    /// <param name="ruleArgs">The rule args.</param>
    /// <param name="primaryProperty">The primary property.</param>
    public static void AddRule<T>(this BusinessRules businessRules, Func<T, RuleArgs, bool> ruleHandler, RuleArgs ruleArgs,
                                  Csla.Core.IPropertyInfo primaryProperty) where T : BusinessBase<T>
    {
      var rule = new CommonRules.Lambda(primaryProperty, o =>
                                          {
                                            var target = (T) o.Target;
                                            using (target.BypassPropertyChecks)
                                            {
                                              if (!ruleHandler(target, ruleArgs))
                                              {
                                                o.Results.Add(new RuleResult(o.Rule.RuleName, o.Rule.PrimaryProperty, ruleArgs.Description) { Severity = ruleArgs.Severity, StopProcessing = ruleArgs.StopProcessing});
                                              }
                                            }
                                          });
      rule.Priority = ruleArgs.Priority;
      businessRules.AddRule(rule);
    }


    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">The business rules.</param>
    /// <param name="ruleHandler">The rule handler.</param>
    /// <param name="primaryProperty">The primary property.</param>
    public static void AddRule<T>(this BusinessRules businessRules, Func<T, RuleArgs, bool> ruleHandler,
                              Csla.Core.IPropertyInfo primaryProperty) where T : BusinessBase<T>
    {
      AddRule(businessRules, ruleHandler, new RuleArgs(), primaryProperty);
    }

    /// <summary>
    /// Adds a lambda object level rule to business rules.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">The business rules.</param>
    /// <param name="ruleHandler">The rule handler.</param>
    /// <param name="ruleArgs">The rule args.</param>
    public static void AddRule<T>(this BusinessRules businessRules, Func<T, RuleArgs, bool> ruleHandler, RuleArgs ruleArgs) where T : BusinessBase<T>
    {
      var rule = new CommonRules.Lambda(null, o =>
      {
        var target = (T)o.Target;
        using (target.BypassPropertyChecks)
        {
          if (!ruleHandler(target, ruleArgs))
          {
            o.Results.Add(new RuleResult(o.Rule.RuleName, o.Rule.PrimaryProperty, ruleArgs.Description) { Severity = ruleArgs.Severity, StopProcessing = ruleArgs.StopProcessing});
          }
        }    
      });
      rule.Priority = ruleArgs.Priority;
      businessRules.AddRule(rule);
    }

    /// <summary>
    /// Adds a lambda object level rule to business rules.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">The business rules.</param>
    /// <param name="ruleHandler">The rule handler.</param>
    public static void AddRule<T>(this BusinessRules businessRules, Func<T, RuleArgs, bool> ruleHandler) where T : BusinessBase<T>
    {
      AddRule(businessRules, ruleHandler, new RuleArgs());
    }
  }
}
