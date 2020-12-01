//-----------------------------------------------------------------------
// <copyright file="BrokenRulesCollection.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A collection of currently broken rules.</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;
using Csla.Properties;
using Csla.Serialization.Mobile;

namespace Csla.Rules
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
  [Serializable]
  public class BrokenRulesCollection : Core.ReadOnlyObservableBindingList<BrokenRule>
  {
    private int _errorCount;
    private int _warnCount;
    private int _infoCount;

    private object _syncRoot = new object();


    /// <summary>
    /// Creates a read-write instance
    /// of the collection.
    /// </summary>
    public BrokenRulesCollection()
      : this(false)
    { }

    internal BrokenRulesCollection(bool readOnly)
    {
      IsReadOnly = readOnly;
    }

    internal void ClearRules()
    {
      lock (_syncRoot)
      {
        IsReadOnly = false;
        base.Clear();
        _errorCount = _warnCount = _infoCount = 0;
        IsReadOnly = true;
      }
    }

    internal void ClearRules(Csla.Core.IPropertyInfo property)
    {
      lock (_syncRoot)
      {
        this.IsReadOnly = false;

        var propertyName = property == null ? null : property.Name;
        for (int i = 0, n = Count; i < n; i++)
        {
            var x = this[i];
            if (x.OriginProperty == propertyName)
            {
                RemoveAt(i);
                i--;
                n--;
            }
        }

        this.IsReadOnly = true;
      }
    }
    
    internal void SetBrokenRules(List<RuleResult> results, string originPropertyName)
    {
      lock (_syncRoot)
      {
        this.IsReadOnly = false;

        ISet<string> rulesDone = new HashSet<string>();

        for(int i = 0, n = results.Count; i < n; i++)
        {
          var result = results[i];
          var resultRuleName = result.RuleName;

          if(!rulesDone.Contains(resultRuleName))
          {
            rulesDone.Add(resultRuleName);

            ClearRules(resultRuleName, originPropertyName);
          }

          if(result.Success)
            continue;

          var resultDescription = result.Description;

          if(string.IsNullOrEmpty(resultDescription))
            throw new ArgumentException(string.Format(Resources.RuleMessageRequired,
                                                      resultRuleName));

          var resultPrimaryProperty = result.PrimaryProperty;

          BrokenRule broken = new BrokenRule
          {
            RuleName = resultRuleName,
            Description = resultDescription,
            Property = resultPrimaryProperty == null
                       ? null : resultPrimaryProperty.Name,
            Severity = result.Severity,
            OriginProperty = originPropertyName
          };

          Add(broken);
        }

        this.IsReadOnly = true;
      }
    }

    /// <summary>
    /// Remove the previous results for the given rule name and origin property.
    /// </summary>
    private void ClearRules(string ruleName, string originProperty)
    {
      for(int i = 0, n = Count; i < n; i++)
      {
        var x = this[i];

        if(x.RuleName == ruleName && x.OriginProperty == originProperty)
        {
          RemoveAt(i);
          i--;
          n--;
        }
      }
    }

    new void RemoveAt(int i)
    {
      CountOne(this[i].Severity, -1);

      base.RemoveAt(i);
    }

    new void Add(BrokenRule item)
    {
      base.Add(item);

      CountOne(item.Severity, 1);
    }

    private void CountOne(RuleSeverity severity, int one)
    {
        switch (severity)
        {
            case RuleSeverity.Error:
                _errorCount += one;
                break;
            case RuleSeverity.Warning:
                _warnCount += one;
                break;
            case RuleSeverity.Information:
                _infoCount += one;
                break;
            default:
                throw new Exception("unhandled severity=" + severity);
        }
    }

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
      get { return _warnCount; }
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
    /// <param name="property">The property affected by the rule.</param>
    /// <returns>
    /// The first BrokenRule object corresponding to the specified property, or null if 
    /// there are no rules defined for the property.
    /// </returns>
    public BrokenRule GetFirstBrokenRule(Csla.Core.IPropertyInfo property)
    {
      return GetFirstMessage(property.Name, RuleSeverity.Error);
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
    public BrokenRule GetFirstMessage(Csla.Core.IPropertyInfo property)
    {
      return this.Where(c => c.Property == property.Name).FirstOrDefault();
    }

    /// <summary>
    /// Returns the first <see cref="BrokenRule"/> object
    /// corresponding to the specified property
    /// and severity.
    /// </summary>
    /// <param name="property">The property affected by the rule.</param>
    /// <param name="severity">The severity of broken rule to return.</param>
    /// <returns>
    /// The first BrokenRule object corresponding to the specified property, or Nothing
    /// (null in C#) if there are no rules defined for the property.
    /// </returns>
    public BrokenRule GetFirstMessage(Csla.Core.IPropertyInfo property, RuleSeverity severity)
    {
      return GetFirstMessage(property.Name, severity);
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
      return this.Where(c => c.Property == property && c.Severity == severity).FirstOrDefault();
    }

    /// <summary>
    /// Returns the text of all broken rule descriptions, each
    /// separated by a <see cref="Environment.NewLine" />.
    /// </summary>
    /// <returns>The text of all broken rule descriptions.</returns>
    public override string ToString()
    {
      return ToString(Environment.NewLine);
    }

    /// <summary>
    /// Returns the text of all broken rule descriptions
    /// for a specific severity, each
    /// separated by a <see cref="Environment.NewLine" />.
    /// </summary>
    /// <param name="severity">The severity of rules to
    /// include in the result.</param>
    /// <returns>The text of all broken rule descriptions
    /// matching the specified severtiy.</returns>
    public string ToString(RuleSeverity severity)
    {
      return ToString(Environment.NewLine, severity);
    }

    /// <summary>
    /// Returns the text of all broken rule descriptions.
    /// </summary>
    /// <param name="separator">
    /// String to place between each broken rule description.
    /// </param>
    /// <returns>The text of all broken rule descriptions.</returns>
    public string ToString(string separator)
    {
      System.Text.StringBuilder result = new System.Text.StringBuilder();
      bool first = true;
      foreach (BrokenRule item in this)
      {
        if (first)
          first = false;
        else
          result.Append(separator);
        result.Append(item.Description);
      }
      return result.ToString();
    }

    /// <summary>
    /// Returns the text of all broken rule descriptions
    /// for a specific severity.
    /// </summary>
    /// <param name="separator">
    /// String to place between each broken rule description.
    /// </param>
    /// <param name="severity">The severity of rules to
    /// include in the result.</param>
    /// <returns>The text of all broken rule descriptions
    /// matching the specified severtiy.</returns>
    public string ToString(string separator, RuleSeverity severity)
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
            result.Append(separator);
          result.Append(item.Description);
        }
      }
      return result.ToString();
    }

    /// <summary>
    /// Returns the text of all broken rule descriptions
    /// for a specific severity and property.
    /// </summary>
    /// <param name="separator">
    /// String to place between each broken rule description.
    /// </param>
    /// <param name="severity">The severity of rules to
    /// include in the result.</param>
    /// <param name="propertyName">Property name</param>
    /// <returns>The text of all broken rule descriptions
    /// matching the specified severtiy.</returns>
    public string ToString(string separator, RuleSeverity severity, string propertyName)
    {
      System.Text.StringBuilder result = new System.Text.StringBuilder();
      bool first = true;
      foreach (BrokenRule item in this.Where(r => r.Property == propertyName))
      {
        if (item.Severity == severity)
        {
          if (first)
            first = false;
          else
            result.Append(separator);
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
      return this.Select(c => c.Description).ToArray();
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
      return this.Where(c => c.Severity == severity).Select(c => c.Description).ToArray();
    }

    /// <summary>
    /// Merges a list of items into the collection.
    /// </summary>
    /// <param name="list">List of items to add.</param>
    public void AddRange(List<BrokenRule> list)
    {
      foreach (var item in list)
        Add(item);
    }

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnGetState(SerializationInfo info)
    {
      info.AddValue("_errorCount", _errorCount);
      info.AddValue("_warnCount", _warnCount);
      info.AddValue("_infoCount", _infoCount);
      base.OnGetState(info);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnSetState(SerializationInfo info)
    {
      _errorCount = info.GetValue<int>("_errorCount");
      _warnCount = info.GetValue<int>("_warnCount");
      _infoCount = info.GetValue<int>("_infoCount");
      base.OnSetState(info);
    }
  }
}