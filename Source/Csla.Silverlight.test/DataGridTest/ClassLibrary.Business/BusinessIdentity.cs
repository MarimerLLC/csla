//-----------------------------------------------------------------------
// <copyright file="BusinessIdentity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;

namespace ClassLibrary.Business
{
  [Serializable()]
  public class BusinessIdentity : CslaIdentity
  {
#if SILVERLIGHT

    public BusinessIdentity() {}

    public static void GetIdentity(string username, string password, string roles, EventHandler<DataPortalResult<BusinessIdentity>> completed)
    {
      GetCslaIdentity<BusinessIdentity>(completed, new CredentialsCriteria(username, password, roles));
    }
#else
    public static void GetIdentity(string username, string password, string roles)
    {
      GetCslaIdentity<BusinessIdentity>(new CredentialsCriteria(username, password, roles));
    }

    private void DataPortal_Fetch(CredentialsCriteria criteria)
    {
      if (criteria.Username == "SergeyB" && criteria.Password == "1234")
      {
        this.Roles = new MobileList<string>(criteria.Roles.Split(';'));
        this.IsAuthenticated = true;
        this.Name = criteria.Username;
        this.AuthenticationType = "Custom";
      }
      else
      {
        this.Roles = new MobileList<string>();
        this.IsAuthenticated = false;
        this.Name = string.Empty;
        this.AuthenticationType = "";
      }
    }
#endif
  }
}