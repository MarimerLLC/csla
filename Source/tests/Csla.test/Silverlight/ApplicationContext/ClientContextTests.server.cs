﻿//-----------------------------------------------------------------------
// <copyright file="ClientContextTests.server.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Configuration;
using System.Security.Principal;
using Csla.Security;
using Csla.Testing.Business.ApplicationContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Silverlight.ApplicationContext
{
  //[TestClass]
  public partial class ClientContextTests
  {

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void ServerShouldReceiveClientContextValue()
    {
      var context = GetContext();

      Csla.ApplicationContext.User = new System.Security.Claims.ClaimsPrincipal();

      Csla.ApplicationContext.DataPortalProxy = "Csla.Testing.Business.TestProxies.AppDomainProxy, Csla.Testing.Business";

      var verifier = new ClientContextBOVerifier(true);

      //This is what we are transferring
      Csla.ApplicationContext.ClientContext["MSG"] = ContextMessageValues.INITIAL_VALUE;

      verifier.Name = "justin";
      var result = verifier.Save();

      context.Assert.AreEqual(ContextMessageValues.INITIAL_VALUE, result.ReceivedContextValue);
      context.Assert.Success();

      context.Complete();

    }
  }


}