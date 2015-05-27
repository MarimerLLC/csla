#if NETFX_PHONE
//-----------------------------------------------------------------------
// <copyright file="ValidationAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Base class used to create validation attributes.</summary>
//-----------------------------------------------------------------------
using System;

namespace System.ComponentModel.DataAnnotations
{
  /// <summary>
  /// Base class used to create validation attributes.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
  public abstract class ValidationAttribute : Attribute
  {
    /// <summary>
    /// Gets or sets an error message to associated with a validation control if validation fails.
    /// </summary>
    public string ErrorMessage { get; set; }
    /// <summary>
    /// Gets or sets the error resource name in order to look up the error message
    /// text from a resource if validation fails.
    /// </summary>
    public string ErrorMessageResourceName { get; set; }
    /// <summary>
    /// Gets or sets the resource type to use for message look-up if validation fails.
    /// </summary>
    public Type ErrorMessageResourceType { get; set; }

    /// <summary>
    /// Validates the specified value with respect to
    /// the current validation attribute.
    /// </summary>
    /// <param name="value">Value of the object to validate.</param>
    public virtual bool IsValid(object value)
    {
      return false;
    }

    /// <summary>
    /// Validates the specified value with respect to
    /// the current validation attribute.
    /// </summary>
    /// <param name="value">Value of the object to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    protected virtual ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      return null;
    }

    /// <summary>
    /// Checks whether the specified value is valid with respect to the current
    /// validation attribute.
    /// </summary>
    /// <param name="value">Value of the object to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    public ValidationResult GetValidationResult(object value, ValidationContext validationContext)
    {
      return IsValid(value, validationContext);
    }
  }
}
#endif