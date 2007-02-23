using System;
using System.Collections.Generic;

namespace Csla.Validation
{
  /// <summary>
  /// Maintains rule methods for a business object
  /// or business object type.
  /// </summary>
  internal class ValidationRulesManager
  {
    private Dictionary<string, RulesList> _rulesList;

    internal Dictionary<string, RulesList> RulesDictionary
    {
      get
      {
        if (_rulesList == null)
          _rulesList = new Dictionary<string, RulesList>();
        return _rulesList;
      }
    }

    internal RulesList GetRulesForProperty(
      string propertyName,
      bool createList)
    {
      // get the list (if any) from the dictionary
      RulesList list = null;
      if (RulesDictionary.ContainsKey(propertyName))
        list = RulesDictionary[propertyName];

      if (createList && list == null)
      {
        // there is no list for this name - create one
        list = new RulesList();
        RulesDictionary.Add(propertyName, list);
      }
      return list;
    }

    #region Adding Rules

    public void AddRule(RuleHandler handler, RuleArgs args, int priority)
    {
      // get the list of rules for the property
      List<IRuleMethod> list = GetRulesForProperty(args.PropertyName, true).GetList(false);

      // we have the list, add out new rule
      list.Add(new RuleMethod(handler, args, priority));
    }

    public void AddRule<T, R>(RuleHandler<T, R> handler, R args, int priority) where R : RuleArgs
    {
      // get the list of rules for the property
      List<IRuleMethod> list = GetRulesForProperty(args.PropertyName, true).GetList(false);

      // we have the list, add out new rule
      list.Add(new RuleMethod<T, R>(handler, args, priority));
    }

    #endregion

    #region Adding Dependencies

    /// <summary>
    /// Adds a property to the list of dependencies for
    /// the specified property
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property.
    /// </param>
    /// <param name="dependantPropertyName">
    /// The name of the dependant property.
    /// </param>
    /// <remarks>
    /// When rules are checked for propertyName, they will
    /// also be checked for any dependant properties associated
    /// with that property.
    /// </remarks>
    public void AddDependantProperty(string propertyName, string dependantPropertyName)
    {
      // get the list of rules for the property
      List<string> list = GetRulesForProperty(propertyName, true).GetDependancyList(true);

      // we have the list, add the dependency
      list.Add(dependantPropertyName);
    }

    #endregion

  }
}
