using System;
using System.Collections;
using System.Collections.Specialized;
using CSLA.Resources;

namespace CSLA
{
  /// <summary>
  /// Tracks the business rules broken within a business object.
  /// </summary>
  [Serializable()]
  public class BrokenRules
  {
    #region Rule structure

    /// <summary>
    /// Stores details about a specific broken business rule.
    /// </summary>
    [Serializable()]
      public struct Rule
    {
      string _name;
      string _description;
      string _property;

      internal Rule(string name, string description)
      {
        _name = name;
        _description = description;
        _property = string.Empty;
      }

      internal Rule(string name, string description, string property)
      {
        _name = name;
        _description = description;
        _property = property;
      }

      /// <summary>
      /// Provides access to the name of the broken rule.
      /// </summary>
      /// <remarks>
      /// This value is actually readonly, not readwrite. Any new
      /// value set into this property is ignored. The property is only
      /// readwrite because that is required to support data binding
      /// within Web Forms.
      /// </remarks>
      /// <value>The name of the rule.</value>
      public string Name
      {
        get
        {
          return _name;
        }
        set
        {
          // the property must be read-write for Web Forms data binding
          // to work, but we really don't want to allow the value to be
          // changed dynamically so we ignore any attempt to set it
        }
      }

      /// <summary>
      /// Provides access to the description of the broken rule.
      /// </summary>
      /// <remarks>
      /// This value is actually readonly, not readwrite. Any new
      /// value set into this property is ignored. The property is only
      /// readwrite because that is required to support data binding
      /// within Web Forms.
      /// </remarks>
      /// <value>The description of the rule.</value>
      public string Description
      {
        get
        {
          return _description;
        }
        set
        {
          // the property must be read-write for Web Forms data binding
          // to work, but we really don't want to allow the value to be
          // changed dynamically so we ignore any attempt to set it
        }
      }

      /// <summary>
      /// Provides access to the property affected by the broken rule.
      /// </summary>
      /// <remarks>
      /// This value is actually readonly, not readwrite. Any new
      /// value set into this property is ignored. The property is only
      /// readwrite because that is required to support data binding
      /// within Web Forms.
      /// </remarks>
      /// <value>The property affected by the rule.</value>
      public string Property
      {
        get
        {
          return _property;
        }
        set
        {
          // the property must be read-write for Web Forms data binding
          // to work, but we really don't want to allow the value to be
          // changed dynamically so we ignore any attempt to set it
        }
      }
    }

    #endregion

    #region RulesCollection

    /// <summary>
    /// A collection of currently broken rules.
    /// </summary>
    /// <remarks>
    /// This collection is readonly and can be safely made available
    /// to code outside the business object such as the UI. This allows
    /// external code, such as a UI, to display the list of broken rules
    /// to the user.
    /// </remarks>
    [Serializable()]
      public class RulesCollection : CSLA.Core.BindableCollectionBase
    {
      bool _validToEdit = false;

      /// <summary>
      /// Returns a <see cref="T:CSLA.BrokenRules.Rule" /> object
      /// containing details about a specific broken business rule.
      /// </summary>
      /// <param name="Index"></param>
      /// <returns></returns>
      public Rule this [int index]
      {
        get 
        { 
          return (Rule)List[index]; 
        }
      }

      /// <summary>
      /// Returns the first <see cref="T:CSLA.BrokenRules.Rule" /> object
      /// corresponding to the specified property.
      /// </summary>
      /// <remarks>
      /// <para>
      /// When a rule is marked as broken, the business developer can provide
      /// an optional Property parameter. This parameter is the name of the
      /// Property on the object that is most affected by the rule. Data binding
      /// may later use the IDataErrorInfo interface to query the object for
      /// details about errors corresponding to specific properties, and this
      /// value will be returned as a result of that query.
      /// </para><para>
      /// Code in a business object or UI can also use this value to retrieve
      /// the first broken rule in <see cref="T:CSLA.BrokenRules" /> that corresponds
      /// to a specfic Property on the object.
      /// </para>
      /// </remarks>
      /// <param name="Property">The name of the property affected by the rule.</param>
      public Rule RuleForProperty(string property)
      {
        foreach(Rule item in List)
          if(item.Property == property)
            return item;
        return new Rule();
      }

      internal RulesCollection()
      {
        AllowEdit = false;
        AllowRemove = false;
        AllowNew = false;
      }

      internal void Add(string name, string description)
      {
        Remove(name);
        _validToEdit = true;
        List.Add(new Rule(name, description));
        _validToEdit = false;
      }

