using System;
using Csla.WcfPortal;

namespace Csla
{
  /// <summary>
  /// This exception is returned for any errors occuring
  /// during the server-side DataPortal invocation.
  /// </summary>
  public class DataPortalException : Exception
  {
    /// <summary>
    /// Gets information about the original
    /// server-side exception. That exception
    /// is not an exception on the client side,
    /// but this property returns information
    /// about the exception.
    /// </summary>
    public WcfPortal.WcfErrorInfo ErrorInfo { get; private set; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">
    /// Exception message.
    /// </param>
    /// <param name="ex">
    /// Inner exception.
    /// </param>
    public DataPortalException(string message, Exception ex)
      : base(message)
    {
      ErrorInfo = ex.ToErrorInfo();
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="info">Info about the exception.</param>
    public DataPortalException(WcfPortal.WcfErrorInfo info)
      : base(info.Message)
    {
      this.ErrorInfo = info;
    }

    /// <summary>
    /// Gets a string representation
    /// of this object.
    /// </summary>
    public override string ToString()
    {
      var sb = new System.Text.StringBuilder();
      var error = this.ErrorInfo;
      while (error != null)
      {
        sb.AppendFormat("{0}: {1}", error.ExceptionTypeName, error.Message);
        sb.Append(Environment.NewLine);
        sb.Append(error.StackTrace);
        sb.Append(Environment.NewLine);
        error = error.InnerError;
      }
      return sb.ToString();
    }
  }
}
