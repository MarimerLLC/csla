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

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    [Obsolete("For use by MobileFormatter")]
    public CslaIdentity()
    {
    }

    #endregion

    /// <summary>
    /// Retrieves an instance of the identity
    /// object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of object.
    /// </typeparam>
    /// <param name="completed">
    /// Method called when the operation is
    /// complete.
    /// </param>
    /// <param name="criteria">
    /// Criteria object for the query.
    /// </param>
    public static void GetCslaIdentity<T>(EventHandler<DataPortalResult<T>> completed, object criteria) where T : CslaIdentity
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.FetchCompleted += completed;
      dp.BeginFetch(criteria);
    }
  }
}
