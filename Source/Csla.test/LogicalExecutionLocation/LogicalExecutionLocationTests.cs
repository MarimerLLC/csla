//-----------------------------------------------------------------------
// <copyright file="LogicalExecutionLocationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.TestHelpers;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.LogicalExecutionLocation
{
  [TestClass]
  public class LogicalExecutionLocationTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void TestLogicalExecutionLocation()
    {
      IDataPortal<LocationBusinessBase> dataPortal = _testDIContext.CreateDataPortal<LocationBusinessBase>();
      var applicationContext = _testDIContext.CreateTestApplicationContext();

      Assert.AreEqual(ApplicationContext.LogicalExecutionLocations.Client, applicationContext.LogicalExecutionLocation, "Should be client");

#pragma warning disable CS0436 // Type conflicts with imported type
      LocationBusinessBase item = LocationBusinessBase.GetLocationBusinessBase(dataPortal);
#pragma warning restore CS0436 // Type conflicts with imported type

      Assert.AreEqual(item.Data,Csla.ApplicationContext.LogicalExecutionLocations.Server.ToString(), "Should be server");
      Assert.AreEqual(item.NestedData, Csla.ApplicationContext.LogicalExecutionLocations.Server.ToString(), "Nested should be server");

      Assert.AreEqual(Csla.ApplicationContext.LogicalExecutionLocations.Client, applicationContext.LogicalExecutionLocation, "Should be client");

    }

    [TestMethod]
    public void TestRulesLogicalExecutionLocation()
    {
      IDataPortal<LocationBusinessBase> dataPortal = _testDIContext.CreateDataPortal<LocationBusinessBase>();

#pragma warning disable CS0436 // Type conflicts with imported type
      LocationBusinessBase item = LocationBusinessBase.GetLocationBusinessBase(dataPortal);
#pragma warning restore CS0436 // Type conflicts with imported type

      Assert.AreEqual(item.Rule, Csla.ApplicationContext.LogicalExecutionLocations.Server.ToString(), "Should be server");

      item.Data = "random";
      Assert.AreEqual(item.Rule, Csla.ApplicationContext.LogicalExecutionLocations.Client.ToString(), "Should be client");

    }
  }
}