using System.ComponentModel;

namespace Csla.Core
{
  /// <summary>
  /// Interface defining an object that notifies when it
  /// is busy executing an asynchronous operation.
  /// </summary>
  public interface INotifyBusy : INotifyUnhandledAsyncException
  {
    /// <summary>
    /// Event raised when the object's busy
    /// status changes.
    /// </summary>
    event BusyChangedEventHandler BusyChanged;

    /// <summary>
    /// Gets a value indicating whether the object,
    /// or any of the object's child objects, are
    /// busy running an asynchronous operation.
    /// </summary>
    bool IsBusy { get; }
    /// <summary>
    /// Gets a value indicating whether the object
    /// is busy running an asynchronous operation.
    /// </summary>
    bool IsSelfBusy { get; }
  }
}
