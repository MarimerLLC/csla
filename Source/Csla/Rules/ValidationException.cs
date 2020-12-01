//-----------------------------------------------------------------------
// <copyright file="ValidationException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exception class indicating that there was a validation</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Rules
{

  /// <summary>
  /// Exception class indicating that there was a validation
  /// problem with a business object.
  /// </summary>
  [Serializable()]
  public class ValidationException : Exception
  {

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public ValidationException()
    {

    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Message describing the exception.</param>
    public ValidationException(string message)
      : base(message)
    {

    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Message describing the exception.</param>
    /// <param name="innerException">Inner exception object.</param>
   public ValidationException(string message, Exception innerException)
      : base(message, innerException)
    {

    }

#if !(ANDROID || IOS) && !NETFX_CORE
    /// <summary>
    /// Creates an instance of the object for serialization.
    /// </summary>
    /// <param name="context">Serialization context.</param>
    /// <param name="info">Serialization info.</param>
    protected ValidationException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    {

    }
#endif
  }
}