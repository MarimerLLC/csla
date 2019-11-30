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

## Code Fix

No code fix is available for this analyzer.