//-----------------------------------------------------------------------
// <copyright file="SplitTest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Csla.Test.DataPortalTest
{
  [TestClass]
  public class SplitTest
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public void TestDpCreate()
    {
      IDataPortal<Split> dataPortal = _testDIContext.CreateDataPortal<Split>();

      Split.NewObject(dataPortal);
      Assert.AreEqual("Created", TestResults.GetResult("Split"));
    }
    [TestMethod]
    public void TestDpFetch()
    {
      IDataPortal<Split> dataPortal = _testDIContext.CreateDataPortal<Split>();

      Split.GetObject(5, dataPortal);
      Assert.AreEqual("Fetched", TestResults.GetResult("Split"));
    }
    [TestMethod]
    public void TestDpInsert()
    {
      IDataPortal<Split> dataPortal = _testDIContext.CreateDataPortal<Split>();

      Split test = null;
      try
      {
        test = Split.NewObject(dataPortal);
      }
      catch { Assert.Inconclusive(); }
      test.Save();
      Assert.AreEqual("Inserted", TestResults.GetResult("Split"));
    }
    [TestMethod]
    public void TestDpUpdate()
    {
      IDataPortal<Split> dataPortal = _testDIContext.CreateDataPortal<Split>();

      Split test = null;
      try
      {
        test = Split.NewObject(dataPortal);
        test = test.Save();
        test.Id = 5;
      }
      catch { Assert.Inconclusive(); }
      test.Save();
      Assert.AreEqual("Updated", TestResults.GetResult("Split"));
    }
    [TestMethod]
    public void TestDpDelete()
    {
      IDataPortal<Split> dataPortal = _testDIContext.CreateDataPortal<Split>();

      Split.DeleteObject(5, dataPortal);
      Assert.AreEqual("Deleted", TestResults.GetResult("Split"));
    }
    [TestMethod]
    public void TestDpDeleteSelf()
    {
      IDataPortal<Split> dataPortal = _testDIContext.CreateDataPortal<Split>();

      Split test = null;
      try
      {
        test = Split.NewObject(dataPortal);
        test = test.Save();
        test.Delete();
      }
      catch { Assert.Inconclusive(); }
      test.Save();
      Assert.AreEqual("SelfDeleted", TestResults.GetResult("Split"));
    }

  }
}