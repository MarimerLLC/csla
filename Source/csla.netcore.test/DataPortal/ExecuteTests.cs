//-----------------------------------------------------------------------
// <copyright file="ExecuteTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla;
using Csla.Configuration;
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

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class ExecuteTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public async Task ExecuteCommand()
    {
      var dp = _testDIContext.CreateDataPortal<ExecuteCommand>();
      var cmd = await dp.CreateAsync();
      cmd.Value = "abc";
      cmd = await dp.ExecuteAsync(cmd);
      Assert.AreEqual("abc.", cmd.Value);
    }

    [TestMethod]
    public async Task ExecuteWithParamters()
    {
      var dp = _testDIContext.CreateDataPortal<ExecuteCommand>();
      var cmd = await dp.ExecuteAsync("xyz");
      Assert.AreEqual("xyz", cmd.Value);
    }

    [TestMethod]
    public async Task ExecuteCommandViaFactory()
    {
      var dp = _testDIContext.CreateDataPortal<ExecuteCommandViaFactory>();
      var cmd = await dp.CreateAsync();
      cmd.Value = "abc";
      cmd = await dp.ExecuteAsync(cmd);
      Assert.AreEqual("abc.", cmd.Value);
    }

    //[TestMethod]
    //public async Task ExecuteCommandViaFactoryWithParameters()
    //{
    //  var dp = _testDIContext.CreateDataPortal<ExecuteCommandViaFactory>();
    //  var cmd = await dp.ExecuteAsync("xyz");
    //  Assert.AreEqual("xyz", cmd.Value);
    //}
  }
}
