//-----------------------------------------------------------------------
// <copyright file="MobileFormatterException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>MobileFormatter exception</summary>
//-----------------------------------------------------------------------

namespace Csla.Serialization.Mobile;

/// <summary>
/// MobileFormatter exception
/// </summary>
public class MobileFormatterException : Exception
{
  /// <summary>
  /// Creates an instance of the object.
  /// </summary>
  /// <param name="message">Exception message</param>
  public MobileFormatterException(string message)
    : base(message)
  { }

  /// <summary>
  /// Creates an instance of the object.
  /// </summary>
  /// <param name="message">Exception message</param>
  /// <param name="innerException">Inner exception</param>
  public MobileFormatterException(string message, Exception innerException)
    : base(message, innerException)
  { }
}
