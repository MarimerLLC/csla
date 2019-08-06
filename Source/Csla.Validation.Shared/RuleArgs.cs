﻿//-----------------------------------------------------------------------
// <copyright file="RuleArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides the RuleArgs base class for CSLA 3.x rule engine</summary>
//----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Rules;

namespace Csla.Validation
{
  /// <summary>
  /// Object providing extra information to methods that
  /// implement business rules. As imlemented in Csla 3.8.x so older style rules will compile. 
  /// </summary>
  //[Obsolete("For migration of older apps to Csla 4.x only")]
  public class RuleArgs
  {
    private string _propertyName;
    private string _propertyFriendlyName;
    private string _description;
    private RuleSeverity _severity = RuleSeverity.Error;
    private bool _stopProcessing;

    /// <summary>
    /// The name of the property to be validated.
    /// </summary>
    public string PropertyName
    {
      get { return _propertyName; }
    }

    /// <summary>
    /// Gets or sets a friendly name for the property, which
    /// will be used in place of the property name when
    /// creating the broken rule description string.
    /// </summary>
    public string PropertyFriendlyName
    {
      get { return _propertyFriendlyName; }
      set { _propertyFriendlyName = value; }
    }

    /// <summary>
    /// Set by the rule handler method to describe the broken
    /// rule.
    /// </summary>
    /// <value>A human-readable description of
    /// the broken rule.</value>
    /// <remarks>
    /// Setting this property only has an effect if
    /// the rule method returns false.
    /// </remarks>
    public string Description
    {
      get
      { return _description; }
      set { _description = value; }
    }


