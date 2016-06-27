#if NETFX_PHONE || NETSTANDARD
//-----------------------------------------------------------------------
// <copyright file="RangeAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Specifies that a data field value must</summary>
//-----------------------------------------------------------------------
using System;

namespace System.ComponentModel.DataAnnotations
{
  /// <summary>
  /// Specifies that a data field value must
  /// fall within the specified range.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class RangeAttribute : ValidationAttribute
  {
    IRangeCompare _comparer;

    private RangeAttribute()
    {
      ErrorMessage = "Value must fall within range.";
    }

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="minimum">Minimum value allowed.</param>
    /// <param name="maximum">Maximum value allowed.</param>
    public RangeAttribute(int minimum, int maximum)
      : this()
    {
      _comparer = new RangeCompare<int>(minimum, maximum);
    }

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="minimum">Minimum value allowed.</param>
    /// <param name="maximum">Maximum value allowed.</param>
    public RangeAttribute(double minimum, double maximum)
      : this()
    {
      _comparer = new RangeCompare<double>(minimum, maximum);
    }

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="type">Type of object to test.</param>
    /// <param name="minimum">Minimum value allowed.</param>
    /// <param name="maximum">Maximum value allowed.</param>
    public RangeAttribute(Type type, string minimum, string maximum)
      : this()
    {
      _comparer = new RangeCompare<string>(minimum, maximum);
    }

    /// <summary>
    /// Validates the specified value with respect to
    /// the current validation attribute.
    /// </summary>
    /// <param name="value">Value of the object to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value != null && !string.IsNullOrEmpty(value.ToString()) && !_comparer.CheckRange(value))
        return new ValidationResult(this.ErrorMessage);
      else
        return null;
    }

    private interface IRangeCompare
    {
      bool CheckRange(object value);
    }

    private class RangeCompare<T> : IRangeCompare 
      where T : IComparable
    {
      T _min;
      T _max;

      public RangeCompare(T min, T max)
      {
        _max = max;
        _min = min;
      }

      public bool CheckRange(T value)
      {
        if (value.CompareTo(_min) > -1 && value.CompareTo(_max) < 1)
          return true;
        else
          return false;
      }

      bool IRangeCompare.CheckRange(object value)
      {
        return CheckRange((T)value);
      }
    }
  }
}
#else
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ComponentModel.DataAnnotations.RangeAttribute))]
#endif