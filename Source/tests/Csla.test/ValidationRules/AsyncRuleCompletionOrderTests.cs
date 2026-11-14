//-----------------------------------------------------------------------
// <copyright file="AsyncRuleCompletionOrderTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the order of async rule completion notifications</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Core;
using Csla.Rules;
using Csla.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{
  /// <summary>
  /// An async rule's completion notifications are raised while the object
  /// is still busy, and the object reports itself idle last, so code
  /// after WaitForIdle() or CheckRulesAsync() never runs before the
  /// completion work is done (#4991).
  /// </summary>
  /// <remarks>
  /// The rules await gates that the tests open. The gates run their
  /// continuations synchronously, so opening a gate completes the rule
  /// on the test thread: the worst case for code that waits for idle.
  /// </remarks>
  [TestClass]
  public class AsyncRuleCompletionOrderTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
      _testHost = CreateHost();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    // Xaml mode raises PropertyChanged for the IsValid, IsSavable and IsBusy meta properties
    private static CslaTestHost CreateHost() =>
      CslaTestHost.Create(t => t.ConfigureCsla(o => o.Binding(bo => bo.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml)));

    private static GatedAsyncRuleRoot Create(CslaTestHost host = null) =>
      (host ?? _testHost).GetDataPortal<GatedAsyncRuleRoot>().Create();

    private static EventLog LogEvents(GatedAsyncRuleRoot obj)
    {
      var log = new EventLog();
      obj.PropertyChanged += (_, e) => log.Add($"PropertyChanged:{e.PropertyName}:busy={obj.IsBusy}");
      obj.ValidationComplete += (_, _) => log.Add($"ValidationComplete:busy={obj.IsBusy}");
      obj.BusyChanged += (_, e) => log.Add($"BusyChanged:{e.PropertyName}:{e.Busy}:busy={obj.IsBusy}");
      return log;
    }

    [TestMethod]
    public void CompletionNotifications_AreRaisedWhileBusy_AndBusyChangedIsLast()
    {
      var obj = Create();
      obj.Name = "a";
      obj.IsBusy.Should().BeTrue();
      var log = LogEvents(obj);
      var propertyBusyInValidationComplete = false;
      obj.ValidationComplete += (_, _) => propertyBusyInValidationComplete = obj.IsPropertyBusy(GatedAsyncRuleRoot.NameProperty);

      obj.Release(GatedAsyncRuleRoot.NameRule);

      var entries = log.Entries;
      entries.Should().Contain("PropertyChanged:Name:busy=True");
      entries.Should().Contain("PropertyChanged:IsValid:busy=True");
      entries.Should().Contain("ValidationComplete:busy=True");
      propertyBusyInValidationComplete.Should().BeTrue();
      entries.Last().Should().Be("BusyChanged:Name:False:busy=False");
      entries.Count(e => e.StartsWith("BusyChanged:")).Should().Be(1);
      entries.Take(entries.Count - 1).Should().OnlyContain(e => e.EndsWith("busy=True") || e.StartsWith("PropertyChanged:IsSavable") || e.StartsWith("PropertyChanged:IsSelfBusy") || e.StartsWith("PropertyChanged:IsBusy"));
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public void PropertyIdle_RaisesIsSavableAndIsBusyChanges_BeforeBusyChanged()
    {
      var obj = Create();
      obj.Name = "a";
      var log = LogEvents(obj);

      obj.Release(GatedAsyncRuleRoot.NameRule);

      var entries = log.Entries;
      entries.Skip(entries.FindLastIndex(e => e.EndsWith("busy=True")) + 1).Should().Equal(
        "PropertyChanged:IsSavable:busy=False",
        "PropertyChanged:IsSelfBusy:busy=False",
        "PropertyChanged:IsBusy:busy=False",
        "BusyChanged:Name:False:busy=False");
    }

    [TestMethod]
    public async Task WaitForIdle_ReturnsAfterAllCompletionNotifications()
    {
      var obj = Create();
      obj.Name = "a";
      var log = LogEvents(obj);
      var waiter = WaitAndLog();

      obj.Release(GatedAsyncRuleRoot.NameRule);
      await waiter;

      log.Entries.Last().Should().Be("WaitForIdle returned");
      log.Entries.Should().Contain("ValidationComplete:busy=True");

      async Task WaitAndLog()
      {
        await obj.WaitForIdle(TimeSpan.FromSeconds(5));
        log.Add("WaitForIdle returned");
      }
    }

    [TestMethod]
    public async Task CheckRulesAsync_ReturnsAfterAllCompletionNotifications()
    {
      var obj = Create();
      var log = LogEvents(obj);
      var waiter = CheckRulesAndLog();
      obj.IsBusy.Should().BeTrue();

      obj.Release(GatedAsyncRuleRoot.NameRule);
      obj.Release(GatedAsyncRuleRoot.CodeRuleA);
      obj.Release(GatedAsyncRuleRoot.CodeRuleB);
      obj.Release(GatedAsyncRuleRoot.OtherRule);
      obj.Release(GatedAsyncRuleRoot.SharedRule);
      await waiter;

      log.Entries.Last().Should().Be("CheckRulesAsync returned");
      log.Entries.Should().Contain("ValidationComplete:busy=True");
      obj.IsBusy.Should().BeFalse();

      async Task CheckRulesAndLog()
      {
        await obj.CheckRulesForTestAsync();
        log.Add("CheckRulesAsync returned");
      }
    }

    [TestMethod]
    public async Task DisposingServicesAfterWaitForIdle_DoesNotBreakCompletion()
    {
      var host = CreateHost();
      try
      {
        var obj = Create(host);
        obj.Name = "a";
        var errors = new EventLog();
        obj.PropertyChanged += (_, _) => UseServices();
        obj.ValidationComplete += (_, _) => UseServices();
        var waiter = WaitAndDispose();

        obj.Release(GatedAsyncRuleRoot.NameRule);
        await waiter;

        errors.Entries.Should().BeEmpty();

        void UseServices()
        {
          try
          {
            obj.UseServices();
          }
          catch (Exception ex)
          {
            errors.Add(ex.GetType().Name);
          }
        }

        async Task WaitAndDispose()
        {
          await obj.WaitForIdle(TimeSpan.FromSeconds(5));
          host.Dispose();
        }
      }
      finally
      {
        host.Dispose();
      }
    }

    [TestMethod]
    public async Task OverlappingRules_PropertyStaysBusyUntilLastRuleCompletes()
    {
      var obj = Create();
      obj.Code = "a";
      var log = LogEvents(obj);
      var waiter = obj.WaitForIdle(TimeSpan.FromSeconds(5));

      obj.Release(GatedAsyncRuleRoot.CodeRuleA);

      obj.IsBusy.Should().BeTrue();
      obj.IsPropertyBusy(GatedAsyncRuleRoot.CodeProperty).Should().BeTrue();
      log.Entries.Should().NotContain(e => e.StartsWith("BusyChanged:"));
      log.Entries.Should().NotContain(e => e.StartsWith("ValidationComplete"));
      waiter.IsCompleted.Should().BeFalse();

      obj.Release(GatedAsyncRuleRoot.CodeRuleB);
      await waiter;

      log.Entries.Should().Contain("ValidationComplete:busy=True");
      log.Entries.Last().Should().Be("BusyChanged:Code:False:busy=False");
    }

    [TestMethod]
    public void RuleWithAffectedProperties_ObjectBecomesIdleWithLastProperty()
    {
      var obj = Create();
      obj.Other = "a";
      obj.IsPropertyBusy(GatedAsyncRuleRoot.ExtraProperty).Should().BeTrue();
      var log = LogEvents(obj);

      obj.Release(GatedAsyncRuleRoot.OtherRule);

      log.Entries.Where(e => e.StartsWith("BusyChanged:")).Should().Equal(
        "BusyChanged:Other:False:busy=True",
        "BusyChanged:Extra:False:busy=False");
      log.Entries.Last().Should().Be("BusyChanged:Extra:False:busy=False");
      log.Entries.IndexOf("ValidationComplete:busy=True").Should().BeLessThan(log.Entries.IndexOf("BusyChanged:Other:False:busy=True"));
    }

    [TestMethod]
    public async Task RuleStartedByCompletionHandler_DelaysValidationComplete()
    {
      var obj = Create();
      obj.Name = "a";
      var log = LogEvents(obj);
      var started = false;
      obj.PropertyChanged += (_, e) =>
      {
        // starts two more async rules while the Name rule is completing
        if (e.PropertyName == nameof(obj.Name) && !started)
        {
          started = true;
          obj.Code = "b";
        }
      };
      var waiter = obj.WaitForIdle(TimeSpan.FromSeconds(5));

      obj.Release(GatedAsyncRuleRoot.NameRule);

      started.Should().BeTrue();
      obj.IsBusy.Should().BeTrue();
      log.Entries.Should().NotContain(e => e.StartsWith("ValidationComplete"));
      log.Entries.Should().Contain("BusyChanged:Name:False:busy=True");

      obj.Release(GatedAsyncRuleRoot.CodeRuleA);
      obj.Release(GatedAsyncRuleRoot.CodeRuleB);
      await waiter;

      log.Entries.Count(e => e.StartsWith("ValidationComplete")).Should().Be(1);
      log.Entries.Last().Should().Be("BusyChanged:Code:False:busy=False");
    }

    [TestMethod]
    public void RuleCompletedByCompletionHandler_SharedPropertyIsStillNotified()
    {
      var obj = Create();
      obj.Other = "a";
      obj.Shared = "b";
      var log = LogEvents(obj);
      var released = false;
      obj.PropertyChanged += (_, e) =>
      {
        // completes the Shared rule, which also affects Extra, while the Other rule is completing
        if (e.PropertyName == nameof(obj.Other) && !released)
        {
          released = true;
          obj.Release(GatedAsyncRuleRoot.SharedRule);
        }
      };

      obj.Release(GatedAsyncRuleRoot.OtherRule);

      released.Should().BeTrue();
      log.Entries.Should().Contain("PropertyChanged:Shared:busy=True");
      log.Entries.Should().Contain("PropertyChanged:Extra:busy=True");
      log.Entries.Count(e => e.StartsWith("ValidationComplete")).Should().Be(1);
      log.Entries.Last().Should().Be("BusyChanged:Extra:False:busy=False");
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public void ChildBecomingIdle_RefreshesParentIsSavable()
    {
      var parent = _testHost.GetDataPortal<GatedAsyncRuleParent>().Create();
      parent.Child.Name = "a";
      parent.IsBusy.Should().BeTrue();
      parent.IsSavable.Should().BeFalse();
      var savable = new EventLog();
      parent.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == nameof(parent.IsSavable))
          savable.Add(parent.IsSavable.ToString());
      };

      parent.Child.Gates.Release(GatedAsyncRuleChild.NameRule);

      parent.IsBusy.Should().BeFalse();
      parent.IsSavable.Should().BeTrue();
      savable.Entries.Should().NotBeEmpty();
      savable.Entries.Last().Should().Be("True");
    }

    private sealed class EventLog
    {
      private readonly List<string> _entries = [];

      public void Add(string entry)
      {
        lock (_entries)
          _entries.Add(entry);
      }

      public List<string> Entries
      {
        get
        {
          lock (_entries)
            return [.. _entries];
        }
      }
    }
  }


  /// <summary>
  /// Gates that async test rules wait at until the test opens them.
  /// </summary>
  public sealed class RuleGates
  {
    private readonly List<(string Tag, TaskCompletionSource<bool> Gate)> _gates = [];

    internal Task WaitAt(string tag)
    {
      // continuations run synchronously, so opening the gate completes the rule inline
      var gate = new TaskCompletionSource<bool>();
      lock (_gates)
        _gates.Add((tag, gate));
      return gate.Task;
    }

    /// <summary>
    /// Gets whether a running rule is waiting at a gate with the given tag.
    /// </summary>
    public bool IsWaitingAt(string tag)
    {
      lock (_gates)
        return _gates.Exists(g => g.Tag == tag);
    }

    /// <summary>
    /// Opens the gate of the first running rule with the given tag.
    /// </summary>
    public void Release(string tag)
    {
      TaskCompletionSource<bool> gate;
      lock (_gates)
      {
        var index = _gates.FindIndex(g => g.Tag == tag);
        if (index < 0)
          throw new InvalidOperationException($"No rule is waiting at gate {tag}.");
        gate = _gates[index].Gate;
        _gates.RemoveAt(index);
      }
      gate.SetResult(true);
    }
  }

  public interface IHasRuleGates
  {
    RuleGates Gates { get; }
  }

  /// <summary>
  /// An async rule that waits at a gate until the test opens it.
  /// </summary>
  public class GatedRule : BusinessRuleAsync
  {
    private readonly string _tag;

    public GatedRule(IPropertyInfo primaryProperty, string tag, params IPropertyInfo[] affectedProperties)
      : base(primaryProperty)
    {
      _tag = tag;
      ProvideTargetWhenAsync = true;
      InputProperties.Add(primaryProperty);
      AffectedProperties.AddRange(affectedProperties);
      RuleUri.AddQueryParameter("gate", tag);
    }

    protected override async Task ExecuteAsync(IRuleContext context)
    {
      await ((IHasRuleGates)context.Target).Gates.WaitAt(_tag);
    }
  }

  public partial class GatedAsyncRuleRoot : BusinessBase<GatedAsyncRuleRoot>, IHasRuleGates
  {
    public const string NameRule = "Name";
    public const string CodeRuleA = "CodeA";
    public const string CodeRuleB = "CodeB";
    public const string OtherRule = "Other";
    public const string SharedRule = "Shared";

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    public static readonly PropertyInfo<string> CodeProperty = RegisterProperty<string>(nameof(Code));
    public string Code
    {
      get => GetProperty(CodeProperty);
      set => SetProperty(CodeProperty, value);
    }

    public static readonly PropertyInfo<string> OtherProperty = RegisterProperty<string>(nameof(Other));
    public string Other
    {
      get => GetProperty(OtherProperty);
      set => SetProperty(OtherProperty, value);
    }

    public static readonly PropertyInfo<string> SharedProperty = RegisterProperty<string>(nameof(Shared));
    public string Shared
    {
      get => GetProperty(SharedProperty);
      set => SetProperty(SharedProperty, value);
    }

    public static readonly PropertyInfo<string> ExtraProperty = RegisterProperty<string>(nameof(Extra));
    public string Extra
    {
      get => GetProperty(ExtraProperty);
      set => SetProperty(ExtraProperty, value);
    }

    [NonSerialized]
    private RuleGates _gates;

    public RuleGates Gates => _gates ??= new RuleGates();

    public void Release(string tag) => Gates.Release(tag);

    public Task<List<string>> CheckRulesForTestAsync() => BusinessRules.CheckRulesAsync(TimeSpan.FromSeconds(5));

    public void UseServices()
    {
      _ = ApplicationContext.GetRequiredService<CslaOptions>();
      _ = IsSavable;
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new GatedRule(NameProperty, NameRule));
      BusinessRules.AddRule(new GatedRule(CodeProperty, CodeRuleA));
      BusinessRules.AddRule(new GatedRule(CodeProperty, CodeRuleB));
      // the Other and Shared rules both affect Extra
      BusinessRules.AddRule(new GatedRule(OtherProperty, OtherRule, ExtraProperty));
      BusinessRules.AddRule(new GatedRule(SharedProperty, SharedRule, ExtraProperty));
    }

    [Create]
    private void Create()
    {
    }
  }

  public partial class GatedAsyncRuleParent : BusinessBase<GatedAsyncRuleParent>
  {
    public static readonly PropertyInfo<GatedAsyncRuleChild> ChildProperty = RegisterProperty<GatedAsyncRuleChild>(nameof(Child));
    public GatedAsyncRuleChild Child => GetProperty(ChildProperty);

    [Create]
    private void Create([Inject] IChildDataPortal<GatedAsyncRuleChild> childPortal)
    {
      LoadProperty(ChildProperty, childPortal.CreateChild());
    }
  }

  public partial class GatedAsyncRuleChild : BusinessBase<GatedAsyncRuleChild>, IHasRuleGates
  {
    public const string NameRule = "ChildName";

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    [NonSerialized]
    private RuleGates _gates;

    public RuleGates Gates => _gates ??= new RuleGates();

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new GatedRule(NameProperty, NameRule));
    }

    [CreateChild]
    private void CreateChild()
    {
    }
  }
}
