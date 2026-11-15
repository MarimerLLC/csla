# Methods with [AddsToRuleContext] should add to the rule context

## Issue
`[AddsToRuleContext]` tells [CSLA0017](CSLA0017-BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer.md) that a call to the method counts as a call to an `Add...()` method of the rule context. This analyzer reports a method or local function with the attribute when:

* it has no parameter of type `IRuleContext`, a type that implements it, or a type parameter constrained to one of those (the `this` parameter of an extension method counts), or
* its body neither calls an `Add...()` method on that rule context parameter, nor calls another method with `[AddsToRuleContext]` and passes that parameter to it (as the receiver or as an argument).

```
using Csla.Rules;

public static class RuleContextExtensions
{
  // CSLA0031: no IRuleContext parameter
  [AddsToRuleContext]
  public static void Log(string message) { }

  // CSLA0031: no Add...() call
  [AddsToRuleContext]
  public static void Check(this IRuleContext context, bool condition) { }
}
```

These methods are not reported:

```
using Csla.Rules;

public static class RuleContextExtensions
{
  [AddsToRuleContext]
  public static void Fail(this IRuleContext context, string message)
  {
    context.AddErrorResult(message);
  }

  // Calls another [AddsToRuleContext] method and passes its rule context
  [AddsToRuleContext]
  public static void FailIf(this IRuleContext context, bool condition, string message)
  {
    if (condition)
      context.Fail(message);
  }
}
```

## Overrides and interface implementations
The attribute is not inherited, but CSLA0017 accepts a call to an abstract, virtual or interface method that has it, and that call runs an override or implementation. So this analyzer also checks overrides and implementations of a method with `[AddsToRuleContext]`, even when they don't have the attribute themselves:

```
using Csla.Rules;

public abstract class MyRuleBase
  : BusinessRule
{
  [AddsToRuleContext]
  protected abstract void Fail(IRuleContext context);
}

public class MyRule
  : MyRuleBase
{
  // CSLA0031: overrides an [AddsToRuleContext] method but has no Add...() call
  protected override void Fail(IRuleContext context) { }

  protected override void Execute(IRuleContext context) => Fail(context);
}
```

Methods without a body, such as abstract, interface and `extern` methods, are not checked themselves.

## Limitations
* The analyzer only checks that the body contains an `Add...()` call. It doesn't check that the call runs on every path through the method.
* A call to the method itself doesn't count, but methods that only call each other (`A` calls `B`, `B` calls `A`) are not reported.
* The rule context must be passed as the parameter itself (`context`, `context?.`, or with a cast). A copy in a local variable (`var c = context; c.AddErrorResult(...)`) is not recognized.

## Code Fix

No code fix is available for this analyzer.