      internal void Add(string name, string description, string property)
      {
        Remove(name);
        _validToEdit = true;
        List.Add(new Rule(name, description, property));
        _validToEdit = false;
      }

      internal void Remove(string name)
      {
        // we loop through using a numeric counter because
        // the base class Remove requires a numeric index
        _validToEdit = true;
        for(int index = 0; index < List.Count; index++)
          if(((Rule)List[index]).Name == name)
          {
            List.Remove(List[index]);
            break;
          }
        _validToEdit = false;
      }

      internal bool Contains(string name)
      {
        for(int index = 0; index < List.Count; index++)
          if(((Rule)List[index]).Name == name)
            return true;
        return false;
      }

      protected override void OnClear()
      {
        if(!_validToEdit)
          throw new NotSupportedException(Strings.GetResourceString("ClearInvalidException"));
      }

      protected override void OnInsert(int index, object val)
      {
        if(!_validToEdit)
          throw new NotSupportedException(Strings.GetResourceString("InsertInvalidException"));
      }

      protected override void OnRemove(int index, object val)
      {
        if(!_validToEdit)
          throw new NotSupportedException(Strings.GetResourceString("RemoveInvalidException"));
      }

      protected override void OnSet(int index, object oldValue, object newValue)
      {
        if(!_validToEdit)
          throw new NotSupportedException(
            Strings.GetResourceString("ChangeInvalidException"));
      }
    }

    #endregion

    private RulesCollection _brokenRules = new RulesCollection();
    [NonSerialized()]
    [NotUndoable()]
    private object _target;

    #region Rule Manager

    /// <summary>
    /// Sets the target object so the Rules Manager functionality
    /// has a reference to the object containing the data to
    /// be validated.
    /// </summary>
    /// <remarks>
    /// The object here is typically your business object. In your
    /// business class you'll implement a method to set up your
    /// business rules. As you do so, you need to call this method
    /// to give BrokenRules a reference to your business object
    /// so it has access to your object's data.
    /// </remarks>
    /// <param name="target">A reference to the object containing
    /// the data to be validated.</param>
    public void SetTargetObject(object target)
    {
      _target = target;
    }

    #region RuleHandler delegate

    /// <summary>
    /// Delegate that defines the method signature for all rule handler methods.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When implementing a rule handler, you must conform to the method signature
    /// defined by this delegate. You should also apply the Description attribute
    /// to your method to provide a meaningful description for your rule.
    /// </para><para>
    /// The method implementing the rule must return True if the data is valid and
    /// return False if the data is invalid.
    /// </para>
    /// </remarks>
    public delegate bool RuleHandler(object target, RuleArgs e);

    #endregion

    #region RuleArgs class

    /// <summary>
    /// Object providing extra information to methods that
    /// implement business rules.
    /// </summary>
    public class RuleArgs
    {
      private string _propertyName;
      private string _description;

      /// <summary>
      /// The (optional) name of the property to be validated.
      /// </summary>
      public string PropertyName
      {
        get
        {
          return _propertyName;
        }
      }

      /// <summary>
      /// Set by the rule handler method to describe the broken
      /// rule.
      /// </summary>
      /// <remarks>
      /// <para>
      /// If the rule handler sets this property, this value will override
      /// any description attribute value associated with the rule handler
      /// method.
      /// </para><para>
      /// The description string returned via this property 
      /// is provided to the UI or other consumer
      /// about which rules are broken. These descriptions are intended
      /// for end-user display.
      /// </para><para>
      /// The description value is a .NET format string, and it can include
      /// the following tokens in addition to literal text:
      /// </para><para>
      /// {0} - the RuleName value
      /// </para><para>
      /// {1} - the PropertyName value
      /// </para><para>
      /// {2} - the full type name of the target object
      /// </para><para>
      /// {3} - the ToString value of the target object
      /// </para><para>
      /// You can use these tokens in your description string and the
      /// appropriate values will be substituted for the tokens at
      /// runtime.
      /// </para>
      /// </remarks>
      public string Description
      {
        get
        {
          return _description;
        }
        set
        {
          _description = value;
        }
      }

      /// <summary>
      /// Creates an instance of RuleArgs.
      /// </summary>
      public RuleArgs()
      {
      }

      /// <summary>
      /// Creates an instance of RuleArgs.
      /// </summary>
      /// <param name="propertyName">The name of the property to be validated.</param>
      public RuleArgs(string propertyName)
      {
        _propertyName = propertyName;
      }

