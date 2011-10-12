using System;
using Csla;

namespace Rolodex
{
    public partial class RolodexPrincipal
    {
        [Obsolete("Internal use only")]
        public RolodexPrincipal() { }

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
    }
}
