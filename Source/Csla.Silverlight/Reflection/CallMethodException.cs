//-----------------------------------------------------------------------
// <copyright file="CallMethodException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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
  }
}