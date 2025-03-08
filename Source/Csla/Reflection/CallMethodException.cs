//-----------------------------------------------------------------------
// <copyright file="CallMethodException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This exception is returned from the </summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Csla.Reflection
{
  /// <summary>
  /// This exception is returned from the 
  /// CallMethod method in the server-side DataPortal
  /// and contains the exception thrown by the
  /// underlying business object method that was
  /// being invoked.
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
  [Serializable]
  public class CallMethodException : Exception
  {
    private string? _innerStackTrace;

    /// <summary>
    /// Get the stack trace from the original
    /// exception.
    /// </summary>
    /// <value></value>
    /// <remarks></remarks>
    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
    public override string StackTrace
    {
      get
      {
        return $"{_innerStackTrace}{Environment.NewLine}{base.StackTrace}";
      }
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Message text describing the exception.</param>
    /// <param name="ex">Inner exception object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="ex"/> is <see langword="null"/>.</exception>
    public CallMethodException(string? message, Exception ex)
      : base(message, ex)
    {
      Guard.NotNull(ex);

      _innerStackTrace = ex.StackTrace;
    }
  }
}