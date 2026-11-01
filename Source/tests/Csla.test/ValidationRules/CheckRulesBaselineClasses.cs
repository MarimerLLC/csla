//-----------------------------------------------------------------------
// <copyright file="CheckRulesBaselineClasses.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business types used to characterize BusinessRules.CheckRules() behavior</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Rules;

namespace Csla.Test.ValidationRules
{
  /// <summary>
  /// Implemented by test business objects so rules can record that they ran.
  /// </summary>
  public interface IRuleExecutionLog
  {
    void Record(string entry);
  }

  /// <summary>
  /// Base type for CheckRules() baseline tests. Records every rule execution
  /// (in order) and exposes BusinessRules operations to the tests.
  /// </summary>
  [Serializable]
  public abstract class RuleLogRoot<T> : BusinessBase<T>, IRuleExecutionLog
    where T : RuleLogRoot<T>
  {
    [NonSerialized]
    private List<string> _ruleLog;
    [NonSerialized]
    private object _ruleLogLock;

    private object RuleLogLock => _ruleLogLock ??= new object();

    /// <summary>
    /// Rule executions recorded so far, in execution order.
    /// </summary>
    public List<string> RuleLog
    {
      get
      {
        lock (RuleLogLock)
          return (_ruleLog ??= []).ToList();
      }
    }

    public void ClearRuleLog()
    {
      lock (RuleLogLock)
        (_ruleLog ??= []).Clear();
    }

    void IRuleExecutionLog.Record(string entry)
    {
      lock (RuleLogLock)
        (_ruleLog ??= []).Add(entry);
    }

    public List<string> RunCheckRules() => BusinessRules.CheckRules();

    public Task<List<string>> RunCheckRulesAsync() => BusinessRules.CheckRulesAsync();

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
  }

  /// <summary>
  /// Synchronous rule that records its label when it runs and then
  /// performs an optional action.
  /// </summary>
  internal class LoggingRule : BusinessRule
  {
    private readonly string _label;
    private readonly Action<IRuleContext> _action;

    public LoggingRule(
      IPropertyInfo primaryProperty,
      string label,
      IPropertyInfo[] inputs = null,
      IPropertyInfo[] affected = null,
      Action<IRuleContext> action = null,
      bool cascadeIfDirty = false)
      : base(primaryProperty)
    {
      _label = label;
      _action = action;
      if (inputs != null)
        InputProperties.AddRange(inputs);
      if (affected != null)
        AffectedProperties.AddRange(affected);
      CascadeIfDirty = cascadeIfDirty;
      // distinct rule name per label so broken rules don't collide
      RuleUri.AddQueryParameter("label", label);
    }

    protected override void Execute(IRuleContext context)
    {
      ((IRuleExecutionLog)context.Target).Record(_label);
      _action?.Invoke(context);
    }
  }

  /// <summary>
  /// Async rule that records "{label}:start" before its first await
  /// and "{label}:end" when it finishes.
  /// </summary>
  internal class LoggingAsyncRule : BusinessRuleAsync
  {
    private readonly string _label;
    private readonly Action<IRuleContext> _action;

    public LoggingAsyncRule(
      IPropertyInfo primaryProperty,
      string label,
      IPropertyInfo[] inputs = null,
      IPropertyInfo[] affected = null,
      Action<IRuleContext> action = null)
      : base(primaryProperty)
    {
      _label = label;
      _action = action;
      ProvideTargetWhenAsync = true;
      if (inputs != null)
        InputProperties.AddRange(inputs);
      if (affected != null)
        AffectedProperties.AddRange(affected);
      RuleUri.AddQueryParameter("label", label);
    }

    protected override async Task ExecuteAsync(IRuleContext context)
    {
      var log = (IRuleExecutionLog)context.Target;
      log.Record($"{_label}:start");
      await Task.Delay(20);
      _action?.Invoke(context);
      log.Record($"{_label}:end");
    }
  }

  /// <summary>
  /// Rules on several properties with priorities that differ across properties (#4235).
  /// </summary>
  [Serializable]
  public class CrossPropertyPriorityRoot : RuleLogRoot<CrossPropertyPriorityRoot>
  {
    public static readonly PropertyInfo<int> AProperty = RegisterProperty<int>(nameof(A));
    public int A
    {
      get => GetProperty(AProperty);
      set => SetProperty(AProperty, value);
    }

    public static readonly PropertyInfo<int> BProperty = RegisterProperty<int>(nameof(B));
    public int B
    {
      get => GetProperty(BProperty);
      set => SetProperty(BProperty, value);
    }