      #region Empty

      private static RuleArgs _emptyArgs = new RuleArgs();

      /// <summary>
      /// Returns an empty RuleArgs object.
      /// </summary>
      public static RuleArgs Empty
      {
        get
        {
          return _emptyArgs;
        }
      }

      #endregion

    }

    #endregion

    #region Description attribute

    /// <summary>
    /// Defines the description of a business rule.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The description in this attribute is used by BusinessRules
    /// as information that is provided to the UI or other consumer
    /// about which rules are broken. These descriptions are intended
    /// for end-user display.
    /// </para><para>
    /// The description value is a .NET format string, and it can include
    /// the following tokens in addition to literal text:
    /// </para><para>
    /// {0} - the RuleName value
    /// </para><para>
    /// {1} - the PropertyName value
    /// </para><para>
    /// {2} - the full type name of the target object
    /// </para><para>
    /// {3} - the ToString value of the target object
    /// </para><para>
    /// You can use these tokens in your description string and the
    /// appropriate values will be substituted for the tokens at
    /// runtime.
    /// </para><para>
    /// Instead of using this attribute, a rule handler method can
    /// set the Description property of the RuleArgs parameter to
    /// a description string. That approach can provide a more dynamic
    /// way to generate descriptions of broken rules.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
      public class DescriptionAttribute : Attribute
    {

      private string _text = string.Empty;

      /// <summary>
      /// Initializes the attribute with a description.
      /// </summary>
      public DescriptionAttribute(string description)
      {
        _text = description;
      }

      /// <summary>
      /// Returns the description value of the attribute.
      /// </summary>
      public override string ToString()
      {
        return _text;
      }

    }

    #endregion

    #region RuleMethod Class

    /// <summary>
    /// Tracks all information for a rule.
    /// </summary>
    private class RuleMethod
    {
      private RuleHandler _handler;
      private object _target;
      private string _ruleName;
      private RuleArgs _args;
      private string _description;

      /// <summary>
      /// Returns the name of the method implementing the rule
      /// and the property, field or column name to which the
      /// rule applies.
      /// </summary>
      public override string ToString()
      {
        if(RuleArgs.PropertyName == null)
          return _handler.Method.Name;
        else
          return _handler.Method.Name + "!" + RuleArgs.PropertyName;
      }
      /// <summary>
      /// Returns the delegate to the method implementing the rule.
      /// </summary>
      public RuleHandler Handler
      {
        get
        {
          return _handler;
        }
      }

      /// <summary>
      /// Returns the user-friendly name of the rule.
      /// </summary>
      public string RuleName
      {
        get
        {
          return _ruleName;
        }
      }

      /// <summary>
      /// Returns the name of the field, property or column
      /// to which the rule applies.
      /// </summary>
      public RuleArgs RuleArgs
      {
        get
        {
          return _args;
        }
      }

      /// <summary>
      /// Returns the formatted description of the rule.
      /// </summary>
      public string Description
      {
        get
        {
          if(_args.Description != null && _args.Description.Length > 0)
            return string.Format(_args.Description, RuleName, RuleArgs.PropertyName, 
              TypeName(_target), _target.ToString());
          else
            return string.Format(_description, RuleName, RuleArgs.PropertyName, TypeName(_target), _target.ToString());
        }
      }

      private string TypeName(object obj)
      {
        return obj.GetType().Name;
      }

      /// <summary>
      /// Retrieves the description text from the Description
      /// attribute on a RuleHandler method.
      /// </summary>
      private string GetDescription(RuleHandler handler)
      {
        object [] attrib = handler.Method.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if(attrib.Length > 0)
          return attrib[0].ToString();
        else
          return "{2}.{0}:<no description>";
      }

      /// <summary>
      /// Creates and initializes the rule.
      /// </summary>
      /// <param name="target">Reference to the object containing the data to validate.</param>
      /// <param name="handler">The address of the method implementing the rule.</param>
      /// <param name="ruleName">The user-friendly name of the rule.</param>
      /// <param name="ruleArgs">A RuleArgs object containing data related to the rule.</param>
      public RuleMethod(object target, RuleHandler handler, string ruleName, RuleArgs ruleArgs)
      {
        _target = target;
        _handler = handler;
        _description = GetDescription(handler);
        _ruleName = ruleName;
        _args = ruleArgs;
      }

