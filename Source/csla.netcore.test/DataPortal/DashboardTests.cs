//-----------------------------------------------------------------------
// <copyright file="DashboardTests.cs" company="Marimer LLC">
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
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace csla.netcore.test.DataPortal
{
  [TestClass]
  public class DashboardTests
  {
    [TestCleanup]
    public void TestCleanup()
    {
      new CslaConfiguration().DataPortal().DashboardType("");
    }

    [TestMethod]
    public void DashboardDefaultIsNullDashboard()
    {
      new CslaConfiguration().DataPortal().DashboardType("");
      var dashboard = Csla.Server.Dashboard.DashboardFactory.GetDashboard();
      Assert.IsInstanceOfType(dashboard, typeof(Csla.Server.Dashboard.NullDashboard));
    }

    [TestMethod]
    public void DashboardUseRealDashboard()
    {
      new CslaConfiguration().DataPortal().DashboardType("Dashboard");
      var dashboard = Csla.Server.Dashboard.DashboardFactory.GetDashboard();
      Assert.IsInstanceOfType(dashboard, typeof(Csla.Server.Dashboard.Dashboard));
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public async Task DashboardSuccessCounter()
    {
      new CslaConfiguration().DataPortal().DashboardType("Dashboard");
      var dashboard = Csla.Server.Dashboard.DashboardFactory.GetDashboard();

      var obj = Csla.DataPortal.Create<SimpleType>();

      await Task.Delay(500);

      Assert.IsTrue(dashboard.FirstCall.Ticks > 0);
      Assert.AreEqual(1, dashboard.TotalCalls, "total");
      Assert.AreEqual(0, dashboard.FailedCalls, "failed");
      Assert.AreEqual(1, dashboard.CompletedCalls, "completed");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    [Ignore]
    public async Task DashboardFailureCounter()
    {
      new CslaConfiguration().DataPortal().DashboardType("Dashboard");
      var dashboard = (Csla.Server.Dashboard.Dashboard)Csla.Server.Dashboard.DashboardFactory.GetDashboard();
      
      try
      {
        var obj = Csla.DataPortal.Fetch<SimpleType>("123");
      }
      catch { /*expected failure*/ }

      await Task.Delay(500);

      Assert.IsTrue(dashboard.FirstCall.Ticks > 0);
      Assert.AreEqual(1, dashboard.TotalCalls, "total");
      Assert.AreEqual(1, dashboard.FailedCalls, "failed");
      Assert.AreEqual(0, dashboard.CompletedCalls, "completed");
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    [Ignore]
    public async Task DashboardRecentActivity()
    {
      new CslaConfiguration().DataPortal().DashboardType("Dashboard");
      var dashboard = (Csla.Server.Dashboard.Dashboard)Csla.Server.Dashboard.DashboardFactory.GetDashboard();

      var obj = Csla.DataPortal.Fetch<SimpleType>(123);
      try
      {
        obj = Csla.DataPortal.Fetch<SimpleType>("123");
      }
      catch { /*expected failure*/ }

      await Task.Delay(500);

      var activity = dashboard.GetRecentActivity();
      Assert.AreEqual(2, activity.Count, "count");
      Assert.IsTrue(activity.Average(r => r.Runtime.TotalMilliseconds) > 0, "runtime");
      Assert.AreEqual(typeof(SimpleType).AssemblyQualifiedName, activity.Select(r => r.ObjectType).First().AssemblyQualifiedName);
      Assert.AreEqual(DataPortalOperations.Fetch, activity.Select(r => r.Operation).First());
    }
  }

  [Serializable]
  public class SimpleType : BusinessBase<SimpleType>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    [Fetch]
    private void DataPortal_Fetch(int id)
    {
      // TODO: load values into object
      System.Threading.Thread.Sleep(10);
    }

    [Insert]
    protected override void DataPortal_Insert()
    {
      // TODO: insert object's data
    }

    [Update]
    protected override void DataPortal_Update()
    {
      // TODO: update object's data
    }

    [DeleteSelf]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(ReadProperty(IdProperty));
    }

    [Delete]
    private void DataPortal_Delete(int id)
    {
      // TODO: delete object's data
    }

  }
}