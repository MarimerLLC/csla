using System;
using System.ComponentModel;
using Csla;

namespace ProjectTracker.Library.CslaBaseTypes
{
  [Serializable]
  public abstract class BusinessDocumentBase<T, C> : Csla.BusinessDocumentBase<T, C>
    where T : Csla.BusinessDocumentBase<T, C>
    where C : notnull, Csla.Core.IEditableBusinessObject
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
