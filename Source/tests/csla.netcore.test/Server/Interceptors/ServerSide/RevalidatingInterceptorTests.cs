using Csla.DataPortalClient;
using Csla.Server;
using Csla.Server.Interceptors.ServerSide;
using Csla.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Server.Interceptors.ServerSide
{
  [TestClass]
  public class RevalidatingInterceptorTests
  {
    private static CslaTestHost _testHost;
    private ApplicationContext _applicationContext;
    private RevalidatingInterceptor _systemUnderTest;

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

    [TestInitialize]
    public void TestSetup()
    {
      _applicationContext = _testHost.ApplicationContext;
      _systemUnderTest = new RevalidatingInterceptor(_applicationContext, _testHost.Services.GetRequiredService<IOptions<RevalidatingInterceptorOptions>>());

      PrepareApplicationContext(_applicationContext);
    }

    private static void PrepareApplicationContext(ApplicationContext applicationContext)
    {
      applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      applicationContext.LocalContext["__logicalExecutionLocation"] = ApplicationContext.LogicalExecutionLocations.Server;
    }


    [TestMethod]
    public async Task Initialize_PrimitiveCriteria_NoExceptionRaised()
    {
      // Arrange
      var criteria = new PrimitiveCriteria(1);
      var args = CreateUpdateArgsOfRoot(criteria);

      // Act
      await _systemUnderTest.InitializeAsync(args);
    }

    [TestMethod]
    public async Task Initialize_ValidRootObjectNoChildren_NoExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();
      Root rootObject = dataPortal.Fetch(new Root.Criteria("Test Data"));
      var args = CreateUpdateArgsOfRoot(rootObject);

      // Act
      await _systemUnderTest.InitializeAsync(args);
    }

    [TestMethod]
    public async Task Initialize_ValidRootObjectWithChild_NoExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();
      Root rootObject = dataPortal.Fetch(new Root.Criteria("Test Data"));
      Child childObject = rootObject.Children.AddNew();
      childObject.Data = "Test child data";
      var args = CreateUpdateArgsOfRoot(rootObject);

      // Act
      await _systemUnderTest.InitializeAsync(args);
    }

    [TestMethod]
    public async Task Initialize_ValidRootObjectWithChildAndGrandChild_NoExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();
      Root rootObject = dataPortal.Fetch(new Root.Criteria("Test Data"));
      Child childObject = rootObject.Children.AddNew();
      childObject.Data = "Test child data";
      GrandChild grandChildObject = childObject.GrandChildren.AddNew();
      grandChildObject.Data = "Test grandchild data";
      var args = CreateUpdateArgsOfRoot(rootObject);

      // Act
      await _systemUnderTest.InitializeAsync(args);
    }

    [TestMethod]
    public async Task Initialize_InvalidRootObject_ExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();
      Root rootObject = dataPortal.Create(new Root.Criteria(""));
      var args = CreateUpdateArgsOfRoot(rootObject);

      // Act and Assert
      await Assert.ThrowsExceptionAsync<Rules.ValidationException>(async () => await _systemUnderTest.InitializeAsync(args));
    }

    [TestMethod]
    public async Task Initialize_InvalidChildObject_ExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();
      Root rootObject = dataPortal.Create(new Root.Criteria("Test Data"));
      rootObject.Children.AddNew();
      var args = CreateUpdateArgsOfRoot(rootObject);

      // Act and Assert
      await Assert.ThrowsExceptionAsync<Rules.ValidationException>(async () => await _systemUnderTest.InitializeAsync(args));
    }

    [TestMethod]
    public async Task Initialize_InvalidGrandChildObject_ExceptionRaised()
    {
      // Arrange
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();
      Root rootObject = dataPortal.Create(new Root.Criteria("Test Data"));
      Child childObject = rootObject.Children.AddNew();
      childObject.Data = "Test child data";
      childObject.GrandChildren.AddNew();
      var args = CreateUpdateArgsOfRoot(rootObject);

      // Act and Assert
      await Assert.ThrowsExceptionAsync<Rules.ValidationException>(async () => await _systemUnderTest.InitializeAsync(args));
    }

    [TestMethod]
    public async Task Initialize_DeletingAnInvalidObjectDoesNotThrowWhenRevalidationForDeleteIsDisabled()
    {
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();
      Root rootObject = dataPortal.Create(new Root.Criteria(""));
      var args = new InterceptArgs(rootObject.GetType(), rootObject, DataPortalOperations.Delete, true);

      using var testHost = CslaTestHost.Create(t => t.ConfigureServices(services =>
      {
        services.Configure<RevalidatingInterceptorOptions>(opts => opts.IgnoreDeleteOperation = true);
      }));
      var appContext = testHost.ApplicationContext;
      PrepareApplicationContext(appContext);

      var sut = new RevalidatingInterceptor(appContext, testHost.Services.GetRequiredService<IOptions<RevalidatingInterceptorOptions>>());

      await sut.InitializeAsync(args);
    }

    private static InterceptArgs CreateUpdateArgsOfRoot(object parameter) => new InterceptArgs(typeof(Root), parameter, DataPortalOperations.Update, true);
  }
}
