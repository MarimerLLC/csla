//-----------------------------------------------------------------------
// <copyright file="BusynessTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.Core;
using Csla.Testing;
using Csla.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cslalighttest.BusyStatus
{
  [TestClass]
  public class BusynessTests
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

    private T CreateWithoutCriteria<T>() where T : ICslaObject
    {
      IDataPortal<T> dataPortal = _testHost.GetDataPortal<T>();

      return dataPortal.Create();
    }
  }
}