    /// <summary>
    /// Gets or sets the severity of the broken rule.
    /// </summary>
    /// <value>The severity of the broken rule.</value>
    /// <remarks>
    /// Setting this property only has an effect if
    /// the rule method returns false.
    /// </remarks>
    public RuleSeverity Severity
    {
      get { return _severity; }
      set { _severity = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// broken rule should stop the processing of subsequent
    /// rules for this property.
    /// </summary>
    /// <value>true if no further
    /// rules should be process for this property.</value>
    /// <remarks>
    /// Setting this property only has an effect if
    /// the rule method returns false.
    /// </remarks>
    public bool StopProcessing
    {
      get { return _stopProcessing; }
      set { _stopProcessing = value; }
    }


    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    public RuleArgs(string propertyName)
    {

      _propertyName = propertyName;

    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyInfo">The PropertyInfo object for the property.</param>
    public RuleArgs(Core.IPropertyInfo propertyInfo)
      : this(propertyInfo.Name)
    {
      _propertyFriendlyName = propertyInfo.FriendlyName;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="friendlyName">A friendly name for the property, which
    /// will be used in place of the property name when
    /// creating the broken rule description string.</param>
    public RuleArgs(string propertyName, string friendlyName)
      : this(propertyName)
    {
      _propertyFriendlyName = friendlyName;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="severity">Initial default severity for the rule.</param>
    /// <remarks>
    /// <para>
    /// The <b>severity</b> parameter defines only the initial default 
    /// severity value. If the rule changes this value by setting
    /// e.Severity, then that new value will become the default for all
    /// subsequent rule invocations.
    /// </para><para>
    /// To avoid confusion, it is recommended that the 
    /// <b>severity</b> constructor parameter 
    /// only be used for rule methods that do not explicitly set
    /// e.Severity.
    /// </para>
    /// </remarks>
    public RuleArgs(string propertyName, RuleSeverity severity)
      : this(propertyName)
    {
      _severity = severity;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyInfo">The PropertyInfo for the property.</param>
    /// <param name="severity">Initial default severity for the rule.</param>
    /// <remarks>
    /// <para>
    /// The <b>severity</b> parameter defines only the initial default 
    /// severity value. If the rule changes this value by setting
    /// e.Severity, then that new value will become the default for all
    /// subsequent rule invocations.
    /// </para><para>
    /// To avoid confusion, it is recommended that the 
    /// <b>severity</b> constructor parameter 
    /// only be used for rule methods that do not explicitly set
    /// e.Severity.
    /// </para>
    /// </remarks>
    public RuleArgs(Core.IPropertyInfo propertyInfo, RuleSeverity severity)
      : this(propertyInfo)
    {
      _severity = severity;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="friendlyName">A friendly name for the property, which
    /// will be used in place of the property name when
    /// creating the broken rule description string.</param>
    /// <param name="severity">Initial default severity for the rule.</param>
    /// <remarks>
    /// <para>
    /// The <b>severity</b> parameter defines only the initial default 
    /// severity value. If the rule changes this value by setting
    /// e.Severity, then that new value will become the default for all
    /// subsequent rule invocations.
    /// </para><para>
    /// To avoid confusion, it is recommended that the 
    /// <b>severity</b> constructor parameter 
    /// only be used for rule methods that do not explicitly set
    /// e.Severity.
    /// </para>
    /// </remarks>
    public RuleArgs(string propertyName, string friendlyName, RuleSeverity severity)
      : this(propertyName, friendlyName)
    {
      _severity = severity;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="severity">The default severity for the rule.</param>
    /// <param name="stopProcessing">
    /// Initial default value for the StopProcessing property.
    /// </param>
    /// <remarks>
    /// <para>
    /// The <b>severity</b> and <b>stopProcessing</b> parameters 
    /// define only the initial default values. If the rule 
    /// changes these values by setting e.Severity or
    /// e.StopProcessing, then the new values will become 
    /// the default for all subsequent rule invocations.
    /// </para><para>
    /// To avoid confusion, It is recommended that the 
    /// <b>severity</b> and <b>stopProcessing</b> constructor 
    /// parameters only be used for rule methods that do 
    /// not explicitly set e.Severity or e.StopProcessing.
    /// </para>
    /// </remarks>
    public RuleArgs(string propertyName, RuleSeverity severity, bool stopProcessing)
      : this(propertyName, severity)
    {
      _stopProcessing = stopProcessing;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyInfo">The PropertyInfo for the property.</param>
    /// <param name="severity">The default severity for the rule.</param>
    /// <param name="stopProcessing">
    /// Initial default value for the StopProcessing property.
    /// </param>
    /// <remarks>
    /// <para>
    /// The <b>severity</b> and <b>stopProcessing</b> parameters 
    /// define only the initial default values. If the rule 
    /// changes these values by setting e.Severity or
    /// e.StopProcessing, then the new values will become 
    /// the default for all subsequent rule invocations.
    /// </para><para>
    /// To avoid confusion, It is recommended that the 
    /// <b>severity</b> and <b>stopProcessing</b> constructor 
    /// parameters only be used for rule methods that do 
    /// not explicitly set e.Severity or e.StopProcessing.
    /// </para>
    /// </remarks>
    public RuleArgs(Core.IPropertyInfo propertyInfo, RuleSeverity severity, bool stopProcessing)
      : this(propertyInfo, severity)
    {
      _stopProcessing = stopProcessing;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="friendlyName">A friendly name for the property, which
    /// will be used in place of the property name when
    /// creating the broken rule description string.</param>
    /// <param name="severity">The default severity for the rule.</param>
    /// <param name="stopProcessing">
    /// Initial default value for the StopProcessing property.
    /// </param>
    /// <remarks>
    /// <para>
    /// The <b>severity</b> and <b>stopProcessing</b> parameters 
    /// define only the initial default values. If the rule 
    /// changes these values by setting e.Severity or
    /// e.StopProcessing, then the new values will become 
    /// the default for all subsequent rule invocations.
    /// </para><para>
    /// To avoid confusion, It is recommended that the 
    /// <b>severity</b> and <b>stopProcessing</b> constructor 
    /// parameters only be used for rule methods that do 
    /// not explicitly set e.Severity or e.StopProcessing.
    /// </para>
    /// </remarks>
    public RuleArgs(string propertyName, string friendlyName, RuleSeverity severity, bool stopProcessing)
      : this(propertyName, friendlyName, severity)
    {
      _stopProcessing = stopProcessing;
    }

    /// <summary>
    /// Returns a string representation of the object.
    /// </summary>
    public override string ToString()
    {
      return _propertyName;
    }

    /// <summary>
    /// Gets the property name from the RuleArgs
    /// object, using the friendly name if one
    /// is defined.
    /// </summary>
    /// <param name="e">Object from which to 
    /// extract the name.</param>
    /// <returns>
    /// The friendly property name if it exists,
    /// otherwise the property name itself.
    /// </returns>
    public static string GetPropertyName(RuleArgs e)
    {
      string propName = null;

      if (string.IsNullOrEmpty(e.PropertyFriendlyName))
        propName = e.PropertyName;
      else
        propName = e.PropertyFriendlyName;
      return propName;
    }
  }
}
