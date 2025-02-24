//-----------------------------------------------------------------------
// <copyright file="DataPortalException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This exception is returned from the </summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  /// <summary>
  /// This exception is returned from the 
  /// server-side DataPortal and contains the exception
  /// and context data from the server.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
  [Serializable]
  public class DataPortalException : Exception
  {
    private string _innerStackTrace;

    /// <summary>
    /// Returns the DataPortalResult object from the server.
    /// </summary>
    public DataPortalResult Result { get; }

    /// <summary>
    /// Get the server-side stack trace from the
    /// original exception.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
    public override string StackTrace
    {
      get { return $"{_innerStackTrace}{Environment.NewLine}{base.StackTrace}"; }
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="ex">Inner exception.</param>
    /// <param name="result">The data portal result object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="ex"/> or <paramref name="result"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public DataPortalException(string message, Exception ex, DataPortalResult result)
      : base(message, ex)
    {
      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(message)), nameof(message));
      if (ex is null)
        throw new ArgumentNullException(nameof(ex));

      Result = result ?? throw new ArgumentNullException(nameof(result));
      _innerStackTrace = ex.StackTrace ?? "";
    }
  }
}