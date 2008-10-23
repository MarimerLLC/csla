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
    private static int _forceInit;

    #region Constructor
    protected CslaIdentity()
    {
      _forceInit = 0;
    }

    #endregion

    public static T GetCslaIdentity<T>(object criteria) where T : CslaIdentity
    {
      return DataPortal.Fetch<T>(criteria);
    }

    protected override void OnDeserialized(StreamingContext context)
    {
      _forceInit = 0;
      base.OnDeserialized(context);
    }
  }
}
