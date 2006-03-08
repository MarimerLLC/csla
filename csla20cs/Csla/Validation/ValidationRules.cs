using System;
using System.Collections.Generic;

namespace Csla.Validation
{

  /// <summary>
  /// Tracks the business rules broken within a business object.
  /// </summary>
  [Serializable()]
  public class ValidationRules
  {
    private BrokenRulesCollection _brokenRules;
    [NonSerialized()]
    private object _target;
    [NonSerialized()]
    private Dictionary<string, List<RuleMethod>> _rulesList;

    internal ValidationRules(object businessObject)
    {
      SetTarget(businessObject);
    }

    internal void SetTarget(object businessObject)
    {
      _target = businessObject;
    }

    private BrokenRulesCollection BrokenRulesList
    {
      get
      {
        if (_brokenRules == null)
          _brokenRules = new BrokenRulesCollection();
        return _brokenRules;
      }
    }

    private Dictionary<string, List<RuleMethod>> RulesList
    {
      get
      {
        if (_rulesList == null)
          _rulesList = new Dictionary<string, List<RuleMethod>>();
        return _rulesList;
      }
    }

    #region Adding Rules

    /// <summary>
    /// Returns the list containing rules for a rule name. If
    /// no list exists one is created and returned.
    /// </summary>
    private List<RuleMethod> GetRulesForProperty(string propertyName)
    {
      // get the list (if any) from the dictionary
      List<RuleMethod> list = null;
      if (RulesList.ContainsKey(propertyName))
        list = RulesList[propertyName];
      if (list == null)
      {
        // there is no list for this name - create one
        list = new List<RuleMethod>();
        RulesList.Add(propertyName, list);
      }
      return list;
    }

    /// <summary>
    /// Adds a rule to the list of rules to be enforced.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A rule is implemented by a method which conforms to the 
    /// method signature defined by the RuleHandler delegate.
    /// </para><para>
    /// The propertyName may be used by the method that implements the rule
    /// in order to retrieve the value to be validated. If the rule
    /// implementation is inside the target object then it probably has
    /// direct access to all data. However, if the rule implementation
    /// is outside the target object then it will need to use reflection
    /// or CallByName to dynamically invoke this property to retrieve
    /// the value to be validated.
    /// </para>
    /// </remarks>
    /// <param name="handler">The method that implements the rule.</param>
    /// <param name="propertyName">
    /// The property name on the target object where the rule implementation can retrieve
    /// the value to be validated.
    /// </param>
    public void AddRule(RuleHandler handler, string propertyName)
    {
      // get the list of rules for the property
      List<RuleMethod> list = GetRulesForProperty(propertyName);

      // we have the list, add our new rule
      list.Add(new RuleMethod(_target, handler, propertyName));
    }

    /// <summary>
    /// Adds a rule to the list of rules to be enforced.
    /// </summary>
    /// <remarks>
    /// A rule is implemented by a method which conforms to the 
    /// method signature defined by the RuleHandler delegate.
    /// </remarks>
    /// <param name="handler">The method that implements the rule.</param>
    /// <param name="args">
    /// A RuleArgs object specifying the property name and other arguments
    /// passed to the rule method
    /// </param>
    public void AddRule(RuleHandler handler, RuleArgs args)
    {
      // get the list of rules for the property
      List<RuleMethod> list = GetRulesForProperty(args.PropertyName);

      // we have the list, add our new rule
      list.Add(new RuleMethod(_target, handler, args));
    }

    #endregion

    #region Checking Rules

    /// <summary>
    /// Invokes all rule methods associated with
    /// the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property to validate.</param>
    public void CheckRules(string propertyName)
    {
      List<RuleMethod> list;
      // get the list of rules to check
      if (RulesList.ContainsKey(propertyName))
      {
        list = RulesList[propertyName];
        if (list == null)
          return;

        // now check the rules
        foreach (RuleMethod rule in list)
        {
          if (rule.Invoke())
            BrokenRulesList.Remove(rule);
          else
            BrokenRulesList.Add(rule);
        }
      }
    }

    /// <summary>
    /// Invokes all rule methods for all properties
    /// in the object.
    /// </summary>
    public void CheckRules()
    {
      // get the rules for each rule name
      foreach (KeyValuePair<string, List<RuleMethod>> de in RulesList)
      {
        List<RuleMethod> list = de.Value;

        // now check the rules
        foreach (RuleMethod rule in list)
        {
          if (rule.Invoke())
            BrokenRulesList.Remove(rule);
          else
            BrokenRulesList.Add(rule);
        }
      }
    }

    #endregion

    #region Status Retrieval

    /// <summary>
    /// Returns a value indicating whether there are any broken rules
    /// at this time. 
    /// </summary>
    /// <returns>A value indicating whether any rules are broken.</returns>
    internal bool IsValid
    {
      get { return BrokenRulesList.Count == 0; }
    }

    /// <summary>
    /// Returns a reference to the readonly collection of broken
    /// business rules.
    /// </summary>
    /// <remarks>
    /// The reference returned points to the actual collection object.
    /// This means that as rules are marked broken or unbroken over time,
    /// the underlying data will change. Because of this, the UI developer
    /// can bind a display directly to this collection to get a dynamic
    /// display of the broken rules at all times.
    /// </remarks>
    /// <returns>A reference to the collection of broken rules.</returns>
    public BrokenRulesCollection GetBrokenRules()
    {
      return BrokenRulesList;
    }

    #endregion

  }
}