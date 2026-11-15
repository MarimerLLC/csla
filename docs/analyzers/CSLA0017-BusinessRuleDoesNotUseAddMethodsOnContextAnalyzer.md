# Business rules should use at least one Add() method on the context

## Issue
This analyzer is tripped if there is a business rule that does not call one of the `Add...()` methods on the given `IRuleContext` at least once:

```
public class ExecuteWithoutAdd  
  : BusinessRule
{
  protected override void Execute(IRuleContext context)
  {
    // The parameter should have an error
    // because there are no Add...() calls.
  }
}
```

## Helper methods
A rule that calls a reusable helper which calls the `Add...()` methods itself, such as an extension method on `IRuleContext`, a static helper, or a helper method in a rule base class, is reported too, because the analyzer only sees the call to the helper. Add `[AddsToRuleContext]` (namespace `Csla.Rules`) to the helper, and the analyzer treats a call to it like a call to an `Add...()` method:

```
using Csla;
using Csla.Rules;

public static class RuleContextExtensions
{
  [AddsToRuleContext]
  public static void SetOutValue<T>(this IRuleContext context, PropertyInfo<T> property, T value)
  {
    // normalization, logging, ...
    context.AddOutValue(property, value);
  }
}

public class SetToFortyTwo
  : BusinessRule
{
  private readonly PropertyInfo<int> _target;

  public SetToFortyTwo(PropertyInfo<int> target)
    : base(target)
  {
    _target = target;
    AffectedProperties.Add(target);
  }

  protected override void Execute(IRuleContext context)
  {
    // No CSLA0017: SetOutValue has [AddsToRuleContext].
    context.SetOutValue(_target, 42);
  }
}
```

The analyzer doesn't check which rule context the rule passes to the helper. [CSLA0031](CSLA0031-AddsToRuleContextAttributeUsageAnalyzer.md) checks that each method with `[AddsToRuleContext]` really adds to the rule context it is given.

## Code Fix

No code fix is available for this analyzer.
