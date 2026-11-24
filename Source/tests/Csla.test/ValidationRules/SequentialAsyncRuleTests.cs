//-----------------------------------------------------------------------
// <copyright file="SequentialAsyncRuleTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for sequential async rules</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Core;
using Csla.Rules;
using Csla.Testing;
using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{
  /// <summary>
  /// A sequential async rule runs in the rule stream: the rules after it
  /// wait until it completes, and its result short-circuits them like a
  /// sync rule's result (#5014).
  /// </summary>
  /// <remarks>
  /// The async rules await gates that the tests open. Opening a gate
  /// completes the rule, and resumes the stream, on the test thread.
  /// </remarks>
  [TestClass]
  public class SequentialAsyncRuleTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    private static SequentialRuleRoot Create(string ruleSet, CslaTestHost host = null)
    {
      var obj = (host ?? _testHost).GetDataPortal<SequentialRuleRoot>().Create();
      obj.UseRuleSet(ruleSet);
      return obj;
    }

    #region IsSequential

    [TestMethod]
    public void IsSequential_IsTrueForSyncRules()
    {
      IBusinessRuleBase rule = new SeqRecordRule(SequentialRuleRoot.AProperty, "x", 0);
      rule.IsSequential.Should().BeTrue();
    }

    [TestMethod]
    public void IsSequential_IsFalseByDefaultForAsyncRules_AndSettable()
    {
      var rule = new SeqGatedRule(SequentialRuleRoot.AProperty, "x", 0, sequential: false);
      rule.IsSequential.Should().BeFalse();
      ((IBusinessRuleBase)rule).IsSequential.Should().BeFalse();

      rule.IsSequential = true;

      rule.IsSequential.Should().BeTrue();
      ((IBusinessRuleBase)rule).IsSequential.Should().BeTrue();
    }

    [TestMethod]
    public void IsSequential_IsLockedAfterTheRuleHasRun()
    {
      var obj = Create("order");
      obj.Outcomes.Enqueue("Lookup", "sync");
      obj.CheckRulesNow();
      var rule = SequentialRuleRoot.OrderLookupRule;

      var act = () => rule.IsSequential = false;

      act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region CheckRules()

    [TestMethod]
    public void CheckRules_LaterRulesOfEveryPropertyWaitForTheSequentialRule()
    {
      var obj = Create("order");

      var affected = obj.CheckRulesNow();

      obj.Log.Should().Equal("A1", "B1", "Lookup:start");
      affected.Should().Contain("A").And.Contain("B");
      obj.IsBusy.Should().BeTrue();
      obj.IsPropertyBusy(SequentialRuleRoot.AProperty).Should().BeTrue();
      obj.IsPropertyBusy(SequentialRuleRoot.BProperty).Should().BeTrue();

      obj.Gates.Release("Lookup");

      obj.Log.Should().Equal("A1", "B1", "Lookup:start", "Lookup:end", "A2", "B2");
      obj.IsBusy.Should().BeFalse();
      obj.IsPropertyBusy(SequentialRuleRoot.BProperty).Should().BeFalse();
    }

    [TestMethod]
    public void CheckRules_BrokenSequentialRule_ShortCircuitsItsProperty()
    {
      var obj = Create("order");
      obj.Outcomes.Enqueue("Lookup", "error");

      obj.CheckRulesNow();
      obj.Gates.Release("Lookup");

      obj.Log.Should().Equal("A1", "B1", "Lookup:start", "Lookup:end", "B2");
      obj.BrokenRulesCollection.ErrorCount.Should().Be(1);
    }

    [TestMethod]
    public void CheckRules_BrokenSequentialRule_WithProcessThroughPriority_RunsLaterRules()
    {
      var obj = Create("order");
      obj.ProcessThroughPriority = 2;
      obj.Outcomes.Enqueue("Lookup", "error");

      obj.CheckRulesNow();
      obj.Gates.Release("Lookup");

      obj.Log.Should().Equal("A1", "B1", "Lookup:start", "Lookup:end", "A2", "B2");
    }

    [TestMethod]
    public void CheckRules_SequentialRuleStopProcessing_StopsItsProperty()
    {
      var obj = Create("order");
      obj.ProcessThroughPriority = 2;
      obj.Outcomes.Enqueue("Lookup", "stop");

      obj.CheckRulesNow();
      obj.Gates.Release("Lookup");

      obj.Log.Should().Equal("A1", "B1", "Lookup:start", "Lookup:end", "B2");
    }

    [TestMethod]
    public void CheckRules_SequentialRuleOutputValue_FeedsTheRerunPass()
    {
      var obj = Create("output");

      obj.CheckRulesNow();
      obj.Gates.Release("Lookup");

      obj.C.Should().Be("Lookup-out");
      obj.Log.Should().Equal("A1", "C1", "Lookup:start", "Lookup:end", "A2", "C1");
    }

    [TestMethod]
    public void CheckRules_EarlierNonSequentialRule_CompletesBeforeTheSequentialRuleStarts()
    {
      var obj = Create("earlier");

      obj.CheckRulesNow();

      obj.Log.Should().Equal("Slow:start", "C1");
      obj.Gates.IsWaitingAt("Lookup").Should().BeFalse();

      obj.Gates.Release("Slow");

      obj.Log.Should().Equal("Slow:start", "C1", "Slow:end", "Lookup:start");

      obj.Gates.Release("Lookup");

      // Slow's output value made C dirty: pass 2 reruns C1
      obj.Log.Should().Equal("Slow:start", "C1", "Slow:end", "Lookup:start", "Lookup:end", "B2", "A2", "C1");
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public void CheckRules_EarlierNonSequentialRuleError_ShortCircuitsItsProperty()
    {
      var obj = Create("earlier");
      obj.Outcomes.Enqueue("Slow", "error");

      obj.CheckRulesNow();
      obj.Gates.Release("Slow");
      obj.Gates.Release("Lookup");

      obj.Log.Should().NotContain("B2");
      obj.Log.Should().Contain("A2");
    }

    [TestMethod]
    public void CheckRules_EarlierNonSequentialRuleStopProcessing_StopsItsProperty()
    {
      var obj = Create("earlier");
      obj.ProcessThroughPriority = 5;
      obj.Outcomes.Enqueue("Slow", "stop");

      obj.CheckRulesNow();
      obj.Gates.Release("Slow");
      obj.Gates.Release("Lookup");

      obj.Log.Should().NotContain("B2");
      obj.Log.Should().Contain("A2");
    }

    [TestMethod]
    public void CheckRules_NonSequentialRuleWithNoLaterSequentialRule_RunsInTheBackground()
    {
      var obj = Create("nonseq");

      obj.CheckRulesNow();

      obj.Log.Should().Equal("Slow:start", "B2", "A2");
      obj.IsBusy.Should().BeTrue();

      obj.Gates.Release("Slow");

      obj.Log.Should().Equal("Slow:start", "B2", "A2", "Slow:end");
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public void CheckRules_TwoSequentialRulesAtTheSamePriority_RunOneAtATimeInRegistrationOrder()
    {
      var obj = Create("tie");

      obj.CheckRulesNow();

      obj.Log.Should().Equal("Lookup:start");
      obj.Gates.IsWaitingAt("LookupB").Should().BeFalse();

      obj.Gates.Release("Lookup");

      obj.Log.Should().Equal("Lookup:start", "Lookup:end", "LookupB:start");

      obj.Gates.Release("LookupB");

      obj.Log.Should().Equal("Lookup:start", "Lookup:end", "LookupB:start", "LookupB:end", "A2");
    }

    [TestMethod]
    public void CheckRules_SequentialRuleThatCompletesSynchronously_DoesNotPause()
    {
      var obj = Create("order");
      obj.Outcomes.Enqueue("Lookup", "sync");

      obj.CheckRulesNow();

      obj.Log.Should().Equal("A1", "B1", "Lookup:start", "Lookup:end", "A2", "B2");
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public void CheckRules_ThrowingSequentialRule_CompletesWithAnErrorAndShortCircuits()
    {
      var obj = Create("order");
      obj.Outcomes.Enqueue("Lookup", "throw");

      obj.CheckRulesNow();
      obj.Gates.Release("Lookup");

      obj.Log.Should().Equal("A1", "B1", "Lookup:start", "B2");
      obj.BrokenRulesCollection.Should().ContainSingle(r => r.Description.Contains("boom"));
      obj.IsBusy.Should().BeFalse();
    }

    #endregion

    #region CheckRulesAsync, WaitForIdle and notifications

    [TestMethod]
    public async Task CheckRulesAsync_ReturnsAfterTheWholeStream_WithAllAffectedProperties()
    {
      var obj = Create("output");
      var log = LogEvents(obj);
      var task = obj.CheckRulesForTestAsync();
      task.IsCompleted.Should().BeFalse();

      obj.Gates.Release("Lookup");
      var affected = await task;

      obj.Log.Should().Contain("A2");
      affected.Should().Contain("A").And.Contain("C");
      log.Count(e => e == "ValidationComplete").Should().Be(1);
      log.Last().Should().StartWith("BusyChanged:").And.EndWith(":False");
      log.IndexOf("ValidationComplete").Should().BeGreaterThan(log.IndexOf("PropertyChanged:C"));
      obj.IsBusy.Should().BeFalse();
      // properties affected after the pause are raised through PropertyChanged
      log.Should().Contain("PropertyChanged:C");
      log.Should().Contain("PropertyChanged:A");
    }

    [TestMethod]
    public async Task WaitForIdle_ReturnsOnlyAfterTheWholeStream()
    {
      var obj = Create("tie");
      obj.CheckRulesNow();
      var waiter = obj.WaitForIdle(TimeSpan.FromSeconds(5));

      obj.Gates.Release("Lookup");

      waiter.IsCompleted.Should().BeFalse();
      obj.IsBusy.Should().BeTrue();

      obj.Gates.Release("LookupB");
      await waiter;

      obj.Log.Last().Should().Be("A2");
    }

    [TestMethod]
    public async Task CheckRulesAsync_AsyncRuleAffectedProperties_AreInTheResult()
    {
      var obj = Create("nonseq");
      var task = obj.CheckRulesForTestAsync();

      obj.Gates.Release("Slow");
      var affected = await task;

      // Slow's output value goes to C, which has no rule of its own in this rule set
      affected.Should().Contain("C");
    }

    #endregion

    #region CheckRules(property) and CheckObjectRules()

    [TestMethod]
    public void CheckRulesForProperty_PausesAndCascadesAfterTheSequentialRule()
    {
      var obj = Create("cascade");
      var log = LogEvents(obj);

      obj.A = "x";

      obj.Log.Should().Equal("Lookup:start");
      obj.IsPropertyBusy(SequentialRuleRoot.AProperty).Should().BeTrue();

      obj.Gates.Release("Lookup");

      obj.Log.Should().Equal("Lookup:start", "Lookup:end", "A2", "D1");
      log.Should().Contain("PropertyChanged:D");
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public async Task CheckRulesAsyncForProperty_ReturnsTheCascadedProperties()
    {
      var obj = Create("cascade");
      var task = obj.CheckPropertyRulesForTestAsync(SequentialRuleRoot.AProperty);

      obj.Gates.Release("Lookup");
      var affected = await task;

      affected.Should().Contain("A").And.Contain("D");
    }

    [TestMethod]
    public void CheckRulesForProperty_TwoPropertyStreams_DoNotBlockEachOther()
    {
      var obj = Create("cascade");

      obj.A = "x";
      obj.B = "y";

      obj.Log.Should().Equal("Lookup:start", "LookupB:start");

      obj.Gates.Release("LookupB");

      obj.Log.Should().Equal("Lookup:start", "LookupB:start", "LookupB:end", "B1");
      obj.IsBusy.Should().BeTrue();

      obj.Gates.Release("Lookup");

      obj.Log.Should().Equal("Lookup:start", "LookupB:start", "LookupB:end", "B1", "Lookup:end", "A2", "D1");
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public void CheckObjectRules_SequentialObjectRule_LaterObjectRulesWait()
    {
      var obj = Create("object");

      obj.CheckObjectRulesNow();

      obj.Log.Should().Equal("ObjLookup:start");
      obj.IsBusy.Should().BeTrue();

      obj.Gates.Release("ObjLookup");

      // the object rule's affected property A cascades
      obj.Log.Should().Equal("ObjLookup:start", "ObjLookup:end", "ObjAfter", "A1");
      obj.IsBusy.Should().BeFalse();
    }

    #endregion

    #region Stale streams

    [TestMethod]
    public void PropertySetDuringPause_DiscardsTheRestAndTheOldResult()
    {
      var obj = Create("cascade");
      obj.Outcomes.Enqueue("Lookup", "error");

      obj.A = "x";
      obj.A = "y";

      obj.Log.Should().Equal("Lookup:start", "Lookup:start");

      // the first run completes: its error is stale and the rest of its stream never runs
      obj.Gates.Release("Lookup");

      obj.Log.Should().Equal("Lookup:start", "Lookup:start", "Lookup:end");
      obj.BrokenRulesCollection.ErrorCount.Should().Be(0);
      obj.IsBusy.Should().BeTrue();

      obj.Gates.Release("Lookup");

      obj.Log.Should().Equal("Lookup:start", "Lookup:start", "Lookup:end", "Lookup:end", "A2", "D1");
      obj.IsValid.Should().BeTrue();
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public void PropertySetDuringPause_RestartsADiscardedWholeObjectStream()
    {
      var obj = Create("order");
      obj.CheckRulesNow();
      obj.Log.Clear();

      obj.B = "z";

      // the property stream runs B's rules, then CheckRules() restarts
      obj.Log.Should().Equal("B1", "B2", "A1", "B1", "Lookup:start");

      obj.Gates.Release("Lookup");

      obj.Log.Should().Equal("B1", "B2", "A1", "B1", "Lookup:start", "Lookup:end");
      obj.IsBusy.Should().BeTrue();

      obj.Gates.Release("Lookup");

      obj.Log.Should().Equal("B1", "B2", "A1", "B1", "Lookup:start", "Lookup:end", "Lookup:end", "A2", "B2");
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public void CheckRulesDuringPause_ThatThrows_ReleasesTheDiscardedStream()
    {
      var obj = Create("unsettled");
      obj.CascadeOnDirtyProperties = true;
      obj.Outcomes.Enqueue("Lookup", "error");
      obj.CheckRulesNow();
      obj.IsPropertyBusy(SequentialRuleRoot.CProperty).Should().BeTrue();
      obj.Outcomes.Enqueue("Lookup", "sync");

      var act = () => obj.CheckRulesNow();

      act.Should().Throw<InvalidOperationException>();
      obj.IsPropertyBusy(SequentialRuleRoot.CProperty).Should().BeFalse();

      // the discarded run's lookup completes: nothing reran it, so its error applies
      obj.Gates.Release("Lookup");

      obj.BrokenRulesCollection.ErrorCount.Should().Be(1);
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public async Task ExceptionAfterAPause_IsRethrownByCheckRulesAsync_AndReleasesBusyState()
    {
      var obj = Create("unsettled");
      obj.CascadeOnDirtyProperties = true;
      var task = obj.CheckRulesForTestAsync();
      obj.IsPropertyBusy(SequentialRuleRoot.CProperty).Should().BeTrue();

      // the rules after the lookup never settle: the resumed run throws
      obj.Gates.Release("Lookup");

      var act = async () => await task;
      await act.Should().ThrowAsync<InvalidOperationException>();
      obj.IsPropertyBusy(SequentialRuleRoot.CProperty).Should().BeFalse();
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public async Task CheckRulesAsync_SynchronousFailure_FaultsTheTask()
    {
      var obj = Create("unsettled");
      obj.CascadeOnDirtyProperties = true;
      obj.Outcomes.Enqueue("Lookup", "sync");

      // the exception is in the returned task, not thrown by the call
      var task = obj.CheckRulesForTestAsync();

      var act = async () => await task;
      await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [TestMethod]
    public void CheckRulesDuringPause_DropsTheDiscardedStreamsEarlierNonSequentialResults()
    {
      var obj = Create("earlier");
      obj.Outcomes.Enqueue("Slow", "error");
      obj.CheckRulesNow();

      obj.CheckRulesNow();
      // the first run's Slow completes after its stream was replaced
      obj.Gates.Release("Slow");

      obj.BrokenRulesCollection.ErrorCount.Should().Be(0);
      obj.C.Should().BeEmpty();

      obj.Gates.Release("Slow");
      obj.Gates.Release("Lookup");

      obj.C.Should().Be("Slow-out");
      obj.IsValid.Should().BeTrue();
      obj.IsBusy.Should().BeFalse();
    }

    #endregion

    #region Completion cascades

    [TestMethod]
    public void AsyncRuleCompletingAfterItsStream_CascadeWaitsForSequentialRules()
    {
      var obj = Create("detached");
      obj.CheckRulesNow();
      obj.Gates.Release("LookupB");
      obj.Log.Should().Equal("LookupB:start", "LookupB:end", "B2", "Bg:start");

      // Bg affects B: B's rules run, and B2 waits for the sequential LookupB
      obj.Gates.Release("Bg");

      obj.Log.Should().Equal("LookupB:start", "LookupB:end", "B2", "Bg:start", "Bg:end", "LookupB:start");
      obj.IsBusy.Should().BeTrue();

      obj.Gates.Release("LookupB");

      obj.Log.Should().Equal("LookupB:start", "LookupB:end", "B2", "Bg:start", "Bg:end", "LookupB:start", "LookupB:end", "B2");
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public void AsyncRuleCompletingBeforeItsStreamEnds_StillRunsItsCompletionCascade()
    {
      var obj = Create("trailing");
      obj.Outcomes.Enqueue("Bg", "sync");
      obj.CheckRulesNow();

      obj.Gates.Release("Lookup");

      // Bg completed during the pass with no sequential rule after it: B's rules rerun, as always
      obj.Log.Should().Equal("Lookup:start", "Lookup:end", "B1", "Bg:start", "Bg:end", "B1");
      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public async Task SequentialRuleWithNoAffectedProperties_ReportsTheObjectBusyAndIdle()
    {
      var obj = Create("objectonly");
      var log = LogEvents(obj);

      obj.CheckObjectRulesNow();

      obj.IsBusy.Should().BeTrue();
      log.Should().Contain("BusyChanged::True");
      var waiter = obj.WaitForIdle(TimeSpan.FromSeconds(5));

      obj.Gates.Release("ObjNone");
      await waiter;

      obj.Log.Should().Equal("ObjNone:start", "ObjNone:end", "ObjAfter");
      obj.IsBusy.Should().BeFalse();
      log.Last().Should().Be("BusyChanged::False");
    }

    #endregion

    #region Legacy runner

    [TestMethod]
    public void LegacyRunner_IgnoresIsSequential()
    {
      using var host = CslaTestHost.Create(o => o.ConfigureCsla(c => c.Rules(r => r.UseLegacyCheckRules())));
      var obj = Create("order", host);

      obj.CheckRulesNow();

      obj.Log.Should().Equal("A1", "Lookup:start", "A2", "B1", "B2");
      obj.Gates.Release("Lookup");
      obj.IsBusy.Should().BeFalse();
    }

    #endregion

    private static List<string> LogEvents(SequentialRuleRoot obj)
    {
      var log = new List<string>();
      obj.PropertyChanged += (_, e) => log.Add($"PropertyChanged:{e.PropertyName}");
      obj.ValidationComplete += (_, _) => log.Add("ValidationComplete");
      obj.BusyChanged += (_, e) => log.Add($"BusyChanged:{e.PropertyName}:{e.Busy}");
      return log;
    }
  }

  /// <summary>
  /// Outcomes for the next runs of gated rules, by tag.
  /// </summary>
  public sealed class RuleOutcomes
  {
    private readonly Dictionary<string, Queue<string>> _outcomes = [];

    public void Enqueue(string tag, string outcome)
    {
      if (!_outcomes.TryGetValue(tag, out var queue))
        _outcomes.Add(tag, queue = new Queue<string>());
      queue.Enqueue(outcome);
    }

    public string Next(string tag) =>
      _outcomes.TryGetValue(tag, out var queue) && queue.Count > 0 ? queue.Dequeue() : "ok";
  }

  /// <summary>
  /// A sync rule that records that it ran.
  /// </summary>
  public class SeqRecordRule : BusinessRule
  {
    private readonly string _tag;

    public SeqRecordRule(IPropertyInfo primaryProperty, string tag, int priority, params IPropertyInfo[] affectedProperties)
      : base(primaryProperty)
    {
      _tag = tag;
      Priority = priority;
      AffectedProperties.AddRange(affectedProperties);
      RuleUri.AddQueryParameter("tag", tag);
    }

    protected override void Execute(IRuleContext context)
    {
      ((SequentialRuleRoot)context.Target).Log.Add(_tag);
    }
  }

  /// <summary>
  /// An async rule that records its start and end, waits at a gate, and
  /// completes with the outcome the test chose.
  /// </summary>
  public class SeqGatedRule : BusinessRuleAsync
  {
    private readonly IPropertyInfo _output;

    public SeqGatedRule(IPropertyInfo primaryProperty, string tag, int priority, bool sequential, IPropertyInfo output = null, params IPropertyInfo[] affectedProperties)
      : base(primaryProperty)
    {
      Tag = tag;
      _output = output;
      Priority = priority;
      IsSequential = sequential;
      ProvideTargetWhenAsync = true;
      if (primaryProperty != null)
        InputProperties.Add(primaryProperty);
      AffectedProperties.AddRange(affectedProperties);
      if (output != null)
        AffectedProperties.Add(output);
      RuleUri.AddQueryParameter("tag", tag);
    }

    public string Tag { get; }

    protected override async Task ExecuteAsync(IRuleContext context)
    {
      var target = (SequentialRuleRoot)context.Target;
      var outcome = target.Outcomes.Next(Tag);
      target.Log.Add(Tag + ":start");
      if (outcome != "sync")
        await target.Gates.WaitAt(Tag);
      if (outcome == "throw")
        throw new InvalidOperationException("boom");
      target.Log.Add(Tag + ":end");
      if (_output != null)
        context.AddOutValue(_output, Tag + "-out");
      if (outcome == "error")
        context.AddErrorResult(Tag + " failed");
      else if (outcome == "stop")
        context.AddSuccessResult(true);
    }
  }

  public partial class SequentialRuleRoot : BusinessBase<SequentialRuleRoot>, IHasRuleGates
  {
    public static readonly PropertyInfo<string> AProperty = RegisterProperty<string>(nameof(A));
    public string A
    {
      get => GetProperty(AProperty);
      set => SetProperty(AProperty, value);
    }

    public static readonly PropertyInfo<string> BProperty = RegisterProperty<string>(nameof(B));
    public string B
    {
      get => GetProperty(BProperty);
      set => SetProperty(BProperty, value);
    }

    public static readonly PropertyInfo<string> CProperty = RegisterProperty<string>(nameof(C));
    public string C
    {
      get => GetProperty(CProperty);
      set => SetProperty(CProperty, value);
    }

    public static readonly PropertyInfo<string> DProperty = RegisterProperty<string>(nameof(D));
    public string D
    {
      get => GetProperty(DProperty);
      set => SetProperty(DProperty, value);
    }

    [NonSerialized]
    private RuleGates _gates;
    public RuleGates Gates => _gates ??= new RuleGates();

    [NonSerialized]
    private List<string> _log;
    public List<string> Log => _log ??= [];

    [NonSerialized]
    private RuleOutcomes _outcomes;
    public RuleOutcomes Outcomes => _outcomes ??= new RuleOutcomes();

    public bool CascadeOnDirtyProperties
    {
      get => BusinessRules.CascadeOnDirtyProperties;
      set => BusinessRules.CascadeOnDirtyProperties = value;
    }

    public int ProcessThroughPriority
    {
      get => BusinessRules.ProcessThroughPriority;
      set => BusinessRules.ProcessThroughPriority = value;
    }

    public void UseRuleSet(string ruleSet) => BusinessRules.RuleSet = ruleSet;

    public List<string> CheckRulesNow() => BusinessRules.CheckRules();

    public List<string> CheckObjectRulesNow() => BusinessRules.CheckObjectRules();

    public Task<List<string>> CheckRulesForTestAsync() => BusinessRules.CheckRulesAsync(TimeSpan.FromSeconds(5));

    public Task<List<string>> CheckPropertyRulesForTestAsync(IPropertyInfo property) => BusinessRules.CheckRulesAsync(property, TimeSpan.FromSeconds(5));

    public static SeqGatedRule OrderLookupRule { get; private set; }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      var rules = BusinessRules;

      // the rules after Lookup, for every property, wait for it
      rules.AddRule(new SeqRecordRule(AProperty, "A1", 0), "order");
      OrderLookupRule = new SeqGatedRule(AProperty, "Lookup", 1, sequential: true);
      rules.AddRule(OrderLookupRule, "order");
      rules.AddRule(new SeqRecordRule(AProperty, "A2", 2), "order");
      rules.AddRule(new SeqRecordRule(BProperty, "B1", 0), "order");
      rules.AddRule(new SeqRecordRule(BProperty, "B2", 2), "order");

      // Lookup's output value makes C dirty
      rules.AddRule(new SeqRecordRule(AProperty, "A1", 0), "output");
      rules.AddRule(new SeqGatedRule(AProperty, "Lookup", 1, sequential: true, output: CProperty), "output");
      rules.AddRule(new SeqRecordRule(CProperty, "C1", 0), "output");
      rules.AddRule(new SeqRecordRule(AProperty, "A2", 2), "output");

      // a non-sequential rule starts before the sequential one
      rules.AddRule(new SeqGatedRule(BProperty, "Slow", 0, sequential: false, output: CProperty), "earlier");
      rules.AddRule(new SeqGatedRule(AProperty, "Lookup", 1, sequential: true), "earlier");
      rules.AddRule(new SeqRecordRule(AProperty, "A2", 2), "earlier");
      rules.AddRule(new SeqRecordRule(BProperty, "B2", 2), "earlier");
      rules.AddRule(new SeqRecordRule(CProperty, "C1", 0), "earlier");

      // no sequential rule
      rules.AddRule(new SeqGatedRule(BProperty, "Slow", 0, sequential: false, output: CProperty), "nonseq");
      rules.AddRule(new SeqRecordRule(AProperty, "A2", 2), "nonseq");
      rules.AddRule(new SeqRecordRule(BProperty, "B2", 2), "nonseq");

      // two sequential rules at the same priority
      rules.AddRule(new SeqGatedRule(AProperty, "Lookup", 1, sequential: true), "tie");
      rules.AddRule(new SeqGatedRule(BProperty, "LookupB", 1, sequential: true), "tie");
      rules.AddRule(new SeqRecordRule(AProperty, "A2", 2), "tie");

      // object rules
      rules.AddRule(new SeqGatedRule(null, "ObjLookup", 0, sequential: true, output: null, AProperty), "object");
      rules.AddRule(new SeqObjectRecordRule("ObjAfter", 1), "object");
      rules.AddRule(new SeqRecordRule(AProperty, "A1", 0), "object");

      // property streams: Lookup affects D
      rules.AddRule(new SeqGatedRule(AProperty, "Lookup", 0, sequential: true, output: null, DProperty), "cascade");
      rules.AddRule(new SeqRecordRule(AProperty, "A2", 1), "cascade");
      rules.AddRule(new SeqRecordRule(DProperty, "D1", 0), "cascade");
      rules.AddRule(new SeqGatedRule(BProperty, "LookupB", 0, sequential: true), "cascade");
      rules.AddRule(new SeqRecordRule(BProperty, "B1", 1), "cascade");

      // C and D keep changing each other: CheckRules() never settles
      rules.AddRule(new SeqGatedRule(AProperty, "Lookup", 0, sequential: true), "unsettled");
      rules.AddRule(new SeqToggleRule(CProperty, DProperty, 1), "unsettled");
      rules.AddRule(new SeqToggleRule(DProperty, CProperty, 1), "unsettled");

      // a background rule that affects B completes after the stream ended
      rules.AddRule(new SeqGatedRule(BProperty, "LookupB", 0, sequential: true), "detached");
      rules.AddRule(new SeqRecordRule(BProperty, "B2", 1), "detached");
      rules.AddRule(new SeqGatedRule(AProperty, "Bg", 5, sequential: false, output: null, BProperty), "detached");

      // a background rule that affects B completes before the stream ends
      rules.AddRule(new SeqGatedRule(AProperty, "Lookup", 0, sequential: true), "trailing");
      rules.AddRule(new SeqRecordRule(BProperty, "B1", 0), "trailing");
      rules.AddRule(new SeqGatedRule(AProperty, "Bg", 1, sequential: false, output: null, BProperty), "trailing");

      // a sequential object rule with no affected properties
      rules.AddRule(new SeqGatedRule(null, "ObjNone", 0, sequential: true), "objectonly");
      rules.AddRule(new SeqObjectRecordRule("ObjAfter", 1), "objectonly");
    }

    [Create]
    private void Create()
    {
    }
  }

  /// <summary>
  /// A sync rule that sets another property to a new value every time it runs.
  /// </summary>
  public class SeqToggleRule : BusinessRule
  {
    private readonly IPropertyInfo _output;

    public SeqToggleRule(IPropertyInfo primaryProperty, IPropertyInfo output, int priority)
      : base(primaryProperty)
    {
      _output = output;
      Priority = priority;
      AffectedProperties.Add(output);
    }

    protected override void Execute(IRuleContext context)
    {
      context.AddOutValue(_output, Guid.NewGuid().ToString());
    }
  }

  /// <summary>
  /// A sync object rule that records that it ran.
  /// </summary>
  public class SeqObjectRecordRule : BusinessRule
  {
    private readonly string _tag;

    public SeqObjectRecordRule(string tag, int priority)
    {
      _tag = tag;
      Priority = priority;
      RuleUri.AddQueryParameter("tag", tag);
    }

    protected override void Execute(IRuleContext context)
    {
      ((SequentialRuleRoot)context.Target).Log.Add(_tag);
    }
  }
}
