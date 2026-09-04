//-----------------------------------------------------------------------
// <copyright file="ExecuteTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class ExecuteTests
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

    [TestMethod]
    public async Task ExecuteCommand()
    {
      var dp = _testHost.GetDataPortal<ExecuteCommand>();
      var cmd = await dp.CreateAsync();
      cmd.Value = "abc";
      cmd = await dp.ExecuteAsync(cmd);
      Assert.AreEqual("abc.", cmd.Value);
    }

    [TestMethod]
    public async Task ExecuteWithParameters()
    {
      var dp = _testHost.GetDataPortal<ExecuteCommand>();
      var cmd = await dp.ExecuteAsync("xyz");
      Assert.AreEqual("xyz", cmd.Value);
    }

    [TestMethod]
    public async Task ExecuteCommandViaFactory()
    {
      var dp = _testHost.GetDataPortal<ExecuteCommandViaFactory>();
      var cmd = await dp.CreateAsync();
      cmd.Value = "abc";
      cmd = await dp.ExecuteAsync(cmd);
      Assert.AreEqual("abc.", cmd.Value);
    }

    //[TestMethod]
    //public async Task ExecuteCommandViaFactoryWithParameters()
    //{
    //  var dp = _testHost.GetDataPortal<ExecuteCommandViaFactory>();
    //  var cmd = await dp.ExecuteAsync("xyz");
    //  Assert.AreEqual("xyz", cmd.Value);
    //}
  }
}
