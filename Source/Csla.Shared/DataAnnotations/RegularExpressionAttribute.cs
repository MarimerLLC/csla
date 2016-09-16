#if NETFX_PHONE
//-----------------------------------------------------------------------
// <copyright file="RegularExpressionAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Specifies that a data field value must</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.ComponentModel.DataAnnotations
{
  /// <summary>
  /// Specifies that a data field value must
  /// match the specified regular expression.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class RegularExpressionAttribute : ValidationAttribute
  {
    private string _expression;

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    public RegularExpressionAttribute(string expression)
    {
      _expression = expression;
      ErrorMessage = "Field must match regular expression.";
    }

    /// <summary>
    /// Validates the specified value with respect to
    /// the current validation attribute.
    /// </summary>
    /// <param name="value">Value of the object to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      Regex expression = new Regex(_expression);
      if (value != null && !string.IsNullOrEmpty(value.ToString()) && !expression.IsMatch(value.ToString()))
        return new ValidationResult(this.ErrorMessage);
      else
        return null;
    }
  }
}
#else
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ComponentModel.DataAnnotations.RegularExpressionAttribute))]
#endif