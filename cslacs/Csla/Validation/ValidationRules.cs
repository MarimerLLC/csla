using System;
using System.Linq;
using System.Collections.Generic;
using Csla.Serialization;
using Csla.Core;
using Csla.Serialization.Mobile;
using System.Collections.ObjectModel;

namespace Csla.Validation
{

  /// <summary>
  /// Tracks the business rules broken within a business object.
  /// </summary>
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [Serializable()]
  public partial class ValidationRules : MobileObject
  {
    // list of broken rules for this business object.
    private BrokenRulesCollection _brokenRules;
    // threshold for short-circuiting to kick in
    private int _processThroughPriority;
    // reference to current business object
    [NonSerialized()]
    private object _target;
    // reference to per-instance rules manager for this object
    [NonSerialized()]
    private ValidationRulesManager _instanceRules;
    // reference to per-type rules manager for this object
    [NonSerialized()]
    private ValidationRulesManager _typeRules;
    // reference to the active set of rules for this object
    [NonSerialized()]
    private ValidationRulesManager _rulesToCheck;

    [NonSerialized]
    private ObservableCollection<IAsyncRuleMethod> _validatingRules;

    //used to synchronize various async operations
    private object SyncRoot = new object();

    internal ValidationRules(object businessObject)
    {
      SetTarget(businessObject);
    }

    internal void SetTarget(object businessObject)
    {
      _target = businessObject;
    }

    internal object Target
    {
      get { return _target; }
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

    internal ObservableCollection<IAsyncRuleMethod> ValidatingRules
    {
      get
      {
        if (_validatingRules == null)
          _validatingRules = new ObservableCollection<IAsyncRuleMethod>();
        
        return _validatingRules;
      }
    }

    private ValidationRulesManager GetInstanceRules(bool createObject)
    {
      if (_instanceRules == null)
        if (createObject)
          _instanceRules = new ValidationRulesManager();
      return _instanceRules;
    }

    private ValidationRulesManager GetTypeRules(bool createObject)
    {
      if (_typeRules == null)
        _typeRules = SharedValidationRules.GetManager(_target.GetType(), createObject);
      return _typeRules;
    }

    private ValidationRulesManager RulesToCheck
    {
      get
      {
        if (_rulesToCheck == null)
        {
          ValidationRulesManager instanceRules = GetInstanceRules(false);
          ValidationRulesManager typeRules = GetTypeRules(false);
          if (instanceRules == null)
          {
            if (typeRules == null)
              _rulesToCheck = null;
            else
              _rulesToCheck = typeRules;
          }
          else if (typeRules == null)
            _rulesToCheck = instanceRules;
          else
          {
            // both have values - consolidate into instance rules
            _rulesToCheck = instanceRules;
            foreach (KeyValuePair<string, RulesList> de in typeRules.RulesDictionary)
            {
              RulesList rules = _rulesToCheck.GetRulesForProperty(de.Key, true);
              List<IRuleMethod> instanceList = rules.GetList(false);
              instanceList.AddRange(de.Value.GetList(false));
              List<string> dependancy = de.Value.GetDependancyList(false);
              if (dependancy != null)
                rules.GetDependancyList(true).AddRange(dependancy);
            }
          }
        }
        return _rulesToCheck;
      }
    }

    /// <summary>
    /// Returns an array containing the text descriptions of all
    /// validation rules associated with this object.
    /// </summary>
    /// <returns>String array.</returns>
    /// <remarks></remarks>
    public string[] GetRuleDescriptions()
    {
      List<string> result = new List<string>();
      ValidationRulesManager rules = RulesToCheck;
      if (rules != null)
      {
        foreach (KeyValuePair<string, RulesList> de in rules.RulesDictionary)
        {
          List<IRuleMethod> list = de.Value.GetList(false);
          for (int i = 0; i < list.Count; i++)
          {
            IRuleMethod rule = list[i];
            result.Add(rule.ToString());
          }
        }
      }
      return result.ToArray();
    }

    #region Short-Circuiting

    /// <summary>
    /// Gets or sets the priority through which
    /// CheckRules should process before short-circuiting
    /// processing on broken rules.
    /// </summary>
    /// <value>Defaults to 0.</value>
    /// <remarks>
    /// All rules for each property are processed by CheckRules
    /// though this priority. Rules with lower priorities are
    /// only processed if no previous rule has been marked as
    /// broken.
    /// </remarks>
    public int ProcessThroughPriority
    {
      get { return _processThroughPriority; }
      set { _processThroughPriority = value; }
    }

