//-----------------------------------------------------------------------
// <copyright file="PerTypeAuthorization.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Authorization
{
#if TESTING
  [DebuggerNonUserCode]
  [DebuggerStepThrough]
#endif
  [TestClass]
  public class PerTypeAuthorizationTests
  {
    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void DenyWritePerType()
    {
      using var testHost = CslaTestHost.Create(t => t.AsPrincipal(new System.Security.Claims.ClaimsPrincipal()));
      IDataPortal<PerTypeAuthorization> dataPortal = testHost.GetDataPortal<PerTypeAuthorization>();

      PerTypeAuthorization root = dataPortal.Create();
      root.Test = "test";
    }
  }

}