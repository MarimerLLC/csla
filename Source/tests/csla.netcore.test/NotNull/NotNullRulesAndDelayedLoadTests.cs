//-----------------------------------------------------------------------
// <copyright file="NotNullRulesAndDelayedLoadTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for rule output and delayed loading of not-null properties</summary>
//-----------------------------------------------------------------------

#nullable enable

using Csla.Core;
using Csla.Rules;
using Csla.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.NotNull
{
  [TestClass]
  public class NotNullRulesAndDelayedLoadTests
  {
    private static CslaTestHost _testHost = default!;

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

    private static T New<T>() => _testHost.ApplicationContext.CreateInstanceDI<T>();

    // Rule output write-back

    [TestMethod]
    public void BusinessRules_SyncRuleOutputsNull_AddsErrorResultAndWritesNothing()
    {
      var obj = New<NotNullRuleRoot>();

      obj.Trigger = 1;

      Assert.IsFalse(((IManageProperties)obj).FieldExists(NotNullRuleRoot.DataProperty));
      Assert.AreEqual("set", obj.Name, "Other output values are still written");
      var broken = obj.BrokenRulesCollection.Single(r => r.Property == nameof(NotNullRuleRoot.Data));
      Assert.AreEqual(RuleSeverity.Error, broken.Severity);
      StringAssert.Contains(broken.Description, nameof(NotNullRuleRoot.Data));
      Assert.IsFalse(obj.IsValid);
    }

    [TestMethod]
    public async Task BusinessRules_AsyncRuleOutputsNull_AddsErrorResultAndWritesNothing()
    {
      var obj = New<NotNullAsyncRuleRoot>();

      obj.Trigger = 1;
      await obj.WaitForIdle(TimeSpan.FromSeconds(10));

      Assert.IsFalse(((IManageProperties)obj).FieldExists(NotNullAsyncRuleRoot.DataProperty));
      var broken = obj.BrokenRulesCollection.Single(r => r.Property == nameof(NotNullAsyncRuleRoot.Data));
      Assert.AreEqual(RuleSeverity.Error, broken.Severity);
    }

    // RuleContext.AddOutValue

    [TestMethod]
    public void RuleContext_AddOutValueNullForPrimaryNotNullProperty_ThrowsArgumentNullException()
    {
      var obj = New<NotNullRuleRoot>();

      obj.RunOutValueRule(OutValueMode.Primary);

      Assert.IsInstanceOfType<ArgumentNullException>(obj.Captured);
    }

    [TestMethod]
    public void RuleContext_AddOutValueNullForNotNullPropertyWithoutDefault_ThrowsArgumentNullException()
    {
      var obj = New<NotNullRuleRoot>();

      obj.RunOutValueRule(OutValueMode.Property);

      Assert.IsInstanceOfType<ArgumentNullException>(obj.Captured);
    }

    [TestMethod]
    public void RuleContext_AddOutValueNullForNotNullPropertyWithDefault_StoresDefault()
    {
      var obj = New<NotNullRuleRoot>();
      obj.LoadCode("x");

      obj.RunOutValueRule(OutValueMode.Defaulted);

      Assert.IsNull(obj.Captured);
      Assert.AreEqual("unknown", obj.Code);
    }

    // Delayed loading

    [TestMethod]
    public void BusinessBase_DelayedLoad_NotNullProperty_ThrowsNotSupportedOnEveryCall()
    {
      var obj = New<NotNullLazyRoot>();

      for (var i = 0; i < 2; i++)
      {
        Assert.ThrowsExactly<NotSupportedException>(() => obj.LazyGet());
        Assert.ThrowsExactly<NotSupportedException>(() => obj.LazyGetAsync());
        Assert.ThrowsExactly<NotSupportedException>(() => obj.LazyRead());
        Assert.ThrowsExactly<NotSupportedException>(() => obj.LazyReadAsync());
        Assert.ThrowsExactly<NotSupportedException>(() => obj.LoadAsync());
      }
      Assert.IsFalse(((IManageProperties)obj).FieldExists(NotNullLazyRoot.NameProperty));
    }

    [TestMethod]
    public void BusinessBase_DelayedLoad_NotNullPropertyAlreadyLoaded_StillThrows()
    {
      var obj = New<NotNullLazyRoot>();
      obj.LoadName("a");

      Assert.ThrowsExactly<NotSupportedException>(() => obj.LazyGet());
      Assert.ThrowsExactly<NotSupportedException>(() => obj.LazyGetAsync());
      Assert.ThrowsExactly<NotSupportedException>(() => obj.LazyRead());
      Assert.ThrowsExactly<NotSupportedException>(() => obj.LazyReadAsync());
    }

    [TestMethod]
    public void ReadOnlyBase_DelayedLoad_NotNullProperty_ThrowsNotSupportedOnEveryCall()
    {
      var obj = New<NotNullLazyInfo>();

      for (var i = 0; i < 2; i++)
      {
        Assert.ThrowsExactly<NotSupportedException>(() => obj.LazyGet());
        Assert.ThrowsExactly<NotSupportedException>(() => obj.LazyGetAsync());
        Assert.ThrowsExactly<NotSupportedException>(() => obj.LazyRead());
        Assert.ThrowsExactly<NotSupportedException>(() => obj.LazyReadAsync());
        Assert.ThrowsExactly<NotSupportedException>(() => obj.LoadAsync());
      }
    }

    [TestMethod]
    public void BusinessBase_DelayedLoad_RegularProperty_StillWorks()
    {
      var obj = New<NotNullLazyRoot>();

      Assert.AreEqual("lazy", obj.LazyGetRegular());
    }
  }

  public enum OutValueMode
  {
    None,
    Primary,
    Property,
    Defaulted
  }

  [Serializable]
  public class NotNullRuleRoot : BusinessBase<NotNullRuleRoot>
  {
    public static readonly PropertyInfo<int> TriggerProperty = RegisterProperty<int>(nameof(Trigger));
    public int Trigger
    {
      get => GetProperty(TriggerProperty);
      set => SetProperty(TriggerProperty, value);
    }

    public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name));
    public string Name => GetProperty(NameProperty);

    public static readonly NotNullPropertyInfo<byte[]> DataProperty = RegisterNotNullProperty<byte[]>(nameof(Data));
    public byte[] Data => GetProperty(DataProperty);

    public static readonly NotNullPropertyInfo<string> CodeProperty = RegisterNotNullProperty<string>(nameof(Code), null, "unknown");
    public string Code => GetProperty(CodeProperty);

    [NonSerialized]
    private OutValueMode _mode;
    public OutValueMode Mode => _mode;

    [NonSerialized]
    private Exception? _captured;
    public Exception? Captured
    {
      get => _captured;
      set => _captured = value;
    }

    public void LoadCode(string value) => LoadProperty(CodeProperty, value);

    public void RunOutValueRule(OutValueMode mode)
    {
      _mode = mode;
      BusinessRules.CheckRules(NameProperty);
      _mode = OutValueMode.None;
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new NullOutputRule(TriggerProperty, DataProperty, NameProperty));
      BusinessRules.AddRule(new OutValueRule(NameProperty, CodeProperty));
    }

    private class NullOutputRule : BusinessRule
    {
      private readonly IPropertyInfo _nullTarget;
      private readonly IPropertyInfo _valueTarget;

      public NullOutputRule(IPropertyInfo primaryProperty, IPropertyInfo nullTarget, IPropertyInfo valueTarget)
        : base(primaryProperty)
      {
        _nullTarget = nullTarget;
        _valueTarget = valueTarget;
        AffectedProperties.Add(nullTarget);
        AffectedProperties.Add(valueTarget);
      }

      protected override void Execute(IRuleContext context)
      {
        // bypasses AddOutValue, which would throw, to reach the write-back backstop
        context.OutputPropertyValues[_nullTarget] = null;
        context.OutputPropertyValues[_valueTarget] = "set";
      }
    }

    private class OutValueRule : BusinessRule
    {
      private readonly IPropertyInfo _defaulted;

      public OutValueRule(IPropertyInfo primaryProperty, IPropertyInfo defaulted)
        : base(primaryProperty)
      {
        _defaulted = defaulted;
        AffectedProperties.Add(defaulted);
      }

      protected override void Execute(IRuleContext context)
      {
        var target = (NotNullRuleRoot)context.Target!;
        try
        {
          switch (target.Mode)
          {
            case OutValueMode.Primary:
              context.AddOutValue(null);
              break;
            case OutValueMode.Property:
              context.AddOutValue(PrimaryProperty!, null);
              break;
            case OutValueMode.Defaulted:
              context.AddOutValue(_defaulted, null);
              break;
          }
        }
        catch (Exception ex)
        {
          target.Captured = ex;
        }
      }
    }
  }

  [Serializable]
  public class NotNullAsyncRuleRoot : BusinessBase<NotNullAsyncRuleRoot>
  {
    public static readonly PropertyInfo<int> TriggerProperty = RegisterProperty<int>(nameof(Trigger));
    public int Trigger
    {
      get => GetProperty(TriggerProperty);
      set => SetProperty(TriggerProperty, value);
    }

    public static readonly NotNullPropertyInfo<byte[]> DataProperty = RegisterNotNullProperty<byte[]>(nameof(Data));
    public byte[] Data => GetProperty(DataProperty);

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new AsyncNullOutputRule(TriggerProperty, DataProperty));
    }

    private class AsyncNullOutputRule : BusinessRuleAsync
    {
      private readonly IPropertyInfo _nullTarget;

      public AsyncNullOutputRule(IPropertyInfo primaryProperty, IPropertyInfo nullTarget)
        : base(primaryProperty)
      {
        _nullTarget = nullTarget;
        AffectedProperties.Add(nullTarget);
      }

      protected override async Task ExecuteAsync(IRuleContext context)
      {
        await Task.Yield();
        context.OutputPropertyValues[_nullTarget] = null;
      }
    }
  }

  [Serializable]
  public class NotNullLazyRoot : BusinessBase<NotNullLazyRoot>
  {
    public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name));
    public string Name => GetProperty(NameProperty);

    public static readonly PropertyInfo<string?> RegularProperty = RegisterProperty<string?>(nameof(Regular));
    public string? Regular => GetProperty(RegularProperty);

    public void LoadName(string value) => LoadProperty(NameProperty, value);
    public string? LazyGet() => LazyGetProperty(NameProperty, () => "lazy");
    public string? LazyGetAsync() => LazyGetPropertyAsync(NameProperty, () => Task.FromResult("lazy"));
    public string? LazyRead() => LazyReadProperty(NameProperty, () => "lazy");
    public string? LazyReadAsync() => LazyReadPropertyAsync(NameProperty, () => Task.FromResult("lazy"));
    public void LoadAsync() => LoadPropertyAsync(NameProperty, Task.FromResult("lazy"));
    public string? LazyGetRegular() => LazyGetProperty(RegularProperty, () => "lazy");
  }

  [Serializable]
  public class NotNullLazyInfo : ReadOnlyBase<NotNullLazyInfo>
  {
    public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name));
    public string Name => GetProperty(NameProperty);

    public string? LazyGet() => LazyGetProperty(NameProperty, () => "lazy");
    public string? LazyGetAsync() => LazyGetPropertyAsync(NameProperty, () => Task.FromResult("lazy"));
    public string? LazyRead() => LazyReadProperty(NameProperty, () => "lazy");
    public string? LazyReadAsync() => LazyReadPropertyAsync(NameProperty, () => Task.FromResult("lazy"));
    public void LoadAsync() => LoadPropertyAsync(NameProperty, Task.FromResult("lazy"));
  }
}
