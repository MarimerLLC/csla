# Methods with [AddsToRuleContext] should add to the rule context

## Issue
`[AddsToRuleContext]` tells [CSLA0017](CSLA0017-BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer.md) that a call to the method counts as a call to an `Add...()` method of the rule context. This analyzer reports a method with the attribute when:

* it has no parameter of type `IRuleContext` (or a type that implements it), including the `this` parameter of an extension method, or
* its body calls neither an `Add...()` method of that rule context, nor another method with `[AddsToRuleContext]` that it passes the rule context to.

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

A call to the method itself doesn't count. Methods without a body, such as abstract, interface and `extern` methods, are not checked.

The analyzer only checks that the body contains an `Add...()` call. It doesn't check that the call runs on every path through the method.

## Code Fix

No code fix is available for this analyzer.
