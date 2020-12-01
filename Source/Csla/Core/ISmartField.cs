//-----------------------------------------------------------------------
// <copyright file="ISmartField.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines members required for smart</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core
{
  /// <summary>
  /// Defines members required for smart
  /// data types that understand the concept
  /// of being empty, and representing an
  /// empty value as an empty string.
  /// </summary>
  public interface ISmartField
  {
    /// <summary>
    /// Sets or gets the text representation
    /// of the value.
    /// </summary>
    /// <remarks>
    /// An empty string indicates an empty value.
    /// </remarks>
    string Text { get; set; }
    /// <summary>
    /// Gets a value indicating whether the
    /// field's value is empty.
    /// </summary>
    bool IsEmpty { get; }
    }
}