//-----------------------------------------------------------------------
// <copyright file="DashboardTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla;
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
    [TestMethod]
    public void DashboardDefaultIsNullDashboard()
    {
      new Csla.Configuration.CslaConfiguration().DataPortal.DashboardType(null);
      var dashboard = Csla.Server.Dashboard.DashboardFactory.GetDashboard();
      Assert.IsInstanceOfType(dashboard, typeof(Csla.Server.Dashboard.NullDashboard));
    }

    [TestMethod]
    public void DashboardUseRealDashboard()
    {
      new Csla.Configuration.CslaConfiguration().DataPortal.DashboardType("Dashboard");
      var dashboard = Csla.Server.Dashboard.DashboardFactory.GetDashboard();
      Assert.IsInstanceOfType(dashboard, typeof(Csla.Server.Dashboard.Dashboard));
    }

    [TestMethod]
    public void DashboardSuccessCounter()
    {
      new Csla.Configuration.CslaConfiguration().DataPortal.DashboardType("Dashboard");
      var dashboard = Csla.Server.Dashboard.DashboardFactory.GetDashboard();

      var obj = Csla.DataPortal.Create<SimpleType>();

      var wait = new System.Threading.SpinWait();
      for (int i = 0; i < 15; i++)
      {
        System.Threading.Thread.Sleep(100);
        wait.SpinOnce();
      }

      Assert.IsTrue(dashboard.FirstCall.Ticks > 0);
      Assert.AreEqual(1, dashboard.TotalCalls);
      Assert.AreEqual(0, dashboard.FailedCalls);
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

    private void DataPortal_Fetch(int id)
    {
      // TODO: load values into object

    }

    protected override void DataPortal_Insert()
    {
      // TODO: insert object's data
    }

    protected override void DataPortal_Update()
    {
      // TODO: update object's data
    }

    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(ReadProperty(IdProperty));
    }

    private void DataPortal_Delete(int id)
    {
      // TODO: delete object's data
    }

  }
}