    public static readonly PropertyInfo<int> CProperty = RegisterProperty<int>(nameof(C));
    public int C
    {
      get => GetProperty(CProperty);
      set => SetProperty(CProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new LoggingRule(AProperty, "A:p1") { Priority = 1 });
      BusinessRules.AddRule(new LoggingRule(BProperty, "B:p-1") { Priority = -1 });
      BusinessRules.AddRule(new LoggingRule(AProperty, "A:p0") { Priority = 0 });
      BusinessRules.AddRule(new LoggingRule(CProperty, "C:p0") { Priority = 0 });
    }

    [Create]
    private void Create()
    {
    }
  }

  /// <summary>
  /// The order / credit hold scenario from issue #3736.
  /// </summary>
  [Serializable]
  public class CreditOrderRoot : RuleLogRoot<CreditOrderRoot>
  {
    public static readonly PropertyInfo<decimal> CreditLimitProperty = RegisterProperty<decimal>(nameof(CreditLimit));
    public decimal CreditLimit
    {
      get => GetProperty(CreditLimitProperty);
      set => SetProperty(CreditLimitProperty, value);
    }

    public static readonly PropertyInfo<decimal> CreditBalanceProperty = RegisterProperty<decimal>(nameof(CreditBalance));
    public decimal CreditBalance
    {
      get => GetProperty(CreditBalanceProperty);
      set => SetProperty(CreditBalanceProperty, value);
    }

    public static readonly PropertyInfo<bool> OnCreditHoldProperty = RegisterProperty<bool>(nameof(OnCreditHold));
    public bool OnCreditHold
    {
      get => GetProperty(OnCreditHoldProperty);
      set => SetProperty(OnCreditHoldProperty, value);
    }

    public static readonly PropertyInfo<decimal> OrderTotalProperty = RegisterProperty<decimal>(nameof(OrderTotal));
    public decimal OrderTotal
    {
      get => GetProperty(OrderTotalProperty);
      set => SetProperty(OrderTotalProperty, value);
    }

    public static readonly PropertyInfo<decimal> ProductAmountProperty = RegisterProperty<decimal>(nameof(ProductAmount));
    public decimal ProductAmount
    {
      get => GetProperty(ProductAmountProperty);
      set => SetProperty(ProductAmountProperty, value);
    }

    public static readonly PropertyInfo<decimal> FreightChargeProperty = RegisterProperty<decimal>(nameof(FreightCharge));
    public decimal FreightCharge
    {
      get => GetProperty(FreightChargeProperty);
      set => SetProperty(FreightChargeProperty, value);
    }

    public static readonly PropertyInfo<decimal> SalesTaxProperty = RegisterProperty<decimal>(nameof(SalesTax));
    public decimal SalesTax
    {
      get => GetProperty(SalesTaxProperty);
      set => SetProperty(SalesTaxProperty, value);
    }

    public static readonly PropertyInfo<bool> CanCheckOutProperty = RegisterProperty<bool>(nameof(CanCheckOut));
    public bool CanCheckOut
    {
      get => GetProperty(CanCheckOutProperty);
      set => SetProperty(CanCheckOutProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new LoggingRule(
        OnCreditHoldProperty, "OnCreditHold.SetCheckoutStatus",
        inputs: [OnCreditHoldProperty],
        affected: [CanCheckOutProperty],
        action: c => c.AddOutValue(CanCheckOutProperty, !(bool)c.InputPropertyValues[OnCreditHoldProperty])));

      BusinessRules.AddRule(new LoggingRule(
        OrderTotalProperty, "OrderTotal.Sum",
        inputs: [ProductAmountProperty, FreightChargeProperty, SalesTaxProperty],
        action: c => c.AddOutValue(OrderTotalProperty,
          (decimal)c.InputPropertyValues[ProductAmountProperty]
          + (decimal)c.InputPropertyValues[FreightChargeProperty]
          + (decimal)c.InputPropertyValues[SalesTaxProperty])));

      BusinessRules.AddRule(new LoggingRule(
        OrderTotalProperty, "OrderTotal.SetCreditHold",
        inputs: [OrderTotalProperty, CreditLimitProperty, CreditBalanceProperty, OnCreditHoldProperty],
        affected: [OnCreditHoldProperty],
        action: c => c.AddOutValue(OnCreditHoldProperty,
          (decimal)c.InputPropertyValues[OrderTotalProperty] + (decimal)c.InputPropertyValues[CreditBalanceProperty]
          > (decimal)c.InputPropertyValues[CreditLimitProperty])));

      AddSignAgreement(ProductAmountProperty, FreightChargeProperty, SalesTaxProperty);
      AddSignAgreement(FreightChargeProperty, ProductAmountProperty, SalesTaxProperty);
      AddSignAgreement(SalesTaxProperty, FreightChargeProperty, ProductAmountProperty);
    }

