//-----------------------------------------------------------------------
// <copyright file="InheritedOperationDispatchTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Generated dispatch of operation methods inherited from a base class</summary>
//-----------------------------------------------------------------------
using System.Runtime.ExceptionServices;
using Csla.Server;
using Csla.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  /// <summary>
  /// Operation methods declared on a base class are dispatched by the
  /// derived class's generated mapping, so the data portal does not throw
  /// DataPortalOperationNotSupportedException before falling back to
  /// reflection.
  /// </summary>
  [TestClass]
  public class InheritedOperationDispatchTests
  {
    private static CslaTestHost _testHost = default!;
    private int _notSupportedCount;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
      _ = context;
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestInitialize]
    public void Setup()
    {
      InheritedOperationLog.Calls.Clear();
      _notSupportedCount = 0;
      AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
    }

    [TestCleanup]
    public void Cleanup()
    {
      AppDomain.CurrentDomain.FirstChanceException -= OnFirstChanceException;
    }

    private void OnFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
    {
      if (e.Exception is DataPortalOperationNotSupportedException)
        Interlocked.Increment(ref _notSupportedCount);
    }

    [TestMethod]
    public void CreateCallsInheritedMethodWithoutFallback()
    {
      var portal = _testHost.GetDataPortal<InheritedOpsRoot>();

      var root = portal.Create(42);

      root.Seed.Should().Be(42);
      InheritedOperationLog.Calls.Should().Equal("Base.Create");
      _notSupportedCount.Should().Be(0);
    }

    [TestMethod]
    public void FetchCallsInheritedOverloadThroughBase()
    {
      var portal = _testHost.GetDataPortal<InheritedOpsRoot>();

      portal.Fetch(7);
      portal.Fetch("name");

      InheritedOperationLog.Calls.Should().Equal("Base.Fetch(int)", "Root.Fetch(string)");
      _notSupportedCount.Should().Be(0);
    }

    [TestMethod]
    public void SaveCallsInheritedRootAndChildMethodsWithoutFallback()
    {
      var portal = _testHost.GetDataPortal<InheritedOpsRoot>();
      var root = portal.Create(1);
      root.Children.AddNew();
      root.Children.AddNew();

      root = root.Save();
      root.Children[0].Name = "changed";
      root.Children[1].Name = "changed";
      root.Save();

      InheritedOperationLog.Calls.Should().Equal(
        "Base.Create",
        "ChildBase.Insert", "ChildBase.Insert", "Base.Insert",
        "ChildBase.Update", "ChildBase.Update", "Base.Update");
      _notSupportedCount.Should().Be(0);
    }
  }

  internal static class InheritedOperationLog
  {
    public static List<string> Calls { get; } = [];
  }

  public abstract partial class InheritedOpsRootBase<T> : BusinessBase<T>
    where T : InheritedOpsRootBase<T>
  {
    public static readonly PropertyInfo<int> SeedProperty = RegisterProperty<int>(nameof(Seed));
    public int Seed
    {
      get => GetProperty(SeedProperty);
      private set => LoadProperty(SeedProperty, value);
    }

    [Create, RunLocal]
    protected virtual void Create(int seed)
    {
      InheritedOperationLog.Calls.Add("Base.Create");
      Seed = seed;
      OnCreated();
    }

    protected virtual void OnCreated() { }

    [Fetch]
    protected void Fetch(int id) => InheritedOperationLog.Calls.Add("Base.Fetch(int)");

    [Insert]
    protected virtual void Insert()
    {
      FieldManager.UpdateChildren();
      InheritedOperationLog.Calls.Add("Base.Insert");
    }

    [Update]
    protected virtual void Update()
    {
      FieldManager.UpdateChildren();
      InheritedOperationLog.Calls.Add("Base.Update");
    }
  }

  public partial class InheritedOpsRoot : InheritedOpsRootBase<InheritedOpsRoot>
  {
    public static readonly PropertyInfo<InheritedOpsChildList> ChildrenProperty = RegisterProperty<InheritedOpsChildList>(nameof(Children));
    public InheritedOpsChildList Children
    {
      get => GetProperty(ChildrenProperty);
      private set => LoadProperty(ChildrenProperty, value);
    }

    protected override void OnCreated()
    {
      var childPortal = ApplicationContext.GetRequiredService<IChildDataPortal<InheritedOpsChildList>>();
      Children = childPortal.CreateChild();
    }

    [Fetch]
    private void Fetch(string name) => InheritedOperationLog.Calls.Add("Root.Fetch(string)");
  }

  public abstract partial class InheritedOpsChildBase<T> : BusinessBase<T>
    where T : InheritedOpsChildBase<T>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    [InsertChild]
    protected virtual void InsertChild() => InheritedOperationLog.Calls.Add("ChildBase.Insert");

    [UpdateChild]
    protected virtual void UpdateChild() => InheritedOperationLog.Calls.Add("ChildBase.Update");
  }

  public partial class InheritedOpsChild : InheritedOpsChildBase<InheritedOpsChild>
  {
    [FetchChild]
    private void FetchChild(string name) => LoadProperty(NameProperty, name);
  }

  // Declares an operation of its own, so it has a generated mapping that
  // must dispatch the inherited Child_Create and Child_UpdateAsync.
  public partial class InheritedOpsChildList : BusinessListBase<InheritedOpsChildList, InheritedOpsChild>
  {
    [FetchChild]
    private void FetchChild(string[] names, [Inject] IChildDataPortal<InheritedOpsChild> childPortal)
    {
      foreach (var name in names)
        Add(childPortal.FetchChild(name));
    }
  }
}
