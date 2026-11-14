using System;
using System.ComponentModel;
using Csla;

namespace ProjectTracker.Library.CslaBaseTypes
{
  [Serializable]
  public abstract class BusinessBase<T> : Csla.BusinessBase<T> where T : Csla.BusinessBase<T>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty =
      RegisterProperty<byte[]>(nameof(TimeStamp));

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public byte[] TimeStamp
    {
      get => GetProperty(TimeStampProperty)!;
      set => SetProperty(TimeStampProperty, value);
    }
  }
}
