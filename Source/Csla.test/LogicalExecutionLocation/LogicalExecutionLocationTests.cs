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
    [TestMethod]
    public void TestLogicalExecutionLocation()
    {
      Assert.AreEqual(Csla.ApplicationContext.LogicalExecutionLocations.Client, Csla.ApplicationContext.LogicalExecutionLocation, "Should be client");

#pragma warning disable CS0436 // Type conflicts with imported type
#pragma warning disable CS0436 // Type conflicts with imported type
      LocationBusinessBase item = LocationBusinessBase.GetLocationBusinessBase();
#pragma warning restore CS0436 // Type conflicts with imported type
#pragma warning restore CS0436 // Type conflicts with imported type
      Assert.AreEqual(item.Data,Csla.ApplicationContext.LogicalExecutionLocations.Server.ToString(), "Should be server");
      Assert.AreEqual(item.NestedData, Csla.ApplicationContext.LogicalExecutionLocations.Server.ToString(), "Nested should be server");

      Assert.AreEqual(Csla.ApplicationContext.LogicalExecutionLocations.Client, Csla.ApplicationContext.LogicalExecutionLocation, "Should be client");

    }

    [TestMethod]
    public void TestRulesLogicalExecutionLocation()
    {
 
#pragma warning disable CS0436 // Type conflicts with imported type
#pragma warning disable CS0436 // Type conflicts with imported type
      LocationBusinessBase item = LocationBusinessBase.GetLocationBusinessBase();
#pragma warning restore CS0436 // Type conflicts with imported type
#pragma warning restore CS0436 // Type conflicts with imported type

      Assert.AreEqual(item.Rule, Csla.ApplicationContext.LogicalExecutionLocations.Server.ToString(), "Should be server");

      item.Data = "random";
      Assert.AreEqual(item.Rule, Csla.ApplicationContext.LogicalExecutionLocations.Client.ToString(), "Should be client");


    }
  }
}