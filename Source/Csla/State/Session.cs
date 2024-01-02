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
using Csla.Serialization.Mobile;

namespace Csla.State
{
  /// <summary>
  /// Per-user session data. The values must be 
  /// serializable via MobileFormatter.
  /// </summary>
  [Serializable]
  public class Session : MobileDictionary<string, object>, INotifyPropertyChanged
  {
    /// <summary>
    /// Gets a unique id for this object.
    /// </summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

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
    /// Event raised when a property has changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raise PropertyChanged event.
    /// </summary>
    /// <param name="propertyName"></param>
    protected virtual void OnPropertyChanged(string propertyName) 
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    protected override void GetState(SerializationInfo info)
    {
      info.AddValue("id", Id);
      info.AddValue("_initializationState", _initializationState);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    protected override void SetState(SerializationInfo info)
    {
      Id = info.GetValue<Guid>("id");
      _initializationState = info.GetValue<int>("_initializationState");
    }
  }
}
