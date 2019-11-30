# Complete() should not be called in an asynchronous business rule

## Issue
This analyzer is tripped if `Complete()` is called in `ExecuteAsync()`:

```
public class CallingComplete
  : BusinessRuleAsync
{
  protected override Task ExecuteAsync(IRuleContext context)
  {
    context.AddDirtyProperty(null);
    // This method should have an error
    // because we call Complete().
    context.Complete();
    return Task.CompletedTask;
  }
}
```

## Code Fix

A code fix is available for this analyzer. It will remove calls to `Complete()`.