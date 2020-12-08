//-----------------------------------------------------------------------
// <copyright file="BusinessRuleBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create business and validation</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Csla.Properties;

namespace Csla.Rules
{
  /// <summary>
  /// Base class used to create business and validation
  /// rules.
  /// </summary>
  public abstract class BusinessRuleBase : IBusinessRuleBase
  {
    private Csla.Core.IPropertyInfo _primaryProperty;
    private RunModes _runMode;
    private bool _provideTargetWhenAsync;
    private int _priority;
    private RuleUri _ruleUri;

    /// <summary>
    /// Gets or sets a value indicating whether
    /// property values should be locked because
    /// an async operation is running.
    /// </summary>
    protected bool PropertiesLocked { get; set; }

    /// <summary>
    /// Gets or sets the primary property affected by this rule.
    /// </summary>
    public virtual Csla.Core.IPropertyInfo PrimaryProperty
    {
      get { return _primaryProperty; }
      set
      {
        CanWriteProperty("PrimaryProperty");
        _primaryProperty = value;
        this.RuleUri = new RuleUri(this, value);
        if (_primaryProperty != null)
          AffectedProperties.Add(_primaryProperty);
      }
    }

    /// <summary>
    /// Gets a list of properties affected by this rule. Rules for these
    /// properties are executed after rules for the primary
    /// property.
    /// </summary>
    public List<Csla.Core.IPropertyInfo> AffectedProperties { get; private set; }

    /// <summary>
    /// Gets a list of secondary property values to be supplied to the
    /// rule when it is executed.
    /// </summary>
    public List<Csla.Core.IPropertyInfo> InputProperties { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the rule will run
    /// on a background thread.
    /// </summary>
    public abstract bool IsAsync { get; protected set; }

    /// <summary>
    /// Gets a value indicating that the Target property should
    /// be set even for an async rule (note that using Target
    /// from a background thread will cause major problems).
    /// </summary>
    public bool ProvideTargetWhenAsync
    {
      get { return _provideTargetWhenAsync; }
      protected set
      {
        CanWriteProperty("ProvideTargetWhenAsync");
        _provideTargetWhenAsync = value;
      }
    }

    /// <summary>
    /// Gets a unique rule:// URI for the specific instance
    /// of the rule within the context of the business object
    /// where the rule is used.
    /// </summary>
    public string RuleName { get { return this.RuleUri.ToString(); } }

    /// <summary>
    /// Sets or gets the rule:// URI object for this rule.
    /// </summary>
    protected RuleUri RuleUri
    {
      get { return _ruleUri; }
      set
      {
        CanWriteProperty("RuleUri");
        _ruleUri = value;
      }
    }

    /// <summary>
    /// Gets the rule priority.
    /// </summary>
    public int Priority
    {
      get { return _priority; }
      set
      {
        CanWriteProperty("Priority");
        _priority = value;
      }
    }

    /// <summary>
    /// Gets or sets the run in context.
    /// </summary>
    /// <value>The run in context.</value>
    public RunModes RunMode
    {
      get { return _runMode; }
      set
      {
        CanWriteProperty("RunMode");
        _runMode = value;
      }
    }

    /// <summary>
    /// Allows or blocks changing a property value.
    /// </summary>
    /// <param name="argument"></param>
    protected void CanWriteProperty(string argument)
    {
      if (PropertiesLocked) throw
        new ArgumentException(string.Format("{0} ({1})", Resources.PropertySetNotAllowed, argument), argument);
    }

    /// <summary>
    /// Creates an instance of the rule that applies
    /// to a specfic property.
    /// </summary>
    /// <param name="primaryProperty">Primary property for this rule.</param>
    protected BusinessRuleBase(Csla.Core.IPropertyInfo primaryProperty)
    {
      AffectedProperties = new List<Core.IPropertyInfo>();
      InputProperties = new List<Core.IPropertyInfo>();
      PrimaryProperty = primaryProperty;
      this.RuleUri = new RuleUri(this, primaryProperty);
      RunMode = RunModes.Default;
    }

    /// <summary>
    /// Loads a property's managed field with the 
    /// supplied value calling PropertyHasChanged 
    /// if the value does change.
    /// </summary>
    /// <param name="obj">
    /// Object on which to call the method. 
    /// </param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <remarks>
    /// No authorization checks occur when this method is called,
    /// and no PropertyChanging or PropertyChanged events are raised.
    /// Loading values does not cause validation rules to be
    /// invoked.
    /// </remarks>
    protected void LoadProperty(object obj, Csla.Core.IPropertyInfo propertyInfo, object newValue)
    {
      if (obj is Core.IManageProperties target)
        target.LoadProperty(propertyInfo, newValue);
      else
        throw new ArgumentException(Resources.IManagePropertiesRequiredException);
    }

    /// <summary>
    /// Reads a property's field value.
    /// </summary>
    /// <param name="obj">
    /// Object on which to call the method. 
    /// </param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <remarks>
    /// No authorization checks occur when this method is called.
    /// </remarks>
    protected object ReadProperty(object obj, Csla.Core.IPropertyInfo propertyInfo)
    {
      if (obj is Core.IManageProperties target)
        return target.ReadProperty(propertyInfo);
      else
        throw new ArgumentException(Resources.IManagePropertiesRequiredException);
    }
  }
}