    private void AddSignAgreement(PropertyInfo<decimal> primary, params PropertyInfo<decimal>[] others)
    {
      IPropertyInfo[] inputs = [primary, .. others];
      BusinessRules.AddRule(new LoggingRule(
        primary, $"{primary.Name}.SignAgreement",
        inputs: inputs,
        action: c =>
        {
          var signs = inputs.Select(p => Math.Sign((decimal)c.InputPropertyValues[p])).Where(s => s != 0).Distinct();
          if (signs.Count() > 1)
            c.AddErrorResult("Amounts must have the same sign");
        }));
    }

    public void LoadSalesTax(decimal value) => LoadProperty(SalesTaxProperty, value);

    [Create]
    private void Create()
    {
      // Totals are consistent, but the order exceeds the credit limit, so it
      // should end up on credit hold. OnCreditHold and CanCheckOut start false.
      LoadProperty(CreditLimitProperty, 100m);
      LoadProperty(CreditBalanceProperty, 50m);
      LoadProperty(ProductAmountProperty, 100m);
      LoadProperty(FreightChargeProperty, 10m);
      LoadProperty(SalesTaxProperty, 5m);
      LoadProperty(OrderTotalProperty, 115m);
    }
  }

  /// <summary>
  /// Increment chain: N1 -> N2 -> N3 -> N4, each rule outputs the next property.
  /// </summary>
  [Serializable]
  public class DependencyChainRoot : RuleLogRoot<DependencyChainRoot>
  {
    public static readonly PropertyInfo<int> N1Property = RegisterProperty<int>(nameof(N1));
    public int N1
    {
      get => GetProperty(N1Property);
      set => SetProperty(N1Property, value);
    }

    public static readonly PropertyInfo<int> N2Property = RegisterProperty<int>(nameof(N2));
    public int N2
    {
      get => GetProperty(N2Property);
      set => SetProperty(N2Property, value);
    }

    public static readonly PropertyInfo<int> N3Property = RegisterProperty<int>(nameof(N3));
    public int N3
    {
      get => GetProperty(N3Property);
      set => SetProperty(N3Property, value);
    }

    public static readonly PropertyInfo<int> N4Property = RegisterProperty<int>(nameof(N4));
    public int N4
    {
      get => GetProperty(N4Property);
      set => SetProperty(N4Property, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      AddIncrement(N1Property, N2Property);
      AddIncrement(N2Property, N3Property);
      AddIncrement(N3Property, N4Property);
    }

    private void AddIncrement(PropertyInfo<int> source, PropertyInfo<int> target)
    {
      BusinessRules.AddRule(new LoggingRule(
        source, $"{source.Name}->{target.Name}",
        inputs: [source],
        affected: [target],
        action: c => c.AddOutValue(target, (int)c.InputPropertyValues[source] + 1)));
    }

    [Create]
    private void Create()
    {
    }
  }

  /// <summary>
  /// Short-circuiting: ProcessThroughPriority and StopProcessing.
  /// </summary>
  [Serializable]
  public class ShortCircuitBaselineRoot : RuleLogRoot<ShortCircuitBaselineRoot>
  {
    public static readonly PropertyInfo<int> XProperty = RegisterProperty<int>(nameof(X));
    public int X
    {
      get => GetProperty(XProperty);
      set => SetProperty(XProperty, value);
    }

    public static readonly PropertyInfo<int> YProperty = RegisterProperty<int>(nameof(Y));
    public int Y
    {
      get => GetProperty(YProperty);
      set => SetProperty(YProperty, value);
    }

    public static readonly PropertyInfo<int> ZProperty = RegisterProperty<int>(nameof(Z));
    public int Z
    {
      get => GetProperty(ZProperty);
      set => SetProperty(ZProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new LoggingRule(XProperty, "X:p0-broken", action: c => c.AddErrorResult("X is broken")) { Priority = 0 });
      BusinessRules.AddRule(new LoggingRule(XProperty, "X:p1") { Priority = 1 });
      BusinessRules.AddRule(new LoggingRule(YProperty, "Y:p1") { Priority = 1 });
      BusinessRules.AddRule(new LoggingRule(ZProperty, "Z:stop", action: c => c.AddSuccessResult(true)));
      BusinessRules.AddRule(new LoggingRule(ZProperty, "Z:after-stop"));
    }

