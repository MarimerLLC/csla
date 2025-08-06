//-----------------------------------------------------------------------
// <copyright file="GlobalContextTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Configuration;
using System.Security.Principal;
using Csla.Testing.Business.ApplicationContext;
using UnitDriven;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Csla.Test.Silverlight.ApplicationContext
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [TestClass]
  public partial class GlobalContextTests : TestBase
  {
    #region Setup/Teardown

    public static IPrincipal _currentPrincipal;
    [TestInitialize]
    public void Setup()
    {
      
    }

    [TestCleanup]
    public void Teardown()
    {
      Csla.ApplicationContext.User = _currentPrincipal;
      Csla.ApplicationContext.GlobalContext.Clear();
      ConfigurationManager.AppSettings["CslaDataPortalProxy"] = null;
    }

    #endregion

    [TestMethod]
    public void GlobalContextShouldBeInitialized()
    {
      var context = GetContext();
      context.Assert.IsNotNull(Csla.ApplicationContext.GlobalContext);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void GlobalContextShouldBeEmptyInitialy()
    {
      var context = GetContext();
      Csla.ApplicationContext.GlobalContext.Clear();
      Assert.IsTrue(Csla.ApplicationContext.GlobalContext.Count == 0);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void ChangeInClientContextDoesNotAffectGlobalContext()
    {
      var context = GetContext();

      Csla.ApplicationContext.ClientContext.Clear();
      Csla.ApplicationContext.GlobalContext.Clear();

      Csla.ApplicationContext.ClientContext["TEST"] = "Test";

      Assert.IsTrue(Csla.ApplicationContext.ClientContext.Count == 1);
      Assert.IsTrue(Csla.ApplicationContext.GlobalContext.Count == 0);

      context.Assert.Success();
      context.Complete();
    }




  }
}