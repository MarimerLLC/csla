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
using Csla.Server.Dashboard;
using Csla.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
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
    //    [TestCleanup]
    //    public void TestCleanup()
    //    {
    //      new CslaConfiguration().DataPortal().DashboardType("");
    //    }

    [TestMethod]
    public void DashboardDefaultIsNullDashboard()
    {
      ServiceProvider serviceProvider;

      // Initialise DI, and add Csla using default settings
      var services = new ServiceCollection();
      services.AddCsla();
      serviceProvider = services.BuildServiceProvider();

      IDashboard dashboard = serviceProvider.GetRequiredService<IDashboard>();
      Assert.IsInstanceOfType(dashboard, typeof(Csla.Server.Dashboard.NullDashboard));
    }

    // This would really be testing the behaviour of the service provider not the dashboard
    // That doesn't really feel all that appropriate as a test
    // [TestMethod]
    // public void DashboardUseRealDashboard()
    // {
    //   new CslaConfiguration().DataPortal().DashboardType("Dashboard");
    //   var dashboard = Csla.Server.Dashboard.DashboardFactory.GetDashboard();
    //   Assert.IsInstanceOfType(dashboard, typeof(Csla.Server.Dashboard.Dashboard));
    // }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public async Task DashboardSuccessCounter()
    {
      IServiceProvider serviceProvider = InitialiseServiceProviderUsingRealDashboard();
      var dashboard = serviceProvider.GetRequiredService<IDashboard>();

      var obj = CreateSimpleType(serviceProvider);

      await Task.Delay(500);

      Assert.IsTrue(dashboard.FirstCall.Ticks > 0);
      Assert.AreEqual(1, dashboard.TotalCalls, "total");
      Assert.AreEqual(0, dashboard.FailedCalls, "failed");
      Assert.AreEqual(1, dashboard.CompletedCalls, "completed");
    }

    // This test fails when included. Are failures not being recorded correctly?
    // N.B. This test was already ignored before CSLA 6, so it appears it used to fail before too!
    [Ignore]
    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public async Task DashboardFailureCounter()
    {
      IServiceProvider serviceProvider = InitialiseServiceProviderUsingRealDashboard();
      var dashboard = serviceProvider.GetRequiredService<IDashboard>();

      try
      {
        var obj = FetchSimpleType(serviceProvider, "123");
      }
      catch { /*expected failure*/ }

      await Task.Delay(500);

      Assert.IsTrue(dashboard.FirstCall.Ticks > 0);
      Assert.AreEqual(1, dashboard.TotalCalls, "total");
      Assert.AreEqual(1, dashboard.FailedCalls, "failed");
      Assert.AreEqual(0, dashboard.CompletedCalls, "completed");
    }

    // This test fails when included. Are failures not being recorded correctly?
    // N.B. This test was already ignored before CSLA 6, so it appears it used to fail before too!
    [Ignore]
    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public async Task DashboardRecentActivity()
    {
      IServiceProvider serviceProvider = InitialiseServiceProviderUsingRealDashboard();
      var dashboard = serviceProvider.GetRequiredService<IDashboard>();

      var obj = FetchSimpleType(serviceProvider, 123);
      try
      {
        obj = FetchSimpleType(serviceProvider, "123");
      }
      catch { /*expected failure*/ }

      await Task.Delay(500);

      var activity = dashboard.GetRecentActivity();
      Assert.AreEqual(2, activity.Count, "count");
      Assert.IsTrue(activity.Average(r => r.Runtime.TotalMilliseconds) > 0, "runtime");
      Assert.AreEqual(typeof(SimpleType).AssemblyQualifiedName, activity.Select(r => r.ObjectType).First().AssemblyQualifiedName);
      Assert.AreEqual(DataPortalOperations.Fetch, activity.Select(r => r.Operation).First());
    }

    private SimpleType CreateSimpleType(IServiceProvider serviceProvider)
    {
      IDataPortal<SimpleType> dataPortal = serviceProvider.GetRequiredService<IDataPortal<SimpleType>>();
      return dataPortal.Create();
    }

    private SimpleType FetchSimpleType(IServiceProvider serviceProvider, int id)
    {
      IDataPortal<SimpleType> dataPortal = serviceProvider.GetRequiredService<IDataPortal<SimpleType>>();
      return dataPortal.Fetch(id);
    }

    private SimpleType FetchSimpleType(IServiceProvider serviceProvider, string idString)
    {
      IDataPortal<SimpleType> dataPortal = serviceProvider.GetRequiredService<IDataPortal<SimpleType>>();
      return dataPortal.Fetch(idString);
    }

    private IServiceProvider InitialiseServiceProviderUsingRealDashboard()
    {
      ServiceProvider serviceProvider;

      // Initialise DI
      var services = new ServiceCollection();
      services.AddCslaTesting();

      // Add Csla, using the real dashboard
      services.AddSingleton<IDashboard, Dashboard>();
      services.AddCsla();
      serviceProvider = services.BuildServiceProvider();

      return serviceProvider;

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

    [Create]
    private void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void DataPortal_Fetch(int id)
    {
      // load values into object
      System.Threading.Thread.Sleep(10);
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      // insert object's data
    }

    [Update]
    protected void DataPortal_Update()
    {
      // update object's data
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(ReadProperty(IdProperty));
    }

    [Delete]
    private void DataPortal_Delete(int id)
    {
      // delete object's data
    }

  }
}