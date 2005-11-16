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

    #region RuleMethod Class

    /// <summary>
    /// Tracks all information for a rule.
    /// </summary>
    internal class RuleMethod
    {
      private object _target;
      private RuleHandler _handler;
      private string _ruleName = String.Empty;
      private RuleArgs _args;

      /// <summary>
      /// Returns the name of the method implementing the rule
      /// and the property, field or column name to which the
      /// rule applies.
      /// </summary>
      public override string ToString()
      {
        return _ruleName;
      }

      /// <summary>
      /// Gets the name of the rule.
      /// </summary>
      /// <remarks>
      /// The rule's name must be unique and is used
      /// to identify a broken rule in the BrokenRules
      /// collection.
      /// </remarks>
      public string RuleName
      {
        get { return _ruleName; }
      }

      /// <summary>
      /// Returns the name of the field, property or column
      /// to which the rule applies.
      /// </summary>
      public RuleArgs RuleArgs
      {
        get { return _args; }
      }

      /// <summary>
      /// Creates and initializes the rule.
      /// </summary>
      /// <param name="target">Reference to the object containing the data to validate.</param>
      /// <param name="handler">The address of the method implementing the rule.</param>
      /// <param name="propertyName">The field, property or column to which the rule applies.</param>
      public RuleMethod(object target, RuleHandler handler, string propertyName)
      {
        _target = target;
        _handler = handler;
        _ruleName = _handler.Method.Name + "!" + propertyName;
        _args = new RuleArgs(propertyName);
      }

      /// <summary>
      /// Creates and initializes the rule.
      /// </summary>
      /// <param name="target">Reference to the object containing the data to validate.</param>
      /// <param name="handler">The address of the method implementing the rule.</param>
      /// <param name="args">A RuleArgs object.</param>
      public RuleMethod(object target, RuleHandler handler, RuleArgs args)
      {
        _target = target;
        _handler = handler;
        _ruleName = _handler.Method.Name + "!" + args.PropertyName;
        _args = args;
      }

      ///// <summary>
      ///// Creates and initializes the rule.
      ///// </summary>
      ///// <param name="target">Reference to the object containing the data to validate.</param>
      ///// <param name="handler">The address of the method implementing the rule.</param>
      ///// <param name="args">A RuleArgs object.</param>
      ///// <param name="ruleName">A unique name for this rule.</param>
      //public RuleMethod(object target, RuleHandler handler, RuleArgs args, string ruleName)
      //{
      //  _target = target;
      //  _handler = handler;
      //  _ruleName = ruleName;
      //  _args = args;
      //}

      /// <summary>
      /// Invokes the rule to validate the data.
      /// </summary>
      /// <returns>True if the data is valid, False if the data is invalid.</returns>
      public bool Invoke()
      {
        return _handler.Invoke(_target, _args);
      }
    }

    #endregion

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

    ///// <summary>
    ///// Adds a rule to the list of rules to be enforced.
    ///// </summary>
    ///// <remarks>
    ///// <para>
    ///// A rule is implemented by a method which conforms to the 
    ///// method signature defined by the RuleHandler delegate.
    ///// </para><para>
    ///// The ruleName is used to group all the rules that apply
    ///// to a specific field, property or concept. All rules applying
    ///// to the field or property should have the same rule name. When
    ///// rules are checked, they can be checked globally or for a 
    ///// specific ruleName.
    ///// </para><para>
    ///// The propertyName may be used by the method that implements the rule
    ///// in order to retrieve the value to be validated. If the rule
    ///// implementation is inside the target object then it probably has
    ///// direct access to all data. However, if the rule implementation
    ///// is outside the target object then it will need to use reflection
    ///// or CallByName to dynamically invoke this property to retrieve
    ///// the value to be validated.
    ///// </para>
    ///// </remarks>
    ///// <param name="handler">The method that implements the rule.</param>
    ///// <param name="args">
    ///// A RuleArgs object specifying the property name and other arguments
    ///// passed to the rule method
    ///// </param>
    ///// <param name="ruleName">Unique name for the rule.</param>
    //public void AddRule(RuleHandler handler, RuleArgs args, string ruleName)
    //{
    //  // get the list of rules for the property
    //  List<RuleMethod> list = GetRulesForProperty(args.PropertyName);

    //  // we have the list, add our new rule
    //  list.Add(new RuleMethod(_target, handler, args, ruleName));
    //}

    #endregion

    #region Checking Rules

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
    /// at this time. If there are broken rules, the business object
    /// is assumed to be invalid and False is returned. If there are no
    /// broken business rules True is returned.
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