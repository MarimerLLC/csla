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
    [field: NonSerialized]
    public event EventHandler IsDirtyChanged;

    virtual protected void OnIsDirtyChanged()
    {
      if (IsDirtyChanged != null)
        IsDirtyChanged(this, EventArgs.Empty);
    }
  }
}
