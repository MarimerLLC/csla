//-----------------------------------------------------------------------
// <copyright file="SingleOverloadTest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Csla.Test.DataPortalTest
{
  [TestClass]
  public class SingleOverloadTest
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

    [TestMethod]
    public void TestDpCreate()
    {
      IDataPortal<SingleOverload> dataPortal = _testHost.GetDataPortal<SingleOverload>();

      SingleOverload test = SingleOverload.NewObject(dataPortal);
      Assert.AreEqual("Created0", TestResults.GetResult("SingleOverload"));
    }
    [TestMethod]
    public void TestDpCreateWithCriteria()
    {
      IDataPortal<SingleOverload> dataPortal = _testHost.GetDataPortal<SingleOverload>();

      SingleOverload test = SingleOverload.NewObjectWithCriteria(dataPortal);
      Assert.AreEqual("Created1", TestResults.GetResult("SingleOverload"));
    }
    [TestMethod]
    public void TestDpFetch()
    {
      IDataPortal<SingleOverload> dataPortal = _testHost.GetDataPortal<SingleOverload>();

      SingleOverload test = SingleOverload.GetObject(5, dataPortal);
      Assert.AreEqual("Fetched", TestResults.GetResult("SingleOverload"));
    }
    [TestMethod]
    public void TestDpDelete()
    {
      IDataPortal<SingleOverload> dataPortal = _testHost.GetDataPortal<SingleOverload>();

      SingleOverload.DeleteObject(5, dataPortal);
      Assert.AreEqual("Deleted", TestResults.GetResult("SingleOverload"));
    }

  }
}