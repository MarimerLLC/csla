//-----------------------------------------------------------------------
// <copyright file="BrokenRule.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Stores details about a specific broken business rule.</summary>
//-----------------------------------------------------------------------

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
    private string _ruleName;
    private string _description;
    private string? _property;
    private RuleSeverity _severity;
    private string? _originProperty;
    private int _priority;
    private int _displayIndex;

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
    public string? Property
    {
      get { return _property; }
    }

    /// <summary>
    /// Gets the severity of the broken rule.
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    public RuleSeverity Severity
    {
      get { return _severity; }
    }

    /// <summary>
    /// Gets or sets the origin property.
    /// </summary>
    /// <value>The origin property.</value>
    public string? OriginProperty
    {
      get { return _originProperty; }
    }

    /// <summary>
    /// Gets or sets the broken rule priority.
    /// </summary>
    public int Priority
    {
      get { return _priority; }
    }

    /// <summary>
    /// Gets or sets the DisplayIndex property.
    /// </summary>
    /// <value>The DisplayIndex property.</value>
    public int DisplayIndex
    {
      get { return _displayIndex; }
    }

    /// <summary>
    /// Creates an instance of this type.
    /// </summary>
    [Obsolete(MobileFormatter.DefaultCtorObsoleteMessage, error: true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable. Only necessary for serialization and fields will be set by the serializer
    public BrokenRule()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    { }

    internal BrokenRule(string ruleName, string description, string? property, RuleSeverity severity, string? originProperty, int priority, int displayIndex)
    {
      _ruleName = ruleName ?? throw new ArgumentNullException(nameof(ruleName));
      _description = description ?? throw new ArgumentNullException(nameof(description));
      _property = property;
      _severity = severity;
      _originProperty = originProperty;
      _priority = priority;
      _displayIndex = displayIndex;
    }

    /// <summary>
    /// Gets a string representation for this object.
    /// </summary>
    public override string ToString()
    {
      return Description;
    }

    #region MobileObject overrides

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialization stream.
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
      info.AddValue("_originProperty", _originProperty);
      info.AddValue("_priority", _priority);
      info.AddValue("_displayIndex", (int)_displayIndex);

      base.OnGetState(info, mode);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      _ruleName = info.GetValue<string>("_ruleName")!;
      _description = info.GetValue<string>("_description")!;
      _property = info.GetValue<string>("_property");
      _severity = info.GetValue<RuleSeverity>("_severity");
      _originProperty = info.GetValue<string>("_originProperty");
      _priority = info.GetValue<int>("_priority");
      _displayIndex = info.GetValue<int>("_displayIndex");

      base.OnSetState(info, mode);
    }

    #endregion
  }
}