using Csla;
using Csla.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Csla.Security;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  namespace Security
  {
    [Serializable()]
    public class PTIdentity : CslaIdentity
    {
      internal static void GetPTIdentity(string username, string password, EventHandler<DataPortalResult<PTIdentity>> callback)
      {
        DataPortal.BeginFetch<PTIdentity>(new UsernameCriteria(username, password), callback);
      }

#if !SILVERLIGHT
      internal static PTIdentity GetIdentity(string username, string password)
      {
        return DataPortal.Fetch<PTIdentity>(new UsernameCriteria(username, password));
      }

      internal static PTIdentity GetIdentity(string username)
      {
        return DataPortal.Fetch<PTIdentity>(username);
      }

      private void DataPortal_Fetch(UsernameCriteria criteria)
      {

      }
#endif
    }
  }
}
