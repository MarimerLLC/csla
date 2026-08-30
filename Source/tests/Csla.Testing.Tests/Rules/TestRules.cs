//-----------------------------------------------------------------------
// <copyright file="TestRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Sample business rules used by the rule tester tests</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Rules;

namespace Csla.Testing.Tests.Rules
{
  /// <summary>
  /// Service resolved by <see cref="RuleUsingInjectedService"/>.
  /// </summary>
  public interface IGreetingService
  {
    string Greet(string? name);
  }

  /// <summary>
  /// Simple implementation of <see cref="IGreetingService"/>.
  /// </summary>
  public class GreetingService : IGreetingService
  {
    public string Greet(string? name) => $"Hello, {name}";
  }

  /// <summary>
  /// Synchronous rule that requires the primary property to have a value.
  /// </summary>
  public class SyncRequiredRule : BusinessRule
  {
    public const string ErrorMessage = "Name required";

    public SyncRequiredRule(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties.Add(primaryProperty);
    }

    protected override void Execute(IRuleContext context)
    {
      var value = context.GetInputValue<string>(PrimaryProperty!);
      if (string.IsNullOrWhiteSpace(value))
        context.AddErrorResult(ErrorMessage);
    }
  }

  /// <summary>
  /// Asynchronous rule that requires the primary property to have a value.
  /// </summary>
  public class AsyncRequiredRule : BusinessRuleAsync
  {
    public const string ErrorMessage = "Name required (async)";

    public AsyncRequiredRule(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties.Add(primaryProperty);
    }

    protected override async Task ExecuteAsync(IRuleContext context)
    {
      await Task.Yield();
      var value = context.GetInputValue<string>(PrimaryProperty!);
      if (string.IsNullOrWhiteSpace(value))
        context.AddErrorResult(ErrorMessage);
    }
  }

  /// <summary>
  /// Asynchronous rule that records whether it was given the target object.
  /// </summary>
  public class AsyncTargetProbeRule : BusinessRuleAsync
  {
    public AsyncTargetProbeRule(IPropertyInfo primaryProperty, bool provideTarget)
      : base(primaryProperty)
    {
      ProvideTargetWhenAsync = provideTarget;
    }

    protected override Task ExecuteAsync(IRuleContext context)
    {
      SawTarget = context.Target;
      return Task.CompletedTask;
    }

    public object? SawTarget { get; private set; }
  }

  /// <summary>
  /// Rule that adds a result of the configured severity.
  /// </summary>
  public class SeverityRule : BusinessRule
  {
    public const string Message = "Severity message";

    private readonly RuleSeverity _severity;

    public SeverityRule(IPropertyInfo primaryProperty, RuleSeverity severity)
      : base(primaryProperty)
    {
      _severity = severity;
    }

    protected override void Execute(IRuleContext context)
    {
      switch (_severity)
      {
        case RuleSeverity.Error:
          context.AddErrorResult(Message);
          break;
        case RuleSeverity.Warning:
          context.AddWarningResult(Message);
          break;
        case RuleSeverity.Information:
          context.AddInformationResult(Message);
          break;
        default:
          context.AddSuccessResult(false);
          break;
      }
    }
  }

  /// <summary>
  /// Object level rule that sets an out value on an affected property and
  /// marks it dirty.
  /// </summary>
  public class OutValueRule : BusinessRule
  {
    public OutValueRule(IPropertyInfo primaryProperty, IPropertyInfo affectedProperty)
      : base(primaryProperty)
    {
      InputProperties.Add(primaryProperty);
      AffectedProperties.Add(affectedProperty);
      AffectedProperty = affectedProperty;
    }

    public IPropertyInfo AffectedProperty { get; }

    protected override void Execute(IRuleContext context)
    {
      var value = context.GetInputValue<string>(PrimaryProperty!);
      context.AddOutValue(AffectedProperty, (value ?? string.Empty).ToUpperInvariant());
      context.AddDirtyProperty(AffectedProperty);
    }
  }

  /// <summary>
  /// Rule that records the execution context mode it was given.
  /// </summary>
  public class ContextModeProbeRule : BusinessRule
  {
    public ContextModeProbeRule(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
    }

    public RuleContextModes SawMode { get; private set; }
    public bool SawCheckRulesContext { get; private set; }

    protected override void Execute(IRuleContext context)
    {
      SawMode = context.ExecuteContext;
      SawCheckRulesContext = context.IsCheckRulesContext;
    }
  }

  /// <summary>
  /// Rule that records the inputs it was given.
  /// </summary>
  public class InputProbeRule : BusinessRule
  {
    public InputProbeRule(IPropertyInfo primaryProperty, params IPropertyInfo[] inputProperties)
      : base(primaryProperty)
    {
      InputProperties.AddRange(inputProperties);
    }

    public Dictionary<IPropertyInfo, object?>? SawInputs { get; private set; }

    protected override void Execute(IRuleContext context)
    {
      SawInputs = new Dictionary<IPropertyInfo, object?>(context.InputPropertyValues);
    }
  }

  /// <summary>
  /// Rule that resolves a service from the application context and reports the
  /// result as an information message.
  /// </summary>
  public class RuleUsingInjectedService : BusinessRule
  {
    public RuleUsingInjectedService(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties.Add(primaryProperty);
    }

    protected override void Execute(IRuleContext context)
    {
      var service = context.ApplicationContext.GetRequiredService<IGreetingService>();
      context.AddInformationResult(service.Greet(context.GetInputValue<string>(PrimaryProperty!)));
    }
  }

  /// <summary>
  /// Rule that records the principal it ran under.
  /// </summary>
  public class PrincipalProbeRule : BusinessRule
  {
    public PrincipalProbeRule(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
    }

    public string? SawName { get; private set; }
    public bool SawIsInAdminRole { get; private set; }
    public bool SawAuthenticated { get; private set; }

    protected override void Execute(IRuleContext context)
    {
      var user = context.ApplicationContext.User;
      SawName = user.Identity?.Name;
      SawIsInAdminRole = user.IsInRole("Admin");
      SawAuthenticated = user.Identity?.IsAuthenticated ?? false;
    }
  }

  /// <summary>
  /// Rule implementing neither <see cref="IBusinessRule"/> nor
  /// <see cref="IBusinessRuleAsync"/>.
  /// </summary>
  public class NotARunnableRule : BusinessRuleBase
  {
    public NotARunnableRule(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
    }

    public override bool IsAsync { get; protected set; }
  }
}
