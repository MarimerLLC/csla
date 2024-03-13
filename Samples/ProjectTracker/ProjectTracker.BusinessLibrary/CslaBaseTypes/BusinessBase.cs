using System;

namespace ProjectTracker.Library.CslaBaseTypes
{
  [Serializable]
  public abstract class BusinessBase<T> : Csla.BusinessBase<T> where T : Csla.BusinessBase<T>
  {

  }
}
