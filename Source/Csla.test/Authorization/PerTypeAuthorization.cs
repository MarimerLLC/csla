//-----------------------------------------------------------------------
// <copyright file="PerTypeAuthorization.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using UnitDriven;
using System.Diagnostics;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


namespace Csla.Test.Authorization
{
#if TESTING
  [DebuggerNonUserCode]
  [DebuggerStepThrough]
#endif
  [TestClass()]
  public class PerTypeAuthorizationTests
  {
    [TestMethod()]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void DenyWritePerType()
    {
      Csla.ApplicationContext.User = new System.Security.Claims.ClaimsPrincipal();
      PerTypeAuthorization root = new PerTypeAuthorization();
      root.Test = "test";
    }
  }

}