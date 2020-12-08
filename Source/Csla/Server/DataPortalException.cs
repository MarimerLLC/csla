//-----------------------------------------------------------------------
// <copyright file="DataPortalException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This exception is returned from the </summary>
//-----------------------------------------------------------------------
using System;

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
    private DataPortalResult _result;
    private string _innerStackTrace;

    /// <summary>
    /// Returns the DataPortalResult object from the server.
    /// </summary>
    public DataPortalResult Result
    {
      get { return _result; }
    }

    /// <summary>
    /// Get the server-side stack trace from the
    /// original exception.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
    public override string StackTrace
    {
      get { return String.Format("{0}{1}{2}", 
        _innerStackTrace, Environment.NewLine, base.StackTrace); }
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="ex">Inner exception.</param>
    /// <param name="result">The data portal result object.</param>
    public DataPortalException(
      string message, Exception ex, DataPortalResult result)
      : base(message, ex)
    {
      _innerStackTrace = ex.StackTrace;
      _result = result;
    }

#if !(ANDROID || IOS) && !NETFX_CORE
    /// <summary>
    /// Creates an instance of the object for serialization.
    /// </summary>
    /// <param name="info">Serialiation info object.</param>
    /// <param name="context">Serialization context object.</param>
    protected DataPortalException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    {
      _result = (DataPortalResult)info.GetValue(
        "_result", typeof(DataPortalResult));
      _innerStackTrace = info.GetString("_innerStackTrace");
    }

    /// <summary>
    /// Serializes the object.
    /// </summary>
    /// <param name="info">Serialiation info object.</param>
    /// <param name="context">Serialization context object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
#if !NET5_0
    [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
    [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
#endif
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_result", _result);
      info.AddValue("_innerStackTrace", _innerStackTrace);
    }
#endif
  }
}