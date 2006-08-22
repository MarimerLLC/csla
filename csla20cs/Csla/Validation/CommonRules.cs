using System;
using Csla.Properties;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Csla.Validation
{
  public static class CommonRules
  {

    #region StringRequired

    /// <summary>
    /// Rule ensuring a string value contains one or more
    /// characters.
    /// </summary>
    /// <param name="target">Object containing the data to validate</param>
    /// <param name="e">Arguments parameter specifying the name of the string
    /// property to validate</param>
    /// <returns><see langword="false" /> if the rule is broken</returns>
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
        e.Description = string.Format(Resources.StringRequiredRule, e.PropertyName);
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
    /// <returns><see langword="false" /> if the rule is broken</returns>
    /// <remarks>
    /// This implementation uses late binding, and will only work
    /// against string property values.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public static bool StringMaxLength(
      object target, RuleArgs e)
    {
      int max = ((MaxLengthRuleArgs)e).MaxLength;
      string value = (string)Utilities.CallByName(
        target, e.PropertyName, CallType.Get);
      if (!String.IsNullOrEmpty(value) && (value.Length > max))
      {
        e.Description = String.Format(
          Resources.StringMaxLengthRule,
          e.PropertyName, max.ToString());
        return false;
      }
      return true;
    }

    /// <summary>
    /// Custom <see cref="RuleArgs" /> object required by the
    /// <see cref="StringMaxLength" /> rule method.
    /// </summary>
    public class MaxLengthRuleArgs : RuleArgs
    {
      private int _maxLength;

      /// <summary>
      /// Get the max length for the string.
      /// </summary>
      public int MaxLength
      {
        get { return _maxLength; }
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
        _maxLength = maxLength;
      }

      /// <summary>
      /// Return a string representation of the object.
      /// </summary>
      public override string ToString()
      {
        return base.ToString() + "?maxLength=" + _maxLength.ToString();
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
    /// <returns><see langword="false"/> if the rule is broken.</returns>
    public static bool IntegerMaxValue(object target, RuleArgs e)
    {
      int max = ((IntegerMaxValueRuleArgs)e).MaxValue;
      int value = (int)Utilities.CallByName(target, e.PropertyName, CallType.Get);
      if (value > max)
      {
        e.Description = String.Format(Resources.MaxValueRule,
          e.PropertyName, max.ToString());
        return false;
      }
      return true;
    }

    /// <summary>
    /// Custom <see cref="RuleArgs" /> object required by the
    /// <see cref="IntegerMaxValue" /> rule method.
    /// </summary>
    public class IntegerMaxValueRuleArgs : RuleArgs
    {
      private int _maxValue;

      /// <summary>
      /// Get the max value for the property.
      /// </summary>
      public int MaxValue
      {
        get { return _maxValue; }
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      public IntegerMaxValueRuleArgs(string propertyName, int maxValue)
        : base(propertyName)
      {
        _maxValue = maxValue;
      }

      /// <summary>
      /// Return a string representation of the object.
      /// </summary>
      public override string ToString()
      {
        return base.ToString() + "?maxValue=" + _maxValue.ToString();
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
    /// <returns><see langword="false"/> if the rule is broken.</returns>
    public static bool IntegerMinValue(object target, RuleArgs e)
    {
      int min = ((IntegerMinValueRuleArgs)e).MinValue;
      int value = (int)Utilities.CallByName(target, e.PropertyName, CallType.Get);
      if (value < min)
      {
        e.Description = String.Format(Resources.MinValueRule,
          e.PropertyName, min.ToString());
        return false;
      }
      return true;
    }

    /// <summary>
    /// Custom <see cref="RuleArgs" /> object required by the
    /// <see cref="IntegerMinValue" /> rule method.
    /// </summary>
    public class IntegerMinValueRuleArgs : RuleArgs
    {
      private int _minValue;

      /// <summary>
      /// Get the min value for the property.
      /// </summary>
      public int MinValue
      {
        get { return _minValue; }
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      public IntegerMinValueRuleArgs(string propertyName, int minValue)
        : base(propertyName)
      {
        _minValue = minValue;
      }

      /// <summary>
      /// Return a string representation of the object.
      /// </summary>
      public override string ToString()
      {
        return base.ToString() + "?minValue=" + _minValue.ToString();
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
      PropertyInfo pi = target.GetType().GetProperty(e.PropertyName);
      T value = (T)pi.GetValue(target, null);
      T max = ((MaxValueRuleArgs<T>)e).MaxValue;

      int result = value.CompareTo(max);
      if (result == 1)
      {
        e.Description = string.Format(Resources.MaxValueRule,
          e.PropertyName, max.ToString());
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
    public class MaxValueRuleArgs<T> : RuleArgs
    {
      T _maxValue = default(T);

      /// <summary>
      /// Get the max value for the property.
      /// </summary>
      public T MaxValue
      {
        get { return _maxValue; }
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="maxValue">Maximum allowed value for the property.</param>
      public MaxValueRuleArgs(string propertyName, T maxValue)
        : base(propertyName)
      {
        _maxValue = maxValue;
      }

      /// <summary>
      /// Returns a string representation of the object.
      /// </summary>
      public override string ToString()
      {
        return base.ToString() + "?maxValue=" + _maxValue.ToString();
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
      PropertyInfo pi = target.GetType().GetProperty(e.PropertyName);
      T value = (T)pi.GetValue(target, null);
      T min = ((MinValueRuleArgs<T>)e).MinValue;

      int result = value.CompareTo(min);
      if (result == -1)
      {
        e.Description = string.Format(Resources.MinValueRule,
          e.PropertyName, min.ToString());
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
    public class MinValueRuleArgs<T> : RuleArgs
    {
      T _minValue = default(T);

      /// <summary>
      /// Get the min value for the property.
      /// </summary>
      public T MinValue
      {
        get { return _minValue; }
      }

      /// <summary>
      /// Create a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property.</param>
      /// <param name="minValue">Minimum allowed value for the property.</param>
      public MinValueRuleArgs(string propertyName, T minValue)
        : base(propertyName)
      {
        _minValue = minValue;
      }

      /// <summary>
      /// Returns a string representation of the object.
      /// </summary>
      public override string ToString()
      {
        return base.ToString() + "?minValue=" + _minValue.ToString();
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
      Regex rx = ((RegExRuleArgs)e).RegEx;
      if (!rx.IsMatch(Utilities.CallByName(target, e.PropertyName, CallType.Get).ToString()))
      {
        e.Description = String.Format(Resources.RegExMatchRule, e.PropertyName);
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
    public class RegExRuleArgs : RuleArgs
    {
      Regex _regEx;

      /// <summary>
      /// The <see cref="RegEx"/> object used to validate
      /// the property.
      /// </summary>
      public Regex RegEx
      {
        get { return _regEx; }
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="pattern">Built-in regex pattern to use.</param>
      public RegExRuleArgs(string propertyName, RegExPatterns pattern)
        :
        base(propertyName)
      {
        _regEx = new Regex(GetPattern(pattern));
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="pattern">Custom regex pattern to use.</param>
      public RegExRuleArgs(string propertyName, string pattern)
        :
        base(propertyName)
      {
        _regEx = new Regex(pattern);
      }

      /// <summary>
      /// Creates a new object.
      /// </summary>
      /// <param name="propertyName">Name of the property to validate.</param>
      /// <param name="regEx"><see cref="RegEx"/> object to use.</param>
      public RegExRuleArgs(string propertyName, System.Text.RegularExpressions.Regex regex)
        :
        base(propertyName)
      {
        _regEx = regex;
      }

      /// <summary>f
      /// Returns a string representation of the object.
      /// </summary>
      public override string ToString()
      {
        return base.ToString() + "?regex=" + _regEx.ToString();
      }

      /// <summary>
      /// Returns the specified built-in regex pattern.
      /// </summary>
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

  }
}