    [Create]
    private void Create()
    {
    }
  }

  /// <summary>
  /// RunModes: DenyCheckRules and DenyAsAffectedProperty.
  /// </summary>
  [Serializable]
  public class RunModeBaselineRoot : RuleLogRoot<RunModeBaselineRoot>
  {
    public static readonly PropertyInfo<int> PProperty = RegisterProperty<int>(nameof(P));
    public int P
    {
      get => GetProperty(PProperty);
      set => SetProperty(PProperty, value);
    }

    public static readonly PropertyInfo<int> QProperty = RegisterProperty<int>(nameof(Q));
    public int Q
    {
      get => GetProperty(QProperty);
      set => SetProperty(QProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new LoggingRule(PProperty, "P:deny-check-rules") { RunMode = RunModes.DenyCheckRules });
      BusinessRules.AddRule(new LoggingRule(PProperty, "P:normal", affected: [QProperty]));
      BusinessRules.AddRule(new LoggingRule(QProperty, "Q:deny-as-affected") { RunMode = RunModes.DenyAsAffectedProperty });
      BusinessRules.AddRule(new LoggingRule(QProperty, "Q:normal"));
    }

    [Create]
    private void Create()
    {
    }
  }

  /// <summary>
  /// Rules that depend on K through InputProperties, with and without CascadeIfDirty.
  /// </summary>
  [Serializable]
  public class InputDependencyRoot : RuleLogRoot<InputDependencyRoot>
  {
    public static readonly PropertyInfo<int> KProperty = RegisterProperty<int>(nameof(K));
    public int K
    {
      get => GetProperty(KProperty);
      set => SetProperty(KProperty, value);
    }

    public static readonly PropertyInfo<int> MProperty = RegisterProperty<int>(nameof(M));
    public int M
    {
      get => GetProperty(MProperty);
      set => SetProperty(MProperty, value);
    }

    public static readonly PropertyInfo<int> M2Property = RegisterProperty<int>(nameof(M2));
    public int M2
    {
      get => GetProperty(M2Property);
      set => SetProperty(M2Property, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new LoggingRule(KProperty, "K:rule"));
      BusinessRules.AddRule(new LoggingRule(MProperty, "M:cascade-if-dirty", inputs: [KProperty], cascadeIfDirty: true));
      BusinessRules.AddRule(new LoggingRule(M2Property, "M2:input-K", inputs: [KProperty]));
    }

    [Create]
    private void Create()
    {
    }
  }

  /// <summary>
  /// Object-level rule that sets a property value.
  /// </summary>
  [Serializable]
  public class ObjectRuleBaselineRoot : RuleLogRoot<ObjectRuleBaselineRoot>
  {
    public static readonly PropertyInfo<int> PProperty = RegisterProperty<int>(nameof(P));
    public int P
    {
      get => GetProperty(PProperty);
      set => SetProperty(PProperty, value);
    }

    public static readonly PropertyInfo<int> QProperty = RegisterProperty<int>(nameof(Q));
    public int Q
    {
      get => GetProperty(QProperty);
      set => SetProperty(QProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new LoggingRule(QProperty, "Q:input-P", inputs: [PProperty]));
      BusinessRules.AddRule(new LoggingRule(PProperty, "P:rule"));
      BusinessRules.AddRule(new LoggingRule(null, "Object:sets-P", affected: [PProperty], action: c => c.AddOutValue(PProperty, 42)));
    }

    [Create]
    private void Create()
    {
    }
  }

  /// <summary>
  /// Async rule on A whose output and affected property is B.
  /// </summary>
  [Serializable]
  public class AsyncBaselineRoot : RuleLogRoot<AsyncBaselineRoot>
  {
    public static readonly PropertyInfo<int> AProperty = RegisterProperty<int>(nameof(A));
    public int A
    {
      get => GetProperty(AProperty);
      set => SetProperty(AProperty, value);
    }

    public static readonly PropertyInfo<int> BProperty = RegisterProperty<int>(nameof(B));
    public int B
    {
      get => GetProperty(BProperty);
      set => SetProperty(BProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new LoggingAsyncRule(AProperty, "A:async", affected: [BProperty], action: c => c.AddOutValue(BProperty, 7)));
      BusinessRules.AddRule(new LoggingRule(BProperty, "B:sync"));
    }

    [Create]
    private void Create()
    {
    }
  }
}
