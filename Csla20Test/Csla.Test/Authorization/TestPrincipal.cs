using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Security
{
    [Serializable()]
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

            Csla.ApplicationContext.User = p;
            //System.Threading.Thread.CurrentPrincipal = p;
        }

        public static void SimulateLogout()
        {
            Csla.ApplicationContext.User = null;
            //System.Threading.Thread.CurrentPrincipal = null;
        }

        //public static void SetUnAuthenticatedPrincipal()
        //{
        //    TestIdentity identity = new TestIdentity();
        //    TestPrincipal principal = new TestPrincipal(identity);

        //    Csla.ApplicationContext.User = principal;
        //    //System.Threading.Thread.CurrentPrincipal = principal;
        //}
    }
}
