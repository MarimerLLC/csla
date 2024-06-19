# Object authorization rules configuration should be declared as static.

## Issue
This analyzer is informing developer, that Object authorization rules method should be declared as `static`:

```
public class Customer
  : BusinessBase<Customer>
{
  [ObjectAuthorizationRules]
  public void AddObjectAuthorizationRules()
  {
  }
}
```

Please add `static` modified to the method to fix an issue.

## Code Fix

No code fix exists for this analyzer.