    #endregion

    #region Adding Instance Rules

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
    public void AddInstanceRule(RuleHandler handler, string propertyName)
    {
      GetInstanceRules(true).AddRule(handler, new RuleArgs(propertyName), 0);
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
    /// <param name="priority">
    /// The priority of the rule, where lower numbers are processed first.
    /// </param>
    public void AddInstanceRule(RuleHandler handler, string propertyName, int priority)
    {
      GetInstanceRules(true).AddRule(handler, new RuleArgs(propertyName), priority);
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
    /// <typeparam name="T">Type of the business object to be validated.</typeparam>
    public void AddInstanceRule<T>(RuleHandler<T, RuleArgs> handler, string propertyName)
    {
      GetInstanceRules(true).AddRule<T, RuleArgs>(handler, new RuleArgs(propertyName), 0);
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
    /// <param name="priority">
    /// The priority of the rule, where lower numbers are processed first.
    /// </param>
    /// <typeparam name="T">Type of the business object to be validated.</typeparam>
    public void AddInstanceRule<T>(RuleHandler<T, RuleArgs> handler, string propertyName, int priority)
    {
      GetInstanceRules(true).AddRule<T, RuleArgs>(handler, new RuleArgs(propertyName), priority);
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
    public void AddInstanceRule(RuleHandler handler, RuleArgs args)
    {
      GetInstanceRules(true).AddRule(handler, args, 0);
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
    /// <param name="priority">
    /// The priority of the rule, where lower numbers are processed first.
    /// </param>
    public void AddInstanceRule(RuleHandler handler, RuleArgs args, int priority)
    {
      GetInstanceRules(true).AddRule(handler, args, priority);
    }

    /// <summary>
    /// Adds a rule to the list of rules to be enforced.
    /// </summary>
    /// <remarks>
    /// A rule is implemented by a method which conforms to the 
    /// method signature defined by the RuleHandler delegate.
    /// </remarks>
    /// <typeparam name="T">Type of the target object.</typeparam>
    /// <typeparam name="R">Type of the arguments parameter.</typeparam>
    /// <param name="handler">The method that implements the rule.</param>
    /// <param name="args">
    /// A RuleArgs object specifying the property name and other arguments
    /// passed to the rule method
    /// </param>
    public void AddInstanceRule<T, R>(RuleHandler<T, R> handler, R args) where R : RuleArgs
    {
      GetInstanceRules(true).AddRule(handler, args, 0);
    }

    /// <summary>
    /// Adds a rule to the list of rules to be enforced.
    /// </summary>
    /// <remarks>
    /// A rule is implemented by a method which conforms to the 
    /// method signature defined by the RuleHandler delegate.
    /// </remarks>
    /// <typeparam name="T">Type of the target object.</typeparam>
    /// <typeparam name="R">Type of the arguments parameter.</typeparam>
    /// <param name="handler">The method that implements the rule.</param>
    /// <param name="args">
    /// A RuleArgs object specifying the property name and other arguments
    /// passed to the rule method
    /// </param>
    /// <param name="priority">
    /// The priority of the rule, where lower numbers are processed first.
    /// </param>
    public void AddInstanceRule<T, R>(RuleHandler<T, R> handler, R args, int priority) where R : RuleArgs
    {
      GetInstanceRules(true).AddRule(handler, args, priority);
    }

    #endregion

    #region  Adding Shared Rules

    /// <summary>
    /// Adds an async rule to the list of rules to be enforced.
    /// </summary>
    /// <param name="handler">
    /// The method that implements the rule.
    /// </param>
    /// <param name="primaryProperty">
    /// The primary property checked by this rule.
    /// </param>
    /// <param name="additionalProperties">
    /// A list of other property values required by
    /// this rule method.
    /// </param>
    public void AddRule(AsyncRuleHandler handler, IPropertyInfo primaryProperty, params IPropertyInfo[] additionalProperties)
    {
      AddRule(handler, new AsyncRuleArgs(primaryProperty, additionalProperties));
    }

    /// <summary>
    /// Adds an async rule to the list of rules to be enforced.
    /// </summary>
    /// <param name="handler">
    /// The method that implements the rule.
    /// </param>
    /// <param name="primaryProperty">
    /// The primary property checked by this rule.
    /// </param>
    /// <param name="priority">Priority for the rule</param>
    public void AddRule(AsyncRuleHandler handler, IPropertyInfo primaryProperty, int priority)
    {
      AddRule(handler, new AsyncRuleArgs(primaryProperty, null), priority);
    }

    /// <summary>
    /// Adds an async rule to the list of rules to be enforced.
    /// </summary>
    /// <param name="handler">
    /// The method that implements the rule.
    /// </param>
    /// <param name="args">
    /// An AsyncRuleArgs object specifying the property name and other arguments
    /// passed to the rule method
    /// </param>
    public void AddRule(AsyncRuleHandler handler, AsyncRuleArgs args)
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule(handler, args, 0);
    }

    /// <summary>
    /// Adds an async rule to the list of rules to be enforced.
    /// </summary>
    /// <param name="handler">
    /// The method that implements the rule.
    /// </param>
    /// <param name="args">
    /// An AsyncRuleArgs object specifying the property name and other arguments
    /// passed to the rule method
    /// </param>
    /// <param name="priority">Priority for the rule</param>
    public void AddRule(AsyncRuleHandler handler, AsyncRuleArgs args, int priority)
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule(handler, args, priority);
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
    /// <param name="propertyInfo">
    /// The PropertyInfo object describing the property.
    /// </param>
    public void AddRule(RuleHandler handler, Core.IPropertyInfo propertyInfo)
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule(handler, new RuleArgs(propertyInfo), 0);
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
      ValidateHandler(handler);
      GetTypeRules(true).AddRule(handler, new RuleArgs(propertyName), 0);
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
    /// <param name="propertyInfo">
    /// The PropertyInfo object describing the property.
    /// </param>
    /// <param name="priority">
    /// The priority of the rule, where lower numbers are processed first.
    /// </param>
    public void AddRule(RuleHandler handler, Core.IPropertyInfo propertyInfo, int priority)
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule(handler, new RuleArgs(propertyInfo), priority);
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
    /// <param name="priority">
    /// The priority of the rule, where lower numbers are processed first.
    /// </param>
    public void AddRule(RuleHandler handler, string propertyName, int priority)
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule(handler, new RuleArgs(propertyName), priority);
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
    /// <param name="propertyInfo">
    /// The PropertyInfo object describing the property.
    /// </param>
    public void AddRule<T>(RuleHandler<T, RuleArgs> handler, Core.IPropertyInfo propertyInfo)
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule<T, RuleArgs>(handler, new RuleArgs(propertyInfo), 0);
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
    public void AddRule<T>(RuleHandler<T, RuleArgs> handler, string propertyName)
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule<T, RuleArgs>(handler, new RuleArgs(propertyName), 0);
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
    /// <param name="args">
    /// A RuleArgs object specifying the property name and other arguments
    /// passed to the rule method
    /// </param>
    public void AddRule<T>(RuleHandler<T, RuleArgs> handler, RuleArgs args)
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule<T, RuleArgs>(handler, args, 0);
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
    /// <param name="propertyInfo">
    /// The PropertyInfo object describing the property.
    /// </param>
    /// <param name="priority">
    /// The priority of the rule, where lower numbers are processed first.
    /// </param>
    public void AddRule<T>(RuleHandler<T, RuleArgs> handler, Core.IPropertyInfo propertyInfo, int priority)
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule<T, RuleArgs>(handler, new RuleArgs(propertyInfo), priority);
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
    /// <param name="priority">
    /// The priority of the rule, where lower numbers are processed first.
    /// </param>
    public void AddRule<T>(RuleHandler<T, RuleArgs> handler, string propertyName, int priority)
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule<T, RuleArgs>(handler, new RuleArgs(propertyName), priority);
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
      ValidateHandler(handler);
      GetTypeRules(true).AddRule(handler, args, 0);
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
    /// <param name="priority">
    /// The priority of the rule, where lower numbers are processed first.
    /// </param>
    public void AddRule(RuleHandler handler, RuleArgs args, int priority)
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule(handler, args, priority);
    }

    /// <summary>
    /// Adds a rule to the list of rules to be enforced.
    /// </summary>
    /// <remarks>
    /// A rule is implemented by a method which conforms to the 
    /// method signature defined by the RuleHandler delegate.
    /// </remarks>
    /// <typeparam name="T">Type of the target object.</typeparam>
    /// <typeparam name="R">Type of the arguments parameter.</typeparam>
    /// <param name="handler">The method that implements the rule.</param>
    /// <param name="args">
    /// A RuleArgs object specifying the property name and other arguments
    /// passed to the rule method
    /// </param>
    public void AddRule<T, R>(RuleHandler<T, R> handler, R args) where R : RuleArgs
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule(handler, args, 0);
    }

    /// <summary>
    /// Adds a rule to the list of rules to be enforced.
    /// </summary>
    /// <remarks>
    /// A rule is implemented by a method which conforms to the 
    /// method signature defined by the RuleHandler delegate.
    /// </remarks>
    /// <typeparam name="T">Type of the target object.</typeparam>
    /// <typeparam name="R">Type of the arguments parameter.</typeparam>
    /// <param name="handler">The method that implements the rule.</param>
    /// <param name="args">
    /// A RuleArgs object specifying the property name and other arguments
    /// passed to the rule method
    /// </param>
    /// <param name="priority">
    /// The priority of the rule, where lower numbers are processed first.
    /// </param>
    public void AddRule<T, R>(RuleHandler<T, R> handler, R args, int priority) where R : RuleArgs
    {
      ValidateHandler(handler);
      GetTypeRules(true).AddRule(handler, args, priority);
    }

    private bool ValidateHandler(AsyncRuleHandler handler)
    {
      return ValidateHandler(handler.Method);
    }

    private bool ValidateHandler(RuleHandler handler)
    {
      return ValidateHandler(handler.Method);
    }

    private bool ValidateHandler<T, R>(RuleHandler<T, R> handler) where R : RuleArgs
    {
      return ValidateHandler(handler.Method);
    }

    private bool ValidateHandler(System.Reflection.MethodInfo method)
    {
      if (!method.IsStatic && method.DeclaringType.IsInstanceOfType(_target))
        throw new InvalidOperationException(string.Format("{0}: {1}", Properties.Resources.InvalidRuleMethodException, method.Name));
      return true;
    }

    #endregion

    #region  Adding per-type dependencies

    /// <summary>
    /// Adds a property to the list of dependencies for
    /// the specified property
    /// </summary>
    /// <param name="propertyInfo">
    /// PropertyInfo for the property.
    /// </param>
    /// <param name="dependentPropertyInfo">
    /// PropertyInfo for the depandent property.
    /// </param>
    /// <remarks>
    /// When rules are checked for propertyName, they will
    /// also be checked for any dependent properties associated
    /// with that property.
    /// </remarks>
    public void AddDependentProperty(Core.IPropertyInfo propertyInfo, Core.IPropertyInfo dependentPropertyInfo)
    {
      GetTypeRules(true).AddDependentProperty(propertyInfo.Name, dependentPropertyInfo.Name);
    }

    /// <summary>
    /// Adds a property to the list of dependencies for
    /// the specified property
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property.
    /// </param>
    /// <param name="dependentPropertyName">
    /// The name of the depandent property.
    /// </param>
    /// <remarks>
    /// When rules are checked for propertyName, they will
    /// also be checked for any dependent properties associated
    /// with that property.
    /// </remarks>
    public void AddDependentProperty(string propertyName, string dependentPropertyName)
    {
      GetTypeRules(true).AddDependentProperty(propertyName, dependentPropertyName);
    }

    /// <summary>
    /// Adds a property to the list of dependencies for
    /// the specified property
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property.
    /// </param>
    /// <param name="dependantPropertyName">
    /// The name of the depandent property.
    /// </param>
    /// <remarks>
    /// When rules are checked for propertyName, they will
    /// also be checked for any dependent properties associated
    /// with that property.
    /// </remarks>
    [Obsolete("Use AddDependentProperty")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void AddDependantProperty(string propertyName, string dependantPropertyName)
    {
      AddDependentProperty(propertyName, dependantPropertyName);
    }

    /// <summary>
    /// Adds a property to the list of dependencies for
    /// the specified property
    /// </summary>
    /// <param name="propertyInfo">
    /// PropertyInfo for the property.
    /// </param>
    /// <param name="dependentPropertyInfo">
    /// PropertyInfo for the depandent property.
    /// </param>
    /// <param name="isBidirectional">
    /// If <see langword="true"/> then a 
    /// reverse dependancy is also established
    /// from dependentPropertyName to propertyName.
    /// </param>
    /// <remarks>
    /// When rules are checked for propertyName, they will
    /// also be checked for any dependent properties associated
    /// with that property. If isBidirectional is 
    /// <see langword="true"/> then an additional association
    /// is set up so when rules are checked for
    /// dependentPropertyName the rules for propertyName
    /// will also be checked.
    /// </remarks>
    public void AddDependentProperty(Core.IPropertyInfo propertyInfo, Core.IPropertyInfo dependentPropertyInfo, bool isBidirectional)
    {
      ValidationRulesManager mgr = GetTypeRules(true);
      mgr.AddDependentProperty(propertyInfo.Name, dependentPropertyInfo.Name);
      if (isBidirectional)
        mgr.AddDependentProperty(dependentPropertyInfo.Name, propertyInfo.Name);
    }

    /// <summary>
    /// Adds a property to the list of dependencies for
    /// the specified property
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property.
    /// </param>
    /// <param name="dependentPropertyName">
    /// The name of the depandent property.
    /// </param>
    /// <param name="isBidirectional">
    /// If <see langword="true"/> then a 
    /// reverse dependancy is also established
    /// from dependentPropertyName to propertyName.
    /// </param>
    /// <remarks>
    /// When rules are checked for propertyName, they will
    /// also be checked for any dependent properties associated
    /// with that property. If isBidirectional is 
    /// <see langword="true"/> then an additional association
    /// is set up so when rules are checked for
    /// dependentPropertyName the rules for propertyName
    /// will also be checked.
    /// </remarks>
    public void AddDependentProperty(string propertyName, string dependentPropertyName, bool isBidirectional)
    {
      ValidationRulesManager mgr = GetTypeRules(true);
      mgr.AddDependentProperty(propertyName, dependentPropertyName);
      if (isBidirectional)
        mgr.AddDependentProperty(dependentPropertyName, propertyName);
    }

    /// <summary>
    /// Adds a property to the list of dependencies for
    /// the specified property
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property.
    /// </param>
    /// <param name="dependantPropertyName">
    /// The name of the depandent property.
    /// </param>
    /// <param name="isBidirectional">
    /// If <see langword="true"/> then a 
    /// reverse dependancy is also established
    /// from dependantPropertyName to propertyName.
    /// </param>
    /// <remarks>
    /// When rules are checked for propertyName, they will
    /// also be checked for any dependent properties associated
    /// with that property. If isBidirectional is 
    /// <see langword="true"/> then an additional association
    /// is set up so when rules are checked for
    /// dependantPropertyName the rules for propertyName
    /// will also be checked.
    /// </remarks>
    [Obsolete("Use AddDependentProperty")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void AddDependantProperty(string propertyName, string dependantPropertyName, bool isBidirectional)
    {
      ValidationRulesManager mgr = GetTypeRules(true);
      mgr.AddDependentProperty(propertyName, dependantPropertyName);
      if (isBidirectional)
        mgr.AddDependentProperty(dependantPropertyName, propertyName);
    }

    #endregion

    #region DataAnnotations

    /// <summary>
    /// Adds validation rules corresponding to property
    /// data annotation attributes.
    /// </summary>
    public void AddDataAnnotations()
    {
      AddDataAnnotations(null);
    }

    /// <summary>
    /// Adds validation rules corresponding to property
    /// data annotation attributes.
    /// </summary>
    /// <param name="ruleAdder">
    /// Method invoked to add rules as data annotation
    /// attributes are found on properties.
    /// </param>
    public void AddDataAnnotations(EventHandler<AddRuleArgs> ruleAdder)
    {
      Type metadataType;
#if SILVERLIGHT
      metadataType = _target.GetType();
#else
      var classAttList = _target.GetType().GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.MetadataTypeAttribute), true);
      if (classAttList.Length > 0)
        metadataType = ((System.ComponentModel.DataAnnotations.MetadataTypeAttribute)classAttList[0]).MetadataClassType;
      else
        metadataType = _target.GetType();
#endif

      var propList = metadataType.GetProperties();
      foreach (var prop in propList)
      {
        var attList = prop.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.ValidationAttribute), true);
        foreach (var att in attList)
        {
          bool added = false;
          if (ruleAdder != null)
          {
            var args = new AddRuleArgs { BusinessObject = _target, PropertyInfo = prop, Attribute = att };
            ruleAdder(this, args);
            added = args.RuleAdded;
          }
          if (!added)
          {
            AddRule(CommonRules.DataAnnotation,
              new CommonRules.DataAnnotationRuleArgs(
                prop.Name, 
                (System.ComponentModel.DataAnnotations.ValidationAttribute)att));
          }
        }
      }
    }

