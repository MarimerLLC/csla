using System;

namespace ProjectTracker.Library.CslaBaseTypes
{
  [Serializable]
  public abstract class BusinessDocumentBase<T, C> : Csla.BusinessDocumentBase<T, C>
    where T : Csla.BusinessDocumentBase<T, C>
    where C : notnull, Csla.Core.IEditableBusinessObject
  {

  }
}
