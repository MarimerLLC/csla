//-----------------------------------------------------------------------
// <copyright file="LazyLoadAsyncTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the async lazy property methods</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.LazyLoad
{
  [TestClass]
  public class LazyLoadAsyncTests
  {
    private static CslaTestHost _testHost;

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

    [TestMethod]
    public async Task BusinessBase_LazyGetPropertyAsync_InvokesFactoryOnce()
    {
      var obj = _testHost.GetDataPortal<LazyAsyncEdit>().Create();

      for (var i = 0; i < 3; i++)
        Assert.IsTrue(string.IsNullOrEmpty(obj.Name), "Name should not be loaded yet");
      Assert.AreEqual(1, obj.NameFactoryCalls, "Factory should run once while loading");

      await CompleteLoad(obj, nameof(LazyAsyncEdit.Name), () => obj.NameSource.SetResult("loaded"));

      for (var i = 0; i < 3; i++)
        Assert.AreEqual("loaded", obj.Name);
      Assert.AreEqual(1, obj.NameFactoryCalls, "Factory should not run again after loading");
    }

    [TestMethod]
    public async Task BusinessBase_LazyReadPropertyAsync_InvokesFactoryOnce()
    {
      var obj = _testHost.GetDataPortal<LazyAsyncEdit>().Create();

      for (var i = 0; i < 3; i++)
        Assert.IsTrue(string.IsNullOrEmpty(obj.Code), "Code should not be loaded yet");
      Assert.AreEqual(1, obj.CodeFactoryCalls, "Factory should run once while loading");

      await CompleteLoad(obj, nameof(LazyAsyncEdit.Code), () => obj.CodeSource.SetResult("loaded"));

      for (var i = 0; i < 3; i++)
        Assert.AreEqual("loaded", obj.Code);
      Assert.AreEqual(1, obj.CodeFactoryCalls, "Factory should not run again after loading");
    }

    [TestMethod]
    public void BusinessBase_LazyGetPropertyAsync_NullFactoryThrows()
    {
      var obj = _testHost.GetDataPortal<LazyAsyncEdit>().Create();

      Assert.ThrowsException<ArgumentNullException>(() => obj.GetWithNullFactory());
      Assert.ThrowsException<ArgumentNullException>(() => obj.ReadWithNullFactory());
    }

    [TestMethod]
    public async Task BusinessBase_LazyGetPropertyAsync_TaskOverloadStillLoads()
    {
      var obj = _testHost.GetDataPortal<LazyAsyncEdit>().Create();
      var source = new TaskCompletionSource<string>();

      Assert.IsTrue(string.IsNullOrEmpty(obj.GetWithTask(source.Task)));

      await CompleteLoad(obj, nameof(LazyAsyncEdit.Legacy), () => source.SetResult("loaded"));

      Assert.AreEqual("loaded", obj.GetWithTask(Task.FromResult("ignored")));
    }

    [TestMethod]
    public async Task ReadOnlyBase_LazyGetPropertyAsync_InvokesFactoryOnce()
    {
      var obj = _testHost.GetDataPortal<LazyAsyncInfo>().Fetch();

      for (var i = 0; i < 3; i++)
        Assert.IsTrue(string.IsNullOrEmpty(obj.Name), "Name should not be loaded yet");
      Assert.AreEqual(1, obj.NameFactoryCalls, "Factory should run once while loading");

      await CompleteLoad(obj, nameof(LazyAsyncInfo.Name), () => obj.NameSource.SetResult("loaded"));

      for (var i = 0; i < 3; i++)
        Assert.AreEqual("loaded", obj.Name);
      Assert.AreEqual(1, obj.NameFactoryCalls, "Factory should not run again after loading");
    }

    [TestMethod]
    public async Task ReadOnlyBase_LazyReadPropertyAsync_InvokesFactoryOnce()
    {
      var obj = _testHost.GetDataPortal<LazyAsyncInfo>().Fetch();

      for (var i = 0; i < 3; i++)
        Assert.IsTrue(string.IsNullOrEmpty(obj.Code), "Code should not be loaded yet");
      Assert.AreEqual(1, obj.CodeFactoryCalls, "Factory should run once while loading");

      await CompleteLoad(obj, nameof(LazyAsyncInfo.Code), () => obj.CodeSource.SetResult("loaded"));

      for (var i = 0; i < 3; i++)
        Assert.AreEqual("loaded", obj.Code);
      Assert.AreEqual(1, obj.CodeFactoryCalls, "Factory should not run again after loading");
    }

    [TestMethod]
    public void ReadOnlyBase_LazyGetPropertyAsync_NullFactoryThrows()
    {
      var obj = _testHost.GetDataPortal<LazyAsyncInfo>().Fetch();

      Assert.ThrowsException<ArgumentNullException>(() => obj.GetWithNullFactory());
      Assert.ThrowsException<ArgumentNullException>(() => obj.ReadWithNullFactory());
    }

    /// <summary>
    /// Completes the load and waits for the PropertyChanged event the
    /// load manager raises once the value is stored.
    /// </summary>
    private static async Task CompleteLoad(INotifyPropertyChanged obj, string propertyName, Action complete)
    {
      var changed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
      PropertyChangedEventHandler handler = (_, e) =>
      {
        if (e.PropertyName == propertyName)
          changed.TrySetResult(true);
      };
      obj.PropertyChanged += handler;
      try
      {
        complete();
        var finished = await Task.WhenAny(changed.Task, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.AreSame(changed.Task, finished, $"PropertyChanged was not raised for {propertyName}");
      }
      finally
      {
        obj.PropertyChanged -= handler;
      }
    }
  }

  public class LazyAsyncEdit : BusinessBase<LazyAsyncEdit>
  {
    public TaskCompletionSource<string> NameSource { get; } = new();
    public int NameFactoryCalls { get; private set; }

    public TaskCompletionSource<string> CodeSource { get; } = new();
    public int CodeFactoryCalls { get; private set; }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name), RelationshipTypes.LazyLoad);
    public string Name => LazyGetPropertyAsync(NameProperty, LoadNameAsync);

    public static readonly PropertyInfo<string> CodeProperty = RegisterProperty<string>(nameof(Code), RelationshipTypes.LazyLoad);
    public string Code => LazyReadPropertyAsync(CodeProperty, () =>
    {
      CodeFactoryCalls++;
      return CodeSource.Task;
    });

    public static readonly PropertyInfo<string> LegacyProperty = RegisterProperty<string>(nameof(Legacy), RelationshipTypes.LazyLoad);
    public string Legacy => GetProperty(LegacyProperty);

#pragma warning disable CS0618 // The Task<P> overload is obsolete; this covers it.
    public string GetWithTask(Task<string> task) => LazyGetPropertyAsync(LegacyProperty, task);
#pragma warning restore CS0618

    public string GetWithNullFactory() => LazyGetPropertyAsync(NameProperty, (Func<Task<string>>)null);
    public string ReadWithNullFactory() => LazyReadPropertyAsync(CodeProperty, (Func<Task<string>>)null);

    private Task<string> LoadNameAsync()
    {
      NameFactoryCalls++;
      return NameSource.Task;
    }

    [Create]
    private void Create()
    { }
  }

  public class LazyAsyncInfo : ReadOnlyBase<LazyAsyncInfo>
  {
    public TaskCompletionSource<string> NameSource { get; } = new();
    public int NameFactoryCalls { get; private set; }

    public TaskCompletionSource<string> CodeSource { get; } = new();
    public int CodeFactoryCalls { get; private set; }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name), RelationshipTypes.LazyLoad);
    public string Name => LazyGetPropertyAsync(NameProperty, () =>
    {
      NameFactoryCalls++;
      return NameSource.Task;
    });

    public static readonly PropertyInfo<string> CodeProperty = RegisterProperty<string>(nameof(Code), RelationshipTypes.LazyLoad);
    public string Code => LazyReadPropertyAsync(CodeProperty, LoadCodeAsync);

    public string GetWithNullFactory() => LazyGetPropertyAsync(NameProperty, (Func<Task<string>>)null);
    public string ReadWithNullFactory() => LazyReadPropertyAsync(CodeProperty, (Func<Task<string>>)null);

    private Task<string> LoadCodeAsync()
    {
      CodeFactoryCalls++;
      return CodeSource.Task;
    }

    [Fetch]
    private void Fetch()
    { }
  }
}