    #endregion

    #region Checking Rules

    private bool _suppressRuleChecking;

    /// <summary>
    /// Gets or sets a value indicating whether calling
    /// CheckRules should result in rule
    /// methods being invoked.
    /// </summary>
    /// <value>True to suppress all rule method invocation.</value>
    public bool SuppressRuleChecking
    {
      get { return _suppressRuleChecking; }
      set { _suppressRuleChecking = value; }
    }

    /// <summary>
    /// Invokes all rule methods associated with
    /// the specified property and any 
    /// dependent properties.
    /// </summary>
    /// <param name="propertyInfo">
    /// Property to validate.
    /// </param>
    public void CheckRules(Csla.Core.IPropertyInfo propertyInfo)
    {
      CheckRules(propertyInfo.Name);
    }

    /// <summary>
    /// Invokes all rule methods associated with
    /// the specified property and any 
    /// dependent properties.
    /// </summary>
    /// <param name="propertyName">The name of the property to validate.</param>
    public string[] CheckRules(string propertyName)
    {
      if (_suppressRuleChecking)
        return new string[] {};

      var result = new List<string>();
      result.Add(propertyName);

      // get the rules dictionary
      ValidationRulesManager rules = RulesToCheck;
      if (rules != null)
      {
        // get the rules list for this property
        RulesList rulesList = rules.GetRulesForProperty(propertyName, false);
        if (rulesList != null)
        {
          // get the actual list of rules (sorted by priority)
          List<IRuleMethod> list = rulesList.GetList(true);
          if (list != null)
            CheckRules(list);
          List<string> dependancies = rulesList.GetDependancyList(false);
          if (dependancies != null)
          {
            for (int i = 0; i < dependancies.Count; i++)
            {
              string dependentProperty = dependancies[i];
              result.Add(dependentProperty);
              CheckRules(rules, dependentProperty);
            }
          }
        }
      }
      return result.ToArray();
    }

