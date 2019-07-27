﻿//-----------------------------------------------------------------------
// <copyright file="DecoratedRuleArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides the DecoratedRuleArgs base class for CSLA 3.x rule engine</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Rules;

#if (ANDROID || IOS)
using Uri = Csla.Utilities;
#endif

namespace Csla.Validation
{
  /// <summary>
  /// Object providing extra information to methods that
  /// implement business rules.
  /// </summary>
  public class DecoratedRuleArgs : RuleArgs
  {
    private Dictionary<string, object> _decorations;

    #region Base Constructors

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    public DecoratedRuleArgs(string propertyName)
      : base(propertyName)
    {
      _decorations = new Dictionary<string, object>();
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyInfo">The PropertyInfo for the property to be validated.</param>
    public DecoratedRuleArgs(Core.IPropertyInfo propertyInfo)
      : base(propertyInfo)
    {
      _decorations = new Dictionary<string, object>();
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="friendlyName">A friendly name for the property, which
    /// will be used in place of the property name when
    /// creating the broken rule description string.</param>
    public DecoratedRuleArgs(string propertyName, string friendlyName)
      : base(propertyName, friendlyName)
    {
      _decorations = new Dictionary<string, object>();
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
    public DecoratedRuleArgs(string propertyName, RuleSeverity severity)
      : base(propertyName, severity)
    {
      _decorations = new Dictionary<string, object>();
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyInfo">The PropertyInfo for the property to be validated.</param>
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
    public DecoratedRuleArgs(Core.IPropertyInfo propertyInfo, RuleSeverity severity)
      : base(propertyInfo, severity)
    {
      _decorations = new Dictionary<string, object>();
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
    public DecoratedRuleArgs(string propertyName, string friendlyName, RuleSeverity severity)
      : base(propertyName, friendlyName, severity)
    {
      _decorations = new Dictionary<string, object>();
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
    public DecoratedRuleArgs(string propertyName, RuleSeverity severity, bool stopProcessing)
      : base(propertyName, severity, stopProcessing)
    {
      _decorations = new Dictionary<string, object>();
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyInfo">The PropertyInfo for the property to be validated.</param>
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
    public DecoratedRuleArgs(Core.IPropertyInfo propertyInfo, RuleSeverity severity, bool stopProcessing)
      : base(propertyInfo, severity, stopProcessing)
    {
      _decorations = new Dictionary<string, object>();
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
    public DecoratedRuleArgs(string propertyName, string friendlyName, RuleSeverity severity, bool stopProcessing)
      : base(propertyName, friendlyName, severity, stopProcessing)
    {
      _decorations = new Dictionary<string, object>();
    }

    #endregion

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="args">Reference to a Dictionary containing 
    /// name/value arguments for use by the rule method.</param>
    public DecoratedRuleArgs(string propertyName, Dictionary<string, object> args)
      : base(propertyName)
    {
      _decorations = args;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyInfo">The PropertyInfo for the property to be validated.</param>
    /// <param name="args">Reference to a Dictionary containing 
    /// name/value arguments for use by the rule method.</param>
    public DecoratedRuleArgs(Core.IPropertyInfo propertyInfo, Dictionary<string, object> args)
      : base(propertyInfo)
    {
      _decorations = args;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="friendlyName">A friendly name for the property, which
    /// will be used in place of the property name when
    /// creating the broken rule description string.</param>
    /// <param name="args">Reference to a Dictionary containing 
    /// name/value arguments for use by the rule method.</param>
    public DecoratedRuleArgs(string propertyName, string friendlyName, Dictionary<string, object> args)
      : base(propertyName, friendlyName)
    {
      _decorations = args;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="severity">Initial default severity for the rule.</param>
    /// <param name="args">Reference to a Dictionary containing 
    /// name/value arguments for use by the rule method.</param>
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
    public DecoratedRuleArgs(string propertyName, RuleSeverity severity, Dictionary<string, object> args)
      : base(propertyName, severity)
    {
      _decorations = args;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyInfo">The PropertyInfo for the property to be validated.</param>
    /// <param name="severity">Initial default severity for the rule.</param>
    /// <param name="args">Reference to a Dictionary containing 
    /// name/value arguments for use by the rule method.</param>
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
    public DecoratedRuleArgs(Core.IPropertyInfo propertyInfo, RuleSeverity severity, Dictionary<string, object> args)
      : base(propertyInfo, severity)
    {
      _decorations = args;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="friendlyName">A friendly name for the property, which
    /// will be used in place of the property name when
    /// creating the broken rule description string.</param>
    /// <param name="severity">Initial default severity for the rule.</param>
    /// <param name="args">Reference to a Dictionary containing 
    /// name/value arguments for use by the rule method.</param>
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
    public DecoratedRuleArgs(string propertyName, string friendlyName, RuleSeverity severity, Dictionary<string, object> args)
      : base(propertyName, friendlyName, severity)
    {
      _decorations = args;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="severity">The default severity for the rule.</param>
    /// <param name="stopProcessing">
    /// Initial default value for the StopProcessing property.
    /// </param>
    /// <param name="args">Reference to a Dictionary containing 
    /// name/value arguments for use by the rule method.</param>
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
    public DecoratedRuleArgs(string propertyName, RuleSeverity severity, bool stopProcessing, Dictionary<string, object> args)
      : base(propertyName, severity, stopProcessing)
    {
      _decorations = args;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyInfo">The PropertyInfo for the property to be validated.</param>
    /// <param name="severity">The default severity for the rule.</param>
    /// <param name="stopProcessing">
    /// Initial default value for the StopProcessing property.
    /// </param>
    /// <param name="args">Reference to a Dictionary containing 
    /// name/value arguments for use by the rule method.</param>
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
    public DecoratedRuleArgs(Core.IPropertyInfo propertyInfo, RuleSeverity severity, bool stopProcessing, Dictionary<string, object> args)
      : base(propertyInfo, severity, stopProcessing)
    {
      _decorations = args;
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
    /// <param name="args">Reference to a Dictionary containing 
    /// name/value arguments for use by the rule method.</param>
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
    public DecoratedRuleArgs(string propertyName, string friendlyName, RuleSeverity severity, bool stopProcessing, Dictionary<string, object> args)
      : base(propertyName, friendlyName, severity, stopProcessing)
    {
      _decorations = args;
    }

    /// <summary>
    /// Gets or sets an argument value for use
    /// by the rule method.
    /// </summary>
    /// <param name="key">The name under which the value is stored.</param>
    /// <returns></returns>
    public object this[string key]
    {
      get
      {
        object result = null;
        if (_decorations.TryGetValue(key, out result))
          return result;
        else
          return null;
      }
      set
      {
        _decorations[key] = value;
      }
    }

    /// <summary>
    /// Return a string representation of
    /// the object using the rule:// URI
    /// format.
    /// </summary>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(base.ToString());
      if (_decorations.Count > 0)
      {
        sb.Append("?");
        bool first = true;
        foreach (System.Collections.Generic.KeyValuePair<string, object> item in _decorations)
        {
          if (first)
            first = false;
          else
            sb.Append("&");
          if (item.Key != null)
          {
            var itemString = Uri.EscapeDataString(item.Key);
            string valueString;
            if (item.Value == null)
              valueString = string.Empty;
            else
              valueString = Uri.EscapeDataString(item.Value.ToString());
            sb.AppendFormat("{0}={1}", itemString, valueString);
          }
        }
      }
      return sb.ToString();
    }
  }
}
