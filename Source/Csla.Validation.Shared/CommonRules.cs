﻿//-----------------------------------------------------------------------
// <copyright file="CommonRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides the Common rules for CSLA 3.x</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Properties;
using System.Text.RegularExpressions;
using System.Reflection;
using Csla.Core;
#if NETFX_CORE
using Csla.Reflection;
#endif
using Csla.Rules;

namespace Csla.Validation
{
  /// <summary>
  /// Implements common business rules.
  /// </summary>
  public static partial class CommonRules
  {

    #region StringRequired

    /// <summary>
    /// Rule ensuring a string value contains one or more
    /// characters.
    /// </summary>
    /// <param name="target">Object containing the data to validate</param>
    /// <param name="e">Arguments parameter specifying the name of the string
    /// property to validate</param>
    /// <returns>false if the rule is broken</returns>
    /// <remarks>
    /// This implementation uses late binding, and will only work
    /// against string property values.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public static bool StringRequired(object target, RuleArgs e)
    {
      string value = (string)Utilities.CallByName(
        target, e.PropertyName, CallType.Get);
      if (string.IsNullOrEmpty(value))
      {
        e.Description = string.Format(Resources.StringRequiredRule, RuleArgs.GetPropertyName(e));
        return false;
      }
      return true;
    }

    #endregion

    #region StringMaxLength

    /// <summary>
    /// Rule ensuring a string value doesn't exceed
    /// a specified length.
    /// </summary>
    /// <param name="target">Object containing the data to validate</param>
    /// <param name="e">Arguments parameter specifying the name of the string
    /// property to validate</param>
    /// <returns>false if the rule is broken</returns>
    /// <remarks>
    /// This implementation uses late binding, and will only work
    /// against string property values.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public static bool StringMaxLength(
      object target, RuleArgs e)
    {
      DecoratedRuleArgs args = (DecoratedRuleArgs)e;
      int max = (int)args["MaxLength"];
      string value = (string)Utilities.CallByName(
        target, e.PropertyName, CallType.Get);
      if (!String.IsNullOrEmpty(value) && (value.Length > max))
      {
        string format = (string)args["Format"];
        string outValue;
        if (string.IsNullOrEmpty(format))
          outValue = max.ToString();
        else
          outValue = max.ToString(format);
        e.Description = String.Format(
          Resources.StringMaxLengthRule,
          RuleArgs.GetPropertyName(e), outValue);
        return false;
      }
      return true;
    }