    private void CheckRules(ValidationRulesManager rules, string propertyName)
    {
      // get the rules list for this property
      RulesList rulesList = rules.GetRulesForProperty(propertyName, false);
      if (rulesList != null)
      {
        // get the actual list of rules (sorted by priority)
        List<IRuleMethod> list = rulesList.GetList(true);
        if (list != null)
          CheckRules(list);
      }
    }

    /// <summary>
    /// Invokes all rule methods for all properties
    /// in the object.
    /// </summary>
    public void CheckRules()
    {
      if (_suppressRuleChecking)
        return;

      ValidationRulesManager rules = RulesToCheck;
      if (rules != null)
      {
        foreach (KeyValuePair<string, RulesList> de in rules.RulesDictionary)
          CheckRules(de.Value.GetList(true));
      }
    }

    /// <summary>
    /// Given a list
    /// containing IRuleMethod objects, this
    /// method executes all those rule methods.
    /// </summary>
    private void CheckRules(List<IRuleMethod> list)
    {
      bool previousRuleBroken = false;
      bool shortCircuited = false;

      // Lock the rules here to ensure that all rules are run before allowing
      // async rules to notify that they have completed.

      for (int index = 0; index < list.Count; index++)
      {
        IRuleMethod rule = list[index];
        // see if short-circuiting should kick in
        if (!shortCircuited && (previousRuleBroken && rule.Priority > _processThroughPriority))
          shortCircuited = true;

        if (shortCircuited)
        {
          // we're short-circuited, so just remove
          // all remaining broken rule entries
          lock (SyncRoot)
            BrokenRulesList.Remove(rule);
        }
        else
        {
          // we're not short-circuited, so check rule
          bool ruleResult;

          IAsyncRuleMethod asyncRule = rule as IAsyncRuleMethod;
          if (asyncRule != null)
          {
            lock (SyncRoot)
              ValidatingRules.Add(asyncRule);

            lock (SyncRoot)
              BrokenRulesList.Remove(rule);
          }
          else
          {
            try
            {
              ruleResult = rule.Invoke(_target);
            }
            catch (Exception ex)
            {
              //// force a broken rule
              //ruleResult = false;
              //rule.RuleArgs.Severity = RuleSeverity.Error;
              //rule.RuleArgs.Description = 
              //  string.Format(Properties.Resources.ValidationRuleException & "{{2}}", rule.RuleArgs.PropertyName, rule.RuleName, ex.Message);
              // throw a more detailed exception
              throw new ValidationException(
                string.Format(Properties.Resources.ValidationRulesException, rule.RuleArgs.PropertyName, rule.RuleName), ex);
            }

            lock (SyncRoot)
            {
              if (ruleResult)
              {
                // the rule is not broken
                BrokenRulesList.Remove(rule);
              }
              else
              {
                // the rule is broken
                BrokenRulesList.Add(rule);
                if (rule.RuleArgs.Severity == RuleSeverity.Error)
                  previousRuleBroken = true;
              }
            }
            if (rule.RuleArgs.StopProcessing)
            {
              shortCircuited = true;
              // reset the value for next time
              rule.RuleArgs.StopProcessing = false;
            }
          }
        }
      }

      IAsyncRuleMethod[] asyncRules = null;
      lock (SyncRoot)
        asyncRules = ValidatingRules.ToArray();
      
      // They must all be added to the ValidatingRules list before you can invoke them.
      // Otherwise you have a race condition, where if a rule completes before the next one is invoked
      // then you may have the ValidationComplete event fire multiple times invalidly.
      foreach (IAsyncRuleMethod rule in asyncRules)
      {
        try
        {
          rule.Invoke(_target, asyncRule_Complete);
        }
        catch (Exception ex)
        {
          // throw a more detailed exception
          throw new ValidationException(
            string.Format(Properties.Resources.ValidationRulesException, rule.RuleArgs.PropertyName, rule.RuleName), ex);
        }
      }

      if (asyncRules.Length == 0)
      {
        var target = _target as Csla.Core.BusinessBase;
        if (target != null)
          target.OnValidationComplete();
      }
    }

