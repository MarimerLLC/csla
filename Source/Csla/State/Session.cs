//-----------------------------------------------------------------------
// <copyright file="Session.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Per-user session data.</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Core;

namespace Csla.State
{
  /// <summary>
  /// Per-user session data. The values must be 
  /// serializable via MobileFormatter.
  /// </summary>
  [Serializable]
  public class Session : MobileDictionary<string, object?>, INotifyPropertyChanged
  {
    /// <summary>
    /// Gets or sets a value indicating the last
    /// time (UTC) this object was interacted with.
    /// </summary>
    public long LastTouched { get; private set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    /// <summary>
    /// Indicate that the session object has been changed.
    /// </summary>
    public void Touch() => LastTouched = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    /// <summary>
    /// Event raised when a property has changed.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raise PropertyChanged event.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
