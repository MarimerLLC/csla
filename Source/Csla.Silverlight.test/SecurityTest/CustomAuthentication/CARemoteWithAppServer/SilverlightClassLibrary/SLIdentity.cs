using System;
using System.Net;
using System.Collections.Generic;
using Csla;
using Csla.Serialization;
using Csla.DataPortalClient;
using Csla.Core;

namespace ClassLibrary
{
  [Serializable()]
  public partial class SLIdentity : Csla.Security.CslaIdentity
  {
    public static PropertyInfo<string> ExtraProperty = RegisterProperty(typeof(SLIdentity), new PropertyInfo<string>("Extra", "Extra"));
    public string Extra
    {
      get { return GetProperty(ExtraProperty); }
      private set { LoadProperty(ExtraProperty, value); }
    }

    public static PropertyInfo<string> MoreDataProperty = RegisterProperty(typeof(SLIdentity), new PropertyInfo<string>("MoreData", "More data"));
    public string MoreData
    {
      get { return GetProperty(MoreDataProperty); }
      private set { LoadProperty(MoreDataProperty, value); }
    }

#if SILVERLIGHT

    internal static void GetIdentity(string username, string password, EventHandler<DataPortalResult<SLIdentity>> completed)
    {
      GetCslaIdentity<SLIdentity>(completed, new UsernameCriteria(username, password));
    }
#endif
  }
}
