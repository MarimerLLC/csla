//-----------------------------------------------------------------------
// <copyright file="GlobalCOntextTests.server.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Configuration;
using Csla.Security;
using Csla.Testing.Business.ApplicationContext;
using UnitDriven;

#if NUNIT
using TestMethod = NUnit.Framework.TestAttribute;

#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.Silverlight.ApplicationContext
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  //[TestClass]
  public partial class GlobalContextTests : TestBase
  {
    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void ServerShouldReceiveGlobalContextValue()
    {
      var context = GetContext();

      Csla.ApplicationContext.User = new System.Security.Claims.ClaimsPrincipal();

      Csla.ApplicationContext.DataPortalProxy = "Csla.Testing.Business.TestProxies.AppDomainProxy, Csla.Testing.Business";

      var verifier = new GlobalContextBOVerifier(true);

      //This is what we are transferring
      Csla.ApplicationContext.GlobalContext["MSG"] = ContextMessageValues.INITIAL_VALUE;

      verifier.Name = "justin";
      var result = Csla.DataPortal.Update<GlobalContextBOVerifier>(verifier);


      context.Assert.IsNotNull(result);
      context.Assert.AreEqual(ContextMessageValues.INITIAL_VALUE, result.ReceivedContextValue);
      context.Assert.Success();

      context.Complete();
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void GlobalContextOnClientShouldBeAffectedByChangeOnServer()
    {
      var context = GetContext();
      Csla.ApplicationContext.User = new System.Security.Claims.ClaimsPrincipal();

      Csla.ApplicationContext.DataPortalProxy = "Csla.Testing.Business.TestProxies.AppDomainProxy, Csla.Testing.Business";

      var verifier = new GlobalContextBOVerifier(false);

      //This is what we are transferring
      Csla.ApplicationContext.GlobalContext["MSG"] = ContextMessageValues.INITIAL_VALUE;

      var result = Csla.DataPortal.Update<GlobalContextBOVerifier>(verifier);

      context.Assert.AreEqual(ContextMessageValues.MODIFIED_VALUE, Csla.ApplicationContext.GlobalContext["MSG"]);

      context.Assert.Success();
      context.Complete();

    }

  }
}