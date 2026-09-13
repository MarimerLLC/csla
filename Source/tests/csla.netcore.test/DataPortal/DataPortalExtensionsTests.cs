//-----------------------------------------------------------------------
// <copyright file="DataPortalExtensionsTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Integration tests for generated data portal operations and extensions</summary>
//-----------------------------------------------------------------------
using System.Collections.Concurrent;
using System.Reflection;
using Csla.Channels.Local;
using Csla.Core;
using Csla.DataPortalClient;
using Csla.Server;
using Csla.Testing;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public partial class DataPortalExtensionsTests
  {
    private static CslaTestHost _testHost = default!;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
      _ = context;
      _testHost = CslaTestHost.Create(options => options.ConfigureServices(services =>
      {
        services.AddSingleton<ExtensionTestDal>();
        services.AddTransient<IDataPortalProxy>(sp => new CountingRemoteProxy(sp.GetRequiredService<LocalProxy>()));
      }));
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestInitialize]
    public void TestSetup()
    {
      CountingRemoteProxy.Reset();
    }

    #region Generated extensions

    [TestMethod]
    public async Task CreateAsync_UsesNamedDispatch()
    {
      var obj = await _testHost.GetDataPortal<ExtensionRoot>().PortalNewAsync();

      obj.Dispatch.Should().Be(DispatchPath.Named);
      obj.Name.Should().Be("new");
    }

    [TestMethod]
    public void Create_Sync_UsesNamedDispatch()
    {
      var obj = _testHost.GetDataPortal<ExtensionRoot>().PortalNewNamed("abc");

      obj.Dispatch.Should().Be(DispatchPath.Named);
      obj.Name.Should().Be("abc");
    }

    [TestMethod]
    public async Task FetchAsync_UsesNamedDispatchAndInjectsServices()
    {
      var obj = await _testHost.GetDataPortal<ExtensionRoot>().PortalGetByIdAsync(42);

      obj.Dispatch.Should().Be(DispatchPath.Named);
      obj.Id.Should().Be(42);
      obj.Name.Should().Be(ExtensionTestDal.Marker);
      obj.IsNew.Should().BeFalse();
    }

    [TestMethod]
    public void Fetch_Sync_UsesNamedDispatch()
    {
      var obj = _testHost.GetDataPortal<ExtensionRoot>().PortalGetById(7);

      obj.Dispatch.Should().Be(DispatchPath.Named);
      obj.Id.Should().Be(7);
    }

    [TestMethod]
    public async Task FetchAsync_StringArrayCriteria_IsNotSplit()
    {
      var obj = await _testHost.GetDataPortal<ExtensionRoot>().PortalGetByNamesAsync(["a", "b", "c"]);

      obj.Dispatch.Should().Be(DispatchPath.Named);
      obj.Name.Should().Be("a,b,c");
    }

    [TestMethod]
    public async Task FetchAsync_NullableCriteria_AcceptsNull()
    {
      var obj = await _testHost.GetDataPortal<ExtensionRoot>().PortalGetByOptionalAsync(null, null);

      obj.Dispatch.Should().Be(DispatchPath.Named);
      obj.Name.Should().Be("<null>|<null>");
    }

    [TestMethod]
    public async Task FetchAsync_AsyncOperationMethod_UsesNamedDispatch()
    {
      var key = Guid.NewGuid();
      var obj = await _testHost.GetDataPortal<ExtensionRoot>().PortalLoadAsync(key);

      obj.Dispatch.Should().Be(DispatchPath.Named);
      obj.Name.Should().Be(key.ToString());
    }

    [TestMethod]
    public void Fetch_Sync_AsyncOperationMethod_ThrowsLikeReflectionDispatch()
    {
      // a local data portal leaves the execution location as client
      using var localHost = CslaTestHost.Create();
      var generated = () => localHost.GetDataPortal<ExtensionRoot>().PortalLoad(Guid.NewGuid());
      var reflection = () => localHost.GetDataPortal<ReflectionRoot>().Fetch(Guid.NewGuid());

      var generatedException = generated.Should().Throw<Exception>().Which;
      var reflectionException = reflection.Should().Throw<Exception>().Which;

      generatedException.GetType().Should().Be(reflectionException.GetType());
      generatedException.GetBaseException().Should().BeOfType<NotSupportedException>();
      reflectionException.GetBaseException().Should().BeOfType<NotSupportedException>();
    }

    [TestMethod]
    public async Task FetchAsync_NonRunLocal_UsesConfiguredProxy()
    {
      _ = await _testHost.GetDataPortal<ExtensionRoot>().PortalGetByIdAsync(1);

      CountingRemoteProxy.Calls.Should().Be(1);
    }

    [TestMethod]
    public async Task FetchAsync_RunLocal_BypassesConfiguredProxy()
    {
      var obj = await _testHost.GetDataPortal<ExtensionRoot>().PortalGetLocalAsync("local".ToCharArray());

      obj.Dispatch.Should().Be(DispatchPath.Named);
      obj.Name.Should().Be("local");
      CountingRemoteProxy.Calls.Should().Be(0);
    }

    [TestMethod]
    public async Task DeleteAsync_UsesNamedDispatch()
    {
      await _testHost.GetDataPortal<ExtensionRoot>().PortalRemoveAsync(11);

      ExtensionRoot.Deleted.TryRemove(11, out var dispatch).Should().BeTrue();
      dispatch.Should().Be(DispatchPath.Named);
    }

    [TestMethod]
    public void Delete_Sync_UsesNamedDispatch()
    {
      _testHost.GetDataPortal<ExtensionRoot>().PortalRemove(12);

      ExtensionRoot.Deleted.TryRemove(12, out var dispatch).Should().BeTrue();
      dispatch.Should().Be(DispatchPath.Named);
    }

    [TestMethod]
    public async Task ExecuteAsync_UsesNamedDispatch()
    {
      var cmd = await _testHost.GetDataPortal<ExtensionCommand>().PortalRunAsync(3);

      cmd.Dispatch.Should().Be(DispatchPath.Named);
      cmd.Result.Should().Be(6);
    }

    [TestMethod]
    public void Execute_Sync_UsesNamedDispatch()
    {
      var cmd = _testHost.GetDataPortal<ExtensionCommand>().PortalRun(4);

      cmd.Dispatch.Should().Be(DispatchPath.Named);
      cmd.Result.Should().Be(8);
    }

    [TestMethod]
    public async Task CreateChildAsync_UsesNamedDispatch()
    {
      var child = await _testHost.GetChildDataPortal<ExtensionChild>().PortalNewChildAsync("kid");

      child.Dispatch.Should().Be(DispatchPath.Named);
      child.Name.Should().Be("kid");
      child.IsChild.Should().BeTrue();
      child.IsNew.Should().BeTrue();
    }

    [TestMethod]
    public void CreateChild_Sync_UsesNamedDispatch()
    {
      var child = _testHost.GetChildDataPortal<ExtensionChild>().PortalNewChild("kid");

      child.Dispatch.Should().Be(DispatchPath.Named);
    }

    [TestMethod]
    public async Task FetchChildAsync_UsesNamedDispatch()
    {
      var child = await _testHost.GetChildDataPortal<ExtensionChild>().PortalLoadChildAsync(9, ["x", "y"]);

      child.Dispatch.Should().Be(DispatchPath.Named);
      child.Name.Should().Be("9:x,y");
      child.IsNew.Should().BeFalse();
    }

    [TestMethod]
    public void FetchChild_Sync_UsesNamedDispatch()
    {
      var child = _testHost.GetChildDataPortal<ExtensionChild>().PortalLoadChild(10, ["z"]);

      child.Dispatch.Should().Be(DispatchPath.Named);
      child.Name.Should().Be("10:z");
    }

    [TestMethod]
    public async Task ExistingParamsApi_StillUsesGeneratedDispatch()
    {
      var obj = await _testHost.GetDataPortal<ExtensionRoot>().FetchAsync(3);

      obj.Dispatch.Should().Be(DispatchPath.Named);
      obj.Id.Should().Be(3);
    }

    [TestMethod]
    public async Task ChildParamsApi_UsesTypeBasedDispatch()
    {
      var child = await _testHost.GetChildDataPortal<ExtensionChild>().CreateChildAsync("kid");

      child.Dispatch.Should().Be(DispatchPath.Type);
    }

    [TestMethod]
    public async Task CustomDataPortal_UsesFallback()
    {
      var fake = new FakeDataPortal();

      var obj = await fake.PortalGetByIdAsync(99);

      obj.Should().BeSameAs(fake.Result);
      fake.FetchCriteria.Should().Equal(99);
    }

    [TestMethod]
    public async Task CustomDataPortal_UsesFallbackWithoutSplittingArrays()
    {
      var fake = new FakeDataPortal();
      string[] names = ["a", "b"];

      _ = await fake.PortalGetByNamesAsync(names);

      fake.FetchCriteria.Should().HaveCount(1);
      fake.FetchCriteria![0].Should().BeSameAs(names);
    }

    #endregion

    #region Name consistency

    [TestMethod]
    public void GeneratedInterfaceMemberNames_MatchRuntimeOperationNames()
    {
      var checkedMembers = 0;
      var types = typeof(DataPortalExtensionsTests).Assembly.GetTypes()
        .Where(t => !t.ContainsGenericParameters)
        .Select(t => (Type: t, Operations: t.GetNestedType("IDataPortalOperations", BindingFlags.NonPublic | BindingFlags.Public)))
        .Where(t => t.Operations is not null)
        .ToList();

      types.Should().NotBeEmpty();

      foreach (var (type, operations) in types)
      {
        foreach (var member in operations!.GetMethods())
        {
          var kind = member.Name.Split('_')[0];
          var attributeType = typeof(FetchAttribute).Assembly.GetType($"Csla.{kind}Attribute", throwOnError: true)!;
          var memberParameters = member.GetParameters().Select(p => p.ParameterType).ToArray();

          var method = type
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
            .Single(m => m.GetCustomAttributes(attributeType, false).Any()
              && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(memberParameters));

          var expected = (string)typeof(DataPortalOperationNameHelper)
            .GetMethod(nameof(DataPortalOperationNameHelper.ComputeOperationName), BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(attributeType)
            .Invoke(null, [method])!;

          member.Name.Should().Match(name => name == expected || name.StartsWith(expected + "_"),
            $"{type.Name}.{method.Name} should map to operation {expected}");
          checkedMembers++;
        }
      }

      checkedMembers.Should().BeGreaterThan(10);
    }

    #endregion

    #region Test types

    public static class DispatchPath
    {
      public const string Named = "Named";
      public const string Type = "Type";
      public const string Reflection = "Reflection";

      public static string Current()
      {
        var stackTrace = Environment.StackTrace;
        if (stackTrace.Contains(nameof(IDataPortalOperationNamedMapping)))
          return Named;
        if (stackTrace.Contains(nameof(IDataPortalOperationMapping)))
          return Type;
        return Reflection;
      }
    }

    public class ExtensionTestDal
    {
      public const string Marker = "from-dal";
    }

    public class CountingRemoteProxy(LocalProxy implementingProxy) : IDataPortalProxy
    {
      private static int _calls;

      public static int Calls => _calls;

      public static void Reset() => _calls = 0;

      public bool IsServerRemote => true;

      public Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
      {
        Interlocked.Increment(ref _calls);
        return implementingProxy.Create(objectType, criteria, context, isSync);
      }

      public Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
      {
        Interlocked.Increment(ref _calls);
        return implementingProxy.Delete(objectType, criteria, context, isSync);
      }

      public Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
      {
        Interlocked.Increment(ref _calls);
        return implementingProxy.Fetch(objectType, criteria, context, isSync);
      }

      public Task<DataPortalResult> Update(ICslaObject obj, DataPortalContext context, bool isSync)
      {
        Interlocked.Increment(ref _calls);
        return implementingProxy.Update(obj, context, isSync);
      }
    }

    [DataPortalExtensions(Prefix = "Portal")]
    public partial class ExtensionRoot : BusinessBase<ExtensionRoot>
    {
      public static readonly ConcurrentDictionary<int, string> Deleted = new();

      public static readonly PropertyInfo<string> DispatchProperty = RegisterProperty<string>(nameof(Dispatch));
      public string Dispatch
      {
        get => GetProperty(DispatchProperty);
        private set => LoadProperty(DispatchProperty, value);
      }

      public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
      public int Id
      {
        get => GetProperty(IdProperty);
        private set => LoadProperty(IdProperty, value);
      }

      public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
      public string Name
      {
        get => GetProperty(NameProperty);
        private set => LoadProperty(NameProperty, value);
      }

      [Create]
      private void New()
      {
        Dispatch = DispatchPath.Current();
        Name = "new";
      }

      [Create]
      private void NewNamed(string name)
      {
        Dispatch = DispatchPath.Current();
        Name = name;
      }

      [Fetch]
      private void GetById(int id, [Inject] ExtensionTestDal dal)
      {
        _ = dal;
        Dispatch = DispatchPath.Current();
        Id = id;
        Name = ExtensionTestDal.Marker;
      }

      [Fetch]
      private void GetByNames(string[] names)
      {
        Dispatch = DispatchPath.Current();
        Name = string.Join(",", names);
      }

      [Fetch]
      private void GetByOptional(int? number, string? text)
      {
        Dispatch = DispatchPath.Current();
        Name = $"{number?.ToString() ?? "<null>"}|{text ?? "<null>"}";
      }

      [Fetch]
      private async Task LoadAsync(Guid key)
      {
        Dispatch = DispatchPath.Current();
        await Task.Yield();
        Name = key.ToString();
      }

      [RunLocal]
      [Fetch]
      private void GetLocal(char[] code)
      {
        Dispatch = DispatchPath.Current();
        Name = new string(code);
      }

      [Delete]
      private void Remove(int id)
      {
        Deleted[id] = DispatchPath.Current();
      }

      [Fetch]
      private void GetByKey((int Id, string Name) key, List<int?> values, Dictionary<string, int[]>[] map)
      {
        _ = key;
        _ = values;
        _ = map;
      }
    }

    /// <summary>
    /// Not partial, so dispatch uses reflection.
    /// </summary>
    public class ReflectionRoot : BusinessBase<ReflectionRoot>
    {
      [Fetch]
      private async Task LoadAsync(Guid key)
      {
        _ = key;
        await Task.Yield();
      }
    }

    [DataPortalExtensions(Prefix = "Portal")]
    public partial class ExtensionCommand : CommandBase<ExtensionCommand>
    {
      public static readonly PropertyInfo<string> DispatchProperty = RegisterProperty<string>(nameof(Dispatch));
      public string Dispatch
      {
        get => ReadProperty(DispatchProperty);
        private set => LoadProperty(DispatchProperty, value);
      }

      public static readonly PropertyInfo<int> ResultProperty = RegisterProperty<int>(nameof(Result));
      public int Result
      {
        get => ReadProperty(ResultProperty);
        private set => LoadProperty(ResultProperty, value);
      }

      [Execute]
      private void Run(int value)
      {
        Dispatch = DispatchPath.Current();
        Result = value * 2;
      }
    }

    [DataPortalExtensions(Prefix = "Portal")]
    public partial class ExtensionChild : BusinessBase<ExtensionChild>
    {
      public static readonly PropertyInfo<string> DispatchProperty = RegisterProperty<string>(nameof(Dispatch));
      public string Dispatch
      {
        get => GetProperty(DispatchProperty);
        private set => LoadProperty(DispatchProperty, value);
      }

      public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
      public string Name
      {
        get => GetProperty(NameProperty);
        private set => LoadProperty(NameProperty, value);
      }

      [CreateChild]
      private void NewChild(string name)
      {
        Dispatch = DispatchPath.Current();
        Name = name;
      }

      [FetchChild]
      private void LoadChild(int id, string[] tags)
      {
        Dispatch = DispatchPath.Current();
        Name = $"{id}:{string.Join(",", tags)}";
      }
    }

    private sealed class FakeDataPortal : IDataPortal<ExtensionRoot>
    {
      public ExtensionRoot Result { get; } = new();

      public object?[]? FetchCriteria { get; private set; }

      public Task<ExtensionRoot> FetchAsync(params object?[]? criteria)
      {
        FetchCriteria = criteria;
        return Task.FromResult(Result);
      }

      public Task<ExtensionRoot> CreateAsync(params object?[]? criteria) => throw new NotSupportedException();
      public Task<ExtensionRoot> UpdateAsync(ExtensionRoot obj) => throw new NotSupportedException();
      public Task<ExtensionRoot> ExecuteAsync(ExtensionRoot command) => throw new NotSupportedException();
      public Task<ExtensionRoot> ExecuteAsync(params object?[]? criteria) => throw new NotSupportedException();
      public Task DeleteAsync(params object?[]? criteria) => throw new NotSupportedException();
      public ExtensionRoot Create(params object?[]? criteria) => throw new NotSupportedException();
      public ExtensionRoot Fetch(params object?[]? criteria) => throw new NotSupportedException();
      public ExtensionRoot Execute(ExtensionRoot obj) => throw new NotSupportedException();
      public ExtensionRoot Execute(params object?[]? criteria) => throw new NotSupportedException();
      public ExtensionRoot Update(ExtensionRoot obj) => throw new NotSupportedException();
      public void Delete(params object?[]? criteria) => throw new NotSupportedException();
    }

    #endregion
  }
}
