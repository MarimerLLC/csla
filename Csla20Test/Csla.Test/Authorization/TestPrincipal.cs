using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Security
{
    public class TestPrincipal : Csla.Security.BusinessPrincipalBase
    {
        public override bool IsInRole(string role)
        {
            TestIdentity identity = (TestIdentity)(this.Identity);
            return identity.IsInRole(role);
        }

        private TestPrincipal(System.Security.Principal.IIdentity identity) : base(identity)
        {}

        public static void SimulateLogin()
        {
            TestIdentity identity = new TestIdentity("User", "password");
            TestPrincipal p = new TestPrincipal(identity);

            System.Threading.Thread.CurrentPrincipal = p;
        }

        public static void SimulateLogout()
        {
            System.Threading.Thread.CurrentPrincipal = null;
        }
    }
}
