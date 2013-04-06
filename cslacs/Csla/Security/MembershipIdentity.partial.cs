#if !CLIENTONLY
using System;
using System.Security.Principal;
using Csla.Serialization;
using System.Collections.Generic;
using Csla.Core.FieldManager;
using System.Runtime.Serialization;
using Csla.DataPortalClient;
using Csla.Silverlight;
using Csla.Core;

namespace Csla.Security
{
  /// <summary>
  /// Implements a .NET identity object that automatically
  /// authenticates against the ASP.NET membership provider.
  /// </summary>
  public partial class MembershipIdentity : ReadOnlyBase<MembershipIdentity>, IIdentity
  {
    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    protected MembershipIdentity()
    {
      _forceInit = _forceInit + 0;
    }

    /// <summary>
    /// Authenticates the user's credentials against the ASP.NET
    /// membership provider.
    /// </summary>
    /// <typeparam name="T">
    /// Type of object (subclass of MembershipIdentity) to retrieve.
    /// </typeparam>
    /// <param name="userName">Username to authenticate.</param>
    /// <param name="password">Password to authenticate.</param>
    /// <param name="isRunOnWebServer">
    /// Specifies whether to access the membership provider locally (true),
    /// or through the data portal (false) presumably to reach an application
    /// server.
    /// </param>
    /// <returns></returns>
    public static T GetMembershipIdentity<T>(string userName, string password, bool isRunOnWebServer)
      where T : MembershipIdentity
    {
      IdentityFactory factory = new IdentityFactory();
      return (T)factory.FetchMembershipIdentity(new Criteria(userName, password, typeof(T), isRunOnWebServer));
    }

    /// <summary>
    /// Method invoked when the object is deserialized.
    /// </summary>
    /// <param name="context">Serialization context.</param>
    protected override void OnDeserialized(StreamingContext context)
    {
      _forceInit = 0;
      base.OnDeserialized(context);
    }
  }
}
#endif