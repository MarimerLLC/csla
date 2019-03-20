//-----------------------------------------------------------------------
// <copyright file="SilverlightIdentity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;
using System.Runtime.Serialization;
using Csla.DataPortalClient;
using Csla.Core;

namespace Csla.Testing.Business.Security
{
  [Serializable()]
  public class SilverlightIdentity : Csla.Security.CslaIdentity
  {

    public SilverlightIdentity() {}

    protected void DataPortal_Fetch(object criteria)
    {
      if (((SilverlightPrincipal.Criteria)criteria).Name == "invalidusername")
      {
        Roles = new MobileList<string>();
        IsAuthenticated = false;
        Name = string.Empty;
        AuthenticationType = "Csla";
      }
      else
      {
        Roles = new MobileList<string>(new string[] { "Admin Role", "User Role" });
        IsAuthenticated = true;
        Name = "SilverlightIdentity";
        AuthenticationType = "SilverLight";
      }
    }
  }
}