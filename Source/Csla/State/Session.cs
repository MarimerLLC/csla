//-----------------------------------------------------------------------
// <copyright file="Session.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Per-user session data.</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using Csla.Core;

namespace Csla.State
{
  /// <summary>
  /// Per-user session data. The values must be 
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
    /// <summary>
    /// Gets a value indicating whether the session
    /// is fully initialized and ready for use.
    /// </summary>
    public bool IsFullyInitialized => _initializationState == 2;
    
    /// <summary>
    /// 0 = entirely uninitialized
    /// 1 = initialized once, not yet available
    /// 2 = initialized 2+ times, ready for use
    /// </summary>
    private int _initializationState;

    /// <summary>
    /// Sets the initialization state of the object.
    /// FOR INTERNAL USE ONLY.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Initialize()
    {
      if (_initializationState < 2) 
        _initializationState++;
    }
  }
}
