//-----------------------------------------------------------------------
// <copyright file="ClientContextTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Configuration;
using System.Security.Principal;
using UnitDriven;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.Silverlight.ApplicationContext
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [TestClass]
  public partial class ClientContextTests : TestBase
  {
    #region Setup/Teardown

    public static IPrincipal _currentPrincipal;
    [TestInitialize]
    public void Setup()
    {
      _currentPrincipal = Csla.ApplicationContext.User;
    }

    [TestCleanup]
    public void Teardown()
    {
      Csla.ApplicationContext.User = _currentPrincipal;
      Csla.ApplicationContext.ClientContext.Clear();
    }

    #endregion

    [TestMethod]
    public void ClientContextShouldBeInitialized()
    {
      var context = GetContext();
      context.Assert.IsNotNull(Csla.ApplicationContext.ClientContext);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void ClientContextShouldBeEmptyInitialy()
    {
      Csla.ApplicationContext.ClientContext.Clear();
      var context = GetContext();
      context.Assert.IsTrue(Csla.ApplicationContext.ClientContext.Count == 0);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void ChangeInGlobalContextDoesNotAffectClientContext()
    {
      var context = GetContext();
      Csla.ApplicationContext.ClientContext.Clear();
      Csla.ApplicationContext.GlobalContext.Clear();

      Csla.ApplicationContext.GlobalContext["TEST"] = "Test";

      context.Assert.IsTrue(Csla.ApplicationContext.ClientContext.Count == 0);
      context.Assert.IsTrue(Csla.ApplicationContext.GlobalContext.Count == 1);

      context.Assert.Success();
      context.Complete();
    }
  }

}