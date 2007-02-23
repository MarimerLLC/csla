using System;
using System.Collections.Generic;

namespace Csla.Validation
{
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
  public class BrokenRulesCollection : Core.ReadOnlyBindingList<BrokenRule>
  {

    private int _errorCount;
    private int _warningCount;
    private int _infoCount;

    /// <summary>
    /// Gets the number of broken rules in
    /// the collection that have a severity
    /// of Error.
    /// </summary>
    /// <value>An integer value.</value>
    public int ErrorCount
    {
      get { return _errorCount; }
    }

    /// <summary>
    /// Gets the number of broken rules in
    /// the collection that have a severity
    /// of Warning.
    /// </summary>
    /// <value>An integer value.</value>
    public int WarningCount
    {
      get { return _warningCount; }
    }

    /// <summary>
    /// Gets the number of broken rules in
    /// the collection that have a severity
    /// of Information.
    /// </summary>
    /// <value>An integer value.</value>
    public int InformationCount
    {
      get { return _infoCount; }
    }

    /// <summary>
    /// Returns the first <see cref="BrokenRule" /> object
    /// corresponding to the specified property.
    /// </summary>
    /// <remarks>
    /// Code in a business object or UI can also use this value to retrieve
    /// the first broken rule in <see cref="BrokenRulesCollection" /> that corresponds
    /// to a specfic property on the object.
    /// </remarks>
    /// <param name="property">The name of the property affected by the rule.</param>
    /// <returns>
    /// The first BrokenRule object corresponding to the specified property, or null if 
    /// there are no rules defined for the property.
    /// </returns>
    public BrokenRule GetFirstBrokenRule(string property)
    {
      return GetFirstMessage(property, RuleSeverity.Error);
    }

    /// <summary>
    /// Returns the first <see cref="BrokenRule" /> object
    /// corresponding to the specified property.
    /// </summary>
    /// <remarks>
    /// Code in a business object or UI can also use this value to retrieve
    /// the first broken rule in <see cref="BrokenRulesCollection" /> that corresponds
    /// to a specfic property.
    /// </remarks>
    /// <param name="property">The name of the property affected by the rule.</param>
    /// <returns>
    /// The first BrokenRule object corresponding to the specified property, or Nothing
    /// (null in C#) if there are no rules defined for the property.
    /// </returns>
    public BrokenRule GetFirstMessage(string property)
    {
      foreach (BrokenRule item in this)
        if (item.Property == property)
          return item;
      return null;
    }

    /// <summary>
    /// Returns the first <see cref="BrokenRule"/> object
    /// corresponding to the specified property
    /// and severity.
    /// </summary>
    /// <param name="property">The name of the property affected by the rule.</param>
    /// <param name="severity">The severity of broken rule to return.</param>
    /// <returns>
    /// The first BrokenRule object corresponding to the specified property, or Nothing
    /// (null in C#) if there are no rules defined for the property.
    /// </returns>
    public BrokenRule GetFirstMessage(string property, RuleSeverity severity)
    {
      foreach (BrokenRule item in this)
        if (item.Property == property && item.Severity == severity)
          return item;
      return null;
    }

    internal BrokenRulesCollection()
    {
      // limit creation to this assembly
    }

    internal void Add(IRuleMethod rule)
    {
      Remove(rule);
      IsReadOnly = false;
      BrokenRule item = new BrokenRule(rule);
      IncrementCount(item);
      Add(item);
      IsReadOnly = true;
    }

    internal void Remove(IRuleMethod rule)
    {
      // we loop through using a numeric counter because
      // removing items within a foreach isn't reliable
      IsReadOnly = false;
      for (int index = 0; index < Count; index++)
        if (this[index].RuleName == rule.RuleName)
        {
          DecrementCount(this[index]);
          RemoveAt(index);
          break;
        }
      IsReadOnly = true;
    }

    private void IncrementCount(BrokenRule item)
    {
      switch (item.Severity)
      {
        case RuleSeverity.Error:
          _errorCount += 1;
          break;
        case RuleSeverity.Warning:
          _warningCount += 1;
          break;
        case RuleSeverity.Information:
          _infoCount += 1;
          break;
      }
    }

    private void DecrementCount(BrokenRule item)
    {
      switch (item.Severity)
      {
        case RuleSeverity.Error:
          _errorCount -= 1;
          break;
        case RuleSeverity.Warning:
          _warningCount -= 1;
          break;
        case RuleSeverity.Information:
          _infoCount -= 1;
          break;
      }
    }

    /// <summary>
    /// Returns the text of all broken rule descriptions, each
    /// separated by a <see cref="Environment.NewLine" />.
    /// </summary>
    /// <returns>The text of all broken rule descriptions.</returns>
    public override string ToString()
    {
      System.Text.StringBuilder result = new System.Text.StringBuilder();
      bool first = true;
      foreach (BrokenRule item in this)
      {
        if (first)
          first = false;
        else
          result.Append(Environment.NewLine);
        result.Append(item.Description);
      }
      return result.ToString();
    }

    /// <summary>
    /// Returns the text of all broken rule descriptions, each
    /// separated by a <see cref="Environment.NewLine" />.
    /// </summary>
    /// <param name="severity">The severity of rules to
    /// include in the result.</param>
    /// <returns>The text of all broken rule descriptions
    /// matching the specified severtiy.</returns>
    public string ToString(RuleSeverity severity)
    {
      System.Text.StringBuilder result = new System.Text.StringBuilder();
      bool first = true;
      foreach (BrokenRule item in this)
      {
        if (item.Severity == severity)
        {
          if (first)
            first = false;
          else
            result.Append(Environment.NewLine);
          result.Append(item.Description);
        }
      }
      return result.ToString();
    }

    /// <summary>
    /// Returns a string array containing all broken
    /// rule descriptions.
    /// </summary>
    /// <returns>The text of all broken rule descriptions
    /// matching the specified severtiy.</returns>
    public string[] ToArray()
    {
      List<string> result = new List<string>();
      foreach (BrokenRule item in this)
        result.Add(item.Description);
      return result.ToArray();
    }

    /// <summary>
    /// Returns a string array containing all broken
    /// rule descriptions.
    /// </summary>
    /// <param name="severity">The severity of rules
    /// to include in the result.</param>
    /// <returns>The text of all broken rule descriptions
    /// matching the specified severtiy.</returns>
    public string[] ToArray(RuleSeverity severity)
    {
      List<string> result = new List<string>();
      foreach (BrokenRule item in this)
        if (item.Severity == severity)
          result.Add(item.Description);
      return result.ToArray();
    }
  }
}