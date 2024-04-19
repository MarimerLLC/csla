//-----------------------------------------------------------------------
// <copyright file="PerTypeAuthorization.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(new System.Security.Claims.ClaimsPrincipal());
      IDataPortal<PerTypeAuthorization> dataPortal = testDIContext.CreateDataPortal<PerTypeAuthorization>();

      PerTypeAuthorization root = dataPortal.Create();
      root.Test = "test";
    }
  }

}