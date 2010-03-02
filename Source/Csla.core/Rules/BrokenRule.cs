using System;
using Csla.Serialization;
using Csla.Core;
using Csla.Serialization.Mobile;

namespace Csla.Rules
{
  /// <summary>
  /// Stores details about a specific broken business rule.
  /// </summary>
  [Serializable]
  public partial class BrokenRule : MobileObject
  {
#if SILVERLIGHT
    public BrokenRule()
    { }
#else
    internal BrokenRule()
    { }
#endif

    private string _ruleName;
    private string _description;
    private string _property;
    private RuleSeverity _severity;

    /// <summary>
    /// Provides access to the name of the broken rule.
    /// </summary>
    /// <value>The name of the rule.</value>
    public string RuleName
    {
      get { return _ruleName; }
      internal set { _ruleName = value; }
    }

    /// <summary>
    /// Provides access to the description of the broken rule.
    /// </summary>
    /// <value>The description of the rule.</value>
    public string Description
    {
      get { return _description; }
      internal set { _description = value; }
    }

    /// <summary>
    /// Provides access to the property affected by the broken rule.
    /// </summary>
    /// <value>The property affected by the rule.</value>
    public string Property
    {
      get { return _property; }
      internal set { _property = value; }
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
      internal set { _severity = value; }
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