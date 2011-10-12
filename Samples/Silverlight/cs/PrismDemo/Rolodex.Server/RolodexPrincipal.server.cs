using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rolodex
{
    public partial class RolodexPrincipal
    {
        private RolodexPrincipal() { }
        public static void Login(string username, string password)
        {
            RolodexIdentity identity = RolodexIdentity.GetIdentity(username, password);
            SetPrincipal(identity);
        }
    }
}
