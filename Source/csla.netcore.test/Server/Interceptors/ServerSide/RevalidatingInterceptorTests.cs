using System;
using System.Collections.Generic;
using System.Text;
using Csla.DataPortalClient;
using Csla.Server;
using Csla.Server.Interceptors.ServerSide;
using Csla.Test.Basic;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Server.Interceptors.ServerSide
{

  [TestClass]
  public class RevalidatingInterceptorTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void Initialize_PrimitiveCriteria_NoExceptionRaised()
    {
      // Arrange
      bool executed = false;
      PrimitiveCriteria criteria = new PrimitiveCriteria(1);
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      RevalidatingInterceptor sut = new RevalidatingInterceptor(applicationContext);
      InterceptArgs args = new InterceptArgs()
      {
        ObjectType = typeof(Root),
        Operation = DataPortalOperations.Update,
        Parameter = criteria,
        IsSync = true
      };
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act
      sut.Initialize(args);
      executed = true;

      // Assert
      Assert.IsTrue(executed);

    }

    [TestMethod]
    public void Initialize_ValidRootObjectNoChildren_NoExceptionRaised()
    {
      // Arrange
      bool executed = false;
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      Root rootObject = dataPortal.Fetch(new Root.Criteria("Test Data"));
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      RevalidatingInterceptor sut = new RevalidatingInterceptor(applicationContext);
      InterceptArgs args = new InterceptArgs() {
        ObjectType = typeof(Root),
        Operation = DataPortalOperations.Update,
        Parameter = rootObject,
        IsSync=true 
      };
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act
      sut.Initialize(args);
      executed = true;

      // Assert
      Assert.IsTrue(executed);
      
    }

    [TestMethod]
    public void Initialize_ValidRootObjectWithChild_NoExceptionRaised()
    {
      // Arrange
      bool executed = false;
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      Root rootObject = dataPortal.Fetch(new Root.Criteria("Test Data"));
      Child childObject = rootObject.Children.AddNew();
      childObject.Data = "Test child data";
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      RevalidatingInterceptor sut = new RevalidatingInterceptor(applicationContext);
      InterceptArgs args = new InterceptArgs()
      {
        ObjectType = typeof(Root),
        Operation = DataPortalOperations.Update,
        Parameter = rootObject,
        IsSync = true
      };
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act
      sut.Initialize(args);
      executed = true;

      // Assert
      Assert.IsTrue(executed);

    }

    [TestMethod]
    public void Initialize_ValidRootObjectWithChildAndGrandChild_NoExceptionRaised()
    {
      // Arrange
      bool executed = false;
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      Root rootObject = dataPortal.Fetch(new Root.Criteria("Test Data"));
      Child childObject = rootObject.Children.AddNew();
      childObject.Data = "Test child data";
      GrandChild grandChildObject = childObject.GrandChildren.AddNew();
      grandChildObject.Data = "Test grandchild data";
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      RevalidatingInterceptor sut = new RevalidatingInterceptor(applicationContext);
      InterceptArgs args = new InterceptArgs()
      {
        ObjectType = typeof(Root),
        Operation = DataPortalOperations.Update,
        Parameter = rootObject,
        IsSync = true
      };
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act
      sut.Initialize(args);
      executed = true;

      // Assert
      Assert.IsTrue(executed);

    }

    [TestMethod]
    public void Initialize_InvalidRootObject_ExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      Root rootObject = dataPortal.Create(new Root.Criteria(""));
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      RevalidatingInterceptor sut = new RevalidatingInterceptor(applicationContext);
      InterceptArgs args = new InterceptArgs()
      {
        ObjectType = typeof(Root),
        Operation = DataPortalOperations.Update,
        Parameter = rootObject,
        IsSync = true
      };
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act and Assert
      Assert.ThrowsException<Rules.ValidationException>(() => sut.Initialize(args));

    }

    [TestMethod]
    public void Initialize_InvalidChildObject_ExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      Root rootObject = dataPortal.Create(new Root.Criteria("Test Data"));
      Child childObject = rootObject.Children.AddNew();
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      RevalidatingInterceptor sut = new RevalidatingInterceptor(applicationContext);
      InterceptArgs args = new InterceptArgs()
      {
        ObjectType = typeof(Root),
        Operation = DataPortalOperations.Update,
        Parameter = rootObject,
        IsSync = true
      };
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act and Assert
      Assert.ThrowsException<Rules.ValidationException>(() => sut.Initialize(args));

    }

    [TestMethod]
    public void Initialize_InvalidGrandChildObject_ExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      Root rootObject = dataPortal.Create(new Root.Criteria("Test Data"));
      Child childObject = rootObject.Children.AddNew();
      childObject.Data = "Test child data";
      GrandChild grandChildObject = childObject.GrandChildren.AddNew();
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      RevalidatingInterceptor sut = new RevalidatingInterceptor(applicationContext);
      InterceptArgs args = new InterceptArgs()
      {
        ObjectType = typeof(Root),
        Operation = DataPortalOperations.Update,
        Parameter = rootObject,
        IsSync = true
      };
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act and Assert
      Assert.ThrowsException<Rules.ValidationException>(() => sut.Initialize(args));

    }
  }
}
