//-----------------------------------------------------------------------
// <copyright file="BrokenRulesCollection.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>A collection of currently broken rules.</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;
using Csla.Properties;
using Csla.Serialization;
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
        RecalculateCounts();
        IsReadOnly = true;
      }
    }

    internal void ClearRules(Csla.Core.IPropertyInfo property)
    {
      lock (_syncRoot)
      {
        this.IsReadOnly = false;
        List<BrokenRule> brokenRules;
        if (property == null)
          brokenRules = this.Where(c => c.OriginProperty == null).ToList();
        else
          brokenRules = this.Where(c => c.OriginProperty == property.Name).ToList();

        foreach (var item in brokenRules)
          Remove(item);
        RecalculateCounts();
        this.IsReadOnly = true;
      }
    }
    
    internal void SetBrokenRules(List<RuleResult> results, string originPropertyName)
    {
      lock (_syncRoot)
      {
        this.IsReadOnly = false;

        var rulenames = results.Select(p => p.RuleName).Distinct();
        var old = this.Where(c => rulenames.Contains(c.RuleName) && c.OriginProperty == originPropertyName).ToList();
        foreach (var item in old)
          Remove(item);

        foreach (var result in results)
        {
          if (!result.Success)
            if (result.PrimaryProperty == null)
              Add(new BrokenRule
                    {
                      RuleName = result.RuleName,
                      Description = result.Description,
                      Property = null,
                      Severity = result.Severity,
                      OriginProperty = originPropertyName
                    });
            else
              Add(new BrokenRule
                    {
                      RuleName = result.RuleName,
                      Description = result.Description,
                      Property = result.PrimaryProperty.Name,
                      Severity = result.Severity,
                      OriginProperty = originPropertyName
                    });
        }
        RecalculateCounts();
        this.IsReadOnly = true;
      }
    }

    private void RecalculateCounts()
    {
      _errorCount = this.Count(c => c.Severity == RuleSeverity.Error);
      _warnCount = this.Count(c => c.Severity == RuleSeverity.Warning);
      _infoCount = this.Count(c => c.Severity == RuleSeverity.Information);
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