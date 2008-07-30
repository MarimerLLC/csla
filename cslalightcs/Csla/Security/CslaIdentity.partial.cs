using System;
using System.Security.Principal;
using Csla.Serialization;
using System.Collections.Generic;
using Csla.Core.FieldManager;
using System.Runtime.Serialization;
using Csla.Core;

namespace Csla.Security
{
  public abstract partial class CslaIdentity : ReadOnlyBase<CslaIdentity>, IIdentity
  {
    #region Constructor
    public CslaIdentity()
    {
    }

    #endregion

    public static void GetCslaIdentity<T>(EventHandler<DataPortalResult<T>> completed, object criteria) where T : CslaIdentity
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.FetchCompleted += completed;
      dp.BeginFetch(criteria);
    }
  }
}
