//-----------------------------------------------------------------------
// <copyright file="ConfigurationErrorsException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Exception class indicating that there was a validation</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Configuration
{
  /// <summary>
  /// Exception thrown due to configuration errors.
  /// </summary>
  [Serializable]
  public class ConfigurationErrorsException : Exception
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public ConfigurationErrorsException()
    {

    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Message describing the exception.</param>
    public ConfigurationErrorsException(string message)
      : base(message)
    {

    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Message describing the exception.</param>
    /// <param name="innerException">Inner exception object.</param>
    public ConfigurationErrorsException(string message, Exception innerException)
       : base(message, innerException)
    {

    }

#if !(ANDROID || IOS) && !NETFX_CORE
    /// <summary>
    /// Creates an instance of the object for serialization.
    /// </summary>
    /// <param name="context">Serialization context.</param>
    /// <param name="info">Serialization info.</param>
    protected ConfigurationErrorsException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    {

    }
#endif
  }
}