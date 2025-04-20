//-----------------------------------------------------------------------
// <copyright file="ConfigurationErrorsException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exception class indicating that there was a validation</summary>
//-----------------------------------------------------------------------

namespace Csla.Configuration
{
  /// <summary>
  /// Exception thrown due to configuration errors.
  /// </summary>
  [Serializable]
  public class ConfigurationErrorsException : Exception
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public ConfigurationErrorsException()
    {

    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Message describing the exception.</param>
    public ConfigurationErrorsException(string? message)
      : base(message)
    {

    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Message describing the exception.</param>
    /// <param name="innerException">Inner exception object.</param>
    public ConfigurationErrorsException(string? message, Exception? innerException)
       : base(message, innerException)
    {

    }
  }
}