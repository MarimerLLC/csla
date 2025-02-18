//-----------------------------------------------------------------------
// <copyright file="DataPortalException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This exception is returned for any errors occuring</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Server.Hosts.DataPortalChannel;

namespace Csla
{
  /// <summary>
  /// This exception is returned for any errors occurring
  /// during the server-side DataPortal invocation.
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
  [Serializable]
  public class DataPortalException : Exception
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="businessObject">The business object
    /// as it was at the time of the exception.</param>
    public DataPortalException(string message, object businessObject)
      : base(message)
    {
      _innerStackTrace = String.Empty;
      BusinessObject = businessObject;
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="ex">Inner exception.</param>
    /// <param name="businessObject">The business object
    /// as it was at the time of the exception.</param>
    public DataPortalException(string message, Exception ex, object businessObject)
      : base(message, ex)
    {
      _innerStackTrace = ex.StackTrace;
      BusinessObject = businessObject;
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">
    /// Exception message.
    /// </param>
    /// <param name="ex">
    /// Inner exception.
    /// </param>
    public DataPortalException(string message, Exception ex)
      : base(message, ex)
    {
      _innerStackTrace = ex.StackTrace;
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="info">Info about the exception.</param>
    public DataPortalException(DataPortalErrorInfo info)
      : base(info.Message)
    {
      ErrorInfo = info;
    }

    /// <summary>
    /// Gets a string representation
    /// of this object.
    /// </summary>
    public override string ToString()
    {
      var sb = new System.Text.StringBuilder();
      sb.AppendLine(base.ToString());
      if (ErrorInfo != null)
      {
        sb.AppendLine("------------------------------");
        var error = ErrorInfo;
        while (error != null)
        {
          sb.AppendFormat("{0}: {1}", error.ExceptionTypeName, error.Message);
          sb.Append(Environment.NewLine);
          sb.Append(error.StackTrace);
          sb.Append(Environment.NewLine);
          error = error.InnerError;
        }
      }
      return sb.ToString();
    }

    /// <summary>
    /// Gets information about the original
    /// server-side exception. That exception
    /// is not an exception on the client side,
    /// but this property returns information
    /// about the exception.
    /// </summary>
    public DataPortalErrorInfo ErrorInfo { get; private set; }

    /// <summary>
    /// Gets the original exception error info
    /// that caused this exception.
    /// </summary>
    public DataPortalErrorInfo BusinessErrorInfo
    {
      get
      {
        var result = ErrorInfo;
        while (result?.InnerError != null)
          result = result.InnerError;
        return result;
      }
    }

    private string _innerStackTrace;

    /// <summary>
    /// Returns a reference to the business object
    /// from the server-side DataPortal.
    /// </summary>
    /// <remarks>
    /// Remember that this object may be in an invalid
    /// or undefined state. This is the business object
    /// (and any child objects) as it existed when the
    /// exception occured on the server. Thus the object
    /// state may have been altered by the server and
    /// may no longer reflect data in the database.
    /// </remarks>
    public object BusinessObject { get; }

    private Exception _businessException;

    /// <summary>
    /// Gets the original server-side exception.
    /// </summary>
    /// <returns>An exception object.</returns>
    /// <remarks>
    /// Removes all DataPortalException and CallMethodException
    /// instances in the exception stack to find the original
    /// exception.
    /// </remarks>
    public Exception BusinessException
    {
      get
      {
        if (_businessException == null)
        {
          _businessException = InnerException;
          while (_businessException is Reflection.CallMethodException || _businessException is DataPortalException)
            _businessException = _businessException.InnerException;
        }
        return _businessException;
      }
    }

    /// <summary>
    /// Gets the Message property from the
    /// BusinessException, falling back to
    /// the Message value from the top-level
    /// exception.
    /// </summary>
    public string BusinessExceptionMessage
    {
      get
      {
        if (ErrorInfo != null)
          return BusinessErrorInfo.Message;
        else if (BusinessException == null)
          return Message;
        else
          return BusinessException.Message;
      }
    }

    /// <summary>
    /// Get the combined stack trace from the server
    /// and client.
    /// </summary>
    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
    public override string StackTrace
    {
      get { return $"{_innerStackTrace}{Environment.NewLine}{base.StackTrace}"; }
    }
  }
}