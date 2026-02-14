//-----------------------------------------------------------------------
// <copyright file="ServiceProviderMethodCallerTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Reflection;
using Csla.Reflection;
using Csla.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class ServiceProviderMethodCallerTests
  {
    private static TestDIContext _diContext = default!;
    private ServiceProviderMethodCaller _systemUnderTest = default!;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
      _ = context;
      _diContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void TestSetup()
    {
      _systemUnderTest = _diContext.CreateTestApplicationContext().CreateInstanceDI<ServiceProviderMethodCaller>();
    }

    [TestMethod]
    public void NoTarget()
    {
      FluentActions.Invoking(() => _systemUnderTest.FindDataPortalMethod<CreateAttribute>(null, null)).Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void FindMethodInterfaceCriteria()
    {
      var obj = new InterfaceCriteria();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [new MyCriteria()]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    [DataRow(123)]
    [DataRow(null)]
    public void FindMethodNullableIntCriteria(int? data)
    {
      var obj = new NullableCriteria();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [data]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    [DataRow(123)]
    [DataRow(null)]
    public async Task FindMethodNullableCriteriaWithValueViaDataPortal(int? data)
    {
      var obj = await _diContext.CreateDataPortal<NullableCriteria>().CreateAsync(data);

      obj.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodNoCriteriaNoDI()
    {
      var obj = new SimpleNoCriteriaCreate();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [null]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodCriteriaDI()
    {
      var obj = new CriteriaCreateWithDI();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.TargetParameterCountException))]
    public void FindMethodBadCriteriaDI()
    {
      var obj = new CriteriaCreateWithDI();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, ["hi"]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodMultipleCriteriaDI()
    {
      var obj = new MultipleCriteriaCreateWithDI();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123, "hi"]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodMultipleCriteriaDIInterleaved()
    {
      var obj = new MultipleCriteriaCreateWithDIInterleaved();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123, "hi"]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodCriteriaMultipleDI()
    {
      var obj = new CriteriaCreateWithMultipleDI();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123]);
      method.PrepForInvocation();

      method.Parameters.Should().HaveCount(3);
    }

    [TestMethod]
    public void FindMethodCriteriaMultipleAmbiguousDI()
    {
      var obj = new CriteriaCreateWithMultipleAmbiguousDI();
      FluentActions.Invoking(() => _ = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123])).Should().Throw<AmbiguousMatchException>();
    }

    [TestMethod]
    public void FindMethodSingleCriteriaInvalid()
    {
      var obj = new SimpleNoCriteriaCreate();
      FluentActions.Invoking(() => _ = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123])).Should().Throw<TargetParameterCountException>();
    }

    [TestMethod]
    public void FindMethodSingleCriteriaValid()
    {
      var obj = new SimpleCriteriaCreate();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodObjectCriteriaValid()
    {
      var obj = new ObjectCriteriaCreate();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodObjectCriteriaSubtype()
    {
      var obj = new ObjectCriteriaCreateSubtype();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    [ExpectedException(typeof(System.Reflection.TargetParameterCountException))]
    public void FindMethodSingleCriteriaBadType()
    {
      var obj = new SimpleCriteriaCreate();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, ["hi"]);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void FindMethod_PrivateBase()
    {
      var obj = new PrivateMethod();
      var method = _systemUnderTest.FindDataPortalMethod<ExecuteAttribute>(obj, null);
      Assert.IsNotNull(method);
      Assert.AreEqual("Execute", method.MethodInfo.Name, "Method name should match");
    }

    [TestMethod]
    public async Task FindMethod_PrivateBase_Invoke()
    {
      var portal = _diContext.CreateDataPortal<PrivateMethod>();
      var obj = await portal.CreateAsync();
      obj = await portal.ExecuteAsync(obj);

      obj.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodDataPortal_CreateBase()
    {
      var obj = new OldStyleNoOverride();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, null);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodDataPortal_CreateCriteria()
    {
      var obj = new OldStyleCreate();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodDataPortal_CreateCriteriaBase()
    {
      var obj = new OldStyleCriteriaBase();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodDataPortal_Create()
    {
      var obj = new OldStyleCreate();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, null);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodDataPortal_Create_Criteria()
    {
      var obj = new OldStyleCreate();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123]);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public void FindMethodAmbiguousCriteria()
    {
      var obj = new AmbiguousNoCriteriaCreate();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, null);
      method.PrepForInvocation();

      Assert.AreEqual(1, method.Parameters.Count());
    }

    [TestMethod]
    public void FindChildLegacyUpdate()
    {
      var obj = new BasicChild();
      var method = _systemUnderTest.FindDataPortalMethod<UpdateChildAttribute>(obj, null);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public void FindChildParamsUpdate()
    {
      var obj = new ParamsChild();
      var method = _systemUnderTest.FindDataPortalMethod<UpdateChildAttribute>(obj, null);

      method.Should().NotBeNull();
    }

    [TestMethod]
    public async Task FindChildIntUpdate()
    {
      var obj = await _diContext.CreateChildDataPortal<ModernChild>().CreateChildAsync();
      object[] paramsArray = [123];
      var method = _systemUnderTest.FindDataPortalMethod<UpdateChildAttribute>(obj, paramsArray);

      method.Should().NotBeNull();

      var dp = _diContext.CreateChildDataPortal<ModernChild>();
      await dp.UpdateChildAsync(obj, 42);

      obj.Id.Should().Be(42);

      obj = await _diContext.CreateChildDataPortal<ModernChild>().CreateChildAsync();
      await dp.UpdateChildAsync(obj, 123);

      obj.Id.Should().Be(123);
    }

    [TestMethod]
    public void FindOverlappingCriteriaInt()
    {
      var obj = new MultipleOverlappingCriteria();
      var method = _systemUnderTest.FindDataPortalMethod<FetchAttribute>(obj, [1]);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public async Task Issue2109()
    {
      var obj = new Issue2109List();
      var method = _systemUnderTest.FindDataPortalMethod<FetchAttribute>(obj, [new string[] { "a" }]);
      Assert.IsNotNull(method, "string[]");
      var criteria = new My2109Criteria();
      method = _systemUnderTest.FindDataPortalMethod<FetchAttribute>(obj, [criteria]);
      Assert.IsNotNull(method, "ICriteriaBase");

      var portal = _diContext.CreateDataPortal<Issue2109List>();

      obj = await portal.FetchAsync(new My2109Criteria());
      obj.Should().NotBeNull("Fetch with criteria.").And.Subject.First().Name.Should().Be("Csla.Test.DataPortal.My2109Criteria");

      obj = await portal.FetchAsync(new string[] { "a" });
      obj.Should().NotBeNull("Fetch with array.").And.Subject.First().Name.Should().Be("a");
    }

    [TestMethod]
    public void Issue2287BusinessBindingListFetch()
    {
      var obj = new Issue2287List();
      var method = _systemUnderTest.FindDataPortalMethod<FetchAttribute>(obj, [new Issue2287List.Criteria()]);
      Assert.IsNotNull(method);
      Assert.AreEqual(obj.GetType().BaseType, method.MethodInfo.DeclaringType);
    }

    [TestMethod]
    public void Issue2287BusinessBaseFetch()
    {
      var obj = new Issue2287Edit();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, []);
      Assert.IsNotNull(method);
    }

    [TestMethod]
    public void Issue2396DeleteSelfChildNoParams()
    {
      var obj = new Issue2396Edit();
      var method = _systemUnderTest.FindDataPortalMethod<DeleteSelfChildAttribute>(obj, []);
      Assert.IsNotNull(method);
      Assert.AreEqual(0, method.MethodInfo.GetParameters().Count());
    }

    [TestMethod]
    public void Issue2396DeleteSelfChildWithParams()
    {
      var obj = new Issue2396EditParams();
      var method = _systemUnderTest.FindDataPortalMethod<DeleteSelfChildAttribute>(obj, [1, 2]);
      Assert.IsNotNull(method);
      Assert.AreEqual(2, method.MethodInfo.GetParameters().Count());
    }

    [TestMethod]
    public void Issue2396DeleteSelfChildFallbackToNoParams()
    {
      var obj = new Issue2396Edit();
      var method = _systemUnderTest.FindDataPortalMethod<DeleteSelfChildAttribute>(obj, [1, 2]);
      Assert.IsNotNull(method);
      Assert.AreEqual(0, method.MethodInfo.GetParameters().Count());
    }

    [TestMethod($"{nameof(ServiceProviderMethodCaller.TryFindDataPortalMethod)} must not throw when the business object is attributed with an object factory which is not loaded/known in the current environment."), GitHubWorkItem("https://github.com/MarimerLLC/csla/issues/4681")]
    public void TryFindDataPortalMethod_Testcase1()
    {
      FluentActions.Invoking(() => _systemUnderTest.TryFindDataPortalMethod<FetchAttribute>(typeof(NotKnownObjectFactoryInCurrentEnvironment), null, out var _)).Should().NotThrow();
    }

    [TestMethod]
    public void FindMethodWithIDataPortalFactoryInjection()
    {
      var obj = new DataPortalFactoryInjection();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, null);

      method.Should().NotBeNull();
      method.PrepForInvocation();
      method.Parameters.Should().HaveCount(1);
      method.IsInjected.Should().HaveCount(1);
      method.IsInjected![0].Should().BeTrue();
    }

    [TestMethod]
    public async Task InvokeMethodWithIDataPortalFactoryInjection()
    {
      var portal = _diContext.CreateDataPortal<DataPortalFactoryInjection>();
      var obj = await portal.CreateAsync();

      obj.Should().NotBeNull();
      obj.Data.Should().Be("Factory injected");
    }

    [TestMethod]
    public async Task FetchMethodWithIDataPortalFactoryInjection()
    {
      var portal = _diContext.CreateDataPortal<DataPortalFactoryInjection>();
      var obj = await portal.FetchAsync(42);

      obj.Should().NotBeNull();
      obj.Data.Should().Be("Fetched 42 with factory: injected");
    }

    [TestMethod]
    public void FindMethodWithOptionalInjection()
    {
      var obj = new OptionalServiceInjection();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, null);

      method.Should().NotBeNull();
      method.PrepForInvocation();
      method.Parameters.Should().HaveCount(1);
      method.IsInjected.Should().HaveCount(1);
      method.IsInjected![0].Should().BeTrue();
      method.AllowNull.Should().HaveCount(1);
      method.AllowNull![0].Should().BeTrue();
    }

    [TestMethod]
    public async Task InvokeMethodWithOptionalServiceInjection()
    {
      // Don't register the service - it should be null since AllowNull = true
      var portal = _diContext.CreateDataPortal<OptionalServiceInjection>();
      var obj = await portal.CreateAsync();

      obj.Should().NotBeNull();
      obj.Data.Should().Be("Optional service is null as expected");
    }

    [TestMethod]
    public void FindMethodWithNullableOptionalInjection()
    {
      var obj = new NullableOptionalServiceInjection();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, null);

      method.Should().NotBeNull();
      method.PrepForInvocation();
      method.Parameters.Should().HaveCount(1);
      method.IsInjected.Should().HaveCount(1);
      method.IsInjected![0].Should().BeTrue();
      method.AllowNull.Should().HaveCount(1);
      method.AllowNull![0].Should().BeTrue();
    }

    [TestMethod]
    public async Task InvokeMethodWithNullableOptionalInjection_ServiceNotRegistered()
    {
      var portal = _diContext.CreateDataPortal<NullableOptionalServiceInjection>();
      var obj = await portal.CreateAsync();

      obj.Should().NotBeNull();
      obj.Data.Should().Be("Nullable optional service is null as expected");
    }

    [TestMethod]
    public async Task InvokeMethodWithNullableOptionalInjection_ServiceRegistered()
    {
      var contextWithService = TestDIContextFactory.CreateDefaultContext(services =>
      {
        services.AddTransient<IOptionalService, FakeOptionalService>();
      });

      var portal = contextWithService.CreateDataPortal<NullableOptionalServiceInjection>();
      var obj = await portal.CreateAsync();

      obj.Should().NotBeNull();
      obj.Data.Should().Be("Fake service data");
    }

    [TestMethod]
    public void FindMethodWithRequiredInjection()
    {
      var obj = new RequiredServiceInjection();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, null);

      method.Should().NotBeNull();
      method.PrepForInvocation();
      method.Parameters.Should().HaveCount(1);
      method.IsInjected.Should().HaveCount(1);
      method.IsInjected![0].Should().BeTrue();
      method.AllowNull.Should().HaveCount(1);
      method.AllowNull![0].Should().BeFalse();
    }

    [TestMethod]
    public async Task InvokeMethodWithRequiredServiceInjection_ServiceRegistered()
    {
      // Create a context with the service registered
      var contextWithService = TestDIContextFactory.CreateDefaultContext(services =>
      {
        services.AddTransient<IOptionalService, FakeOptionalService>();
      });

      var portal = contextWithService.CreateDataPortal<RequiredServiceInjection>();
      var obj = await portal.CreateAsync();

      obj.Should().NotBeNull();
      obj.Data.Should().Be("Fake service data");
    }

    [TestMethod]
    public async Task InvokeMethodWithRequiredServiceInjection_ThrowsWhenServiceNotRegistered()
    {
      var portal = _diContext.CreateDataPortal<RequiredServiceInjection>();
      
      // This should throw because the service is required but not registered
      // The exception might be wrapped in other exceptions from the data portal
      await FluentActions.Invoking(async () => await portal.CreateAsync())
        .Should().ThrowAsync<Exception>();
    }

#if NET8_0_OR_GREATER
    [TestMethod]
    public void FindMethodWithKeyedServiceInjection()
    {
      var obj = new KeyedServiceInjection();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, null);

      method.Should().NotBeNull();
      method.PrepForInvocation();
      method.Parameters.Should().HaveCount(1);
      method.IsInjected.Should().HaveCount(1);
      method.IsInjected![0].Should().BeTrue();
      method.ServiceKeys.Should().HaveCount(1);
      method.ServiceKeys![0].Should().Be("serviceA");
    }

    [TestMethod]
    public async Task InvokeMethodWithKeyedServiceInjection()
    {
      var contextWithService = TestDIContextFactory.CreateDefaultContext(services =>
      {
        services.AddKeyedTransient<IOptionalService, ServiceAImplementation>("serviceA");
      });

      var portal = contextWithService.CreateDataPortal<KeyedServiceInjection>();
      var obj = await portal.CreateAsync();

      obj.Should().NotBeNull();
      obj.Data.Should().Be("Service A data");
    }

    [TestMethod]
    public void FindMethodWithKeyedServiceAndCriteriaInjection()
    {
      var obj = new KeyedServiceWithCriteriaInjection();
      var method = _systemUnderTest.FindDataPortalMethod<CreateAttribute>(obj, [123]);

      method.Should().NotBeNull();
      method.PrepForInvocation();
      method.Parameters.Should().HaveCount(2);
      method.IsInjected.Should().HaveCount(2);
      method.IsInjected![0].Should().BeFalse(); // First param is criteria
      method.IsInjected![1].Should().BeTrue();  // Second param is injected
      method.ServiceKeys.Should().HaveCount(2);
      method.ServiceKeys![0].Should().BeNull(); // First param is not injected
      method.ServiceKeys![1].Should().Be("serviceB");
    }

    [TestMethod]
    public async Task InvokeMethodWithKeyedServiceAndCriteriaInjection()
    {
      var contextWithService = TestDIContextFactory.CreateDefaultContext(services =>
      {
        services.AddKeyedTransient<IOptionalService, ServiceBImplementation>("serviceB");
      });

      var portal = contextWithService.CreateDataPortal<KeyedServiceWithCriteriaInjection>();
      var obj = await portal.CreateAsync(42);

      obj.Should().NotBeNull();
      obj.Id.Should().Be(42);
      obj.Data.Should().Be("Service B data");
    }

    [TestMethod]
    public async Task InvokeMethodWithOptionalKeyedServiceInjection_ServiceNotRegistered()
    {
      var portal = _diContext.CreateDataPortal<OptionalKeyedServiceInjection>();
      var obj = await portal.CreateAsync();

      obj.Should().NotBeNull();
      obj.Data.Should().Be("Keyed service is null as expected");
    }

    [TestMethod]
    public async Task InvokeMethodWithKeyedServiceInjection_ThrowsWhenServiceNotRegistered()
    {
      var portal = _diContext.CreateDataPortal<KeyedServiceInjection>();
      
      // This should throw because the keyed service is required but not registered
      await FluentActions.Invoking(async () => await portal.CreateAsync())
        .Should().ThrowAsync<Exception>();
    }
