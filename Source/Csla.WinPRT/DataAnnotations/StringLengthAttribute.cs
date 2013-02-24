﻿using System;

namespace System.ComponentModel.DataAnnotations
{
  /// <summary>
  /// Specifies that a data field value must
  /// fall within the specified range.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class StringLengthAttribute : ValidationAttribute
  {
    private int _max;

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="stringMaxLength">Maximum string length allowed.</param>
    public StringLengthAttribute(int stringMaxLength)
    {
      _max = stringMaxLength;
      ErrorMessage = "Field length must not exceed maximum length.";
    }

      /// <summary>
    /// Validates the specified value with respect to
    /// the current validation attribute.
    /// </summary>
    /// <param name="value">Value of the object to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value != null && value.ToString().Length > _max)
        return new ValidationResult(this.ErrorMessage);
      else
        return null;
    }
  }
}
