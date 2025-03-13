using Csla.DataPortalClient;
using Csla.Server;
using Csla.Server.Interceptors.ServerSide;
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
    public async Task Initialize_PrimitiveCriteria_NoExceptionRaised()
    {
      // Arrange
      var criteria = new PrimitiveCriteria(1);
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      var sut = new RevalidatingInterceptor(applicationContext);
      var args = CreateUpdateArgsOfRoot(criteria);
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act
      await sut.InitializeAsync(args);
    }

    [TestMethod]
    public async Task Initialize_ValidRootObjectNoChildren_NoExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      Root rootObject = dataPortal.Fetch(new Root.Criteria("Test Data"));
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      RevalidatingInterceptor sut = new RevalidatingInterceptor(applicationContext);
      var args = CreateUpdateArgsOfRoot(rootObject);
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act
      await sut.InitializeAsync(args);
    }

    [TestMethod]
    public async Task Initialize_ValidRootObjectWithChild_NoExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      Root rootObject = dataPortal.Fetch(new Root.Criteria("Test Data"));
      Child childObject = rootObject.Children.AddNew();
      childObject.Data = "Test child data";
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      var sut = new RevalidatingInterceptor(applicationContext);
      var args = CreateUpdateArgsOfRoot(rootObject);
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act
      await sut.InitializeAsync(args);
    }

    [TestMethod]
    public async Task Initialize_ValidRootObjectWithChildAndGrandChild_NoExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      Root rootObject = dataPortal.Fetch(new Root.Criteria("Test Data"));
      Child childObject = rootObject.Children.AddNew();
      childObject.Data = "Test child data";
      GrandChild grandChildObject = childObject.GrandChildren.AddNew();
      grandChildObject.Data = "Test grandchild data";
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      var sut = new RevalidatingInterceptor(applicationContext);
      var args = CreateUpdateArgsOfRoot(rootObject);
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act
      await sut.InitializeAsync(args);
    }

    [TestMethod]
    public async Task Initialize_InvalidRootObject_ExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      Root rootObject = dataPortal.Create(new Root.Criteria(""));
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      var sut = new RevalidatingInterceptor(applicationContext);
      var args = CreateUpdateArgsOfRoot(rootObject);
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act and Assert
      await Assert.ThrowsExceptionAsync<Rules.ValidationException>(async () => await sut.InitializeAsync(args));
    }

    [TestMethod]
    public async Task Initialize_InvalidChildObject_ExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      Root rootObject = dataPortal.Create(new Root.Criteria("Test Data"));
      rootObject.Children.AddNew();
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      var sut = new RevalidatingInterceptor(applicationContext);
      var args = CreateUpdateArgsOfRoot(rootObject);
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act and Assert
      await Assert.ThrowsExceptionAsync<Rules.ValidationException>(async () => await sut.InitializeAsync(args));
    }

    [TestMethod]
    public async Task Initialize_InvalidGrandChildObject_ExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      Root rootObject = dataPortal.Create(new Root.Criteria("Test Data"));
      Child childObject = rootObject.Children.AddNew();
      childObject.Data = "Test child data";
      childObject.GrandChildren.AddNew();
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      var sut = new RevalidatingInterceptor(applicationContext);
      var args = CreateUpdateArgsOfRoot(rootObject);
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;

      // Act and Assert
      await Assert.ThrowsExceptionAsync<Rules.ValidationException>(async () => await sut.InitializeAsync(args));
    }

    private static InterceptArgs CreateUpdateArgsOfRoot(object parameter) => new InterceptArgs(typeof(Root), parameter, DataPortalOperations.Update, true);
  }
}