#endif
  }

  #region Classes for testing various scenarios of loading/finding data portal methods

  [Csla.Server.ObjectFactory("NotLoadedAssembly.NotKnownObjectFactoryInCurrentEnvironmentFactory, NotLoadedAssembly")]
  public class NotKnownObjectFactoryInCurrentEnvironment : BusinessBase<NotKnownObjectFactoryInCurrentEnvironment>
  {

  }

  public class OldStyleNoOverride : BusinessBase<OldStyleNoOverride>
  {
    [Create]
    private void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }
  }

  public class PrivateMethodBase<T> : CommandBase<T>
  where T : CommandBase<T>
  {
    [Create]
    private void Create()
    { }

    [Execute]
    private void Execute()
    { }
  }

  public class PrivateMethod : PrivateMethodBase<PrivateMethod>
  {

  }

  public class OldStyleCreate : BusinessBase<OldStyleCreate>
  {
    [Create]
    protected void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }

    [Create]
    private void DataPortal_Create(int id)
    {
      BusinessRules.CheckRules();
    }
  }

  public class OldStyleCriteria : BusinessBase<OldStyleCriteria>
  {
    [Create]
    private void DataPortal_Create(int id)
    {
      BusinessRules.CheckRules();
    }
  }

  public class OldStyleCriteriaBase : OldStyleCriteria
  {
  }

  public interface ICriteria : IReadOnlyBase
  {
    int Id { get; }
  }

  public class MyCriteria : ReadOnlyBase<MyCriteria>, ICriteria
  {
    public int Id => 123;
  }

  public class InterfaceCriteria : BusinessBase<InterfaceCriteria>
  {
    [Create]
    private void Create(ICriteria criteria) { _ = criteria; }
  }

  public class SimpleNoCriteriaCreate : BusinessBase<SimpleNoCriteriaCreate>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create(ICloneable x) { }
  }

  public class SimpleCriteriaCreate : BusinessBase<SimpleCriteriaCreate>
  {
    [Create]
    private void Create(int id) { }
  }

  public class ObjectCriteriaCreate : BusinessBase<ObjectCriteriaCreate>
  {
    [Create]
    private void Create(object id) { }
  }

  public class ObjectCriteriaCreateSubtype : ObjectCriteriaCreate
  { }

  public class AmbiguousNoCriteriaCreate : BusinessBase<AmbiguousNoCriteriaCreate>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create([Inject] ICloneable x) { }
  }

  public class CriteriaCreateWithDI : BusinessBase<CriteriaCreateWithDI>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create(int id, [Inject] ICloneable x) { }
  }

  public class MultipleCriteriaCreateWithDI : BusinessBase<MultipleCriteriaCreateWithDI>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create(int id, string foo, [Inject] ICloneable x) { }
  }

  public class CriteriaCreateWithMultipleDI : BusinessBase<CriteriaCreateWithMultipleDI>
  {
    [Create]
    private void Create(int id, [Inject] ICloneable x) { }

    [Create]
    private void Create(int id, [Inject] ICloneable x, [Inject] IAsyncResult y) { }
  }

  public class CriteriaCreateWithMultipleAmbiguousDI : BusinessBase<CriteriaCreateWithMultipleAmbiguousDI>
  {
    [Create]
    private void Create(int id, [Inject] ICloneable x) { }

    [Create]
    private void Create(int id, [Inject] ICloneable x, [Inject] IAsyncResult y) { }

    [Create]
    private void Create(int id, [Inject] ICloneable x, [Inject] IFormattable y) { }
  }

  public class MultipleCriteriaCreateWithDIInterleaved : BusinessBase<MultipleCriteriaCreateWithDIInterleaved>
  {
    [Create]
    private void Create() { }

    [Create]
    private void Create(int id, [Inject] ICloneable x, string foo) { }
  }

  public class BasicChild : BusinessBase<BasicChild>
  {
    private void Child_Update()
    {
      // nada
    }
  }

  public class ParamsChild : BusinessBase<ParamsChild>
  {
    private void Child_Update(params object[] parameters)
    {
      // nada
    }
  }

  public class ModernChild : BusinessBase<ModernChild>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    [UpdateChild]
    [InsertChild]
    private void UpdateOrInsert(int id)
    {
      using (BypassPropertyChecks)
        Id = id;
    }
  }

  public class NullableCriteria : BusinessBase<NullableCriteria>
  {
    [Create]
    private void Create(int? c) { _ = c; }
  }

  public class MultipleOverlappingCriteria : BusinessBase<MultipleOverlappingCriteria>
  {
    [Fetch]
    private void Fetch(int id)
    {
    }

    [Fetch]
    private void Fetch(int id, bool? value)
    {
    }

    [Fetch]
    private void Fetch(int id, int? value)
    {
    }

    [Fetch]
    private void Fetch(List<int?> values)
    {
    }

    [Fetch]
    private void Fetch(List<DateTime?> values)
    {
    }
  }

  public class Issue2109List : ReadOnlyListBase<Issue2109List, Issue2109>
  {
    private void DataPortal_Fetch(ICriteriaBase criteria, [Inject] IDataPortal<Issue2109> dp)
    {
      using (LoadListMode)
      {
        Add(dp.Fetch(criteria.ToString()));
      }
    }

    private void DataPortal_Fetch(IEnumerable<string> criteria, [Inject] IDataPortal<Issue2109> dp)
    {
      using (LoadListMode)
      {
        Add(dp.Fetch(criteria.First().ToString()));
      }
    }
  }

  public class Issue2109 : BusinessBase<Issue2109>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    private void DataPortal_Fetch(string name)
    {
      using (BypassPropertyChecks)
      {
        Name = name;
      }
    }
  }

  public interface ICriteriaBase
  { }

  public class My2109Criteria : ICriteriaBase
  {
  }

  public class Issue2287List : Issue2287ListBase
  {
  }

  public class Issue2287ListBase : Csla.BusinessBindingListBase<Issue2287List, Issue2287Edit>
  {
    private void DataPortal_Fetch(Criteria criteria)
    {
    }

    public class Criteria : Csla.CriteriaBase<Criteria>
    {
    }
  }

  public class Issue2287Edit : Issue2287EditBase<Issue2287Edit>
  {
    private new void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }
  }

  public class Issue2287EditBase<T> : BusinessBase<Issue2287EditBase<T>>
  {
    protected void DataPortal_Create()
    {
    }
  }

  public class Issue2396Edit : BusinessBase<Issue2396Edit>
  {
    [DeleteSelfChild]
    private void DeleteSelf()
    { }
  }

  public class Issue2396EditParams : BusinessBase<Issue2396EditParams>
  {
    [DeleteSelfChild]
    private void DeleteSelf(int x, int y)
    { }
  }

  public class DataPortalFactoryInjection : BusinessBase<DataPortalFactoryInjection>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));
    public string Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    [Create]
    private void Create([Inject] IDataPortalFactory factory)
    {
      using (BypassPropertyChecks)
      {
        Data = factory != null ? "Factory injected" : "Factory is null";
      }
    }

    [Fetch]
    private void Fetch(int id, [Inject] IDataPortalFactory factory)
    {
      using (BypassPropertyChecks)
      {
        Data = $"Fetched {id} with factory: {(factory != null ? "injected" : "null")}";
      }
    }
  }

  // Fake service interface for testing optional injection
  public interface IOptionalService
  {
    string GetData();
  }

  // Fake implementation for testing
  public class FakeOptionalService : IOptionalService
  {
    public string GetData() => "Fake service data";
  }

  public class OptionalServiceInjection : BusinessBase<OptionalServiceInjection>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));
    public string Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    [Create]
    private void Create([Inject(AllowNull = true)] IOptionalService optionalService)
    {
      using (BypassPropertyChecks)
      {
        Data = optionalService == null ? "Optional service is null as expected" : optionalService.GetData();
      }
    }
  }

