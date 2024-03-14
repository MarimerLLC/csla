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
  public class Session : MobileDictionary<string, object>, INotifyPropertyChanged
  {
    private bool _isCheckedOut;

    /// <summary>
    /// Gets or sets a value indicating whether
    /// the session is currently checked out to
    /// a WebAssembly client component.
    /// </summary>
    /// <value>
    /// true if in use by a wasm component, 
    /// false if available for use on the server
    /// </value>
    public bool IsCheckedOut
    {
      get => _isCheckedOut;
      set
      {
        _isCheckedOut = value;
        OnPropertyChanged(nameof(IsCheckedOut));
      }
    }
    /// <summary>
    /// Gets or sets a value indicating the last
    /// time (UTC) this object was interacted with.
    /// </summary>
    public DateTimeOffset LastTouched { get; set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Event raised when a property has changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raise PropertyChanged event.
    /// </summary>
    /// <param name="propertyName"></param>
    protected virtual void OnPropertyChanged(string propertyName) 
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}
