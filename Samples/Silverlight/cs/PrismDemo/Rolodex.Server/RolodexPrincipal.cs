using System;
using Csla.Security;
using System.Security.Principal;
using Csla.Serialization;

namespace Rolodex
{
    [Serializable]
    public partial class RolodexPrincipal : CslaPrincipal
    {
        private RolodexPrincipal(IIdentity identity)
            : base(identity)
        { }

        private static void SetPrincipal(IIdentity identity)
        {
            RolodexPrincipal principal = new RolodexPrincipal(identity);
            Csla.ApplicationContext.User = principal;
        }

        public void Logout()
        {
            RolodexIdentity identity = RolodexIdentity.UnauthenticatedIdentity();
            RolodexPrincipal principal = new RolodexPrincipal(identity);
            Csla.ApplicationContext.User = principal;
        }

    }
}
