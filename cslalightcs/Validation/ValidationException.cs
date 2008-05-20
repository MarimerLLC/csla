using System;

namespace Csla.Validation
{

  /// <summary>
  /// Exception class indicating that there was a validation
  /// problem with a business object.
  /// </summary>
  public class ValidationException : Exception
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public ValidationException()
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Message describing the exception.</param>
    public ValidationException(string message)
      : base(message)
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Message describing the exception.</param>
    /// <param name="innerException">Inner exception object.</param>
    public ValidationException(string message, Exception innerException)
      : base(message, innerException)
    { }
  }
}