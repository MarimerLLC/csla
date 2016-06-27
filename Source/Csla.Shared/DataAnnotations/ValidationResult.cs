#if NETFX_PHONE || NETSTANDARD
//-----------------------------------------------------------------------
// <copyright file="ValidationResult.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Results of a validation operation.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
  /// <summary>
  /// Results of a validation operation.
  /// </summary>
  public class ValidationResult
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="errorMessage">The error message for the validation.</param>
    public ValidationResult(string errorMessage)
    {
      ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="errorMessage">The error message for the validation.</param>
    /// <param name="memberNames">The collection of member names that indicate
    /// which fields have validation errors.</param>
    public ValidationResult(string errorMessage, IEnumerable<string> memberNames)
      : this(errorMessage)
    {
      MemberNames = memberNames;
    }

    /// <summary>
    /// Gets the error message for the validation.
    /// </summary>
    public string ErrorMessage { get; protected set; }
    /// <summary>
    /// Gets the collection of member names that indicate which
    /// fields have validation errors.
    /// </summary>
    public IEnumerable<string> MemberNames { get; protected set; }
  }
}
#else
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ComponentModel.DataAnnotations.ValidationResult))]
#endif