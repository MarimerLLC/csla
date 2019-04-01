//-----------------------------------------------------------------------
// <copyright file="TestPrincipal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Test.Security
{
    [Serializable()]
    public class TestPrincipal : Csla.Security.CslaPrincipal
    {
      public TestPrincipal() { }
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
        }
    }
}