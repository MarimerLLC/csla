using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Csla.Server;
using Csla.Server.Dashboard;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class OpenTelemetryDashboardTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }
    [TestMethod]
    public void OpenTelemetryDashboard_CanInstantiate()
    {
      // Act
      using var dashboard = new OpenTelemetryDashboard();

      // Assert
      Assert.IsNotNull(dashboard);
      Assert.IsTrue(dashboard.FirstCall <= DateTimeOffset.Now);
    }

    [TestMethod]
    public void OpenTelemetryDashboard_TracksInitializeCall()
    {
      // Arrange
      using var dashboard = new OpenTelemetryDashboard();
      var args = new InterceptArgs(
        typeof(string), 
        null, 
        DataPortalOperations.Create, 
        isSync: false);

      // Act
      ((IDashboard)dashboard).InitializeCall(args);

      // Assert
      Assert.AreEqual(1, dashboard.TotalCalls);
      Assert.IsTrue(dashboard.LastCall <= DateTimeOffset.Now);
    }

    [TestMethod]
    public void OpenTelemetryDashboard_TracksCompleteCall_Success()
    {
      // Arrange
      using var dashboard = new OpenTelemetryDashboard();
      var applicationContext = _testDIContext.CreateTestApplicationContext();
      var result = new Server.DataPortalResult(applicationContext);
      var args = new InterceptArgs(
        typeof(string), 
        null, 
        result,
        DataPortalOperations.Create, 
        isSync: false)
      {
        Runtime = TimeSpan.FromMilliseconds(100)
      };

      // Act
      ((IDashboard)dashboard).CompleteCall(args);

      // Assert
      Assert.AreEqual(1, dashboard.CompletedCalls);
      Assert.AreEqual(0, dashboard.FailedCalls);
    }

    [TestMethod]
    public void OpenTelemetryDashboard_TracksCompleteCall_Failed()
    {
      // Arrange
      using var dashboard = new OpenTelemetryDashboard();
      var exception = new Exception("Test exception");
      var args = new InterceptArgs(
        typeof(string), 
        null, 
        exception,
        DataPortalOperations.Create, 
        isSync: false)
      {
        Runtime = TimeSpan.FromMilliseconds(100)
      };

      // Act
      ((IDashboard)dashboard).CompleteCall(args);

      // Assert
      Assert.AreEqual(0, dashboard.CompletedCalls);
      Assert.AreEqual(1, dashboard.FailedCalls);
    }

    [TestMethod]
    public void OpenTelemetryDashboard_GetRecentActivity_ReturnsEmptyList()
    {
      // Arrange
      using var dashboard = new OpenTelemetryDashboard();

      // Act
      var activities = dashboard.GetRecentActivity();

      // Assert
      Assert.IsNotNull(activities);
      Assert.AreEqual(0, activities.Count);
    }

    [TestMethod]
    public void OpenTelemetryDashboard_MultipleCallsTracked()
    {
      // Arrange
      using var dashboard = new OpenTelemetryDashboard();
      var applicationContext = _testDIContext.CreateTestApplicationContext();
      
      // Act - Initialize multiple calls
      for (int i = 0; i < 5; i++)
      {
        var initArgs = new InterceptArgs(
          typeof(string), 
          null, 
          DataPortalOperations.Create, 
          isSync: false);
        ((IDashboard)dashboard).InitializeCall(initArgs);
      }

      // Complete 3 successfully
      for (int i = 0; i < 3; i++)
      {
        var result = new Server.DataPortalResult(applicationContext);
        var completeArgs = new InterceptArgs(
          typeof(string), 
          null, 
          result,
          DataPortalOperations.Create, 
          isSync: false)
        {
          Runtime = TimeSpan.FromMilliseconds(50)
        };
        ((IDashboard)dashboard).CompleteCall(completeArgs);
      }

      // Complete 2 with failures
      for (int i = 0; i < 2; i++)
      {
        var exception = new Exception("Test exception");
        var completeArgs = new InterceptArgs(
          typeof(string), 
          null, 
          exception,
          DataPortalOperations.Create, 
          isSync: false)
        {
          Runtime = TimeSpan.FromMilliseconds(75)
        };
        ((IDashboard)dashboard).CompleteCall(completeArgs);
      }

      // Assert
      Assert.AreEqual(5, dashboard.TotalCalls);
      Assert.AreEqual(3, dashboard.CompletedCalls);
      Assert.AreEqual(2, dashboard.FailedCalls);
    }

    [TestMethod]
    public void OpenTelemetryDashboard_MeterNameIsCorrect()
    {
      // Assert
      Assert.AreEqual("Csla.DataPortal", OpenTelemetryDashboard.MeterName);
      Assert.AreEqual("1.0.0", OpenTelemetryDashboard.MeterVersion);
    }
  }
}