    void asyncRule_Complete(object target, AsyncRuleResult e)
    {
      IAsyncRuleMethod rule = (IAsyncRuleMethod)target;

      lock (SyncRoot)
      {
        if (e.Result)
          BrokenRulesList.Remove(rule);
        else
          BrokenRulesList.Add(rule, e);
      }

      // remove from rules list after broken rules so that IsValid is 
      // correct in any async handlers.
      lock (SyncRoot)
        ValidatingRules.Remove(rule);
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
      get 
      {
        lock (SyncRoot)
          return BrokenRulesList.ErrorCount == 0; 
      }
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
      // TODO: I'm not sure this is actually thread safe... We might have to change
      // this to a BrokenRule[]. Otherwise we could remove locking from this class
      // and lock inside of the BrokenRuleCollection itself.
      return BrokenRulesList;
    }

    internal bool IsValidating
    {
      get
      {
        lock (SyncRoot)
          return ValidatingRules.Count > 0;
      }
    }

    #endregion

    #region MobileObject overrides

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    protected override void OnGetState(SerializationInfo info, StateMode mode)
    {
      info.AddValue("_processThroughPriority", _processThroughPriority);
#if SILVERLIGHT
      OnGetStatePartial(info, mode);
#endif
      base.OnGetState(info, mode);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      _processThroughPriority = info.GetValue<int>("_processThroughPriority");
#if SILVERLIGHT
      OnSetStatePartial(info, mode);
#endif
      base.OnSetState(info, mode);
    }

    /// <summary>
    /// Override this method to insert your child object
    /// references into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (_brokenRules != null && _brokenRules.Count > 0)
      {
        SerializationInfo brInfo = formatter.SerializeObject(_brokenRules);
        info.AddChild("_brokenRules", brInfo.ReferenceId);
      }

      base.OnGetChildren(info, formatter);
    }

    /// <summary>
    /// Override this method to retrieve your child object
    /// references from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info.Children.ContainsKey("_brokenRules"))
      {
        int referenceId = info.Children["_brokenRules"].ReferenceId;
        _brokenRules = (BrokenRulesCollection)formatter.GetObject(referenceId);
      }

      base.OnSetChildren(info, formatter);
    }
    #endregion
  }
}