      /// <summary>
      /// Creates and initializes the rule.
      /// </summary>
      /// <param name="target">Reference to the object containing the data to validate.</param>
      /// <param name="handler">The address of the method implementing the rule.</param>
      /// <param name="ruleName">The user-friendly name of the rule.</param>
      /// <param name="propertyName">The field, property or column to which the rule applies.</param>
      public RuleMethod(object target, RuleHandler handler, string ruleName, string propertyName)
      {
        _target = target;
        _handler = handler;
        _description = GetDescription(handler);
        _ruleName = ruleName;
        _args = new RuleArgs(propertyName);
      }

      /// <summary>
      /// Invokes the rule to validate the data.
      /// </summary>
      /// <returns>True if the data is valid, False if the data is invalid.</returns>
      public bool Invoke()
      {
        return _handler(_target, _args);
      }
    }

    #endregion

    #region RulesList property

    [NonSerialized()]
    [NotUndoable()]
    private HybridDictionary _rulesList;

    private HybridDictionary RulesList
    {
      get
      {
        if(_rulesList == null)
          _rulesList = new HybridDictionary();
        return _rulesList;
      }
    }

    #endregion

    #region Adding Rules

    /// Returns the ArrayList containing rules for a rule name. If
    /// no ArrayList exists one is created and returned.
    /// </summary>
    private ArrayList GetRulesForName(string ruleName)
    {
      // get the ArrayList (if any) from the Hashtable
      ArrayList list = (ArrayList)RulesList[ruleName];
      if(list == null)
      {
        // there is no list for this name - create one
        list = new ArrayList();
        RulesList.Add(ruleName, list);
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
    /// The ruleName is used to group all the rules that apply
    /// to a specific field, property or concept. All rules applying
    /// to the field or property should have the same rule name. When
    /// rules are checked, they can be checked globally or for a 
    /// specific ruleName.
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
    /// <param name="ruleName">
    /// A user-friendly identifier for the field/property 
    /// to which the rule applies.
    /// </param>
    public void AddRule(RuleHandler handler, string ruleName)
    {
      // get the ArrayList (if any) from the Hashtable
      ArrayList list = GetRulesForName(ruleName);

      // we have the list, add our new rule
      list.Add(new RuleMethod(_target, handler, ruleName, RuleArgs.Empty));
    }

    /// <summary>
    /// Adds a rule to the list of rules to be enforced.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A rule is implemented by a method which conforms to the 
    /// method signature defined by the RuleHandler delegate.
    /// </para><para>
    /// The ruleName is used to group all the rules that apply
    /// to a specific field, property or concept. All rules applying
    /// to the field or property should have the same rule name. When
    /// rules are checked, they can be checked globally or for a 
    /// specific ruleName.
    /// </para>
    /// </remarks>
    /// <param name="handler">The method that implements the rule.</param>
    /// <param name="ruleName">
    /// A user-friendly identifier for the field/property 
    /// to which the rule applies.
    /// </param>
    /// <param name="ruleArgs">A RuleArgs object containing data
    /// to be passed to the method implementing the rule.</param>
    public void AddRule(RuleHandler handler, string ruleName, RuleArgs ruleArgs)
    {
      // get the ArrayList (if any) from the Hashtable
      ArrayList list = GetRulesForName(ruleName);

      // we have the list, add our new rule
      list.Add(new RuleMethod(_target, handler, ruleName, ruleArgs));
    }

    /// <summary>
    /// Adds a rule to the list of rules to be enforced.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A rule is implemented by a method which conforms to the 
    /// method signature defined by the RuleHandler delegate.
    /// </para><para>
    /// The ruleName is used to group all the rules that apply
    /// to a specific field, property or concept. All rules applying
    /// to the field or property should have the same rule name. When
    /// rules are checked, they can be checked globally or for a 
    /// specific ruleName.
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
    /// <param name="ruleName">
    /// A user-friendly identifier for the field/property 
    /// to which the rule applies.
    /// </param>
    /// <param name="propertyName">
    /// The property name on the target object where the rule implementation can retrieve
    /// the value to be validated.
    /// </param>
    public void AddRule(RuleHandler handler, string ruleName, string propertyName)
    {
      // get the ArrayList (if any) from the Hashtable
      ArrayList list = GetRulesForName(ruleName);

      // we have the list, add our new rule
      list.Add(new RuleMethod(_target, handler, ruleName, propertyName));
    }

    #endregion

    #region Checking Rules

    /// <summary>
    /// Checks all the rules for a specific ruleName.
    /// </summary>
    /// <param name="ruleName">The ruleName to be validated.</param>
    public void CheckRules(string ruleName)
    {
      // get the list of rules to check
      ArrayList list = (ArrayList)RulesList[ruleName];
      if(list == null) return;

      // now check the rules
      foreach(RuleMethod rule in list)
        if(rule.Invoke())
          UnBreakRule(rule);
        else
          BreakRule(rule);
    }

    /// <summary>
    /// Checks all the rules for a target object.
    /// </summary>
    public void CheckRules()
    {
      // get the rules for each rule name
      foreach(DictionaryEntry de in RulesList)
      {
        ArrayList list = (ArrayList)de.Value;

        // now check the rules
        foreach(RuleMethod rule in list)
          if(rule.Invoke())
            UnBreakRule(rule);
          else
            BreakRule(rule);
      }
    }

    private void UnBreakRule(RuleMethod rule)
    {
      if(rule.RuleArgs.PropertyName == null)
        Assert(rule.ToString(), string.Empty, false);
      else
        Assert(rule.ToString(), string.Empty, rule.RuleArgs.PropertyName, false);
    }

    private void BreakRule(RuleMethod rule)
    {
      if(rule.RuleArgs.PropertyName == null)
        Assert(rule.ToString(), rule.Description, true);
      else
        Assert(rule.ToString(), rule.Description, rule.RuleArgs.PropertyName, true);
    }

    #endregion

    #endregion // Rule Manager

    #region Assert methods
    
    RulesCollection _rules = new RulesCollection();

    /// <summary>
    /// This method is called by business logic within a business class to
    /// indicate whether a business rule is broken.
    /// </summary>
    /// <remarks>
    /// Rules are identified by their names. The description field is merely a 
    /// comment that is used for display to the end user. When a rule is marked as
    /// broken, it is recorded under the rule name value. To mark the rule as not
    /// broken, the same rule name must be used.
    /// </remarks>
    /// <param name="Rule">The name of the business rule.</param>
    /// <param name="Description">The description of the business rule.</param>
    /// <param name="IsBroken">True if the value is broken, False if it is not broken.</param>
    public void Assert(string name, string description, bool isBroken)
    {
      if(isBroken)
        _rules.Add(name, description);
      else
        _rules.Remove(name);
    }

    /// <summary>
    /// This method is called by business logic within a business class to
    /// indicate whether a business rule is broken.
    /// </summary>
    /// <remarks>
    /// Rules are identified by their names. The description field is merely a 
    /// comment that is used for display to the end user. When a rule is marked as
    /// broken, it is recorded under the rule name value. To mark the rule as not
    /// broken, the same rule name must be used.
    /// </remarks>
    /// <param name="Rule">The name of the business rule.</param>
    /// <param name="Description">The description of the business rule.</param>
    /// <param name="Property">The property affected by the business rule.</param>
    /// <param name="IsBroken">True if the value is broken, False if it is not broken.</param>
    public void Assert(string rule, string description, string property, bool isBroken)
    {
      if(isBroken)
        _rules.Add(rule, description, property);
      else
        _rules.Remove(rule);
    }

    #endregion

    #region Status retrieval

    /// <summary>
    /// Returns a value indicating whether there are any broken rules
    /// at this time. If there are broken rules, the business object
    /// is assumed to be invalid and False is returned. If there are no
    /// broken business rules True is returned.
    /// </summary>
    /// <returns>A value indicating whether any rules are broken.</returns>
    public bool IsValid
    {
      get
      {
        return (_rules.Count == 0);
      }
    }

    /// <summary>
    /// Returns a value indicating whether a particular business rule
    /// is currently broken.
    /// </summary>
    /// <param name="Rule">The name of the rule to check.</param>
    /// <returns>A value indicating whether the rule is currently broken.</returns>
    public bool IsBroken(string name)
    {
      return _rules.Contains(name);
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
    public RulesCollection BrokenRulesCollection
    {
      get
      {
        return _rules;
      }
    }

    /// <summary>
    /// Returns the text of all broken rule descriptions, each
    /// separated by cr/lf.
    /// </summary>
    /// <returns>The text of all broken rule descriptions.</returns>
    public override string ToString()
    {
      System.Text.StringBuilder obj = new System.Text.StringBuilder();
      bool first = true;

      foreach(Rule item in _rules)
      {
        if(first)
          first = false;
        else
          obj.Append(Environment.NewLine);
        obj.Append(item.Description);
      }
      return obj.ToString();
    }

    #endregion
	}
}
