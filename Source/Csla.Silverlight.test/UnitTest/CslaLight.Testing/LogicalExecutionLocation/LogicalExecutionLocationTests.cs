//-----------------------------------------------------------------------
// <copyright file="LogicalExecutionLocationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.DataPortalClient;
using UnitDriven;
using Csla.Testing.Business.CommandBase;

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


namespace Csla.Test.LogicalExecutionLocation
{
  [TestClass]
  public class LogicalExecutionLocationTests : TestBase
  {

    [TestInitialize]
    public void Setup()
    {
      Csla.DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
#if WINDOWS_PHONE
      WcfProxy.DefaultUrl = "http://localhost:4832/WcfPortal.svc";
#else
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
#endif
    }

    [TestMethod]
    public void TestLogicalExecutionLocation()
    {
      var context = GetContext();

      Assert.AreEqual(Csla.ApplicationContext.LogicalExecutionLocations.Client, Csla.ApplicationContext.LogicalExecutionLocation, "Should be client");

      LocationBusinessBase item;
      LocationBusinessBase.GetLocationBusinessBase((o, e) =>
        {
          item = e.Object;
          context.Assert.AreEqual(item.Data, Csla.ApplicationContext.LogicalExecutionLocations.Server.ToString(), "Should be server");

          context.Assert.AreEqual(Csla.ApplicationContext.LogicalExecutionLocations.Client, Csla.ApplicationContext.LogicalExecutionLocation, "Should be client");
          context.Assert.Success();
        });

      context.Complete();
    }

    [TestMethod]
    public void TestRulesLogicalExecutionLocation()
    {
      var context = GetContext();

      LocationBusinessBase item;
      LocationBusinessBase.GetLocationBusinessBase((o, e) =>
      {
        item = e.Object;
        context.Assert.AreEqual(item.Data, Csla.ApplicationContext.LogicalExecutionLocations.Server.ToString(), "Should be server");

        item.Data = "random";
        context.Assert.AreEqual(item.Rule, Csla.ApplicationContext.LogicalExecutionLocations.Client.ToString(), "Should be client");

        context.Assert.Success();
      });

      context.Complete();

    }
  }
}