//-----------------------------------------------------------------------
// <copyright file="BrokenRule.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Stores details about a specific broken business rule.</summary>
//-----------------------------------------------------------------------
using System;
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
    /// <summary>
    /// Creates an instance of this type.
    /// </summary>
    public BrokenRule()
    { }

    private string _ruleName;
    private string _description;
    private string _property;
    private RuleSeverity _severity;
    private string _originProperty;

    /// <summary>
    /// Gets a string representation for this object.
    /// </summary>
    public override string ToString()
    {
      return Description;
    }

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

    /// <summary>
    /// Gets or sets the origin property.
    /// </summary>
    /// <value>The origin property.</value>
    public string OriginProperty
    {
      get { return _originProperty; }
      internal set { _originProperty = value; }
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
      info.AddValue("_originProperty", _originProperty);
      
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
      _originProperty = info.GetValue<string>("_originProperty");

      base.OnSetState(info, mode);
    }

    #endregion
  }
}