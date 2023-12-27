//-----------------------------------------------------------------------
// <copyright file="Session.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Per-user session data.</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Core;

namespace Csla.State
{
  /// <summary>
  /// Per-user session data. The object must be 
  /// serializable via MobileFormatter.
  /// </summary>
  [Serializable]
  public class Session : MobileDictionary<string, object>
  {
    /// <summary>
    /// Gets or sets the Session Id value.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether
    /// the session is currently checked out to
    /// a WebAssembly client component.
    /// </summary>
    /// <value>
    /// true if in use by a wasm component, 
    /// false if available for use on the server
    /// </value>
    public bool IsCheckedOut { get; set; }
  }
}
