using System;

namespace Csla.Validation
{
  public static class CommonRules
  {

    /// <summary>
    /// Rule ensuring a String value contains one or more
    /// characters.
    /// </summary>
    /// <param name="target">Object containing the data to validate</param>
    /// <param name="e">Arguments parameter specifying the name of the String
    /// property to validate</param>
    /// <returns>False if the rule is broken</returns>
    /// <remarks>
    /// This implementation uses late binding, and will only work
    /// against String property values.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public static bool StringRequired(object target, RuleArgs e)
    {
      string value = (string)Utilities.CallByName(target, e.PropertyName, CallType.Get);
      if (string.IsNullOrEmpty(value))
      {
        e.Description = e.PropertyName + " required";
        return false;
      }
      return true;
    }

    /// <summary>
    /// Rule ensuring a String value doesn't exceed
    /// a specified length.
    /// </summary>
    /// <param name="target">Object containing the data to validate</param>
    /// <param name="e">Arguments parameter specifying the name of the String
    /// property to validate</param>
    /// <returns>False if the rule is broken</returns>
    /// <remarks>
    /// This implementation uses late binding, and will only work
    /// against String property values.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public static bool StringMaxLength(object target, RuleArgs e)
    {
      int max = ((MaxLengthRuleArgs)e).MaxLength;
      string value = (string)Utilities.CallByName(target, e.PropertyName, CallType.Get);
      if (!String.IsNullOrEmpty(value) && (value.Length > max))
      {
        e.Description = String.Format("{0} can not exceed {1} characters", 
          e.PropertyName, max.ToString());
        return false;
      }
      return true;
    }

    public class MaxLengthRuleArgs : RuleArgs
    {
      private int _maxLength;

      public int MaxLength
      {
        get { return _maxLength; }
      }

      public MaxLengthRuleArgs(string propertyName, int maxLength)
        : base(propertyName)
      {
        _maxLength = maxLength;
      }
    }

    public static bool IntegerMaxValue(object target, RuleArgs e)
    {
      int max = ((IntegerMaxValueRuleArgs)e).MaxValue;
      int value = (int)Utilities.CallByName(target, e.PropertyName, CallType.Get);
      if (value > max)
      {
        e.Description = String.Format("{0} can not exceed {1}",
          e.PropertyName, max.ToString());
        return false;
      }
      return true;
    }

    public class IntegerMaxValueRuleArgs : RuleArgs
    {
      private int _maxValue;

      public int MaxValue
      {
        get { return _maxValue; }
      }

      public IntegerMaxValueRuleArgs(string propertyName, int maxValue)
        : base(propertyName)
      {
        _maxValue = maxValue;
      }
    }

  }
}
