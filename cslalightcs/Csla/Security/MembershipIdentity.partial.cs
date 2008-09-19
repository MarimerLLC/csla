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
    public static void GetMembershipIdentity<T>(EventHandler<DataPortalResult<T>> completed, string userName, string password, bool isRunOnWebServer) where T : MembershipIdentity
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.FetchCompleted += completed;
      dp.BeginFetch(new Criteria(userName, password, typeof(T), isRunOnWebServer));
    }
  }
}
