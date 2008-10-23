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
  public partial class MembershipIdentity : ReadOnlyBase<MembershipIdentity>, IIdentity
  {

    public static T GetMembershipIdentity<T>(string userName, string password, bool isRunOnWebServer)
      where T : MembershipIdentity
    {
      IdentityFactory factory = new IdentityFactory();
      return (T)factory.FetchMembershipIdentity(new Criteria(userName, password, typeof(T), isRunOnWebServer));
    }

    protected override void OnDeserialized(StreamingContext context)
    {
      _forceInit = 0;
      base.OnDeserialized(context);
    }
  }
}
#endif