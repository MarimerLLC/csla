using System;

namespace CSLA.Core 
{
  /// <summary>
  /// This base class declares the IsDirtyChanged event
  /// to be NonSerialized so serialization will work.
  /// </summary>
  [Serializable()]
  public abstract class BindableBase
  {
    /// <summary>
    /// Declares a serialization-safe IsDirtyChanged event.
    /// </summary>
    [field: NonSerialized]
    public event EventHandler IsDirtyChanged;

    /// <summary>
    /// Call this method to raise the IsDirtyChanged event.
    /// </summary>
    virtual protected void OnIsDirtyChanged()
    {
      if (IsDirtyChanged != null)
        IsDirtyChanged(this, EventArgs.Empty);
    }
  }
}
