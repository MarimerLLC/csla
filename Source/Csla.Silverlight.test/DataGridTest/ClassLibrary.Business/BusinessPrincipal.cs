//-----------------------------------------------------------------------
// <copyright file="BusinessPrincipal.cs" company="Marimer LLC">
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
using System.Security.Principal;
using Csla.Serialization;

namespace ClassLibrary.Business
{
  [Serializable]
  public class BusinessPrincipal : BusinessPrincipalBase
  {
    private BusinessPrincipal(IIdentity identity)
      : base(identity)
    { }

    public BusinessPrincipal() : base() { }

#if SILVERLIGHT

    public static void Login(string username, string password, string roles, EventHandler<EventArgs> completed)
    {
      BusinessIdentity.GetIdentity(username, password, roles, (o, e) =>
      {
        if (e.Object == null)
        {
          SetPrincipal(BusinessIdentity.UnauthenticatedIdentity());
        }
        else
        {
          SetPrincipal(e.Object);
        }
        completed(null, EventArgs.Empty);
      });
    }
#else
    public static void Login(string username, string password, string roles)
    {
      BusinessIdentity.GetIdentity(username, password, roles);
    }
#endif

    private static void SetPrincipal(Csla.Security.CslaIdentity identity)
    {
      BusinessPrincipal principal = new BusinessPrincipal(identity);
      Csla.ApplicationContext.User = principal;
    }

    public static void Logout()
    {
      Csla.Security.CslaIdentity identity = BusinessIdentity.UnauthenticatedIdentity();
      BusinessPrincipal principal = new BusinessPrincipal(identity);
      Csla.ApplicationContext.User = principal;
    }

    public override bool IsInRole(string role)
    {
      return ((ICheckRoles)base.Identity).IsInRole(role);
    }

  }
}