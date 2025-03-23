//-----------------------------------------------------------------------
// <copyright file="BusynessTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.TestHelpers;
using Csla.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cslalighttest.BusyStatus
{
  [TestClass]
  public class BusynessTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void IsBusyWhenHavingTheSameInvocationsOfMarkBusyAndMarkIdleItShouldBeInAnIdleStateAgain()
    {
      var root = CreateWithoutCriteria<ObjectBusy>();
      root.IdealStateSubMethod();
      Assert.IsFalse(root.IsBusy);
    }

    [TestMethod]
    public void IsBusyWhenHavingTheInvocationsOfMarkBusyAndMarkIdleItShouldNotBeInAnIdleStateAgain()
    {
      var root = CreateWithoutCriteria<ObjectBusy>();

      root.NonIdealStateMethod();
      Assert.IsTrue(root.IsBusy);
    }

    [TestMethod]
    public void IsBusyWhenHavingTheInvocationsOFMarkIdleItShoulBeInIdleState()
    {
      var root = CreateWithoutCriteria<ObjectBusy>();

      root.IsBusyShouldNotGoInMinus();
      Assert.IsFalse(root.IsBusy);
    }

    [Serializable]
    public class ObjectBusy : BusinessBase<ObjectBusy>
    {
      [Create]
      private void Create()
      {

      }

      public void IdealStateMethod()
      {
        try
        {
          MarkBusy();
          IdealStateSubMethod();
        }
        finally
        {
          MarkIdle();
        }
      }

      public void IdealStateSubMethod()
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

      public void NonIdealStateMethod()
      {
        MarkBusy();
        MarkBusy();
        MarkIdle();
      }
      public void IsBusyShouldNotGoInMinus()
      {
        MarkBusy();
        MarkIdle();
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