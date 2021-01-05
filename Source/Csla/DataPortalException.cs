//-----------------------------------------------------------------------
// <copyright file="DataPortalException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This exception is returned for any errors occuring</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Permissions;

namespace Csla
{

  /// <summary>
  /// This exception is returned for any errors occurring
  /// during the server-side DataPortal invocation.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
  [Serializable]
  public class DataPortalException : Exception
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="businessObject">The business object
    /// as it was at the time of the exception.</param>
    public DataPortalException(string message, object businessObject)
      : base(message)
    {
      _innerStackTrace = String.Empty;
      _businessObject = businessObject;
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="ex">Inner exception.</param>
    /// <param name="businessObject">The business object
    /// as it was at the time of the exception.</param>
    public DataPortalException(string message, Exception ex, object businessObject)
      : base(message, ex)
    {
      _innerStackTrace = ex.StackTrace;
      _businessObject = businessObject;
    }

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
      : base(message, ex)
    {
      _innerStackTrace = ex.StackTrace;
    }

#if !NETSTANDARD2_0 && !NET5_0
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="info">Info about the exception.</param>
    public DataPortalException(WcfPortal.WcfErrorInfo info)
      : base(info.Message)
    {
      this.ErrorInfo = new Csla.Server.Hosts.HttpChannel.HttpErrorInfo(info);
    }
#endif

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="info">Info about the exception.</param>
    public DataPortalException(Csla.Server.Hosts.HttpChannel.HttpErrorInfo info)
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
      sb.AppendLine(base.ToString());
      if (ErrorInfo != null)
      {
        sb.AppendLine("------------------------------");
        var error = this.ErrorInfo;
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
    public Csla.Server.Hosts.HttpChannel.HttpErrorInfo ErrorInfo { get; private set; }

    /// <summary>
    /// Gets the original exception error info
    /// that caused this exception.
    /// </summary>
    public Server.Hosts.HttpChannel.HttpErrorInfo BusinessErrorInfo
    {
      get
      {
        var result = ErrorInfo;
        while (result.InnerError != null)
          result = result.InnerError;
        return result;
      }
    }

    private object _businessObject;
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
    public object BusinessObject
    {
      get { return _businessObject; }
    }

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
          _businessException = this.InnerException;
          while (_businessException is Csla.Reflection.CallMethodException || _businessException is DataPortalException)
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
    public override string StackTrace
    {
      get { return String.Format("{0}{1}{2}", _innerStackTrace, Environment.NewLine, base.StackTrace); }
    }

    /// <summary>
    /// Creates an instance of the object for serialization.
    /// </summary>
    /// <param name="info">Serialization info object.</param>
    /// <param name="context">Serialization context object.</param>
    protected DataPortalException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    {
      _businessObject = info.GetValue("_businessObject", typeof(object));
      _innerStackTrace = info.GetString("_innerStackTrace");
    }

    /// <summary>
    /// Serializes the object.
    /// </summary>
    /// <param name="info">Serialization info object.</param>
    /// <param name="context">Serialization context object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
#if !NET5_0
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
#endif
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_businessObject", _businessObject);
      info.AddValue("_innerStackTrace", _innerStackTrace);
    }
  }
}