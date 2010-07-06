using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using System.Security.Principal;

#if!SILVERLIGHT
using System.Data.SqlClient;
using Rolodex.Business.Data;
#endif


namespace Rolodex.Business.Security
{
    [Serializable]
    public class RolodexPrincipal : CslaPrincipal
    {
        private RolodexPrincipal(IIdentity identity)
            : base(identity)
        { }

        public RolodexPrincipal() : base() { }

#if SILVERLIGHT

    public static void Login(string username, string password, EventHandler<DataPortalResult<RolodexIdentity>> completed)
    {
      RolodexIdentity.GetIdentity(username, password, (o, e) =>
      {
        if (e.Object == null)
        {
          SetPrincipal(RolodexIdentity.UnauthenticatedIdentity());
        }
        else
        {
          SetPrincipal(e.Object);
        }
        completed(e.Object, e);
      });
    }
#else
        public static void Login(string username, string password)
        {
            RolodexIdentity identity = RolodexIdentity.GetIdentity(username, password);
            SetPrincipal(identity);
        }
#endif

        private static void SetPrincipal(Csla.Security.CslaIdentity identity)
        {
            RolodexPrincipal principal = new RolodexPrincipal(identity);
            Csla.ApplicationContext.User = principal;
        }

        public static void Logout()
        {
            Csla.Security.CslaIdentity identity = RolodexIdentity.UnauthenticatedIdentity();
            RolodexPrincipal principal = new RolodexPrincipal(identity);
            Csla.ApplicationContext.User = principal;
        }

        public override bool IsInRole(string role)
        {
            return ((ICheckRoles)base.Identity).IsInRole(role);
        }
    }
}
