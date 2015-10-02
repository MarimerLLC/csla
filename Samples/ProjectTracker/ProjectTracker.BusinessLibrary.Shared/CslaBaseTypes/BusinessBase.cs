using System;
using System.Threading.Tasks;
using Csla;
using System.Collections.Generic;

namespace ProjectTracker.Library.CslaBaseTypes
{
  [Serializable]
  public abstract class BusinessBase<T> : Csla.BusinessBase<T> where T : Csla.BusinessBase<T>
  {

  }
}
