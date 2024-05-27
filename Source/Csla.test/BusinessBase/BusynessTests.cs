//-----------------------------------------------------------------------
// <copyright file="BusynessTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using UnitDriven;
using Csla.TestHelpers;
using Csla.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cslalighttest.BusyStatus
{
  [TestClass]
  public class BusynessTests : TestBase
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
    public void ObjectIsNotBusy()
    {
      UnitTestContext context = GetContext();
      var root = CreateWithoutCriteria<ObjectBusy>();
      root.One();
      Assert.IsFalse(root.IsBusy);
    }

    [TestMethod]
    public void ObjectIsBusy()
    {
      UnitTestContext context = GetContext();
      var root = CreateWithoutCriteria<ObjectBusy>();

      root.Three();
      Assert.IsTrue(root.IsBusy);
    }

    [Serializable]
    public class ObjectBusy : BusinessBase<ObjectBusy>
    {
      [Create]
      private void Create()
      {

      }

      public void One()
      {
        try
        {
          MarkBusy();
          Two();
        }
        finally
        {
          MarkIdle();
        }
      }

      public void Two()
      {
        try
        {
          MarkBusy();
        }
        finally
        {
          MarkIdle();
        }
      }

      public void Three()
      {
        MarkBusy();
        MarkBusy();
        MarkIdle();
      }
    }

    private T CreateWithoutCriteria<T>()
    {
      IDataPortal<T> dataPortal = _testDIContext.CreateDataPortal<T>();

      return dataPortal.Create();
    }
  }
}