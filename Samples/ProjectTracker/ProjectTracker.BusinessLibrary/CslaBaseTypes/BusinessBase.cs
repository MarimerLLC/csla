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
#pragma warning disable CSLA0007 // null-forgiving on GetProperty matches framework convention for non-nullable managed properties
    public byte[] TimeStamp
    {
      get => GetProperty(TimeStampProperty)!;
      set => SetProperty(TimeStampProperty, value);
    }
#pragma warning restore CSLA0007
  }
}
