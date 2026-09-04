//-----------------------------------------------------------------------
// <copyright file="LogicalExecutionLocationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Csla.Test.LogicalExecutionLocation
{
  [TestClass]
  public class LogicalExecutionLocationTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void TestLogicalExecutionLocation()
    {
      IDataPortal<LocationBusinessBase> dataPortal = _testHost.GetDataPortal<LocationBusinessBase>();
      var applicationContext = _testHost.ApplicationContext;

      Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Client, applicationContext.LogicalExecutionLocation, "Should be client");

#pragma warning disable CS0436 // Type conflicts with imported type
      LocationBusinessBase item = LocationBusinessBase.GetLocationBusinessBase(dataPortal);
#pragma warning restore CS0436 // Type conflicts with imported type

      Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Server.ToString(), item.Data, "Should be server");
      Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Server.ToString(), item.NestedData, "Nested should be server");

      Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Client, applicationContext.LogicalExecutionLocation, "Should be client");

    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void TestRulesLogicalExecutionLocation()
    {
      IDataPortal<LocationBusinessBase> dataPortal = _testHost.GetDataPortal<LocationBusinessBase>();

#pragma warning disable CS0436 // Type conflicts with imported type
      LocationBusinessBase item = LocationBusinessBase.GetLocationBusinessBase(dataPortal);
#pragma warning restore CS0436 // Type conflicts with imported type

      Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Server.ToString(), item.Rule, "Should be server");

      item.Data = "random";
      Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Client.ToString(), item.Rule, "Should be client");

    }
  }
}