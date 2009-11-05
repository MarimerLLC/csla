using System;
using System.Security.Principal;
using Csla.Serialization;
using System.Collections.Generic;
using Csla.Core.FieldManager;
using System.Runtime.Serialization;
using Csla.DataPortalClient;
using Csla.Xaml;
using Csla.Core;

namespace Csla.Security
{
  public partial class MembershipIdentity : ReadOnlyBase<MembershipIdentity>, IIdentity
  {
    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    public MembershipIdentity()
    {
      _forceInit = _forceInit + 0;
    }

    /// <summary>
    /// Gets a MembershipIdentity object by loading the
    /// object with membership data from the server.
    /// </summary>
    /// <param name="completed">Callback handler for async operation</param>
    /// <param name="userName">Username to validate on server.</param>
    /// <param name="password">Password to validate on server.</param>
    /// <param name="isRunOnWebServer">True if membership database is on web server (2- or 3-tier), false to use app server (4-tier)</param>
    public static void GetMembershipIdentity(EventHandler<DataPortalResult<MembershipIdentity>> completed, string userName, string password, bool isRunOnWebServer)
    {
      GetMembershipIdentity<MembershipIdentity>(completed, userName, password, isRunOnWebServer);
    }

    /// <summary>
    /// Gets a MembershipIdentity object by loading the
    /// object with membership data from the server.
    /// </summary>
    /// <typeparam name="T">Type of object (subclass) to retrieve</typeparam>
    /// <param name="completed">Callback handler for async operation</param>
    /// <param name="userName">Username to validate on server.</param>
    /// <param name="password">Password to validate on server.</param>
    /// <param name="isRunOnWebServer">True if membership database is on web server (2- or 3-tier), false to use app server (4-tier)</param>
    public static void GetMembershipIdentity<T>(EventHandler<DataPortalResult<T>> completed, string userName, string password, bool isRunOnWebServer) where T : MembershipIdentity
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.FetchCompleted += completed;
      dp.BeginFetch(new Criteria(userName, password, typeof(T), isRunOnWebServer));
    }
  }
}