#nullable enable
  public class NullableOptionalServiceInjection : BusinessBase<NullableOptionalServiceInjection>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));
    public string Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    [Create]
    private void Create([Inject] IOptionalService? optionalService)
    {
      using (BypassPropertyChecks)
      {
        Data = optionalService == null ? "Nullable optional service is null as expected" : optionalService.GetData();
      }
    }
  }
#nullable restore

  public class RequiredServiceInjection : BusinessBase<RequiredServiceInjection>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));
    public string Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    [Create]
    private void Create([Inject] IOptionalService requiredService)
    {
      using (BypassPropertyChecks)
      {
        Data = requiredService.GetData();
      }
    }
  }

#if NET8_0_OR_GREATER
  // Tests for keyed service injection (NET8+ only)
  public class KeyedServiceInjection : BusinessBase<KeyedServiceInjection>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));
    public string Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    [Create]
    private void Create([Inject(Key = "serviceA")] IOptionalService keyedService)
    {
      using (BypassPropertyChecks)
      {
        Data = keyedService.GetData();
      }
    }
  }

  public class KeyedServiceWithCriteriaInjection : BusinessBase<KeyedServiceWithCriteriaInjection>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));
    public string Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      set => SetProperty(IdProperty, value);
    }

    [Create]
    private void Create(int id, [Inject(Key = "serviceB")] IOptionalService keyedService)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Data = keyedService.GetData();
      }
    }
  }

  public class OptionalKeyedServiceInjection : BusinessBase<OptionalKeyedServiceInjection>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));
    public string Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    [Create]
    private void Create([Inject(Key = "nonExistent", AllowNull = true)] IOptionalService? keyedService)
    {
      using (BypassPropertyChecks)
      {
        Data = keyedService == null ? "Keyed service is null as expected" : keyedService.GetData();
      }
    }
  }

  public class ServiceAImplementation : IOptionalService
  {
    public string GetData() => "Service A data";
  }

  public class ServiceBImplementation : IOptionalService
  {
    public string GetData() => "Service B data";
  }
#endif

  #endregion
}