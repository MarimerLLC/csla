//-----------------------------------------------------------------------
// <copyright file="CallMethodException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This exception is returned from the </summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Permissions;

namespace Csla.Reflection
{
  /// <summary>
  /// This exception is returned from the 
  /// CallMethod method in the server-side DataPortal
  /// and contains the exception thrown by the
  /// underlying business object method that was
  /// being invoked.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
  [Serializable()]
  public class CallMethodException : Exception
  {
    private string _innerStackTrace;

    /// <summary>
    /// Get the stack trace from the original
    /// exception.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
    public override string StackTrace
    {
      get
      {
        return string.Format("{0}{1}{2}", 
          _innerStackTrace, Environment.NewLine, base.StackTrace);
      }
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Message text describing the exception.</param>
    /// <param name="ex">Inner exception object.</param>
    public CallMethodException(string message, Exception ex)
      : base(message, ex)
    {
      _innerStackTrace = ex.StackTrace;
    }

    /// <summary>
    /// Creates an instance of the object for deserialization.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="context">Serialiation context.</param>
    protected CallMethodException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    {
      _innerStackTrace = info.GetString("_innerStackTrace");
    }


    /// <summary>
    /// Serializes the object.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="context">Serialization context.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
#if !NET5_0
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
#endif
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_innerStackTrace", _innerStackTrace);
    }
  }
}