    /// <summary>
    /// Custom <see cref="RuleArgs" /> object required by the
    /// <see cref="StringMaxLength" /> rule method.
    /// </summary>
    public class MaxLengthRuleArgs : DecoratedRuleArgs
    {
      /// <summary>
      /// Get the max length for the string.
      /// </summary>
      public int MaxLength
      {
        get { return (int)this["MaxLength"]; }
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="maxLength">Max length of characters allowed.</param>
      public MaxLengthRuleArgs(
        string propertyName, int maxLength)
        : base(propertyName)
      {
        this["MaxLength"] = maxLength;
        this["Format"] = string.Empty;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyInfo">PropertyInfo for the property to validate.</param>
      /// <param name="maxLength">Max length of characters allowed.</param>
      public MaxLengthRuleArgs(Core.IPropertyInfo propertyInfo, int maxLength)
        : base(propertyInfo)
      {
        this["MaxLength"] = maxLength;
        this["Format"] = string.Empty;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="maxLength">Max length of characters allowed.</param>
      public MaxLengthRuleArgs(
        string propertyName, string friendlyName, int maxLength)
        : base(propertyName, friendlyName)
      {
        this["MaxLength"] = maxLength;
        this["Format"] = string.Empty;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="maxLength">Max length of characters allowed.</param>
      /// <param name="format">Format string for the max length
      /// value in the broken rule string.</param>
      public MaxLengthRuleArgs(
        string propertyName, int maxLength, string format)
        : base(propertyName)
      {
        this["MaxLength"] = maxLength;
        this["Format"] = format;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyInfo">PropertyInfo for the property to validate.</param>
      /// <param name="maxLength">Max length of characters allowed.</param>
      /// <param name="format">Format string for the max length
      /// value in the broken rule string.</param>
      public MaxLengthRuleArgs(Core.IPropertyInfo propertyInfo, int maxLength, string format)
        : base(propertyInfo)
      {
        this["MaxLength"] = maxLength;
        this["Format"] = format;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="maxLength">Max length of characters allowed.</param>
      /// <param name="format">Format string for the max length
      /// value in the broken rule string.</param>
      public MaxLengthRuleArgs(
        string propertyName, string friendlyName, int maxLength, string format)
        : base(propertyName, friendlyName)
      {
        this["MaxLength"] = maxLength;
        this["Format"] = format;
      }
    }

    #endregion

    #region StringMinLength

    /// <summary>
    /// Rule ensuring a string value has a
    /// minimum length.
    /// </summary>
    /// <param name="target">Object containing the data to validate</param>
    /// <param name="e">Arguments parameter specifying the name of the string
    /// property to validate</param>
    /// <returns>false if the rule is broken</returns>
    /// <remarks>
    /// This implementation uses late binding, and will only work
    /// against string property values.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public static bool StringMinLength(
      object target, RuleArgs e)
    {
      DecoratedRuleArgs args = (DecoratedRuleArgs)e;
      int min = (int)args["MinLength"];
      string value = (string)Utilities.CallByName(
        target, e.PropertyName, CallType.Get);
      if (String.IsNullOrEmpty(value) || (value.Length < min))
      {
        string format = (string)args["Format"];
        string outValue;
        if (string.IsNullOrEmpty(format))
          outValue = min.ToString();
        else
          outValue = min.ToString(format);
        e.Description = String.Format(
          Resources.StringMinLengthRule,
          RuleArgs.GetPropertyName(e), outValue);
        return false;
      }
      return true;
    }

    /// <summary>
    /// Custom <see cref="RuleArgs" /> object required by the
    /// <see cref="StringMinLength" /> rule method.
    /// </summary>
    public class MinLengthRuleArgs : DecoratedRuleArgs
    {
      /// <summary>
      /// Get the min length for the string.
      /// </summary>
      public int MinLength
      {
        get { return (int)this["MinLength"]; }
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="minLength">min length of characters allowed.</param>
      public MinLengthRuleArgs(
        string propertyName, int minLength)
        : base(propertyName)
      {
        this["MinLength"] = minLength;
        this["Format"] = string.Empty;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="minLength">min length of characters allowed.</param>
      public MinLengthRuleArgs(
        Core.IPropertyInfo propertyInfo, int minLength)
        : base(propertyInfo)
      {
        this["MinLength"] = minLength;
        this["Format"] = string.Empty;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="minLength">min length of characters allowed.</param>
      public MinLengthRuleArgs(
        string propertyName, string friendlyName, int minLength)
        : base(propertyName, friendlyName)
      {
        this["MinLength"] = minLength;
        this["Format"] = string.Empty;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="minLength">min length of characters allowed.</param>
      /// <param name="format">Format string for the min length
      /// value in the broken rule string.</param>
      public MinLengthRuleArgs(
        string propertyName, int minLength, string format)
        : base(propertyName)
      {
        this["MinLength"] = minLength;
        this["Format"] = format;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="minLength">min length of characters allowed.</param>
      /// <param name="format">Format string for the min length
      /// value in the broken rule string.</param>
      public MinLengthRuleArgs(
        Core.IPropertyInfo propertyInfo, int minLength, string format)
        : base(propertyInfo)
      {
        this["MinLength"] = minLength;
        this["Format"] = format;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="minLength">min length of characters allowed.</param>
      /// <param name="format">Format string for the min length
      /// value in the broken rule string.</param>
      public MinLengthRuleArgs(
        string propertyName, string friendlyName, int minLength, string format)
        : base(propertyName, friendlyName)
      {
        this["MinLength"] = minLength;
        this["Format"] = format;
      }
    }

    #endregion

    #region IntegerMaxValue

    /// <summary>
    /// Rule ensuring an integer value doesn't exceed
    /// a specified value.
    /// </summary>
    /// <param name="target">Object containing the data to validate.</param>
    /// <param name="e">Arguments parameter specifying the name of the
    /// property to validate.</param>
    /// <returns>false if the rule is broken.</returns>
    public static bool IntegerMaxValue(object target, RuleArgs e)
    {
      DecoratedRuleArgs args = (DecoratedRuleArgs)e;
      int max = (int)args["MaxValue"];
      int value = (int)Utilities.CallByName(target, e.PropertyName, CallType.Get);
      if (value > max)
      {
        string format = (string)args["Format"];
        string outValue;
        if (string.IsNullOrEmpty(format))
          outValue = max.ToString();
        else
          outValue = max.ToString(format);
        e.Description = String.Format(Resources.MaxValueRule,
          RuleArgs.GetPropertyName(e), outValue);
        return false;
      }
      return true;
    }

    /// <summary>
    /// Custom <see cref="RuleArgs" /> object required by the
    /// <see cref="IntegerMaxValue" /> rule method.
    /// </summary>
    public class IntegerMaxValueRuleArgs : DecoratedRuleArgs
    {
      /// <summary>
      /// Get the max value for the property.
      /// </summary>
      public int MaxValue
      {
        get { return (int)this["MaxValue"]; }
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      public IntegerMaxValueRuleArgs(string propertyName, int maxValue)
        : base(propertyName)
      {
        this["MaxValue"] = maxValue;
        this["Format"] = string.Empty;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      public IntegerMaxValueRuleArgs(Core.IPropertyInfo propertyInfo, int maxValue)
        : base(propertyInfo)
      {
        this["MaxValue"] = maxValue;
        this["Format"] = string.Empty;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      public IntegerMaxValueRuleArgs(
        string propertyName, string friendlyName, int maxValue)
        : base(propertyName, friendlyName)
      {
        this["MaxValue"] = maxValue;
        this["Format"] = string.Empty;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      /// <param name="format">Format string for the max value
      /// value in the broken rule string.</param>
      public IntegerMaxValueRuleArgs(string propertyName, int maxValue, string format)
        : base(propertyName)
      {
        this["MaxValue"] = maxValue;
        this["Format"] = format;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      /// <param name="format">Format string for the max value
      /// value in the broken rule string.</param>
      public IntegerMaxValueRuleArgs(Core.IPropertyInfo propertyInfo, int maxValue, string format)
        : base(propertyInfo)
      {
        this["MaxValue"] = maxValue;
        this["Format"] = format;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      /// <param name="format">Format string for the max value
      /// value in the broken rule string.</param>
      public IntegerMaxValueRuleArgs(
        string propertyName, string friendlyName, int maxValue, string format)
        : base(propertyName, friendlyName)
      {
        this["MaxValue"] = maxValue;
        this["Format"] = format;
      }
    }

    #endregion

    #region IntegerMinValue

    /// <summary>
    /// Rule ensuring an integer value doesn't go below
    /// a specified value.
    /// </summary>
    /// <param name="target">Object containing the data to validate.</param>
    /// <param name="e">Arguments parameter specifying the name of the
    /// property to validate.</param>
    /// <returns>false if the rule is broken.</returns>
    public static bool IntegerMinValue(object target, RuleArgs e)
    {
      DecoratedRuleArgs args = (DecoratedRuleArgs)e;
      int min = (int)args["MinValue"];
      int value = (int)Utilities.CallByName(target, e.PropertyName, CallType.Get);
      if (value < min)
      {
        string format = (string)args["Format"];
        string outValue;
        if (string.IsNullOrEmpty(format))
          outValue = min.ToString();
        else
          outValue = min.ToString(format);
        e.Description = String.Format(Resources.MinValueRule,
          RuleArgs.GetPropertyName(e), outValue);
        return false;
      }
      return true;
    }

    /// <summary>
    /// Custom <see cref="RuleArgs" /> object required by the
    /// <see cref="IntegerMinValue" /> rule method.
    /// </summary>
    public class IntegerMinValueRuleArgs : DecoratedRuleArgs
    {
      /// <summary>
      /// Get the min value for the property.
      /// </summary>
      public int MinValue
      {
        get { return (int)this["MinValue"]; }
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      public IntegerMinValueRuleArgs(string propertyName, int minValue)
        : base(propertyName)
      {
        this["MinValue"] = minValue;
        this["Format"] = string.Empty;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      public IntegerMinValueRuleArgs(Core.IPropertyInfo propertyInfo, int minValue)
        : base(propertyInfo)
      {
        this["MinValue"] = minValue;
        this["Format"] = string.Empty;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      public IntegerMinValueRuleArgs(
        string propertyName, string friendlyName, int minValue)
        : base(propertyName, friendlyName)
      {
        this["MinValue"] = minValue;
        this["Format"] = string.Empty;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      /// <param name="format">Format string for the min value
      /// value in the broken rule string.</param>
      public IntegerMinValueRuleArgs(string propertyName, int minValue, string format)
        : base(propertyName)
      {
        this["MinValue"] = minValue;
        this["Format"] = format;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      /// <param name="format">Format string for the min value
      /// value in the broken rule string.</param>
      public IntegerMinValueRuleArgs(Core.IPropertyInfo propertyInfo, int minValue, string format)
        : base(propertyInfo)
      {
        this["MinValue"] = minValue;
        this["Format"] = format;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      /// <param name="format">Format string for the min value
      /// value in the broken rule string.</param>
      public IntegerMinValueRuleArgs(
        string propertyName, string friendlyName, int minValue, string format)
        : base(propertyName, friendlyName)
      {
        this["MinValue"] = minValue;
        this["Format"] = format;
      }
    }

    #endregion

    #region MaxValue

    /// <summary>
    /// Rule ensuring that a numeric value
    /// doesn't exceed a specified maximum.
    /// </summary>
    /// <typeparam name="T">Type of the property to validate.</typeparam>
    /// <param name="target">Object containing value to validate.</param>
    /// <param name="e">Arguments variable specifying the
    /// name of the property to validate, along with the max
    /// allowed value.</param>
    public static bool MaxValue<T>(object target, RuleArgs e) where T : IComparable
    {
      DecoratedRuleArgs args = (DecoratedRuleArgs)e;
      PropertyInfo pi = target.GetType().GetProperty(e.PropertyName);
      T value = (T)pi.GetValue(target, null);
      T max = (T)args["MaxValue"];

      int result = value.CompareTo(max);
      if (result >= 1)
      {
        string format = (string)args["Format"];
        string outValue;
        if (string.IsNullOrEmpty(format))
          outValue = max.ToString();
        else
          outValue = string.Format(string.Format("{{0:{0}}}", format), max);
        e.Description = string.Format(Resources.MaxValueRule,
          RuleArgs.GetPropertyName(e), outValue);
        return false;
      }
      else
        return true;
    }

    /// <summary>
    /// Custom <see cref="RuleArgs" /> object required by the
    /// <see cref="MaxValue" /> rule method.
    /// </summary>
    /// <typeparam name="T">Type of the property to validate.</typeparam>
    public class MaxValueRuleArgs<T> : DecoratedRuleArgs
    {
      /// <summary>
      /// Get the max value for the property.
      /// </summary>
      public T MaxValue
      {
        get { return (T)this["MaxValue"]; }
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      public MaxValueRuleArgs(string propertyName, T maxValue)
        : base(propertyName)
      {
        this["MaxValue"] = maxValue;
        this["Format"] = string.Empty;
        this["ValueType"] = typeof(T).FullName;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      public MaxValueRuleArgs(Core.IPropertyInfo propertyInfo, T maxValue)
        : base(propertyInfo)
      {
        this["MaxValue"] = maxValue;
        this["Format"] = string.Empty;
        this["ValueType"] = typeof(T).FullName;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      public MaxValueRuleArgs(
        string propertyName, string friendlyName, T maxValue)
        : base(propertyName, friendlyName)
      {
        this["MaxValue"] = maxValue;
        this["Format"] = string.Empty;
        this["ValueType"] = typeof(T).FullName;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      /// <param name="format">Format string for the max value
      /// value in the broken rule string.</param>
      public MaxValueRuleArgs(string propertyName, T maxValue, string format)
        : base(propertyName)
      {
        this["MaxValue"] = maxValue;
        this["Format"] = format;
        this["ValueType"] = typeof(T).FullName;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      /// <param name="format">Format string for the max value
      /// value in the broken rule string.</param>
      public MaxValueRuleArgs(Core.IPropertyInfo propertyInfo, T maxValue, string format)
        : base(propertyInfo)
      {
        this["MaxValue"] = maxValue;
        this["Format"] = format;
        this["ValueType"] = typeof(T).FullName;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      /// <param name="format">Format string for the max value
      /// value in the broken rule string.</param>
      public MaxValueRuleArgs(
        string propertyName, string friendlyName, T maxValue, string format)
        : base(propertyName, friendlyName)
      {
        this["MaxValue"] = maxValue;
        this["Format"] = format;
        this["ValueType"] = typeof(T).FullName;
      }
    }

    #endregion

    #region MinValue

    /// <summary>
    /// Rule ensuring that a numeric value
    /// doesn't exceed a specified minimum.
    /// </summary>
    /// <typeparam name="T">Type of the property to validate.</typeparam>
    /// <param name="target">Object containing value to validate.</param>
    /// <param name="e">Arguments variable specifying the
    /// name of the property to validate, along with the min
    /// allowed value.</param>
    public static bool MinValue<T>(object target, RuleArgs e) where T : IComparable
    {
      DecoratedRuleArgs args = (DecoratedRuleArgs)e;
      PropertyInfo pi = target.GetType().GetProperty(e.PropertyName);
      T value = (T)pi.GetValue(target, null);
      T min = (T)args["MinValue"];

      int result = value.CompareTo(min);
      if (result <= -1)
      {
        string format = (string)args["Format"];
        string outValue;
        if (string.IsNullOrEmpty(format))
          outValue = min.ToString();
        else
          outValue = string.Format(string.Format("{{0:{0}}}", format), min);
        e.Description = string.Format(Resources.MinValueRule,
          RuleArgs.GetPropertyName(e), outValue);
        return false;
      }
      else
        return true;
    }

    /// <summary>
    /// Custom <see cref="RuleArgs" /> object required by the
    /// <see cref="MinValue" /> rule method.
    /// </summary>
    /// <typeparam name="T">Type of the property to validate.</typeparam>
    public class MinValueRuleArgs<T> : DecoratedRuleArgs
    {
      /// <summary>
      /// Get the min value for the property.
      /// </summary>
      public T MinValue
      {
        get { return (T)this["MinValue"]; }
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      public MinValueRuleArgs(string propertyName, T minValue)
        : base(propertyName)
      {
        this["MinValue"] = minValue;
        this["Format"] = string.Empty;
        this["ValueType"] = typeof(T).FullName;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      public MinValueRuleArgs(Core.IPropertyInfo propertyInfo, T minValue)
        : base(propertyInfo)
      {
        this["MinValue"] = minValue;
        this["Format"] = string.Empty;
        this["ValueType"] = typeof(T).FullName;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      public MinValueRuleArgs(
        string propertyName, string friendlyName, T minValue)
        : base(propertyName, friendlyName)
      {
        this["MinValue"] = minValue;
        this["Format"] = string.Empty;
        this["ValueType"] = typeof(T).FullName;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      /// <param name="format">Format string for the min value
      /// value in the broken rule string.</param>
      public MinValueRuleArgs(string propertyName, T minValue, string format)
        : base(propertyName)
      {
        this["MinValue"] = minValue;
        this["Format"] = format;
        this["ValueType"] = typeof(T).FullName;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      /// <param name="format">Format string for the min value
      /// value in the broken rule string.</param>
      public MinValueRuleArgs(Core.IPropertyInfo propertyInfo, T minValue, string format)
        : base(propertyInfo)
      {
        this["MinValue"] = minValue;
        this["Format"] = format;
        this["ValueType"] = typeof(T).FullName;
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      /// <param name="format">Format string for the min value
      /// value in the broken rule string.</param>
      public MinValueRuleArgs(
        string propertyName, string friendlyName, T minValue, string format)
        : base(propertyName, friendlyName)
      {
        this["MinValue"] = minValue;
        this["Format"] = format;
        this["ValueType"] = typeof(T).FullName;
      }
    }

    #endregion

    #region RegEx

    /// <summary>
    /// Rule that checks to make sure a value
    /// matches a given regex pattern.
    /// </summary>
    /// <param name="target">Object containing the data to validate</param>
    /// <param name="e">RegExRuleArgs parameter specifying the name of the 
    /// property to validate and the regex pattern.</param>
    /// <returns>False if the rule is broken</returns>
    /// <remarks>
    /// This implementation uses late binding.
    /// </remarks>
    public static bool RegExMatch(object target, RuleArgs e)
    {
      bool ruleSatisfied = false;
      DecoratedRuleArgs args = (DecoratedRuleArgs)e;
      RegExRuleArgs.NullResultOptions nullOption =
        (RegExRuleArgs.NullResultOptions)args["NullOption"];
      Regex expression = (Regex)args["RegEx"];

      object value = Utilities.CallByName(target, e.PropertyName, CallType.Get);
      if (value == null && nullOption == RegExRuleArgs.NullResultOptions.ConvertToEmptyString)
        value = string.Empty;

      if (value == null)
      {
        // if the value is null at this point
        // then return the pre-defined result value
        ruleSatisfied = (nullOption == RegExRuleArgs.NullResultOptions.ReturnTrue);
      }
      else
      {
        // the value is not null, so run the 
        // regular expression
        ruleSatisfied = expression.IsMatch(value.ToString());
      }

      if (!ruleSatisfied)
      {
        e.Description = String.Format(Resources.RegExMatchRule, RuleArgs.GetPropertyName(e));
        return false;
      }
      else
        return true;
    }

    /// <summary>
    /// List of built-in regex patterns.
    /// </summary>
    public enum RegExPatterns
    {
      /// <summary>
      /// US Social Security number pattern.
      /// </summary>
      SSN,
      /// <summary>
      /// Email address pattern.
      /// </summary>
      Email
    }

    /// <summary>
    /// Custom <see cref="RuleArgs" /> object required by the
    /// <see cref="RegExMatch" /> rule method.
    /// </summary>
    public class RegExRuleArgs : DecoratedRuleArgs
    {
      #region NullResultOptions

      /// <summary>
      /// List of options for the NullResult
      /// property.
      /// </summary>
      public enum NullResultOptions
      {
        /// <summary>
        /// Indicates that a null value
        /// should always result in the 
        /// rule returning false.
        /// </summary>
        ReturnFalse,
        /// <summary>
        /// Indicates that a null value
        /// should always result in the 
        /// rule returning true.
        /// </summary>
        ReturnTrue,
        /// <summary>
        /// Indicates that a null value
        /// should be converted to an
        /// empty string before the
        /// regular expression is
        /// evaluated.
        /// </summary>
        ConvertToEmptyString
      }

      #endregion

      /// <summary>
      /// The <see cref="RegEx"/> object used to validate
      /// the property.
      /// </summary>
      public Regex RegEx
      {
        get { return (Regex)this["RegEx"]; }
      }

      /// <summary>
      /// Gets a value indicating whether a null value
      /// means the rule will return true or false.
      /// </summary>
      public NullResultOptions NullResult
      {
        get
        {
          return (NullResultOptions)this["NullOption"];
        }
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="pattern">Built-in regex pattern to use.</param>
      public RegExRuleArgs(string propertyName, RegExPatterns pattern)
        : base(propertyName)
      {
        this["RegEx"] = new Regex(GetPattern(pattern));
        this["NullOption"] = NullResultOptions.ReturnFalse;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="pattern">Built-in regex pattern to use.</param>
      public RegExRuleArgs(Core.IPropertyInfo propertyInfo, RegExPatterns pattern)
        : base(propertyInfo)
      {
        this["RegEx"] = new Regex(GetPattern(pattern));
        this["NullOption"] = NullResultOptions.ReturnFalse;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="pattern">Built-in regex pattern to use.</param>
      public RegExRuleArgs(
        string propertyName, string friendlyName, RegExPatterns pattern)
        : base(propertyName, friendlyName)
      {
        this["RegEx"] = new Regex(GetPattern(pattern));
        this["NullOption"] = NullResultOptions.ReturnFalse;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="pattern">Custom regex pattern to use.</param>
      public RegExRuleArgs(string propertyName, string pattern)
        : base(propertyName)
      {
        this["RegEx"] = new Regex(pattern);
        this["NullOption"] = NullResultOptions.ReturnFalse;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="pattern">Custom regex pattern to use.</param>
      public RegExRuleArgs(Core.IPropertyInfo propertyInfo, string pattern)
        : base(propertyInfo)
      {
        this["RegEx"] = new Regex(pattern);
        this["NullOption"] = NullResultOptions.ReturnFalse;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="pattern">Custom regex pattern to use.</param>
      public RegExRuleArgs(
        string propertyName, string friendlyName, string pattern)
        : base(propertyName, friendlyName)
      {
        this["RegEx"] = new Regex(pattern);
        this["NullOption"] = NullResultOptions.ReturnFalse;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="regEx"><see cref="RegEx"/> object to use.</param>
      public RegExRuleArgs(string propertyName, System.Text.RegularExpressions.Regex regEx)
        : base(propertyName)
      {
        this["RegEx"] = regEx;
        this["NullOption"] = NullResultOptions.ReturnFalse;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="regEx"><see cref="RegEx"/> object to use.</param>
      public RegExRuleArgs(Core.IPropertyInfo propertyInfo, System.Text.RegularExpressions.Regex regEx)
        : base(propertyInfo)
      {
        this["RegEx"] = regEx;
        this["NullOption"] = NullResultOptions.ReturnFalse;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="regEx"><see cref="RegEx"/> object to use.</param>
      public RegExRuleArgs(
        string propertyName, string friendlyName, System.Text.RegularExpressions.Regex regEx)
        : base(propertyName, friendlyName)
      {
        this["RegEx"] = regEx;
        this["NullOption"] = NullResultOptions.ReturnFalse;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="pattern">Built-in regex pattern to use.</param>
      /// <param name="nullResult">
      /// Value indicating how a null value should be
      /// handled by the rule method.
      /// </param>
      public RegExRuleArgs(string propertyName, RegExPatterns pattern, NullResultOptions nullResult)
        : base(propertyName)
      {
        this["RegEx"] = new Regex(GetPattern(pattern));
        this["NullOption"] = nullResult;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="pattern">Built-in regex pattern to use.</param>
      /// <param name="nullResult">
      /// Value indicating how a null value should be
      /// handled by the rule method.
      /// </param>
      public RegExRuleArgs(Core.IPropertyInfo propertyInfo, RegExPatterns pattern, NullResultOptions nullResult)
        : base(propertyInfo)
      {
        this["RegEx"] = new Regex(GetPattern(pattern));
        this["NullOption"] = nullResult;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="pattern">Built-in regex pattern to use.</param>
      /// <param name="nullResult">
      /// Value indicating how a null value should be
      /// handled by the rule method.
      /// </param>
      public RegExRuleArgs(
        string propertyName, string friendlyName, RegExPatterns pattern, NullResultOptions nullResult)
        : base(propertyName, friendlyName)
      {
        this["RegEx"] = new Regex(GetPattern(pattern));
        this["NullOption"] = nullResult;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="pattern">Custom regex pattern to use.</param>
      /// <param name="nullResult">
      /// Value indicating how a null value should be
      /// handled by the rule method.
      /// </param>
      public RegExRuleArgs(string propertyName, string pattern, NullResultOptions nullResult)
        : base(propertyName)
      {
        this["RegEx"] = new Regex(pattern);
        this["NullOption"] = nullResult;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="pattern">Custom regex pattern to use.</param>
      /// <param name="nullResult">
      /// Value indicating how a null value should be
      /// handled by the rule method.
      /// </param>
      public RegExRuleArgs(Core.IPropertyInfo propertyInfo, string pattern, NullResultOptions nullResult)
        : base(propertyInfo)
      {
        this["RegEx"] = new Regex(pattern);
        this["NullOption"] = nullResult;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="pattern">Custom regex pattern to use.</param>
      /// <param name="nullResult">
      /// Value indicating how a null value should be
      /// handled by the rule method.
      /// </param>
      public RegExRuleArgs(
        string propertyName, string friendlyName, string pattern, NullResultOptions nullResult)
        : base(propertyName, friendlyName)
      {
        this["RegEx"] = new Regex(pattern);
        this["NullOption"] = nullResult;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="regEx"><see cref="RegEx"/> object to use.</param>
      /// <param name="nullResult">
      /// Value indicating how a null value should be
      /// handled by the rule method.
      /// </param>
      public RegExRuleArgs(string propertyName, System.Text.RegularExpressions.Regex regEx, NullResultOptions nullResult)
        : base(propertyName)
      {
        this["RegEx"] = regEx;
        this["NullOption"] = nullResult;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyInfo">Property to validate.</param>
      /// <param name="regEx"><see cref="RegEx"/> object to use.</param>
      /// <param name="nullResult">
      /// Value indicating how a null value should be
      /// handled by the rule method.
      /// </param>
      public RegExRuleArgs(Core.IPropertyInfo propertyInfo, System.Text.RegularExpressions.Regex regEx, NullResultOptions nullResult)
        : base(propertyInfo)
      {
        this["RegEx"] = regEx;
        this["NullOption"] = nullResult;
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="friendlyName">A friendly name for the property, which
      /// will be used in place of the property name when
      /// creating the broken rule description string.</param>
      /// <param name="regEx"><see cref="RegEx"/> object to use.</param>
      /// <param name="nullResult">
      /// Value indicating how a null value should be
      /// handled by the rule method.
      /// </param>
      public RegExRuleArgs(
        string propertyName, string friendlyName, System.Text.RegularExpressions.Regex regEx, NullResultOptions nullResult)
        : base(propertyName, friendlyName)
      {
        this["RegEx"] = regEx;
        this["NullOption"] = nullResult;
      }

      /// <summary>
      /// Returns the specified built-in regex pattern.
      /// </summary>
      /// <param name="pattern">Pattern to return.</param>
      public static string GetPattern(RegExPatterns pattern)
      {
        switch (pattern)
        {
          case RegExPatterns.SSN:
            return @"^\d{3}-\d{2}-\d{4}$";
          case RegExPatterns.Email:
            return @"^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$";
          default:
            return string.Empty;
        }
      }
    }

    #endregion

    #region CanRead

    /// <summary>
    /// Rule indicating whether the user is authorized
    /// to read the property value.
    /// </summary>
    /// <param name="target">Target object.</param>
    /// <param name="e">Rule arguments.</param>
    /// <remarks>
    /// Combine this property with short-circuiting to
    /// prevent evaluation of other rules in the case
    /// that the user isn't allowed to read the value.
    /// </remarks>
    public static bool CanRead(object target, RuleArgs e)
    {
      bool isAuthorized = true;

      BusinessBase business = target as BusinessBase;
      if (business != null && !string.IsNullOrEmpty(e.PropertyName))
        isAuthorized = business.CanReadProperty(e.PropertyName);

      if (!isAuthorized)
      {
        e.Severity = RuleSeverity.Information;
        e.Description = string.Format("You are not authorized to read this field {0}", RuleArgs.GetPropertyName(e));
      }

      return isAuthorized;
    }

    #endregion

    #region CanWrite

    /// <summary>
    /// Rule indicating whether the user is authorized
    /// to change the property value.
    /// </summary>
    /// <param name="target">Target object.</param>
    /// <param name="e">Rule arguments.</param>
    /// <remarks>
    /// Combine this property with short-circuiting to
    /// prevent evaluation of other rules in the case
    /// that the user isn't allowed to change the value.
    /// </remarks>
    public static bool CanWrite(object target, RuleArgs e)
    {
      bool isAuthorized = true;

      BusinessBase business = target as BusinessBase;
      if (business != null && !string.IsNullOrEmpty(e.PropertyName))
        isAuthorized = business.CanWriteProperty(e.PropertyName);

      if (!isAuthorized)
      {
        e.Severity = RuleSeverity.Information;
        e.Description = string.Format("You are not authorized to write to this field {0}", RuleArgs.GetPropertyName(e));
      }

      return isAuthorized;
    }

    #endregion

    #region DataAnnotations

    /// <summary>
    /// Arguments provided to the DataAnnotation
    /// rule method
    /// </summary>
    public class DataAnnotationRuleArgs : RuleArgs
    {
      /// <summary>
      /// Creates an instance of the object.
      /// </summary>
      /// <param name="name">
      /// Name of the property to be validated.
      /// </param>
      /// <param name="attribute">
      /// System.ComponentModel.DataAnnotations.ValidationAttribute object
      /// containing the rule implementation.
      /// </param>
      public DataAnnotationRuleArgs(string name, System.ComponentModel.DataAnnotations.ValidationAttribute attribute)
        : base(name)
      {
        Attribute = attribute;
      }

      /// <summary>
      /// The attribute containing the rule implementation.
      /// </summary>
      public System.ComponentModel.DataAnnotations.ValidationAttribute Attribute { get; set; }

      /// <summary>
      /// Gets a string representation of the object.
      /// </summary>
      public override string ToString()
      {
        return string.Format("{0}?Attribute={1}", base.ToString(), Attribute.GetType().FullName);
      }
    }

    /// <summary>
    /// Rule method that executes a rule contained in an 
    /// System.ComponentModel.DataAnnotations.ValidationAttribute
    /// object.
    /// </summary>
    /// <param name="target">
    /// Business object containing the value to validate.
    /// </param>
    /// <param name="e">
    /// DataAnnotationRuleArgs object.
    /// </param>
    /// <returns>True if the rule is satisfied, false if the rule fails.</returns>
    public static bool DataAnnotation(object target, RuleArgs e)
    {
      var args = (DataAnnotationRuleArgs)e;
      object pValue = Utilities.CallByName(target, e.PropertyName, CallType.Get);

      var ctx = new System.ComponentModel.DataAnnotations.ValidationContext(target, null, null)
      {
        MemberName = RuleArgs.GetPropertyName(e)
      };

      var result = args.Attribute.GetValidationResult(pValue, ctx);

      if (result != null)
      {
        e.Description = result.ErrorMessage;
        return false;
      }

      return true;
    }

    #endregion
  }
}
