//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.DataPortalClient;
using System.ComponentModel;

#if !SILVERLIGHT
using System.Data.SqlClient;
#endif


namespace Sample.Business
{
  [Serializable(), Csla.Server.ObjectFactory("Sample.Business.SampleIdentityFactory, Sample.Business", "", "GetSampleIdentity", "", "")]
  public class SampleIdentity : CslaIdentity
  {

#if SILVERLIGHT

    public static void GetIdentity(string username, string password, EventHandler<DataPortalResult<SampleIdentity>> completed)
    {
      GetCslaIdentity<SampleIdentity>(completed, new CredentialsCriteria(username, password));
    }

#else

	  internal void LoadData(string userName, MobileList<string> rolesList)
	  {
		AuthenticationType = "Csla";
		Roles = rolesList;
		Name = userName;
		IsAuthenticated = true;
	  }

#endif

  }

} //end of root namespace