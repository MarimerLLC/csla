using System;
using System.Collections.Generic;
using Csla.Properties;
using Csla.Serialization;
using Csla.Serialization.Mobile;

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
  public partial class BrokenRulesCollection : Core.ReadOnlyBindingList<BrokenRule>
  {

    private int _errorCount;
    private int _warningCount;
    private int _infoCount;
    private bool _customList;

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

    internal void Add(IAsyncRuleMethod rule, AsyncRuleResult result)
    {
      Remove(rule);
      IsReadOnly = false;
      BrokenRule item = new BrokenRule(rule, result);
      IncrementCount(item);
      Add(item);
      IsReadOnly = true;
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
      IncrementRevision();
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
      IncrementRevision();
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

    #region Custom List

    /// <summary>
    /// Gets an empty collection on which the Merge()
    /// method may be called to merge in data from
    /// other BrokenRuleCollection objects.
    /// </summary>
    public static BrokenRulesCollection CreateCollection() 
    {
      BrokenRulesCollection result = new BrokenRulesCollection();
      result._customList = true;
      return result;
    }

    /// <summary>
    /// Merges data from the supplied list into this
    /// list, changing the rule names to be unique
    /// based on the source value.
    /// </summary>
    /// <param name="source">
    /// A unique source name for each list being
    /// merged into this master list.
    /// </param>
    /// <param name="list">
    /// A list to merge into this master list.
    /// </param>
    /// <remarks>
    /// This method is intended to allow merging of
    /// all BrokenRulesCollection objects in a business
    /// object graph into a single list. To this end,
    /// no attempt is made to remove duplicates during
    /// the merge process. Also, the source parameter
    /// value must be unqiue for each object instance
    /// in the graph, or rule name collisions may
    /// occur.
    /// </remarks>
    public void Merge(string source, BrokenRulesCollection list)
    {
      if (!_customList)
        throw new NotSupportedException(Resources.BrokenRulesMergeException);
      IsReadOnly = false;
      foreach (BrokenRule item in list)
      {
        BrokenRule newItem = new BrokenRule(source, item);
        IncrementCount(newItem);
        Add(newItem);
      }
      IsReadOnly = true;
    }

    #endregion

    #region Revision

    private int _revision;

    private void IncrementRevision()
    {
      _revision = (_revision + 1) % int.MaxValue;
    }

    /// <summary>
    /// Gets the current revision number of
    /// the collection.
    /// </summary>
    /// <remarks>
    /// The revision value changes each time an
    /// item is added or removed from the collection.
    /// </remarks>
    public int Revision
    {
      get { return _revision; }
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
    protected override void OnGetState(SerializationInfo info)
    {
      info.AddValue("_errorCount", _errorCount);
      info.AddValue("_warningCount", _warningCount);
      info.AddValue("_infoCount", _infoCount);
      info.AddValue("_customList", _customList);

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
      _warningCount = info.GetValue<int>("_warningCount");
      _infoCount = info.GetValue<int>("_infoCount");
      _customList = info.GetValue<bool>("_customList");

      base.OnSetState(info);
    }

    #endregion
  }
}