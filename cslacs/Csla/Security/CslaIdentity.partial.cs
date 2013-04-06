using System;
using System.Security.Principal;
using Csla.Serialization;
using System.Collections.Generic;
using Csla.Core.FieldManager;
using System.Runtime.Serialization;
using Csla.Core;

namespace Csla.Security
{
  /// <summary>
  /// Provides a base class to simplify creation of
  /// a .NET identity object for use with BusinessPrincipalBase.
  /// </summary>
  public abstract partial class CslaIdentity : ReadOnlyBase<CslaIdentity>, IIdentity
  {
    #region Constructor

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected CslaIdentity()
    {
      _forceInit = _forceInit && false;
    }

    #endregion

    /// <summary>
    /// Invokes the data portal to get an instance of
    /// the identity object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the CslaIdentity subclass to retrieve.
    /// </typeparam>
    /// <param name="criteria">
    /// Object containing the user's credentials.
    /// </param>
    /// <returns></returns>
    public static T GetCslaIdentity<T>(object criteria) where T : CslaIdentity
    {
      return DataPortal.Fetch<T>(criteria);
    }

    /// <summary>
    /// Method called when the object is deserialized.
    /// </summary>
    /// <param name="context">Serialization context.</param>
    protected override void OnDeserialized(StreamingContext context)
    {
      _forceInit = _forceInit && false;
      base.OnDeserialized(context);
    }
  }
}
