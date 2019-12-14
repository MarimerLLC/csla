# Asynchronous business rules should derive from BusinessRuleAsync

## Issue
This analyzer is tripped if there is a `BusinessRule` that does asynchronous work. It should derive from `BusinessRuleAsync`:

```
public class AsynchronousRuleOldSchool
  : BusinessRule
{

  protected override async void Execute(IRuleContext context)
  {
    context.AddDirtyProperty(null);
    await DummyAsync();
    context.Complete();
  }

  private static Task DummyAsync() => Task.CompletedTask;
}
```

## Code Fix

A code fix is available for this analyzer. It will change the base class to `BusinessRuleAsync`, rename the method to `ExecuteAsync`, and change the return type to `Task`.