﻿//-----------------------------------------------------------------------
// <copyright file="PropertyLoaderException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Exception indicating a failure to</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization;

namespace Csla
{
  /// <summary>
  /// Exception indicating a failure to
  /// set a property's field.
  /// </summary>
  /// <remarks></remarks>
  [Serializable]
  public class PropertyLoadException : Exception
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    public PropertyLoadException(string message)
      : base(message)
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="ex">Inner exception.</param>
    public PropertyLoadException(string message, Exception ex)
      : base(message, ex)
    { }
  }
}