using System;
using Csla.Serialization;
using Csla.Core;
using Csla.Serialization.Mobile;

namespace Csla.Validation
{
  /// <summary>
  /// Stores details about a specific broken business rule.
  /// </summary>
  [Serializable()]
  public partial class BrokenRule : MobileObject
  {
    private string _ruleName;
    private string _description;
    private string _property;
    private RuleSeverity _severity;

    internal BrokenRule(IAsyncRuleMethod asyncRule, AsyncRuleResult result)
    {
      _ruleName = asyncRule.RuleName;
      _description = result.Description;
      _severity = result.Severity;
      _property = asyncRule.AsyncRuleArgs.Properties[0].Name;
    }

    internal BrokenRule(IRuleMethod rule)
    {
      _ruleName = rule.RuleName;
      _description = rule.RuleArgs.Description;
      _property = rule.RuleArgs.PropertyName;
      _severity = rule.RuleArgs.Severity;
    }

    internal BrokenRule(string source, BrokenRule rule)
    {
      _ruleName = string.Format("rule://{0}.{1}", source, rule.RuleName.Replace("rule://", string.Empty));
      _description = rule.Description;
      _property = rule.Property;
      _severity = rule.Severity;
    }

    /// <summary>
    /// Provides access to the name of the broken rule.
    /// </summary>
    /// <value>The name of the rule.</value>
    public string RuleName
    {
      get { return _ruleName; }
    }

    /// <summary>
    /// Provides access to the description of the broken rule.
    /// </summary>
    /// <value>The description of the rule.</value>
    public string Description
    {
      get { return _description; }
    }

    /// <summary>
    /// Provides access to the property affected by the broken rule.
    /// </summary>
    /// <value>The property affected by the rule.</value>
    public string Property
    {
      get { return _property; }
    }

    /// <summary>
    /// Gets the severity of the broken rule.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public RuleSeverity Severity
    {
      get { return _severity; }
    }

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
      info.AddValue("_ruleName", _ruleName);
      info.AddValue("_description", _description);
      info.AddValue("_property", _property);
      info.AddValue("_severity", (int)_severity);
      
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
      _ruleName = info.GetValue<string>("_ruleName");
      _description = info.GetValue<string>("_description");
      _property = info.GetValue<string>("_property");
      _severity = info.GetValue<RuleSeverity>("_severity");

      base.OnSetState(info, mode);
    }

    #endregion
  }
}