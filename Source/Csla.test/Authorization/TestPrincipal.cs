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
#pragma warning disable CS0436 // Type conflicts with imported type
            return ((TestIdentity)Identity).IsInRole(role);
#pragma warning restore CS0436 // Type conflicts with imported type
        }

        private TestPrincipal(System.Security.Principal.IIdentity identity) : base(identity)
        {}

        public static void SimulateLogin()
        {
#pragma warning disable CS0436 // Type conflicts with imported type
#pragma warning disable CS0436 // Type conflicts with imported type
            Csla.ApplicationContext.User = new TestPrincipal(new TestIdentity("User", "password"));
#pragma warning restore CS0436 // Type conflicts with imported type
#pragma warning restore CS0436 // Type conflicts with imported type
        }

        public static void SimulateLogout()
        {
            Csla.ApplicationContext.User = null;
        }
    }
}