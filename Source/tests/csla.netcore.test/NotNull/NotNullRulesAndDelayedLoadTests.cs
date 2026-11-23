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
using FluentAssertions;
using FluentAssertions.Execution;
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

      var broken = obj.BrokenRulesCollection.Single(r => r.Property == nameof(NotNullRuleRoot.Data));
      using (new AssertionScope())
      {
        ((IManageProperties)obj).FieldExists(NotNullRuleRoot.DataProperty).Should().BeFalse();
        obj.Name.Should().Be("set", "other output values are still written");
        obj.Label.Should().Be(string.Empty, "a null string output is stored as string.Empty");
        obj.BrokenRulesCollection.Should().NotContain(r => r.Property == nameof(NotNullRuleRoot.Label));
        broken.Severity.Should().Be(RuleSeverity.Error);
        broken.Description.Should().Contain(nameof(NotNullRuleRoot.Data));
        obj.IsValid.Should().BeFalse();
      }
    }

    [TestMethod]
    public async Task BusinessRules_AsyncRuleOutputsNull_AddsErrorResultAndWritesNothing()
    {
      var obj = New<NotNullAsyncRuleRoot>();

      obj.Trigger = 1;
      await obj.WaitForIdle(TimeSpan.FromSeconds(10));

      var broken = obj.BrokenRulesCollection.Single(r => r.Property == nameof(NotNullAsyncRuleRoot.Data));
      using (new AssertionScope())
      {
        ((IManageProperties)obj).FieldExists(NotNullAsyncRuleRoot.DataProperty).Should().BeFalse();
        broken.Severity.Should().Be(RuleSeverity.Error);
      }
    }

    // RuleContext.AddOutValue

    [TestMethod]
    public void RuleContext_AddOutValueNullForPrimaryNotNullProperty_ThrowsArgumentNullException()
    {
      var obj = New<NotNullRuleRoot>();

      obj.RunOutValueRule(OutValueMode.Primary);

      obj.Captured.Should().BeAssignableTo<ArgumentNullException>();
    }

    [TestMethod]
    public void RuleContext_AddOutValueNullForNotNullPropertyWithoutDefault_ThrowsArgumentNullException()
    {
      var obj = New<NotNullRuleRoot>();

      obj.RunOutValueRule(OutValueMode.Property);

      obj.Captured.Should().BeAssignableTo<ArgumentNullException>();
    }

    [TestMethod]
    public void RuleContext_AddOutValueNullForNotNullPropertyWithDefault_StoresDefault()
    {
      var obj = New<NotNullRuleRoot>();
      obj.LoadCode("x");

      obj.RunOutValueRule(OutValueMode.Defaulted);

      using (new AssertionScope())
      {
        obj.Captured.Should().BeNull();
        obj.Code.Should().Be("unknown");
      }
    }

    [TestMethod]
    public void RuleContext_AddOutValueNullForNotNullString_StoresEmptyString()
    {
      var obj = New<NotNullRuleRoot>();
      obj.LoadName("x");

      obj.RunOutValueRule(OutValueMode.String);

      using (new AssertionScope())
      {
        obj.Captured.Should().BeNull();
        obj.CapturedOutValue.Should().Be(string.Empty, "the output value is the one the object stores");
        obj.Name.Should().Be(string.Empty);
      }
    }

    // Delayed loading

    [TestMethod]
    public void BusinessBase_DelayedLoad_NotNullProperty_ThrowsNotSupportedOnEveryCall()
    {
      var obj = New<NotNullLazyRoot>();

      for (var i = 0; i < 2; i++)
      {
        FluentActions.Invoking(() => obj.LazyGet()).Should().ThrowExactly<NotSupportedException>();
        FluentActions.Invoking(() => obj.LazyGetAsync()).Should().ThrowExactly<NotSupportedException>();
        FluentActions.Invoking(() => obj.LazyRead()).Should().ThrowExactly<NotSupportedException>();
        FluentActions.Invoking(() => obj.LazyReadAsync()).Should().ThrowExactly<NotSupportedException>();
        FluentActions.Invoking(() => obj.LoadAsync()).Should().ThrowExactly<NotSupportedException>();
      }
      ((IManageProperties)obj).FieldExists(NotNullLazyRoot.NameProperty).Should().BeFalse();
    }

    [TestMethod]
    public void BusinessBase_DelayedLoad_NotNullPropertyAlreadyLoaded_StillThrows()
    {
      var obj = New<NotNullLazyRoot>();
      obj.LoadName("a");

      using (new AssertionScope())
      {
        FluentActions.Invoking(() => obj.LazyGet()).Should().ThrowExactly<NotSupportedException>();
        FluentActions.Invoking(() => obj.LazyGetAsync()).Should().ThrowExactly<NotSupportedException>();
        FluentActions.Invoking(() => obj.LazyRead()).Should().ThrowExactly<NotSupportedException>();
        FluentActions.Invoking(() => obj.LazyReadAsync()).Should().ThrowExactly<NotSupportedException>();
      }
    }

    [TestMethod]
    public void ReadOnlyBase_DelayedLoad_NotNullProperty_ThrowsNotSupportedOnEveryCall()
    {
      var obj = New<NotNullLazyInfo>();

      for (var i = 0; i < 2; i++)
      {
        FluentActions.Invoking(() => obj.LazyGet()).Should().ThrowExactly<NotSupportedException>();
        FluentActions.Invoking(() => obj.LazyGetAsync()).Should().ThrowExactly<NotSupportedException>();
        FluentActions.Invoking(() => obj.LazyRead()).Should().ThrowExactly<NotSupportedException>();
        FluentActions.Invoking(() => obj.LazyReadAsync()).Should().ThrowExactly<NotSupportedException>();
        FluentActions.Invoking(() => obj.LoadAsync()).Should().ThrowExactly<NotSupportedException>();
      }
    }

    [TestMethod]
    public void BusinessBase_DelayedLoad_RegularProperty_StillWorks()
    {
      var obj = New<NotNullLazyRoot>();

      obj.LazyGetRegular().Should().Be("lazy");
    }
  }

  public enum OutValueMode
  {
    None,
    Primary,
    Property,
    Defaulted,
    String
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

    public static readonly NotNullPropertyInfo<string> LabelProperty = RegisterNotNullProperty<string>(nameof(Label));
    public string Label => GetProperty(LabelProperty);

    public void LoadName(string value) => LoadProperty(NameProperty, value);
    public void LoadLabel(string value) => LoadProperty(LabelProperty, value);

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

    [NonSerialized]
    private object? _capturedOutValue;
    public object? CapturedOutValue
    {
      get => _capturedOutValue;
      set => _capturedOutValue = value;
    }

    public void LoadCode(string value) => LoadProperty(CodeProperty, value);

    public void RunOutValueRule(OutValueMode mode)
    {
      _mode = mode;
      BusinessRules.CheckRules(DataProperty);
      _mode = OutValueMode.None;
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new NullOutputRule(TriggerProperty, DataProperty, NameProperty, LabelProperty));
      BusinessRules.AddRule(new OutValueRule(DataProperty, CodeProperty, NameProperty));
    }

    private class NullOutputRule : BusinessRule
    {
      private readonly IPropertyInfo _nullTarget;
      private readonly IPropertyInfo _valueTarget;
      private readonly IPropertyInfo _nullStringTarget;

      public NullOutputRule(IPropertyInfo primaryProperty, IPropertyInfo nullTarget, IPropertyInfo valueTarget, IPropertyInfo nullStringTarget)
        : base(primaryProperty)
      {
        _nullTarget = nullTarget;
        _valueTarget = valueTarget;
        _nullStringTarget = nullStringTarget;
        AffectedProperties.Add(nullTarget);
        AffectedProperties.Add(valueTarget);
        AffectedProperties.Add(nullStringTarget);
      }

      protected override void Execute(IRuleContext context)
      {
        // bypasses AddOutValue, which would throw, to reach the write-back backstop
        context.OutputPropertyValues[_nullTarget] = null;
        context.OutputPropertyValues[_valueTarget] = "set";
        context.OutputPropertyValues[_nullStringTarget] = null;
      }
    }

    private class OutValueRule : BusinessRule
    {
      private readonly IPropertyInfo _defaulted;
      private readonly IPropertyInfo _string;

      public OutValueRule(IPropertyInfo primaryProperty, IPropertyInfo defaulted, IPropertyInfo stringProperty)
        : base(primaryProperty)
      {
        _defaulted = defaulted;
        _string = stringProperty;
        AffectedProperties.Add(defaulted);
        AffectedProperties.Add(stringProperty);
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
            case OutValueMode.String:
              context.AddOutValue(_string, null);
              target.CapturedOutValue = context.OutputPropertyValues[_string];
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
