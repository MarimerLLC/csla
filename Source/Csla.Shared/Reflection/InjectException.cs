//-----------------------------------------------------------------------
// <copyright file="InjectException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This exception is returned when there is an attempt to inject parameters without an IServiceProvider in scope. </summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Permissions;

namespace Csla.Reflection
{
  /// <summary>
  /// This exception is returned from the 
  /// CallMethod method in the server-side DataPortal
  /// if there is an attempt to inject parameters without an <see cref="IServiceProvider"/> in scope.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
  [Serializable()]
  public class InjectException : Exception
  {      

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Message text describing the exception.</param>
    public InjectException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Creates an instance of the object for deserialization.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="context">Serialiation context.</param>
    protected InjectException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    {
    }
  }
}